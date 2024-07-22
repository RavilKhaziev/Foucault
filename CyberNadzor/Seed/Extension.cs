using CyberNadzor.Data;
using CyberNadzor.Extensions;

namespace CyberNadzor.Seed
{
    public static class Extension
    {
        public static IServiceCollection AddSeedData(this IServiceCollection services)
        {
            services.TryAddRepositories();
            services.AddTransient<SurveySeed>();
            services.AddTransient<UserSeed>();
            return services;
        }

        public static IApplicationBuilder UseSeedData(this IApplicationBuilder app)
        {
            using (var scope = app.ApplicationServices.CreateScope())
            {
                var userSeed = scope.ServiceProvider.GetService<UserSeed>();
                ArgumentNullException.ThrowIfNull(userSeed, nameof(userSeed));
                userSeed.SeedUsers().Wait();
                
            }
            using (var scope = app.ApplicationServices.CreateScope())
            {
                var syrveySeed = scope.ServiceProvider.GetService<SurveySeed>();
                ArgumentNullException.ThrowIfNull(syrveySeed, nameof(syrveySeed));
                syrveySeed.SeedSurvey().Wait();
            }
            return app;
        }
    }
}
