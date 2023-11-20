using api.Escolas;
using app.Controllers;
using app.Entidades;
using app.Services;
using System.Threading.Tasks;
using test.Fixtures;
using test.Stubs;
using Xunit.Abstractions;

namespace test
{
    public class SuperintendenciaControllerTest : AuthTest, IDisposable
    {
        private readonly SuperintendenciaController controller;
        private readonly AppDbContext dbContext;

        public SuperintendenciaControllerTest(ITestOutputHelper testOutputHelper, Base fixture) : base(testOutputHelper, fixture)
        {
            dbContext = fixture.GetService<AppDbContext>(testOutputHelper)!;
            controller = fixture.GetService<SuperintendenciaController>(testOutputHelper)!;
            AutenticarUsuario(controller);
            dbContext.PopulaSuperintendencias(5);
        }

        [Fact]
        public async Task GetSuperItendencia_QuandoExistir_DeveRetornar()
        {
            var superintendencia = await controller.Obter(1);

            Assert.NotNull(superintendencia);
            Assert.Equal(1, superintendencia.Id);
            Assert.NotNull(superintendencia.Cep);
            Assert.NotNull(superintendencia.Longitude);
            Assert.NotNull(superintendencia.Latitude);
            Assert.NotNull(superintendencia.Endereco);
        }

        [Fact]
        public async Task GetSuperItendencia_QuandoNaoExistir_DeveLancarExcessao()
        {
            await Assert.ThrowsAsync<ApiException>(async () => await controller.Obter(0));
        }

        public new void Dispose()
        {
            dbContext.Clear();
        }
    }
}
