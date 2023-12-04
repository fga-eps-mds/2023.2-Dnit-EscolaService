using Moq;
using service.Interfaces;
using Microsoft.Extensions.Configuration;
using System.Net.Http;
using System.Linq;
using System.Threading.Tasks;
using test.Stubs;
using app.Services;
using app.Repositorios.Interfaces;
using app.Entidades;
using Xunit.Microsoft.DependencyInjection.Abstracts;
using test.Fixtures;
using Xunit.Abstractions;
using api.Escolas;

namespace test
{
    public class SolicitacaoAcaoServicePersistenciaTest : TestBed<Base>
    {
        private readonly Mock<ISmtpClientWrapper> smtpClientWrapperMock;
        private readonly Mock<IHttpClientFactory> httpClientFactoryMock;
        private readonly Mock<IConfiguration> configurationMock;
        private readonly ISolicitacaoAcaoRepositorio solicitacaoAcaoRepositorio;
        private readonly IEscolaRepositorio escolaRepositorio;
        private readonly AppDbContext db;
        private readonly SolicitacaoAcaoService service;

        public SolicitacaoAcaoServicePersistenciaTest(ITestOutputHelper testOutputHelper, Base fixture) : base(testOutputHelper, fixture)
        {
            db = fixture.GetService<AppDbContext>(testOutputHelper)!;
            db.Clear();
            smtpClientWrapperMock = new();
            httpClientFactoryMock = new();
            configurationMock = new();

            solicitacaoAcaoRepositorio = fixture.GetService<ISolicitacaoAcaoRepositorio>(testOutputHelper)!;
            escolaRepositorio = fixture.GetService<IEscolaRepositorio>(testOutputHelper)!;

            service = new SolicitacaoAcaoService(db, smtpClientWrapperMock.Object, httpClientFactoryMock.Object, configurationMock.Object, solicitacaoAcaoRepositorio, escolaRepositorio);
        }

        [Fact]
        public async Task Criar_QuandoEscolaNaoCadastrada_DeveCriarSolicitacaoSemEscolaRelacionada()
        {
            Assert.Empty(db.Solicitacoes);

            var sol = new SolicitacaoAcaoStub().ObterSolicitacaoAcaoDTO();
            await service.CriarOuAtualizar(sol);

            Assert.Single(db.Solicitacoes.ToList());

            var solicitacao = db.Solicitacoes.First();
            Assert.Null(solicitacao.Escola);
            Assert.Null(solicitacao.EscolaId);
        }

        [Fact]
        public async Task Criar_QuandoEscolaJaCadastrada_DeveRelacionarSolicitacaoComEscola()
        {
            var escola = db.PopulaEscolas(1).First();
            var sol = new SolicitacaoAcaoStub().ObterSolicitacaoAcaoDTO();
            escola.Codigo = 123;
            sol.EscolaCodigoInep = 123;
            db.SaveChanges();

            await service.CriarOuAtualizar(sol);

            var solicitacao = db.Solicitacoes.First();
            Assert.Equal(escola.Id, solicitacao.EscolaId);
            Assert.Single(db.Solicitacoes);
        }

        [Fact]
        public async Task Criar_QuandoEscolaJaTemSolicitacao_DeveAtualizarData()
        {
            var escola = db.PopulaEscolas(1).First();
            var sol = db.PopulaSolicitacoes(1).First();
            var dataRealizada = DateTimeOffset.Now.AddDays(-3);
            sol.DataRealizada = dataRealizada;
            sol.EscolaCodigoInep = escola.Codigo;
            sol.EscolaId = escola.Id;
            db.SaveChanges();

            var solicitacaoDto = new SolicitacaoAcaoStub().ObterSolicitacaoAcaoDTO();
            solicitacaoDto.EscolaCodigoInep = escola.Codigo;
            await service.CriarOuAtualizar(solicitacaoDto);

            var hoje = DateTimeOffset.Now;
            sol = db.Solicitacoes.First();
            Assert.Single(db.Solicitacoes);
            // FIXME: Essas três asserções poderiam ser resumidas 
            // em uma só se fosse possível controlar e acessar o DateTimeOffset
            // retornado em SolicitacaoAcaoRepositorio.CriarOuAtualizar().
            Assert.Equal(hoje.Year, sol.DataRealizada!.Value.Year);
            Assert.Equal(hoje.Month, sol.DataRealizada.Value.Month);
            Assert.Equal(hoje.Day, sol.DataRealizada.Value.Day);
        }

        [Fact]
        public async Task ObterSolicitacoesAsync_QuandoNormal_RetornaSolicitacoes()
        {
            db.PopulaSolicitacoes(4);

            var filtro = new PesquisaSolicitacaoFiltro();
            var models = await service.ObterSolicitacoesAsync(filtro);

            Assert.Equal(4, models.Items.Count);
        }

        [Fact]
        public async Task ObterSolicitacoesAsync_AlgumasSolicitacoesTemEscola_OutrasNao()
        {
            var escolas = db.PopulaEscolas(4);
            var sols = db.PopulaSolicitacoes(8);

            sols[0].EscolaId = escolas[0].Id;
            sols[1].EscolaId = escolas[1].Id;
            sols[2].EscolaId = escolas[2].Id;
            sols[3].EscolaId = escolas[3].Id;
            db.SaveChanges();

            var filtro = new PesquisaSolicitacaoFiltro();
            var modelos = await service.ObterSolicitacoesAsync(filtro);

            var solicitacoesRelacionadasComEscolas = 0;
            var solicitacoesSemEscolas = 0;
            foreach (var m in modelos.Items)
                if (m.Escola != null)
                    solicitacoesRelacionadasComEscolas++;
                else
                    solicitacoesSemEscolas++;

            Assert.Equal(4, solicitacoesRelacionadasComEscolas);
            Assert.Equal(4, solicitacoesSemEscolas);
        }

        public new void Dispose()
        {
            db.Clear();
        }
    }
}
