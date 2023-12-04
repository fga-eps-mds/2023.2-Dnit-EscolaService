using api;
using api.Escolas;
using app.Controllers;
using app.Services;
using auth;
using Microsoft.AspNetCore.Mvc;
using Moq;
using service.Interfaces;
using System.Collections.Generic;
using System.Net.Mail;
using System.Threading.Tasks;
using test.Fixtures;
using test.Stubs;
using Xunit.Abstractions;

namespace test
{
    public class SolicitacaoAcaoControllerTest : AuthTest
    {
        const int INTERNAL_SERVER_ERROR = 500;
        private readonly SolicitacaoAcaoController controller;
        private readonly Mock<ISolicitacaoAcaoService> solicitacaoAcaoServiceMock;

        public SolicitacaoAcaoControllerTest(ITestOutputHelper testOutputHelper, Base fixture) : base(testOutputHelper, fixture)
        {
            solicitacaoAcaoServiceMock = new Mock<ISolicitacaoAcaoService>();
            var authService = fixture.GetService<AuthService>(testOutputHelper)!;
            controller = new SolicitacaoAcaoController(authService, solicitacaoAcaoServiceMock.Object);
        }

        [Fact]
        public async Task EnviarSolicitacaoAcao_QuandoSolicitacaoForEnviada_DeveRetornarOk()
        {
            SolicitacaoAcaoStub solicitacaoAcaoStub = new();
            var solicitacaoAcaoDTO = solicitacaoAcaoStub.ObterSolicitacaoAcaoDTO();

            var result = await controller.EnviarSolicitacaoAcao(solicitacaoAcaoDTO);

            solicitacaoAcaoServiceMock.Verify(service => service.EnviarSolicitacaoAcao(solicitacaoAcaoDTO), Times.Once);
            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public async Task EnviarSolicitacaoAcao_QuandoEnvioDoEmailFalhar_DeveRetornarErro()
        {
            SolicitacaoAcaoStub solicitacaoAcaoStub = new();
            var solicitacaoAcaoDTO = solicitacaoAcaoStub.ObterSolicitacaoAcaoDTO();

            solicitacaoAcaoServiceMock.Setup(x => x.EnviarSolicitacaoAcao(solicitacaoAcaoDTO)).Throws<SmtpException>();

            var result = await controller.EnviarSolicitacaoAcao(solicitacaoAcaoDTO);

            solicitacaoAcaoServiceMock.Verify(service => service.EnviarSolicitacaoAcao(solicitacaoAcaoDTO), Times.Once);
            var objeto = Assert.IsType<ObjectResult>(result);
            Assert.Equal(INTERNAL_SERVER_ERROR, objeto.StatusCode);
        }

        [Fact]
        public async Task ObterEscolas_QuandoEscolasForemObtidas_DeveRetornarListaEscolas()
        {
            List<EscolaInep> listaEscolas = new()
            {
                new EscolaInep { Cod = 1, Estado = "SP", Nome = "Escola A" },
                new EscolaInep { Cod = 2, Estado = "SP", Nome = "Escola B" },
                new EscolaInep { Cod = 3, Estado = "SP", Nome = "Escola C" }
            };

            var task = Task.FromResult<IEnumerable<EscolaInep>>(listaEscolas);

            solicitacaoAcaoServiceMock.Setup(service => service.ObterEscolas(It.IsAny<int>())).Returns(task);

            int municipio = 1;
            var resultado = await controller.ObterEscolas(municipio);

            solicitacaoAcaoServiceMock.Verify(service => service.ObterEscolas(municipio), Times.Once);

            Assert.NotNull(resultado);
            Assert.IsAssignableFrom<IEnumerable<EscolaInep>>(resultado);
            Assert.Equal(listaEscolas, resultado);
        }

        [Fact]
        public async Task ObterSolicitacoesAsync_QuandoNaoTemPermissao_LancaExcessao()
        {
            AutenticarUsuario(controller, permissoes: new());
            await Assert.ThrowsAsync<AuthForbiddenException>(async () =>
                await controller.ObterSolicitacoesAsync(new PesquisaSolicitacaoFiltro()));
        }

        [Fact]
        public async Task ObterSolicitacoesAsync_QuandoTemPermissao_ServicoDeveSerChamadoUmaVez()
        {
            AutenticarUsuario(controller, permissoes: new() { Permissao.SolicitacaoVisualizar });

            await controller.ObterSolicitacoesAsync(new PesquisaSolicitacaoFiltro());

            solicitacaoAcaoServiceMock.Verify(service => service.ObterSolicitacoesAsync(It.IsAny<PesquisaSolicitacaoFiltro>()), Times.Once);
        }
    }
}
