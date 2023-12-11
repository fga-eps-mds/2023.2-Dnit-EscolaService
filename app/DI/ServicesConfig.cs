using auth;
using app.Entidades;
using app.Services;
using Microsoft.EntityFrameworkCore;
using service.Interfaces;
using app.Services.Interfaces;
using Hangfire;
using Hangfire.PostgreSql;
using app.Repositorios;
using app.Repositorios.Interfaces;

namespace app.DI
{
    public static class ServicesConfig
    {
        public static void AddConfigServices(this IServiceCollection services, IConfiguration configuration)
        {
            var mode = Environment.GetEnvironmentVariable("MODE");
            var connectionString = mode == "container" ? "PostgreSqlDocker" : "PostgreSql";

            services.AddDbContext<AppDbContext>(optionsBuilder => optionsBuilder.UseNpgsql(configuration.GetConnectionString(connectionString)));

            services.AddSingleton<ISmtpClientWrapper, SmtpClientWrapper>();
            services.AddSingleton<ModelConverter>();

            services.AddScoped<IEscolaService, EscolaService>();
            services.AddScoped<IMunicipioService, MunicipioService>();
            services.AddScoped<IPoloService, PoloService>();
            services.AddScoped<ISolicitacaoAcaoService, SolicitacaoAcaoService>();
            services.AddScoped<IRanqueService, RanqueService>();
            services.AddScoped<IBackgroundJobClient, BackgroundJobClient>();
            services.AddScoped<ICalcularUpsJob, CalcularUpsJob>();
            services.AddScoped<IUpsService, UpsService>();
            services.AddScoped<IPriorizacaoService, PriorizacaoService>();
            services.AddScoped<IPriorizacaoRepositorio, PriorizacaoRepositorio>();
            services.AddScoped<ICalcularRanqueJob, CalcularRanqueJob>();
            services.AddScoped<IPlanejamentoService, PlanejamentoService>();
            services.AddHttpClient<UpsService>();

            services.Configure<UpsServiceConfig>(configuration.GetSection("UpsServiceConfig"));
            services.Configure<CalcularUpsJobConfig>(configuration.GetSection("CalcularUpsJobConfig"));

            services.AddControllers(o => o.Filters.Add(typeof(HandleExceptionFilter)));

            services.AddHttpClient();
            services.AddAuth(configuration);

            var conexaoHangfire = mode == "container" ? "HangfireDocker" : "Hangfire";
            services.AddHangfire(config => config
                .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UsePostgreSqlStorage(c =>
                    c.UseNpgsqlConnection(configuration.GetConnectionString(conexaoHangfire)))
            );
            services.AddHangfireServer();
            services.AddMvc();
        }
    }
}
