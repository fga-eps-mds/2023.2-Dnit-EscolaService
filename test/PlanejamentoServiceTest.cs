using System.Linq;
using System.Threading.Tasks;
using api.Planejamento;
using app.Entidades;
using app.Repositorios.Interfaces;
using app.Services;
using Microsoft.EntityFrameworkCore;
using service.Interfaces;
using test.Fixtures;
using test.Stubs;
using Xunit.Abstractions;
using Xunit.Microsoft.DependencyInjection.Abstracts;

namespace test
{
    public class PlanejamentoServiceTest : TestBed<Base>, IDisposable
    {
        private readonly IPlanejamentoService planejamentoService;
        private readonly IPlanejamentoRepositorio planejamentoRepositorio;
        private readonly AppDbContext dbContext;
        
        public PlanejamentoServiceTest(ITestOutputHelper testOutputHelper, Base fixture) : base(testOutputHelper, fixture)
        {
            planejamentoService = fixture.GetService<IPlanejamentoService>(testOutputHelper)!;
            dbContext = fixture.GetService<AppDbContext>(testOutputHelper)!;
            planejamentoRepositorio = fixture.GetService<IPlanejamentoRepositorio>(testOutputHelper)!;
            dbContext.PopulaPlanejamentoMacro(5);
        }
        
        [Fact]
        public async Task ObterPlanejamentoMacroPorIdAsync_QuandoExistir_DeveRetornar()
        {
            dbContext.PopulaPlanejamentoMacro(1);
            var planejBanco = await dbContext.PlanejamentoMacro.FirstOrDefaultAsync();
            Assert.NotNull(planejBanco);
            
            var planejamentoMacro = await planejamentoRepositorio.ObterPlanejamentoMacroAsync(planejBanco.Id);
            Assert.Equal(planejBanco, planejamentoMacro);
            Assert.IsNotType<ApiException>(async () => await planejamentoRepositorio.ObterPlanejamentoMacroAsync(planejBanco.Id));
        }

        [Fact]
        public async Task ObterPlanejamentoMacroPorIdAsync_QuandoNaoExistir_DeveLancarExcecao()
        {
            await Assert.ThrowsAsync<ApiException>(() => planejamentoRepositorio.ObterPlanejamentoMacroAsync(Guid.NewGuid()));
        }
        
        [Fact]
        public async Task DeletePlanejamentoMacro_QuandoExistir_DeveDeletar()
        {
            dbContext.PopulaPlanejamentoMacro(1);
            var planejamento = await dbContext.PlanejamentoMacro.FirstOrDefaultAsync();
            Assert.NotNull(planejamento);

            planejamentoRepositorio.ExcluirPlanejamentoMacro(planejamento);
            await dbContext.SaveChangesAsync();

            Assert.False(await dbContext.PlanejamentoMacro.AnyAsync(e => e.Id == planejamento.Id));
            Assert.IsNotType<ApiException>(() => planejamentoRepositorio.ExcluirPlanejamentoMacro(planejamento));
        }

        [Fact]
        public async Task DeletePlanejamentoMacro_QuandoNaoExistir_DeveLancarExcecao()
        {
            await Assert.ThrowsAsync<ApiException>(async() => await planejamentoRepositorio.ObterPlanejamentoMacroAsync(Guid.NewGuid()));
        }

        [Fact]
        public async Task CriarPlanejamentoMacro_QuandoPossuiSolicitacao_RetornaPlanejamentoMacro(){
            dbContext.Clear();
            dbContext.PopulaEscolas(5);

            var cad = new PlanejamentoMacro
            {
                Nome = "Planejamento ",
                MesInicio = api.Mes.Janeiro,
                MesFim = api.Mes.Fevereiro,
                AnoInicio = "2023",
                AnoFim = "2023",
                Responsavel = "Robertinho",
                QuantidadeAcoes = 10,
            };

            planejamentoService.CriarPlanejamentoMacro(cad);

            var planejamento = dbContext.PlanejamentoMacro.First();

            Assert.NotNull(planejamento);
        }

        public new void Dispose()
        {
            dbContext.Clear();
        }
    }
}

