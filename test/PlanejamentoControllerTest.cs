using app.Controllers;
using app.Entidades;
using app.Services;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using test.Fixtures;
using test.Stubs;
using Xunit.Abstractions;
using Moq;
using service.Interfaces;
using System.Linq;
using api.Planejamento;
using auth;
using System.Collections.Generic;
using MathNet.Numerics;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using System.Security;

namespace test
{
    public class PlanejamentoControllerTest : AuthTest, IDisposable
    {
        private readonly PlanejamentoController controller;
        private readonly AppDbContext dbContext;
        private readonly Mock<IPlanejamentoService> planejamentoServiceMock;
        public PlanejamentoControllerTest(ITestOutputHelper testOutputHelper, Base fixture) : base(testOutputHelper, fixture)
        {
            planejamentoServiceMock = new Mock<IPlanejamentoService>();
            dbContext = fixture.GetService<AppDbContext>(testOutputHelper)!;
            controller = fixture.GetService<PlanejamentoController>(testOutputHelper)!;
            AutenticarUsuario(controller);
            dbContext.PopulaPlanejamentoMacro(5);        
        }
        
        [Fact]
        public async Task ObterPlanejamentoMacro_QuandoExistir_DeveRetornar()
        {
            dbContext.PopulaPlanejamentoMacro(1);
            var planejBanco = await dbContext.PlanejamentoMacro.FirstOrDefaultAsync();
            Assert.NotNull(planejBanco);
            
            var planejamentoMacro = await controller.ObterPlanejamentoMacro(planejBanco.Id);
            Assert.NotNull(planejamentoMacro);
            Assert.IsNotType<ApiException>(async () => await controller.ObterPlanejamentoMacro(planejBanco.Id));
        }
        
        [Fact]
        public async Task ObterPlanejamentoMacro_QuandoNaoExistir_DeveLancarExcessao()
        {
            dbContext.PopulaPlanejamentoMacro(1);
            await Assert.ThrowsAsync<ApiException>(async () => await controller.ObterPlanejamentoMacro(Guid.NewGuid()));
        }
        
        [Fact]
        public async Task DeletePlanejamentoMacro_QuandoExistir_DeveDeletar()
        {
            dbContext.PopulaPlanejamentoMacro(1);
            var planejBanco = await dbContext.PlanejamentoMacro.FirstOrDefaultAsync();
            Assert.NotNull(planejBanco);
            
            await controller.ExcluirPlanejamentoMacro(planejBanco.Id);
            Assert.False(await dbContext.PlanejamentoMacro.AnyAsync(e => e.Id == planejBanco.Id));
            Assert.IsNotType<ApiException>(async () => await controller.ExcluirPlanejamentoMacro(planejBanco.Id));
        }
        
        [Fact]
        public async Task DeletePlanejamentoMacro_QuandoNaoExistir_DeveLancarExcecao()
        {
            dbContext.PopulaPlanejamentoMacro(1);
            await Assert.ThrowsAsync<ApiException>(async () => await controller.ExcluirPlanejamentoMacro(Guid.NewGuid()));
        }
        
        [Fact]
        public async Task CriarPlanejamentoMacro_QuandoSolicitacaoForEnviada_DeveRetornarOk()
        {
            PlanejamentoMacroStub stub = new();
            var criaPlanejamentoDTO = stub.CriarPlanejamentoMacroDTO();

            var recomendacao = planejamentoServiceMock.Setup(x => x.GerarRecomendacaoDePlanejamento(criaPlanejamentoDTO));

           Assert.NotNull(recomendacao);
        }

        [Fact]
        public async Task CriarPlanejamentoMacro_QuandoNaoTiverPermissao_DeveSerBloqueado(){
            PlanejamentoMacroStub stub = new();
            var criaPlanejamentoDTO = stub.CriarPlanejamentoMacroDTO();

            AutenticarUsuario(controller, permissoes: new() {});

            await Assert.ThrowsAsync<AuthForbiddenException>(async() => await controller.CriarPlanejamentoMacro(criaPlanejamentoDTO));
        }

        [Fact]
        public async Task ObterPlanejamentosAsync_QuandoMetodoForChamado_DeveRetornarListaDePlanejamentos()
        {
            var planejamentoDb = dbContext.PlanejamentoMacro.ToList();

            var filtro = new PesquisaPlanejamentoFiltro();
            filtro.Pagina = 1;
            filtro.TamanhoPagina = planejamentoDb.Count();

            var resultado = await controller.ObterPlanejamentosAsync(filtro);

            Assert.Equal(filtro.Pagina, resultado.Pagina);
            Assert.Equal(filtro.TamanhoPagina, resultado.ItemsPorPagina);
            Assert.True(planejamentoDb.All(e => resultado.Items.Exists(ee => ee.Id == e.Id)));
        }

        [Fact]
        public async Task ObterPlanejamentosAsync_QuandoNaoTiverPermissao_DeveSerBloqueado(){
            var planejamentoDb = dbContext.PlanejamentoMacro.ToList();

            var filtro = new PesquisaPlanejamentoFiltro();
            filtro.Pagina = 1;
            filtro.TamanhoPagina = planejamentoDb.Count();

            AutenticarUsuario(controller, permissoes: new() { });
            
            await Assert.ThrowsAsync<AuthForbiddenException>(async() => await controller.ObterPlanejamentosAsync(filtro));
        }

        [Fact]
        public async Task EditarPlanejamentoAsync_QuandoNaoTiverPermissao_DeveSerBloqueado(){

            AutenticarUsuario(controller, permissoes: new() {});
            await Assert.ThrowsAsync<AuthForbiddenException>(async() => await controller.EditarPlanejamentoMacro(Guid.NewGuid(), new PlanejamentoMacroDetalhadoDTO()));
        }

        public new void Dispose()
        {
            dbContext.Clear();
        }
    }
}