using CyberNadzor.Repositories;
using CyberNadzor.Repositories.Interfaces;
using CyberNadzor.Seed;
using CyberNadzor.Services;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace CyberNadzor.Extensions
{
    public static class Extension
    {
        public static IServiceCollection TryAddSurveyService(this IServiceCollection services)
        {
            services.TryAddRepositories();
            services.TryAddTransient<SurveyService>();
            return services;
        }

        public static IServiceCollection TryAddRepositories(this IServiceCollection services)
        {
            services.TryAddTransient<ISurveyRepository, SurveyRepository>();
            return services;
        }

    }
}
