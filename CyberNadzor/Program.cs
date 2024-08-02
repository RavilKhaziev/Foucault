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
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authentication.JwtBearer;

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
            options.UseNpgsql(connectionString, options => options.EnableRetryOnFailure()));
        builder.Services.AddDatabaseDeveloperPageExceptionFilter();
        builder.Services.TryAddAuth();
        builder.Services.AddControllersWithViews();

        builder.Services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Version = "v1",
                Title = "",
                Description = ""
            });
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description = @"JWT Authorization header using the Bearer scheme. \r\n\r\n 
                      Enter 'Bearer' [space] and then your token in the text input below.
                      \r\n\r\nExample: 'Bearer 12345abcdef'",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http,
                Scheme = "Bearer"
            });
            options.AddSecurityRequirement(new OpenApiSecurityRequirement()
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                            {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                            },
                        Scheme = "oauth2",
                        Name = "Bearer",
                        In = ParameterLocation.Header,

                    },
                    new List<string>()
                }
            });
            var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            Console.WriteLine(Path.Combine(AppContext.BaseDirectory, xmlFilename));
            options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
        });
        builder.Services.TryAddStatisticService(builder.Configuration.GetRequiredSection("AISummarizationOptions"));
        builder.Services.AddSeedData();
        builder.Services.TryAddSurveyService();
        builder.Services.AddHttpClient();
        builder.Services.AddRazorPages();
        builder.Services.AddEndpointsApiExplorer();
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
        app.UseAuthentication();
        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}");
        app.MapRazorPages();

        
        app.Run();
    }
}
