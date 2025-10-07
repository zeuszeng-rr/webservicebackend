using Hangfire;
using Hangfire.PostgreSql;
using Hangfire.SqlServer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using WebServiceBackend.Application;
using WebServiceBackend.Core;
using WebServiceBackend.Core.Options;
using WebServiceBackend.Core.Options.WebServiceBackend.Core.Options;
using WebServiceBackend.Infrastructure.Interceptors;

namespace WebServiceBackend.Api.Extensions
{
    public static class StartupExtension
    {
        public static IServiceCollection AddAppServices(this IServiceCollection service, IConfiguration configuration)
        {
            service.Configure<CorsOptions>(configuration.GetSection(CorsOptions.SectionName));
            service.Configure<ApiBehaviorOptions>(options => { 
                options.SuppressModelStateInvalidFilter = true;
            });

            var corsOption = configuration
                    .GetSection(CorsOptions.SectionName)
                    .Get<CorsOptions>() ?? new CorsOptions();

            #region Cors
            service.AddCors(option => {
                option.AddPolicy("AllowViewClient", policy => {

                    if (corsOption.AllowedOrigins?.Length > 0)
                    {
                        policy.WithOrigins(corsOption.AllowedOrigins)
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials();
                    }
                    else
                    {
                        policy.AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials();
                    }
                });
                
                
            });
            #endregion

            #region Controller & Swagger
            service.AddControllers();
            service.AddEndpointsApiExplorer();
            service.AddSwaggerGen(config => {
                config.SwaggerDoc("v1", new OpenApiInfo { Title = "Web Service BackEnd API", Version = "v1" });

                config.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme { 
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme  = "Bearer",
                    In = ParameterLocation.Header,
                    Description = "ใส่ JWT token แบบนี้ : Bearer {token}",
                });
                config.AddSecurityRequirement(new OpenApiSecurityRequirement {
                    {
                        new OpenApiSecurityScheme 
                        {
                            Reference = new OpenApiReference {
                            Type = ReferenceType.SecurityScheme,
                            Id  = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });

            });

            #endregion

            #region DI & Interceptors
            service.AddCoreDI(configuration);
            service.AddApplicationDI();
            service.AddSingleton<AuditTrailInterceptor>();
            #endregion

            #region JWT Authentication
            var jwtOptions = configuration
                .GetSection(JwtConfigurationOptions.SectionName)
                .Get<JwtConfigurationOptions>();

            if (string.IsNullOrWhiteSpace(jwtOptions?.SecretKey))
            {
                throw new Exception("JWT SecretKey is not configured properly in appsettings.json");
            }

            var key = Encoding.ASCII.GetBytes(jwtOptions.SecretKey);

            service.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = "JwtBearer";
                options.DefaultChallengeScheme = "JwtBearer";
            })
            .AddJwtBearer("JwtBearer", options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateIssuerSigningKey = true,
                    ValidateLifetime = true,
                    ValidIssuer = jwtOptions.Issuer,
                    ValidAudience = jwtOptions.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ClockSkew = TimeSpan.Zero
                };
            });
            #endregion

            #region Hangfire Background Jobs
            var hangfireOptions = configuration.GetSection(HangfireOptions.SectionName)
                                              .Get<HangfireOptions>() ?? new HangfireOptions();

            var connectionOptions = configuration.GetSection(ConnectionDatabaseOptions.SectionName)
                                                 .Get<ConnectionDatabaseOptions>() ?? new ConnectionDatabaseOptions();

            if (hangfireOptions.EnableHangfire)
            {
                service.AddHangfire(config =>
                {
                    config.SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                          .UseSimpleAssemblyNameTypeSerializer()
                          .UseRecommendedSerializerSettings();

                    var db = hangfireOptions.Database?.ToLowerInvariant();

                    if (db == "sql server")
                    {
                        config.UseSqlServerStorage(connectionOptions.HangfireConnection, new SqlServerStorageOptions
                        {
                            CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
                            SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
                            QueuePollInterval = TimeSpan.Zero,
                            UseRecommendedIsolationLevel = true,
                            DisableGlobalLocks = true
                        });
                    }
                    else if (db == "postgres" || db == "postgresql")
                    {
                        config.UsePostgreSqlStorage(connectionOptions.HangfireConnection);
                    }
                    else
                    {
                        throw new Exception($"Unsupported Hangfire database provider: {hangfireOptions.Database}");
                    }
                });

                service.AddHangfireServer();
            }

            #endregion
            return service;
        }
    }
}
