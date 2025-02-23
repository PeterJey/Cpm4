using Cpm.Core.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Cpm.Infrastructure.Data
{
    public sealed class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
            ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        }

        public DbSet<Farm> Farms { get; set; }

        public DbSet<Field> Fields { get; set; }

        public DbSet<HarvestRegister> HarvestRegisters { get; set; }

        public DbSet<PickingPlan> PickingPlans { get; set; }

        public DbSet<Site> Sites { get; set; }

        public DbSet<Scenario> Scenarios { get; set; }

        public DbSet<PinnedNote> PinnedNotes { get; set; }

        public DbSet<FieldScore> FieldScores { get; set; }

        public DbSet<SiteUserPermission> SiteUserPermissions { get; set; }

        public DbSet<HarvestProfile> HarvestProfiles { get; set; }

        public DbSet<TempProfile> TempProfiles { get; set; }

        public DbSet<Allocation> Allocations { get; set; }

        public DbSet<ForecastStat> ForecastStats { get; set; }

        public DbSet<WeatherStat> WeatherStats { get; set; }

        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    base.OnConfiguring(optionsBuilder);
        //    optionsBuilder.EnableSensitiveDataLogging(true);
        //}

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<SiteUserPermission>()
                .HasKey(x => new { x.SiteId, x.UserId });

            builder.Entity<PinnedNote>()
                .HasKey(x => new { x.FieldId, x.Date, x.Version });

            builder.Entity<FieldScore>()
                .HasKey(x => new { x.FieldId, x.Version });

            builder.Entity<Scenario>()
                .HasKey(x => new { x.ScenarioId, x.Version });

            builder.Entity<Field>()
                .Property(x => x.AreaInHectares)
                .HasColumnType("DECIMAL(15,3)");

            builder.Entity<ForecastStat>()
                .HasIndex(x => new { x.AlgorithmName, x.FieldId, x.ScenarioId })
                .IsUnique();

            builder.Entity<WeatherStat>()
                .Property(x => x.When)
                .HasColumnType("DATE");

            builder.Entity<WeatherStat>()
                .Property(x => x.TempMin)
                .HasColumnType("DECIMAL(16,2)");

            builder.Entity<WeatherStat>()
                .Property(x => x.TempMax)
                .HasColumnType("DECIMAL(16,2)");

            builder.Entity<WeatherStat>()
                .HasIndex(x => new { x.When, x.Location })
                .IsUnique();

            // to avoid creating navigation property - decoupled from ApplicationUser
            builder.Entity<SiteUserPermission>()
                .HasOne<ApplicationUser>()
                .WithMany(x => x.SitePermissions)
                .HasForeignKey(x => x.UserId);

            builder.Entity<HarvestRegister>()
                .HasKey(x => new { x.FieldId, x.Version });

            builder.Entity<PickingPlan>()
                .HasKey(x => new { x.FieldId, x.Version });

            builder.Entity<Allocation>()
                .HasKey(x => new { x.FieldId, x.Date, x.ProductType, x.PerTray, x.PerPunnet, x.Version });

            // following is a workaround for MySQL maximum key length of 767 bytes (255 characters)
            builder.Entity<ApplicationUser>(entity => entity.Property(m => m.Id)
                .HasMaxLength(255));
            builder.Entity<ApplicationUser>(entity => entity.Property(m => m.Email)
                .HasMaxLength(255));
            builder.Entity<ApplicationUser>(entity => entity.Property(m => m.NormalizedEmail)
                .HasMaxLength(255));
            builder.Entity<ApplicationUser>(entity => entity.Property(m => m.UserName)
                .HasMaxLength(255));
            builder.Entity<ApplicationUser>(entity => entity.Property(m => m.NormalizedUserName)
                .HasMaxLength(255));

            builder.Entity<IdentityRole>(entity => entity.Property(m => m.Id)
                .HasMaxLength(255));
            builder.Entity<IdentityRole>(entity => entity.Property(m => m.NormalizedName)
                .HasMaxLength(255));

            builder.Entity<IdentityUserLogin<string>>(entity => entity.Property(m => m.LoginProvider)
                .HasMaxLength(255));
            builder.Entity<IdentityUserLogin<string>>(entity => entity.Property(m => m.ProviderKey)
                .HasMaxLength(255));
            builder.Entity<IdentityUserLogin<string>>(entity => entity.Property(m => m.UserId)
                .HasMaxLength(255));

            builder.Entity<IdentityUserRole<string>>(entity => entity.Property(m => m.UserId)
                .HasMaxLength(255));
            builder.Entity<IdentityUserRole<string>>(entity => entity.Property(m => m.RoleId)
                .HasMaxLength(255));

            builder.Entity<IdentityUserToken<string>>(entity => entity.Property(m => m.UserId)

                .HasMaxLength(255));
            builder.Entity<IdentityUserToken<string>>(entity => entity.Property(m => m.LoginProvider)
                .HasMaxLength(255));
            builder.Entity<IdentityUserToken<string>>(entity => entity.Property(m => m.Name)
                .HasMaxLength(255));

            builder.Entity<IdentityUserClaim<string>>(entity => entity.Property(m => m.Id)
                .HasMaxLength(255));
            builder.Entity<IdentityUserClaim<string>>(entity => entity.Property(m => m.UserId)
                .HasMaxLength(255));
            builder.Entity<IdentityRoleClaim<string>>(entity => entity.Property(m => m.Id)
                .HasMaxLength(255));
            builder.Entity<IdentityRoleClaim<string>>(entity => entity.Property(m => m.RoleId)
                .HasMaxLength(255));
        }
    }
}
