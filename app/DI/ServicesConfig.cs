using auth;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using app.Entidades;
using app.Services;
using Microsoft.EntityFrameworkCore;
using service.Interfaces;
using System.Text;

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
            services.AddScoped<ISolicitacaoAcaoService, SolicitacaoAcaoService>();

            services.AddControllers(o => o.Filters.Add(typeof(HandleExceptionFilter)));

            services.AddHttpClient();
            services.AddAuth(configuration);
            
        }
    }
}
