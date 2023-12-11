using app.Controllers;
using app.Entidades;
using app.Repositorios;
using app.Repositorios.Interfaces;
using app.Services;
using app.Services.Interfaces;
using auth;
using Hangfire;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using service.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit.Microsoft.DependencyInjection;
using Xunit.Microsoft.DependencyInjection.Abstracts;

namespace test.Fixtures
{
    public class Base : TestBedFixture
    {
        protected override void AddServices(IServiceCollection services, IConfiguration? configuration)
        {
            // Para evitar a colisão durante a testagem paralela, o nome deve ser diferente para cada classe de teste
            var databaseName = "DbInMemory" + Random.Shared.Next().ToString();
            services.AddDbContext<AppDbContext>(o => o.UseInMemoryDatabase(databaseName));

            // Repositorios
            services.AddScoped<IEscolaRepositorio, EscolaRepositorio>();
            services.AddScoped<IMunicipioRepositorio, MunicipioRepositorio>();
            services.AddScoped<IRanqueRepositorio, RanqueRepositorio>();
            services.AddScoped<IPriorizacaoRepositorio, PriorizacaoRepositorio>();
            services.AddScoped<IPlanejamentoRepositorio, PlanejamentoRepositorio>();
            services.AddScoped<IPoloRepositorio, PoloRepositorio>();
            services.AddScoped<ISolicitacaoAcaoRepositorio, SolicitacaoAcaoRepositorio>();

            // Services
            services.AddScoped<IEscolaService, EscolaService>();
            services.AddScoped<IMunicipioService, MunicipioService>();
            services.AddScoped<ISolicitacaoAcaoService, SolicitacaoAcaoService>();
            services.AddScoped<IRanqueService, RanqueService>();
            services.AddScoped<IUpsService, UpsServiceMock>();
            services.AddScoped<IBackgroundJobClient, BackgroundJobClientFake>();
            services.AddSingleton<ModelConverter>();
            services.AddScoped<IPriorizacaoService, PriorizacaoService>();
            services.AddScoped<IPlanejamentoService, PlanejamentoService>();
            services.AddScoped<IPoloService, PoloService>();
            services.AddScoped<ICalcularRanqueJob, CalcularRanqueJob>();

            // Controllers
            services.AddScoped<DominioController>();
            services.AddScoped<EscolaController>();
            services.AddScoped<RanqueController>();
            services.AddScoped<PriorizacaoController>();
            services.AddScoped<PlanejamentoController>();
            services.AddScoped<PolosController>();

            services.AddAuth(configuration);
        }

        protected override ValueTask DisposeAsyncCore() => new();

        protected override IEnumerable<TestAppSettings> GetTestAppSettings()
        {
            yield return new() { Filename = "appsettings.Test.json", IsOptional = false };
        }
    }
}
