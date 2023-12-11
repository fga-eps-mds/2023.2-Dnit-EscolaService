using api;
using app.Controllers;
using app.Entidades;
using System.Linq;
using System.Threading.Tasks;
using test.Fixtures;
using test.Stubs;
using Xunit.Abstractions;
using Xunit.Microsoft.DependencyInjection.Abstracts;

namespace test
{
    public class DominioControllerTest : TestBed<Base>, IDisposable
    {
        DominioController dominioController;
        AppDbContext dbContext;

        public DominioControllerTest(ITestOutputHelper testOutputHelper, Base fixture) : base(testOutputHelper, fixture)
        {
            dbContext = fixture.GetService<AppDbContext>(testOutputHelper)!;
            dominioController = fixture.GetService<DominioController>(testOutputHelper)!;
        }

        [Fact]
        public void ObterListaUF_QuandoMetodoForChamado_DeveRetornarTodosUfs()
        {
            var ufs = dominioController.ObterListaUF().ToList();
            Assert.True(Enum.GetValues<UF>().ToList().All(uf => ufs.Any(u => u.Id == (int)uf)));
        }

        [Fact]
        public void ObterListaEtapasdeEnsino_QuandoMetodoForChamado_DeveTodasAsEtapas()
        {
            var etapas = dominioController.ObterListaEtapasdeEnsino();

            Assert.True(Enum.GetValues<EtapaEnsino>().ToList().All(etapa => etapas.Any(e => e.Id == (int)etapa)));
        }

        [Fact]
        public void ObterListaSituacao_QuandoMetodoForChamado_DeveTodasSituacoes()
        {
            var situacoes = dominioController.ObterListaSituacao();
            Assert.True(Enum.GetValues<Situacao>().ToList().All(situacao => situacoes.Any(s => s.Id == (int)situacao)));
        }

        [Fact]
        public async Task ObterListaMunicipio_QuandoMetodoForChamado_DeveRetornarHttpOk()
        {
            var municipiosDb = dbContext.PopulaMunicipios(10)!;
            var uf = municipiosDb.First().Uf;
            var municipioUf = municipiosDb.Where(m => m.Uf == uf).ToList();
            var municipios = (await dominioController.ObterListaMunicipio((int)uf)).ToList();

            Assert.NotEmpty(municipios);
            Assert.True(municipioUf.All(mdb => municipios.Any(m => m.Id == mdb.Id)));

        }

        [Fact]
        public void ObterPropriedades_QuandoMetodoForChamado_DeveRetornarTodasPropriedades()
        {
            var propriedades = dominioController.ObterPropriedades().ToList();
            Assert.True(Enum.GetValues<PropriedadeCondicao>().ToList().All(propriedade => propriedades.Any(p => p.Id == (int)propriedade)));
        }

        [Fact]
        public void ObterPorte_QuandoMetodoForChamado_DeveRetornarTodosOsPortes()
        {
            var portes = dominioController.ObterPorteEscola().ToList();
            Assert.True(Enum.GetValues<Porte>().ToList().All(porte => portes.Any(p => p.Id == porte)));
        }
        public new void Dispose()
        {
            dbContext.Clear();
        }
    }
}