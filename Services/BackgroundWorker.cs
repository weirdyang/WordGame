
using wordgame.Contracts;

namespace wordgame.Services
{
    internal class BackgroundWorker : BackgroundService
    {
        private readonly ILogger<BackgroundWorker> _logger;
        private readonly IServiceProvider _serviceProvider;

        public BackgroundWorker(
            ILogger<BackgroundWorker> logger,
            IServiceProvider services)
        {
            _logger = logger;
            _serviceProvider = services;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            throw new NotImplementedException();
        }

        private async Task DoWork(CancellationToken stoppingToken)
        {
            _logger.LogInformation(
                "Consume Scoped Service Hosted Service is working.");

            using (var scope = _serviceProvider.CreateScope())
            {
                var wordReader =
                    scope.ServiceProvider
                        .GetRequiredService<IWordReader>();

                wordReader.RefreshWord();
            }
            await Task.CompletedTask;
        }
    }
}
