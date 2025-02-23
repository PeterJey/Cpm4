using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cpm.Core;
using Cpm.Core.Models;
using Cpm.Core.Services;
using Cpm.Core.Services.Forecast;
using Cpm.Core.Services.Profiles;
using Cpm.Core.Services.Scenarios;
using Cpm.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Optional;

namespace Cpm.Infrastructure
{
    public class EfCachedProfileRepository : IProfileRepository
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IOptions<ProfileRepositoryOptions> _options;
        private readonly ILogger<EfCachedProfileRepository> _logger;
        private readonly IAuditDataProvider _auditDataProvider;
        private readonly IMemoryCache _cache;

        public EfCachedProfileRepository(
            ApplicationDbContext dbContext,
            IOptions<ProfileRepositoryOptions> options,
            ILogger<EfCachedProfileRepository> logger,
            IAuditDataProvider auditDataProvider,
            IMemoryCache cache
            )
        {
            _dbContext = dbContext;
            _options = options;
            _logger = logger;
            _auditDataProvider = auditDataProvider;
            _cache = cache;
        }

        public async Task<MatchedProfile> FindMatch(string profileName, SeasonsProfile seasonsProfile)
        {
            if (string.IsNullOrEmpty(profileName) || seasonsProfile == null)
            {
                return null;
            }

            var key = CreateKey(profileName, seasonsProfile);

            if (_cache.TryGetValue(key, out var match))
            {
                _logger.LogInformation("Cache hit for profile {profileKey}", key);
                return await (Task<MatchedProfile>) match;
            }

            _logger.LogInformation("Cache miss for profile {profileKey}", key);

            var profileTask = FetchProfile(profileName, seasonsProfile);

            _logger.LogInformation("Setting cache for profile {profileKey}", key);
            Set(key, profileTask);

            return await profileTask;
        }

        public async Task<IEnumerable<MatchedProfile>> GetByNamePattern(string pattern)
        {
            var variants = (await _dbContext.HarvestProfiles
                .Where(x => !x.ProfileName.StartsWith("DELETED"))
                .ToListAsync())
                .Where(x => PatternMatches(pattern, x.ProfileName))
                .OrderBy(x => x.ProfileName)
                .ThenBy(x => x.Order)
                .ToList();

            return variants
                .Select(ToMatchedProfile);
        }

        public async Task<string> Save(string id, Func<MatchedProfile, string> updateFunc)
        {
            var isNew = string.IsNullOrEmpty(id);

            var existing = await _dbContext.HarvestProfiles.SingleOrDefaultAsync(x => x.Id == id);

            var match = existing != null 
                    ? ToMatchedProfile(existing)
                    : new MatchedProfile();

            var error = updateFunc(match);

            if (!string.IsNullOrEmpty(error))
            {
                return error;
            }

            var entity = new HarvestProfile
            {
                Id = isNew ? IdHelper.NewId() : match.Id,
                ProfileName = match.Name,
                Order = match.Order,
                Quality = match.Quality.ToString(),
                StartingWeek = match.StartingWeek,
                ExtraPotential = match.ExtraPotential,
                SerializedCriteria = JsonCriteriaSerializer.Instance.Serialize(match.SeasonsProfile),
                SerializedPoints = JsonConvert.SerializeObject(match.Points),
                Description = match.Description,
                CreatedBy = _auditDataProvider.GetUserField(),
                CreatedOn  = _auditDataProvider.GetTimestampField(),
                Comment = match.Comments == null ? null : string.Join("\n", match.Comments)
            };

            DetachAllEntities();
            _dbContext.Entry(entity).State = isNew ? EntityState.Added : EntityState.Modified;

            await _dbContext.SaveChangesAsync();

            var key = CreateKey(match.Name, match.SeasonsProfile);
            _logger.LogInformation("Profile was updated, setting the cache for profile {profileKey}", key);
            Set(key, Task.FromResult(match));

            return string.Empty;
        }

        public async Task Delete(string id)
        {
            var variants = await _dbContext.HarvestProfiles
                .Where(x => x.Id == id)
                .ToListAsync();

            DetachAllEntities();

            foreach (var variant in variants)
            {
                variant.ProfileName = $"DELETED:{IdHelper.NewId()}:{variant.ProfileName}";
                _dbContext.Entry(variant).State = EntityState.Modified;
            }
            
            await _dbContext.SaveChangesAsync();
        }

        private void DetachAllEntities()
        {
            _dbContext.ChangeTracker.Entries()
                .ToList()
                .ForEach(entry => _dbContext.Entry(entry.Entity).State = EntityState.Detached);
        }

        private void Set(string key, Task<MatchedProfile> profileTask)
        {
            var expiration = _options.Value.KeepUntilEndOfDay
                ? DateTime.Today.AddDays(1).Subtract(DateTime.Now)
                : _options.Value.Expiration;

            _cache.Set(key, profileTask, expiration);
        }

        private string CreateKey(string profileName, SeasonsProfile seasonsProfile)
        {
            return new StringBuilder()
                .Append("profile:")
                .Append(profileName)
                .Append("-")
                .AppendJoin(
                    ",",
                    seasonsProfile
                        .ToScores()
                        .Select(x => $"{x.Season}:{x.Score}")
                )
                .ToString();
        }

        private bool PatternMatches(string pattern, string name)
        {
            if (string.IsNullOrEmpty(pattern))
            {
                return true;
            }

            if (string.IsNullOrEmpty(name))
            {
                return false;
            }

            var patternTokens = pattern.Split("-");
            var nameTokens = name.Split("-");

            if (patternTokens.Length != nameTokens.Length)
            {
                return false;
            }

            return patternTokens
                .Zip(
                    nameTokens,
                    PatternMatchesToken
                        )
                .All(x => x);
        }

        private bool PatternMatchesToken(string pattern, string token)
        {
            if (pattern == "*")
            {
                return true;
            }

            if (pattern == "")
            {
                return token == string.Empty;
            }

            return pattern == token;
        }

        private async Task<MatchedProfile> FetchProfile(string profileName, SeasonsProfile seasonsProfile)
        {
            var variants = await _dbContext.HarvestProfiles
                .AsNoTracking()
                .Where(x => x.ProfileName == profileName)
                .OrderBy(x => x.Order)
                .ToListAsync();

            return variants
                .Select(ToMatchedProfile)
                .FirstOrDefault(x => x.SeasonsProfile.Includes(seasonsProfile));
        }

        private MatchedProfile ToMatchedProfile(HarvestProfile variant)
        {
            var seasonsCriteria = JsonCriteriaSerializer.Instance.Deserialize(variant.SerializedCriteria);

            var comments = variant.Comment
                .Some()
                .Filter(x => !string.IsNullOrEmpty(x))
                .Match(
                    x => new List<string> {x},
                    () => new List<string>()
                );

            if (!Enum.TryParse<ForecastQuality>(variant.Quality, out var quality))
            {
                quality = ForecastQuality.Unknown;
                _logger.LogWarning("Unknown quality value: {quality} in profile variant id: {id}", variant.Quality, variant.Id);
            }

            var points = JsonConvert.DeserializeObject<ProfilePoint[]>(variant.SerializedPoints);

            return new MatchedProfile
            {
                Id = variant.Id,
                Name = variant.ProfileName,
                Description = variant.Description,
                Order = variant.Order,
                SeasonsProfile = seasonsCriteria,
                Seasons = new HashSet<Season>(seasonsCriteria
                    .ToScores()
                    .Where(x => !string.IsNullOrEmpty(x.Score))
                    .Select(x => x.Season)
                ),
                Quality = quality,
                Comments = comments,
                StartingWeek = variant.StartingWeek,
                ExtraPotential = variant.ExtraPotential,
                Points = points,
            };
        }
    }
}