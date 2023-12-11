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
using api;

namespace test
{
    public class FiltrarSolicitacoesDepoisDeCriarUmaEscolaTest : TestBed<Base>
    {
        private readonly Mock<ISmtpClientWrapper> smtpClientWrapperMock;
        private readonly Mock<IHttpClientFactory> httpClientFactoryMock;
        private readonly Mock<IConfiguration> configurationMock;
        private readonly ISolicitacaoAcaoRepositorio solicitacaoAcaoRepositorio;
        private readonly IEscolaRepositorio escolaRepositorio;
        private readonly AppDbContext dbContext;
        private readonly SolicitacaoAcaoService solicitacaoService;
        private readonly IEscolaService escolaService;

        public FiltrarSolicitacoesDepoisDeCriarUmaEscolaTest(ITestOutputHelper testOutputHelper, Base fixture) : base(testOutputHelper, fixture)
        {
            dbContext = fixture.GetService<AppDbContext>(testOutputHelper)!;
            smtpClientWrapperMock = new();
            httpClientFactoryMock = new();
            configurationMock = new();

            solicitacaoAcaoRepositorio = fixture.GetService<ISolicitacaoAcaoRepositorio>(testOutputHelper)!;
            escolaRepositorio = fixture.GetService<IEscolaRepositorio>(testOutputHelper)!;
            escolaService = fixture.GetService<IEscolaService>(testOutputHelper)!;

            solicitacaoService = new SolicitacaoAcaoService(dbContext, smtpClientWrapperMock.Object, httpClientFactoryMock.Object, configurationMock.Object, solicitacaoAcaoRepositorio, escolaRepositorio);
        }

        [Fact]
        public async Task ListarAsync_DepoisDeCriarEscolaAPartirDeSolicitacao_DeveSerPossivelFiltrarPelasInformacoesSobrescritasDaEscola()
        {
            // Precisa ser duas escolas pq 2 municípios são criados juntos
            dbContext.PopulaEscolas(2);
            var municipios = dbContext.Municipios.Take(2).ToList();
            var solicitacao = dbContext.PopulaSolicitacoes(1).First();
            // Nesse teste, essas informações vão ser sobrescritas quando for criada uma escola a partir dela
            solicitacao.EscolaCodigoInep = 1234;
            solicitacao.EscolaUf = UF.AM;
            solicitacao.EscolaMunicipioId = municipios[0].Id;
            solicitacao.TotalAlunos = 40;
            dbContext.SaveChanges();

            var escola = EscolaStub.ListarEscolasDto(dbContext.Municipios.ToList(), false).First();
            escola.NomeEscola = "Nome teste";
            escola.CodigoEscola = 1234;
            escola.IdUf = (int)UF.DF;
            escola.IdMunicipio = municipios[1].Id;
            escola.NumeroTotalDeAlunos = 110;

            await escolaService.CadastrarAsync(escola);

            var filtro = new PesquisaSolicitacaoFiltro()
            {
                // O filtro usa as informações sobrescritas pelo cadastro da 
                // escola.
                Uf = UF.DF,
                IdMunicipio = escola.IdMunicipio,
                QuantidadeAlunosMin = 109,
                QuantidadeAlunosMax = 111,
            };
            var escolas = await solicitacaoService.ObterSolicitacoesAsync(filtro);
            Assert.Single(escolas.Items);
        }

        public new void Dispose()
        {
            dbContext.Clear();
        }
    }
}
