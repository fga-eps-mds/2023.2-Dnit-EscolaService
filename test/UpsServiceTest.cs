using api;
using app.Entidades;
using app.Services;
using Microsoft.Extensions.Options;
using RichardSzalay.MockHttp;
using System.Threading.Tasks;
using test.Fixtures;
using test.Stubs;
using Xunit.Abstractions;
using Xunit.Microsoft.DependencyInjection.Abstracts;

namespace test
{
    public class UpsServiceTest : TestBed<Base>
    {
        private readonly AppDbContext db;
        private readonly UpsService service;
        private readonly MockHttpMessageHandler handler;
        private readonly UpsServiceConfig config = new() { Host = "http://localhost/" };

        public UpsServiceTest(ITestOutputHelper testOutputHelper, Base fixture) : base(testOutputHelper, fixture)
        {
            db = fixture.GetService<AppDbContext>(testOutputHelper)!;
            handler = new();
            service = new UpsService(
                handler.ToHttpClient(),
                Options.Create(config));
        }

        [Fact]
        public async Task CalcularUpsEscolasAsync_QuandoTudoNormal_RetornaListaDeUps()
        {
            var escolas = db.PopulaEscolas(5);
            var ano = DateTime.Now.AddYears(-2).Year;
            handler
                .When(config.Host + "api/calcular/ups/escolas*")
                .Respond("application/json", "[1, 2, 3]");

            var upss = await service.CalcularUpsEscolasAsync(escolas, 2.0, ano, -1);

            Assert.Equal(1, upss[0]);
            Assert.Equal(2, upss[1]);
            Assert.Equal(3, upss[2]);
        }

        [Fact]
        public async Task CalcularUpsEscolasAsync_QuandoRespostaNaoEhListaDeInteiros_LancaExcecao()
        {
            var escolas = db.PopulaEscolas(5);
            var ano = DateTime.Now.AddYears(-2).Year;
            handler
                .When(config.Host + "api/calcular/ups/escolas*")
                .Respond("application/json", @"{""objeto"": ""nao array""}");

            var e = await Assert.ThrowsAsync<ApiException>(async () => 
                await service.CalcularUpsEscolasAsync(escolas, 2.0, ano, -1));
            
            Assert.IsType<ApiException>(e);
            Assert.Equal(ErrorCodes.FormatoJsonNaoReconhecido, e.Error.Code);
        }
    }
}