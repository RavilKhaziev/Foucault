using CyberNadzor.Repositories;
using CyberNadzor.Repositories.Interfaces;
using CyberNadzor.Seed;
using CyberNadzor.Services;
using Microsoft.AspNetCore.Authentication;
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

        public class AISummarizationOptions
        {
            public string? Url { get; set; }
        }

        public static IServiceCollection TryAddStatisticService(this IServiceCollection services, IConfiguration config)
        {
            var ai = config.Get<AISummarizationOptions>();
            if (ai == null) throw new ArgumentNullException(nameof(ai));
            services.Configure<AISummarizationOptions>(x => {
               x.Url = ai.Url;
            });
            services.TryAddTransient<AIStatisticService>();
            return services;
        }
    }
}
