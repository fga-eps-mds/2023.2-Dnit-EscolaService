using System.Linq;
using api.Escolas;
using app.Controllers;
using app.Entidades;
using app.Services;
using System.Threading.Tasks;
using api.Polos;
using auth;
using Microsoft.EntityFrameworkCore;
using test.Fixtures;
using test.Stubs;
using Xunit.Abstractions;

namespace test
{
    public class PolosControllerTest : AuthTest, IDisposable
    {
        private readonly PolosController controller;
        private readonly AppDbContext dbContext;

        public PolosControllerTest(ITestOutputHelper testOutputHelper, Base fixture) : base(testOutputHelper, fixture)
        {
            dbContext = fixture.GetService<AppDbContext>(testOutputHelper)!;
            controller = fixture.GetService<PolosController>(testOutputHelper)!;
            AutenticarUsuario(controller);
            dbContext.PopulaPolos(5);
        }

        [Fact]
        public async Task GetPolo_QuandoExistir_DeveRetornar()
        {
            var polo = await controller.Obter(1);

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
        public async Task GetPolo_QuandoNaoExistir_DeveLancarExcessao()
        {
            await Assert.ThrowsAsync<ApiException>(async () => await controller.Obter(0));
        }
        
        [Fact]
        public async Task ObterPolosAsync_QuandoMetodoForChamado_DeveRetornarListaDePolos()
        {
            var polos = dbContext.Polos.ToList();
            var filtro = new PesquisaPoloFiltro {
                Pagina = 1,
                TamanhoPagina = polos.Count()
            };
            var result = await controller.ObterPolosAsync(filtro);

            Assert.Equal(polos.Count(), result.Total);
            Assert.Equal(filtro.Pagina, result.Pagina);
            Assert.Equal(filtro.TamanhoPagina, result.ItemsPorPagina);
            Assert.True(polos.All(e => result.Items.Exists(ee => ee.Id == e.Id)));
        }

        [Fact]
        public async Task ObterPolosAsync_QuandoNaoTiverPermissao_DeveSerBloqueado()
        {
            var polos = dbContext.Polos.ToList();
            var filtro = new PesquisaPoloFiltro()
            {
                Pagina = 1,
                TamanhoPagina = polos.Count(),
            };
            AutenticarUsuario(controller, permissoes: new() { });

            await Assert.ThrowsAsync<AuthForbiddenException>(async () => await controller.ObterPolosAsync(filtro));
        }

        [Fact]
        public async Task Excluir_QuandoIdPoloForPassado_DeveExcluirPolo()
        {
            var idPolo = dbContext.Polos.First().Id;
            await controller.ExcluirPolo(idPolo);

            Assert.False(dbContext.Polos.Any(e => e.Id == idPolo));
        }

        [Fact]
        public async Task Excluir_QuandoNaoTiverPermissao_DeveSerBloqueado()
        {
            var idPolo = dbContext.Polos.First().Id;

            AutenticarUsuario(controller, permissoes: new());

            await Assert.ThrowsAsync<AuthForbiddenException>(async () => await controller.ExcluirPolo(idPolo));
        }

         [Fact]
        public async Task CadastrarPolo_QuandoPoloForCadastrado_DeveRetornarHttpOk()
        {
            var polo = PoloStub.ListarPolosDto(dbContext.Municipios.ToList()).First();

            await controller.CriarPolo(polo);

            var poloDb = dbContext.Polos.First(p => p.Nome == polo.Nome);

            Assert.Equal(polo.Cep, poloDb.Cep);
            Assert.Equal(polo.Nome, poloDb.Nome);
            Assert.Equal(polo.Endereco, poloDb.Endereco);
            Assert.Equal(polo.IdUf, (int?)poloDb.Uf);
            Assert.Equal(polo.Latitude, poloDb.Latitude);
            Assert.Equal(polo.Longitude, poloDb.Longitude);
            Assert.Equal(polo.MunicipioId, poloDb.MunicipioId);
        }

        [Fact]
        public async Task CadastrarPolo_QuandoNaoTiverPermissao_DeverBloquear()
        {
            var polo = PoloStub.ListarPolosDto(dbContext.Municipios.ToList()).First();

            AutenticarUsuario(controller, permissoes: new());
            await Assert.ThrowsAsync<AuthForbiddenException>(async () => await controller.CriarPolo(polo));
        }

        [Fact]
        public async Task AlterarDadosPoloAsync_QuandoPoloForAlterado_DeveRetornarHttpOk()
        {
            var polo = PoloStub.ListarPolosDto(dbContext.Municipios.ToList()).First();

            await controller.CriarPolo(polo);

            polo.Nome =  dbContext.Polos.First().Nome;
            polo.Cep =  dbContext.Polos.First().Cep;
            polo.Endereco =  dbContext.Polos.First().Endereco;
            polo.MunicipioId =  dbContext.Polos.First().MunicipioId;
            polo.Latitude =  dbContext.Polos.First().Latitude;
            polo.Longitude =  dbContext.Polos.First().Longitude;
            polo.IdUf =  (int)dbContext.Polos.First().Uf;

            var poloDb = dbContext.Polos.First(p => p.Cep == polo.Cep);

            Assert.Equal(polo.Cep, poloDb.Cep);
            Assert.Equal(polo.Nome, poloDb.Nome);
            Assert.Equal(polo.Endereco, poloDb.Endereco);
            Assert.Equal(polo.IdUf, (int?)poloDb.Uf);
            Assert.Equal(polo.Latitude, poloDb.Latitude);
            Assert.Equal(polo.Longitude, poloDb.Longitude);
            Assert.Equal(polo.MunicipioId, poloDb.MunicipioId);
        }

        [Fact]
        public async Task AlterarDadosPoloAsync_QuandoNaoTiverPermissao_DeveBloquear()
        {
            var polo = PoloStub.ListarPolosDto(dbContext.Municipios.ToList()).First();

            AutenticarUsuario(controller, permissoes: new());

            await Assert.ThrowsAsync<AuthForbiddenException>(async () => await controller.AtualizarPolo(1, polo));
        }

        public new void Dispose()
        {
            dbContext.Clear();
        }
    }
}
