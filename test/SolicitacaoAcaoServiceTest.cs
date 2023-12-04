using Moq;
using service.Interfaces;
using System.Net;
using System.Net.Mail;
using Moq.Protected;
using Microsoft.Extensions.Configuration;
using System.Net.Http;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using test.Stubs;
using app.Services;
using app.Repositorios.Interfaces;
using app.Entidades;
using Xunit.Microsoft.DependencyInjection.Abstracts;
using test.Fixtures;
using Xunit.Abstractions;

namespace test
{
    public class SolicitacaoAcaoServiceTest : TestBed<Base>
    {
        private readonly Mock<ISmtpClientWrapper> smtpClientWrapperMock;
        private readonly Mock<IHttpClientFactory> httpClientFactoryMock;
        private readonly Mock<IConfiguration> configurationMock;
        private readonly Mock<ISolicitacaoAcaoRepositorio> solicitacaoAcaoRepositorioMock;
        private readonly Mock<IEscolaRepositorio> escolaRepositorioMock;
        private readonly AppDbContext db;
        private SolicitacaoAcaoService service;

        public SolicitacaoAcaoServiceTest(ITestOutputHelper testOutputHelper, Base fixture) : base(testOutputHelper, fixture)
        {
            Environment.SetEnvironmentVariable("EMAIL_SERVICE_ADDRESS", "teste_email@exemplo.com");
            Environment.SetEnvironmentVariable("EMAIL_SERVICE_PASSWORD", "teste");
            Environment.SetEnvironmentVariable("EMAIL_DNIT", "teste_email@exemplo.com");

            db = fixture.GetService<AppDbContext>(testOutputHelper)!;
            smtpClientWrapperMock = new();
            httpClientFactoryMock = new();
            configurationMock = new();
            solicitacaoAcaoRepositorioMock = new();
            escolaRepositorioMock = new();

            service = new SolicitacaoAcaoService(db, smtpClientWrapperMock.Object, httpClientFactoryMock.Object, configurationMock.Object, solicitacaoAcaoRepositorioMock.Object, escolaRepositorioMock.Object);
        }

        [Fact]
        public void EnviarSolicitacaoAcao_QuandoSolicitacaoForPassada_DeveEnviarMensagemEsperada()
        {
            var solicitacaoAcaoStub = new SolicitacaoAcaoStub();
            var solicitacaoAcaoDTO = solicitacaoAcaoStub.ObterSolicitacaoAcaoDTO();

            service.EnviarSolicitacaoAcao(solicitacaoAcaoDTO);

            var mensagemEsperada = "Nova solicitação de ação em escola.\n\n" +
                                        "Escola: Escola Teste\n" +
                                        "UF: DF\n" +
                                        "Municipio: Brasília\n" +
                                        "Nome do Solicitante: João Testador\n" +
                                        "Vínculo com a escola: Professor\n" +
                                        "Email: joao@email.com\n" +
                                        "Telefone: 123123123\n" +
                                        "Ciclos de ensino: \n" +
                                        "    > Ensino Médio\n" +
                                        "    > Ensino Fundamental\n" +
                                        "Quantidade de alunos: 503\n" +
                                        "Observações: Teste de Solicitação\n";

            smtpClientWrapperMock.Verify(wrapper =>
                wrapper.Send(It.Is<MailMessage>(msg => msg.Body == mensagemEsperada)), Times.Once);
        }

        [Fact]
        public void EnviarEmail_QuandoDestinatarioForPassado_DeveEnviarEmail()
        {
            var emailDestinatario = "dnit@email.com";
            var assunto = "Solicitação de ação";
            var corpo = "Nova solicitação de ação";

            service.EnviarEmail(emailDestinatario, assunto, corpo);

            smtpClientWrapperMock.Verify(wrapper =>
                wrapper.Send(It.Is<MailMessage>(msg =>
                    msg.Subject == assunto &&
                    msg.Body == corpo &&
                    msg.To.Contains(new MailAddress(emailDestinatario))
            )), Times.Once);
        }

        [Fact]
        public void EnviarEmail_QuandoDestinatarioForVazio_DeveLancarExcecao()
        {
            var emailDestinatario = "";
            var assunto = "Solicitação de ação";
            var corpo = "Nova solicitação de ação";

            Assert.Throws<ArgumentException>(() => service.EnviarEmail(emailDestinatario, assunto, corpo));
        }

        [Fact]
        public async Task ObterEscolas_QuandoRequisicaoForFeita_DeveRetornarListaDeEscolas()
        {
            Mock<ISmtpClientWrapper> smtpClientWrapperMock = new();
            Mock<IConfiguration> configurationMock = new();

            configurationMock.Setup(x => x["ApiInepUrl"]).Returns("http://inep.com");

            var resposta = @"[2,[{
                ""cod"": 12345678,
                ""nome"": ""Escola A"",
                ""estado"": ""DF"",
                ""cidade"": 100
            }, {
                ""cod"": 87654321,
                ""nome"": ""Escola B"",
                ""estado"": ""DF"",
                ""cidade"": 100
            }]]";

            var handlerMock = new Mock<HttpMessageHandler>();
            var httpClient = new HttpClient(handlerMock.Object);

            var httpClientFactoryMock = new Mock<IHttpClientFactory>();
            httpClientFactoryMock
                .Setup(x => x.CreateClient(It.IsAny<string>()))
                .Returns(httpClient);

            handlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(resposta)
                })
                .Verifiable();

            int municipio = 100;
            int codEscolaA = 12345678;
            int codEscolaB = 87654321;
            int primeiraPosicao = 0;
            int segundaPosicao = 1;

            service = new SolicitacaoAcaoService(db, smtpClientWrapperMock.Object, httpClientFactoryMock.Object, configurationMock.Object, solicitacaoAcaoRepositorioMock.Object, escolaRepositorioMock.Object);
            var escolas = await service.ObterEscolas(municipio);

            Assert.Equal(codEscolaA, escolas.ElementAt(primeiraPosicao).Cod);
            Assert.Equal("Escola A", escolas.ElementAt(primeiraPosicao).Nome);
            Assert.Equal("DF", escolas.ElementAt(primeiraPosicao).Estado);

            Assert.Equal(codEscolaB, escolas.ElementAt(segundaPosicao).Cod);
            Assert.Equal("Escola B", escolas.ElementAt(segundaPosicao).Nome);
            Assert.Equal("DF", escolas.ElementAt(segundaPosicao).Estado);
        }
    }
}
