using System.Linq;
using app.Controllers;
using app.Entidades;
using app.Services;
using System.Threading.Tasks;
using app.Repositorios.Interfaces;
using auth;
using Microsoft.EntityFrameworkCore;
using test.Fixtures;
using test.Stubs;
using Xunit.Abstractions;
using api.Planejamento;
using Microsoft.AspNetCore.Authorization.Infrastructure;

namespace test
{
    public class PlanejamentoRepositorioTest: AuthTest, IDisposable
    {
        private readonly IPlanejamentoRepositorio planejamentoRepositorio;
        private readonly AppDbContext dbContext;
    
        public PlanejamentoRepositorioTest(ITestOutputHelper testOutputHelper, Base fixture) : base(testOutputHelper, fixture)
        {
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
            var planejBanco = await dbContext.PlanejamentoMacro.FirstOrDefaultAsync();
            Assert.NotNull(planejBanco);
    
            planejamentoRepositorio.ExcluirPlanejamentoMacro(planejBanco);
            dbContext.SaveChangesAsync();

            Assert.False(await dbContext.PlanejamentoMacro.AnyAsync(e => e.Id == planejBanco.Id));
            Assert.IsNotType<ApiException>(() => planejamentoRepositorio.ExcluirPlanejamentoMacro(planejBanco));
        }
        
        [Fact]
        public async Task DeletePlanejamentoMacro_QuandoNaoExistir_DeveLancarExcecao()
        {
            await Assert.ThrowsAsync<ApiException>(async() => await planejamentoRepositorio.ObterPlanejamentoMacroAsync(Guid.NewGuid()));
        }

        [Fact]
        public async void ListarPaginadaAsync_QuandoFiltrosForemNulos_DeveRetornarListaDePlanejamentosPaginados()
        {
            var planDb = dbContext.PopulaPlanejamentoMacro(5);

            var filtro = new PesquisaPlanejamentoFiltro();
            filtro.Pagina = 1;
            filtro.TamanhoPagina = 2;

            var listaPaginada = await planejamentoRepositorio.ListarPaginadaAsync(filtro);

            Assert.Equal(filtro.Pagina, listaPaginada.Pagina);
            Assert.Equal(filtro.TamanhoPagina, listaPaginada.ItemsPorPagina);
            Assert.Equal(planDb.Count, listaPaginada.Total);
        }
        public new void Dispose()
        {
            dbContext.Clear();
        }
    }
}