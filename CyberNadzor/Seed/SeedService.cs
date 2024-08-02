
using CyberNadzor.Data;

namespace CyberNadzor.Seed
{
    public class SeedService : BackgroundService
    {
        private readonly ILogger<SeedService> _logger;
        private readonly IServiceProvider _serviceProvider;
        public SeedService(
            ILogger<SeedService> logger,
           IServiceProvider services
            )
        {
            _logger = logger;
            _serviceProvider = services;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var answerSeed = scope.ServiceProvider.GetService<AnswerSeed>();
                var userSeed = scope.ServiceProvider.GetService<UserSeed>();
                var surveySeed = scope.ServiceProvider.GetService<SurveySeed>();
                await userSeed.SeedUsers();
                await surveySeed.SeedSurvey();
                await answerSeed.SeedAnswers(new() { "Анкета студента ФРГФ" });
            }
           
        }
    }
}
