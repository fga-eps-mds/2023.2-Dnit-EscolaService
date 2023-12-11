using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using api.Polos;
using app.Entidades;
using app.Repositorios.Interfaces;
using app.Services;
using app.Services.Interfaces;
using test.Fixtures;
using test.Stubs;
using Xunit.Abstractions;
using Xunit.Microsoft.DependencyInjection.Abstracts;

namespace test;

public class PoloRepositorioTest: TestBed<Base>, IDisposable
{
    private readonly IPoloRepositorio poloRepositorio;
    private readonly AppDbContext dbContext;
    
    public PoloRepositorioTest(ITestOutputHelper testOutputHelper, Base fixture) : base(testOutputHelper, fixture)
    {
        dbContext = fixture.GetService<AppDbContext>(testOutputHelper)!;
        poloRepositorio = fixture.GetService<IPoloRepositorio>(testOutputHelper)!;
    }
    
    [Fact]
    public async Task ObterPorIdAsync_QuandoExistir_DeveRetornar()
    {
        dbContext.PopulaPolos(5);
        var polo = await poloRepositorio.ObterPorIdAsync(1);
        
        Assert.NotNull(polo);
        Assert.Equal(1, polo.Id);
        Assert.NotNull(polo.Nome);
        Assert.NotNull(polo.Municipio);
        Assert.NotNull(polo.Cep);
        Assert.NotNull(polo.Longitude);
        Assert.NotNull(polo.Latitude);
        Assert.NotNull(polo.Endereco);
    }

    [Fact]
    public async Task ObterPorIdAsync_QuandoNaoExistir_DeveLancarExcecao()
    {
        await Assert.ThrowsAsync<ApiException>(() => poloRepositorio.ObterPorIdAsync(-9999));
    }

    
    [Fact]
    public async Task ListarAsync_QuandoVazio_DeveRetornarListaVazia()
    {
        var polos = await poloRepositorio.ListarAsync();
        Assert.Empty(polos);
    }

    
    [Fact]
    public async Task ListarAsync_QuandoPreenchido_DeveRetornarListaCompleta()
    {
        var polosDb = dbContext.PopulaPolos(5)!;
        var polos = await poloRepositorio.ListarAsync(null);
        Assert.Equal(polosDb.Count, polos.Count);
        Assert.True(polosDb.All(mdb => polos.Exists(m => m.Id == mdb.Id)));
    }

    [Fact]
    public async Task ListarAsync_QuandoFiltro_DeveRetornarListaFiltrada()
    {
        var polosDb = dbContext.PopulaPolos(5);
        var filtroUF = polosDb.First().Uf;
        var polos = await poloRepositorio.ListarAsync(p => p.Uf == filtroUF);
        Assert.Equal(polosDb.Where(p => p.Uf == filtroUF).Count(), polos.Count);
    }

    [Fact]
    public async Task ListarAsync_QuandoNaoExistir_DeveRetornarListaVazia()
    {
        var polosDb = dbContext.PopulaPolos(5);
        Expression<Func<Polo, bool>> filtroUf =  p => p.Id == -99;

        var Polos = await poloRepositorio.ListarAsync(filtroUf);
        Assert.Empty(Polos);
    }

    [Fact]
    public async Task ListarPaginadaAsync_QuandoFiltroForPassado_DeveRetornarListaDePolosFiltradas()
    {
        var polosDb = dbContext.PopulaPolos(5);

        var poloPesquisa = polosDb.First();

        var filtro = new PesquisaPoloFiltro()
        {
            Pagina = 1,
            TamanhoPagina = 2,
            Nome = poloPesquisa.Nome,
            IdUf = (int?)poloPesquisa.Uf,
            IdMunicipio = poloPesquisa.MunicipioId,
            Cep = poloPesquisa.Cep,
        };

        var listaPaginada = await poloRepositorio.ListarPaginadaAsync(filtro);

        Assert.Contains(poloPesquisa, listaPaginada.Items);
    }
    
    [Fact]
    public async Task ListarPaginadaAsync_QuandoMetodoForChamado_DeveRetornarListaDePolos()
    {
        var polos = dbContext.Polos.ToList();
        var filtro = new PesquisaPoloFiltro {
            Pagina = 1,
            TamanhoPagina = polos.Count()
        };
        var result = await poloRepositorio.ListarPaginadaAsync(filtro);

        Assert.Equal(polos.Count(), result.Total);
        Assert.Equal(filtro.Pagina, result.Pagina);
        Assert.Equal(filtro.TamanhoPagina, result.ItemsPorPagina);
        Assert.True(polos.All(e => result.Items.Exists(ee => ee.Id == e.Id)));
    }
    
    [Fact]
    public async Task ListarPaginadaAsync_QuandoFiltroNaoExistir_DeveRetornarListaVazia()
    {
        var polos = dbContext.Polos.ToList();
        var filtro = new PesquisaPoloFiltro {
            Pagina = 999999,
            TamanhoPagina = 9999999
        };
        var result = await poloRepositorio.ListarPaginadaAsync(filtro);
        
        Assert.Empty(result.Items);
    }
    public new void Dispose()
    {
        dbContext.Clear();
    }
}
