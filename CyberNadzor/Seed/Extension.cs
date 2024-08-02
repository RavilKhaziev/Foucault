using CyberNadzor.Data;
using CyberNadzor.Extensions;

namespace CyberNadzor.Seed
{
    public static class Extension
    {
        public static IServiceCollection AddSeedData(this IServiceCollection services)
        {
            services.TryAddRepositories();
            services.AddScoped<SurveySeed>();
            services.AddScoped<UserSeed>();
            services.AddScoped<AnswerSeed>();
            services.AddHostedService<SeedService>();
            return services;
        }

        public static IApplicationBuilder UseSeedData(this IApplicationBuilder app)
        {
            using (var scope = app.ApplicationServices.CreateScope())
            {
                var db = scope.ServiceProvider.GetService<ApplicationDbContext>();
                db.Database.EnsureCreated();
                
            }
            return app;
        }
    }
}
