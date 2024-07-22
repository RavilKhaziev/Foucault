using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using CyberNadzor.Data;
using Microsoft.OpenApi.Models;
using System.Reflection;
using CyberNadzor.Seed;
using CyberNadzor.Extensions;
using Mapster;
using MapsterMapper;
using CyberNadzor.Mapper;

namespace CyberNadzor;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.AddServiceDefaults();

        // Add services to the container.
        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
        builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(connectionString));
        builder.Services.AddDatabaseDeveloperPageExceptionFilter();

        builder.Services.ConfigureApplicationCookie(options =>
        {
            // Cookie settings
            options.Cookie.HttpOnly = true;
            //options.Cookie.Expiration 

            options.ExpireTimeSpan = TimeSpan.FromMinutes(5);
            options.LoginPath = "/Identity/Account/Login";
            options.LogoutPath = "/Identity/Account/Logout";
            options.AccessDeniedPath = "/Identity/Account/AccessDenied";
            options.SlidingExpiration = true;
            //options.ReturnUrlParameter=""
        });


        builder.Services.AddDefaultIdentity<IdentityUser>(options => { 
            options.SignIn.RequireConfirmedAccount = true;
        })
            .AddEntityFrameworkStores<ApplicationDbContext>();
        builder.Services.AddControllersWithViews();

        builder.Services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Version = "v1",
                Title = "",
                Description = ""
            });
            var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            Console.WriteLine(Path.Combine(AppContext.BaseDirectory, xmlFilename));
            options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
        });
        builder.Services.AddSeedData();
        builder.Services.TryAddSurveyService();

        var app = builder.Build();

        app.MapDefaultEndpoints();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseMigrationsEndPoint();

            app.UseSwagger(options =>
            {
                options.RouteTemplate = $"api/CyberNadzor/{{documentName}}/swagger.json";
            });
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint($"/api/CyberNadzor/v1/swagger.json", "v1");
                options.RoutePrefix = string.Empty;
            });

            app.UseSeedData();
        }
        else
        {
            app.UseExceptionHandler("/Home/Error");
        }
        app.UseStaticFiles();

        app.UseRouting();

        app.UseAuthorization();

        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}");
        app.MapRazorPages();

        app.Run();
    }
}
