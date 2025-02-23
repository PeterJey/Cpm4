using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Cpm.Core.Services.Mailing;
using Cpm.Core.Services.Weather;
using FluentEmail.Core;
using FluentEmail.Core.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Stateless;

namespace Cpm.Monitor
{
    public class ApixuMonitorService : BackgroundService
    {
        private readonly IMonitorProbe _probe;
        private readonly ISender _sender;
        private readonly ILogger<ApixuMonitorService> _logger;
        private readonly ApixuMonitorOptions _settings;

        public ApixuMonitorService(
            IOptions<ApixuMonitorOptions> settings,
            ApixuProbe probe,
            ISender sender,
            ILogger<ApixuMonitorService> logger
        )
        {
            _probe = probe;
            _sender = sender;
            _logger = logger;
            _settings = settings.Value;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            //stoppingToken.Register(() =>
            //    _logger.LogWarning($"GracePeriod background task is stopping."));

            _handler = HandleOk;
            while (!stoppingToken.IsCancellationRequested)
            {
                var status = await _probe.Check(stoppingToken);
                var delay = await _handler(status, stoppingToken);
                await Task.Delay(delay, stoppingToken);
            }

            _logger.LogInformation($"Monitor task is stopping.");
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            return base.StopAsync(cancellationToken);
            //cleanup
        }

        readonly List<ProbeStatus> _checks = new List<ProbeStatus>();
        private Func<ProbeStatus, CancellationToken, Task<TimeSpan>> _handler;

        private Task<TimeSpan> HandleOk(ProbeStatus status, CancellationToken cancellationToken)
        {
            AppendCheck(status);

            if (_checks.Count(x => !x.IsOk) >= _settings.MinNokToDegraded)
            {
                _handler = HandleDegraded;
                return Task.FromResult(_settings.RetryTimeout);
            }

            return Task.FromResult(
                status.IsOk 
                ? _settings.HeartbeatTimeout
                : _settings.RetryTimeout
                    );
        }

        private async Task<TimeSpan> HandleDegraded(ProbeStatus status, CancellationToken cancellationToken)
        {
            AppendCheck(status);

            if (AllChecksOk())
            {
                _handler = HandleOk;
                await SendOkEmail(cancellationToken);
                _logger.LogWarning("Apixu service was restored");
                return _settings.HeartbeatTimeout;
            }

            if (AllChecksFail())
            {
                _handler = HandleUnavailable;
                await SendFailEmail(cancellationToken);
                _logger.LogWarning("Apixu service became unavailable");
                return _settings.RetryTimeout;
            }

            return _settings.RetryTimeout;
        }

        private Task SendFailEmail(CancellationToken cancellationToken)
            => SendEmail(
                $"Apixu service unavailable ({_settings.Identifier})", 
                $"The service is unavailable for instance {_settings.Identifier}.", 
                cancellationToken
                );

        private Task SendOkEmail(CancellationToken cancellationToken)
            =>  SendEmail(
                $"Apixu service restored ({_settings.Identifier})",
                $"The service was restored for instance {_settings.Identifier}.", 
                cancellationToken
                );

        private async Task SendEmail(string subject, string message, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(_settings.RecipientEmail))
            {
                _logger.LogWarning("No service report email recipient configured");
                return;
            }

            var email = new Email(Email.DefaultRenderer, _sender, "monitor@cpm.agrecruitment.eu", "CPM4 Monitor");

            var response = await email
                .To(_settings.RecipientEmail, _settings.RecipientName)
                .Subject(subject)
                .Body(message, isHtml: false)
                .SendAsync(cancellationToken);

            if (!response.Successful)
            {
                _logger.LogWarning("Could not send the email: {0}", string.Join(Environment.NewLine, response.ErrorMessages));
            }
        }

        private Task<TimeSpan> HandleUnavailable(ProbeStatus status, CancellationToken cancellationToken)
        {
            AppendCheck(status);

            if (_checks.Count(x => x.IsOk) >= _settings.MinOkToDegraded)
            {
                _handler = HandleDegraded;
            }

            return Task.FromResult(_settings.RetryTimeout);
        }

        private void ClearChecks()
        {
            _checks.Clear();
        }

        private void AppendCheck(ProbeStatus status)
        {
            _checks.Add(status);
            while (_checks.Count > _settings.CheckCount)
            {
                _checks.RemoveAt(0);
            }
            _logger.LogDebug("Checks: {0} ({1})", _checks.Count, string.Join("", _checks.Select(x => x.IsOk ? "+" : "-")));
        }

        private bool AllChecksOk()
        {
            return _checks.Count >= _settings.CheckCount && _checks.All(x => x.IsOk);
        }

        private bool AllChecksFail()
        {
            return _checks.Count >= _settings.CheckCount && _checks.All(x => !x.IsOk);
        }
    }
}