using api;
using api.Escolas;
using app.Controllers;
using app.Entidades;
using app.Repositorios;
using app.Repositorios.Interfaces;
using app.Services;
using auth;
using EnumsNET;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Authentication;
using System.Text;
using System.Threading.Tasks;
using test.Fixtures;
using test.Stubs;
using Xunit.Abstractions;

namespace test
{
    public class EscolaControllerTest : AuthTest
    {
        AppDbContext dbContext;
        EscolaController escolaController;

        public EscolaControllerTest(ITestOutputHelper testOutputHelper, Base fixture) : base(testOutputHelper, fixture)
        {
            dbContext = fixture.GetService<AppDbContext>(testOutputHelper)!;
            dbContext.PopulaEscolas(5);

            escolaController = fixture.GetService<EscolaController>(testOutputHelper)!;
            AutenticarUsuario(escolaController);
        }

        [Fact]
        public async Task ObterEscolasAsync_QuandoMetodoForChamado_DeveRetornarListaDeEscolas()
        {
            var escolasDb = dbContext.Escolas.ToList();
            var filtro = new PesquisaEscolaFiltro();
            filtro.Pagina = 1;
            filtro.TamanhoPagina = escolasDb.Count();
            var result = await escolaController.ObterEscolasAsync(filtro);

            Assert.Equal(escolasDb.Count(), result.TotalEscolas);
            Assert.Equal(filtro.Pagina, result.Pagina);
            Assert.Equal(filtro.TamanhoPagina, result.EscolasPorPagina);
            Assert.True(escolasDb.All(e => result.Escolas.Exists(ee => ee.IdEscola == e.Id)));
        }

        [Fact]
        public async Task ObterEscolasAsync_QuandoNaoTiverPermissao_DeveSerBloqueado()
        {
            var escolasDb = dbContext.Escolas.ToList();
            var filtro = new PesquisaEscolaFiltro();
            filtro.Pagina = 1;
            filtro.TamanhoPagina = escolasDb.Count();

            AutenticarUsuario(escolaController, permissoes: new() { });

            await Assert.ThrowsAsync<AuthForbiddenException>(async () => await escolaController.ObterEscolasAsync(filtro));
        }

        [Fact]
        public async Task Excluir_QuandoIdEscolaForPassado_DeveExcluirEscola()
        {
            var idEscola = dbContext.Escolas.First().Id;
            await escolaController.ExcluirEscolaAsync(idEscola);

            Assert.False(dbContext.Escolas.Any(e => e.Id == idEscola));
        }

        [Fact]
        public async Task Excluir_QuandoNaoTiverPermissao_DeveSerBloqueado()
        {
            var idEscola = dbContext.Escolas.First().Id;

            AutenticarUsuario(escolaController, permissoes: new());

            await Assert.ThrowsAsync<AuthForbiddenException>(async () => await escolaController.ExcluirEscolaAsync(idEscola));
        }

        [Fact]
        public async Task CadastrarEscola_QuandoEscolaForCadastrada_DeveRetornarHttpOk()
        {
            var escola = EscolaStub.ListarEscolasDto(dbContext.Municipios.ToList(), comEtapas: true).First();

            await escolaController.CadastrarEscolaAsync(escola);

            var escolaDb = dbContext.Escolas.First(e => e.Codigo == escola.CodigoEscola);

            Assert.Equal(escola.Cep, escolaDb.Cep);
            Assert.Equal(escola.NomeEscola, escolaDb.Nome);
            Assert.Equal(escola.Endereco, escolaDb.Endereco);
            Assert.Equal(escola.IdLocalizacao, (int?)escolaDb.Localizacao);
            Assert.Equal(escola.IdPorte, (int?)escolaDb.Porte);
            Assert.Equal(escola.IdUf, (int?)escolaDb.Uf);
            Assert.Equal(escola.Latitude, escolaDb.Latitude);
            Assert.Equal(escola.Longitude, escolaDb.Longitude);
            Assert.Equal(escola.IdRede, (int?)escolaDb.Rede);
            Assert.Equal(escola.NumeroTotalDeAlunos, escolaDb.TotalAlunos);
            Assert.Equal(escola.NumeroTotalDeDocentes, escolaDb.TotalDocentes);
        }

        [Fact]
        public async Task CadastrarEscola_QuandoNaoTiverPermissao_DeverBloquear()
        {
            var escola = EscolaStub.ListarEscolasDto(dbContext.Municipios.ToList(), comEtapas: true).First();

            AutenticarUsuario(escolaController, permissoes: new());
            await Assert.ThrowsAsync<AuthForbiddenException>(async () => await escolaController.CadastrarEscolaAsync(escola));
        }

        [Fact]
        public async Task RemoverSituacaoAsync_QuandoSituacaoForRemovida_DeveRetornarHttpOk()
        {
            var idEscola = dbContext.Escolas.First().Id;
            await escolaController.RemoverSituacaoAsync(idEscola);

            Assert.Null(dbContext.Escolas.First(e => e.Id == idEscola).Situacao);
        }

        [Fact]
        public async Task RemoverSituacaoAsync_QuandoNaoTiverPermissao_DeverBloquear()
        {
            var idEscola = dbContext.Escolas.First().Id;

            AutenticarUsuario(escolaController, permissoes: new());

            await Assert.ThrowsAsync<AuthForbiddenException>(async () => await escolaController.RemoverSituacaoAsync(idEscola));
        }

        [Fact]
        public async Task AlterarDadosEscolaAsync_QuandoNaoTiverPermissao_DeveBloquear()
        {
            var escola = EscolaStub.ListarAtualizarEscolasDto(dbContext.Municipios.ToList(), true).First();
            escola.IdEscola = dbContext.Escolas.Last().Id;

            AutenticarUsuario(escolaController, permissoes: new());

            await Assert.ThrowsAsync<AuthForbiddenException>(async () => await escolaController.AlterarDadosEscolaAsync(escola));
        }

        [Fact]
        public async Task EnviarPlanilhaAsync_QuandoCadastrarEscola_DeveEstarNoBancoDeDados()
        {
            var escolaCodigo = 41127226;
            var etapas = new List<EtapaEnsino>() { EtapaEnsino.Fundamental, EtapaEnsino.JovensAdultos };
            var descricoesEtapa = string.Join(',', etapas.Select(e => e.AsString(EnumFormat.Description)));
            var planilha = new StringBuilder();
            planilha.AppendLine("Ano do Censo Escolar;ID;Cod. INEP;Nome da Instituição de Ensino;Rede;Porte da Instituição de Ensino;Endereço;CEP;Cidade;UF;Localização;Latitude;Longitude;DDD;Telefone da instituição;Etapas de Ensino Contempladas;Nº de Matrículas Ensino Infantil;Nº de Matrículas 1º ano Ensino Fundamental;Nº de Matrículas 2º ano Ensino Fundamental;Nº de Matrículas 3º ano Ensino Fundamental;Nº de Matrículas 4º ano Ensino Fundamental;Nº de Matrículas 5º ano Ensino Fundamental;Nº de Matrículas 6º ano Ensino Fundamental;Nº de Matrículas 7º ano Ensino Fundamental;Nº de Matrículas 8º ano Ensino Fundamental;Nº de Matrículas 9º ano Ensino Fundamental;Nº de Docentes");
            planilha.AppendLine($"2019;1;{escolaCodigo};ANISIO TEIXEIRA E M EF;Municipal;Entre 201 e 500 matrículas de escolarização;RUA JOAO BATISTA SCUCATO, 80 ATUBA. 82860-130 Curitiba - PR.;82860130;Curitiba;PR;Urbana;-25,38443;-49,2011;41;32562393;{descricoesEtapa};;70;90;92;65;73;0;0;0;0;126\r\n");

            var bytes = Encoding.UTF8.GetBytes(planilha.ToString());
            var memoryStream = new MemoryStream(bytes);

            var arquivo = new FormFile(memoryStream, 0, bytes.Length, "planilha", "planilha.csv");
            arquivo.Headers = new HeaderDictionary();
            arquivo.Headers.ContentType = "text/csv";
            var resultado = await escolaController.EnviarPlanilhaAsync(arquivo);

            Assert.IsType<OkObjectResult>(resultado);
            var valor = (resultado as OkObjectResult)?.Value as List<string>;

            var escolaDb = dbContext.Escolas.First(e => e.Codigo == escolaCodigo);
            Assert.True(valor?.Exists(v => v == escolaDb.Nome));
        }

        [Fact]
        public async Task EnviarPlanilhaAsync_QuandoNaoTiverPermissao_DeveSerBloqueado()
        {
            var escolaCodigo = 41127226;
            var etapas = new List<EtapaEnsino>() { EtapaEnsino.Fundamental, EtapaEnsino.JovensAdultos };
            var descricoesEtapa = string.Join(',', etapas.Select(e => e.AsString(EnumFormat.Description)));
            var planilha = new StringBuilder();
            planilha.AppendLine("Ano do Censo Escolar;ID;Cod. INEP;Nome da Instituição de Ensino;Rede;Porte da Instituição de Ensino;Endereço;CEP;Cidade;UF;Localização;Latitude;Longitude;DDD;Telefone da instituição;Etapas de Ensino Contempladas;Nº de Matrículas Ensino Infantil;Nº de Matrículas 1º ano Ensino Fundamental;Nº de Matrículas 2º ano Ensino Fundamental;Nº de Matrículas 3º ano Ensino Fundamental;Nº de Matrículas 4º ano Ensino Fundamental;Nº de Matrículas 5º ano Ensino Fundamental;Nº de Matrículas 6º ano Ensino Fundamental;Nº de Matrículas 7º ano Ensino Fundamental;Nº de Matrículas 8º ano Ensino Fundamental;Nº de Matrículas 9º ano Ensino Fundamental;Nº de Docentes");
            planilha.AppendLine($"2019;1;{escolaCodigo};ANISIO TEIXEIRA E M EF;Municipal;Entre 201 e 500 matrículas de escolarização;RUA JOAO BATISTA SCUCATO, 80 ATUBA. 82860-130 Curitiba - PR.;82860130;Curitiba;PR;Urbana;-25,38443;-49,2011;41;32562393;{descricoesEtapa};;70;90;92;65;73;0;0;0;0;126\r\n");

            var bytes = Encoding.UTF8.GetBytes(planilha.ToString());
            var memoryStream = new MemoryStream(bytes);

            var arquivo = new FormFile(memoryStream, 0, bytes.Length, "planilha", "planilha.csv");
            arquivo.Headers = new HeaderDictionary();
            arquivo.Headers.ContentType = "text/csv";

            AutenticarUsuario(escolaController, permissoes: new() { });

            await Assert.ThrowsAsync<AuthForbiddenException>(async () => await escolaController.EnviarPlanilhaAsync(arquivo));
        }

        [Fact]
        public async Task EnviarPlanilhaAsync_QuandoNaoForCsv_DeveDevolverBadRequest()
        {
            var escolaCodigo = 41127226;
            var etapas = new List<EtapaEnsino>() { EtapaEnsino.Fundamental, EtapaEnsino.JovensAdultos };
            var descricoesEtapa = string.Join(',', etapas.Select(e => e.AsString(EnumFormat.Description)));
            var planilha = new StringBuilder();
            planilha.AppendLine("Ano do Censo Escolar;ID;Cod. INEP;Nome da Instituição de Ensino;Rede;Porte da Instituição de Ensino;Endereço;CEP;Cidade;UF;Localização;Latitude;Longitude;DDD;Telefone da instituição;Etapas de Ensino Contempladas;Nº de Matrículas Ensino Infantil;Nº de Matrículas 1º ano Ensino Fundamental;Nº de Matrículas 2º ano Ensino Fundamental;Nº de Matrículas 3º ano Ensino Fundamental;Nº de Matrículas 4º ano Ensino Fundamental;Nº de Matrículas 5º ano Ensino Fundamental;Nº de Matrículas 6º ano Ensino Fundamental;Nº de Matrículas 7º ano Ensino Fundamental;Nº de Matrículas 8º ano Ensino Fundamental;Nº de Matrículas 9º ano Ensino Fundamental;Nº de Docentes");
            planilha.AppendLine($"2019;1;{escolaCodigo};ANISIO TEIXEIRA E M EF;Municipal;Entre 201 e 500 matrículas de escolarização;RUA JOAO BATISTA SCUCATO, 80 ATUBA. 82860-130 Curitiba - PR.;82860130;Curitiba;PR;Urbana;-25,38443;-49,2011;41;32562393;{descricoesEtapa};;70;90;92;65;73;0;0;0;0;126\r\n");

            var bytes = Encoding.UTF8.GetBytes(planilha.ToString());
            var memoryStream = new MemoryStream(bytes);

            var arquivo = new FormFile(memoryStream, 0, bytes.Length, "planilha", "planilha.pdf");
            arquivo.Headers = new HeaderDictionary();
            arquivo.Headers.ContentType = "application/pdf";
            var resultado = await escolaController.EnviarPlanilhaAsync(arquivo);

            Assert.IsType<BadRequestObjectResult>(resultado);
            var valor = (resultado as BadRequestObjectResult)?.Value as string;

            Assert.Equal("O arquivo deve estar no formato CSV.", valor);
        }

        [Fact]
        public async Task EnviarPlanilhaAsync_QuandoTiverVazio_DeveDevolverBadRequest()
        {
            var bytes = Encoding.UTF8.GetBytes("");
            var memoryStream = new MemoryStream(bytes);

            var arquivo = new FormFile(memoryStream, 0, bytes.Length, "planilha", "planilha.csv");
            arquivo.Headers = new HeaderDictionary();
            arquivo.Headers.ContentType = "text/csv";

            var resultado = await escolaController.EnviarPlanilhaAsync(arquivo);

            Assert.IsType<BadRequestObjectResult>(resultado);
            var valor = (resultado as BadRequestObjectResult)?.Value as string;

            Assert.Equal("Nenhum arquivo enviado.", valor);
        }

        [Fact]
        public async Task EnviarPlanilhaAsync_QuandoTiverAcimaDoLimite_DeveDevolverNotAcceptableRequest()
        {
            var caminhoArquivo = Path.Join("..", "..", "..", "Stubs", "planilha_maior_max.csv");
            var bytes = File.ReadAllBytes(caminhoArquivo);
            var stream = new MemoryStream(bytes);
            var arquivo = new FormFile(stream, 0, bytes.Length, "planilha_maior_max", "planilha_maior_max.csv");
            arquivo.Headers = new HeaderDictionary();
            arquivo.Headers.ContentType = "text/csv";
            var resultado = await escolaController.EnviarPlanilhaAsync(arquivo);

            Assert.IsType<ObjectResult>(resultado);
            var resultadoObjeto = resultado as ObjectResult;
            Assert.Equal(406, resultadoObjeto?.StatusCode);
            Assert.Equal("Tamanho máximo de arquivo ultrapassado!", resultadoObjeto?.Value);
        }

        [Fact]
        public async Task EnviarPlanilhaAsync_QuandoNaoTiverEtapa_DeveRetornarInternalServerError()
        {
            var escolaCodigo = 41127226;
            var etapas = new List<EtapaEnsino>() { };
            var descricoesEtapa = string.Join(',', etapas.Select(e => e.AsString(EnumFormat.Description)));
            var planilha = new StringBuilder();
            planilha.AppendLine("Ano do Censo Escolar;ID;Cod. INEP;Nome da Instituição de Ensino;Rede;Porte da Instituição de Ensino;Endereço;CEP;Cidade;UF;Localização;Latitude;Longitude;DDD;Telefone da instituição;Etapas de Ensino Contempladas;Nº de Matrículas Ensino Infantil;Nº de Matrículas 1º ano Ensino Fundamental;Nº de Matrículas 2º ano Ensino Fundamental;Nº de Matrículas 3º ano Ensino Fundamental;Nº de Matrículas 4º ano Ensino Fundamental;Nº de Matrículas 5º ano Ensino Fundamental;Nº de Matrículas 6º ano Ensino Fundamental;Nº de Matrículas 7º ano Ensino Fundamental;Nº de Matrículas 8º ano Ensino Fundamental;Nº de Matrículas 9º ano Ensino Fundamental;Nº de Docentes");
            planilha.AppendLine($"2019;1;{escolaCodigo};ANISIO TEIXEIRA E M EF;Municipal;Entre 201 e 500 matrículas de escolarização;RUA JOAO BATISTA SCUCATO, 80 ATUBA. 82860-130 Curitiba - PR.;82860130;Curitiba;PR;Urbana;-25,38443;-49,2011;41;32562393;{descricoesEtapa};;70;90;92;65;73;0;0;0;0;126\r\n");

            var bytes = Encoding.UTF8.GetBytes(planilha.ToString());
            var memoryStream = new MemoryStream(bytes);

            var arquivo = new FormFile(memoryStream, 0, bytes.Length, "planilha", "planilha.csv");
            arquivo.Headers = new HeaderDictionary();
            arquivo.Headers.ContentType = "text/csv";
            var resultado = await escolaController.EnviarPlanilhaAsync(arquivo);

            Assert.IsType<ObjectResult>(resultado);
            var resultadoObjeto = resultado as ObjectResult;
            Assert.Equal(500, resultadoObjeto?.StatusCode);
            Assert.True((resultadoObjeto?.Value as string)?.Contains("descrição das etapas de ensino inválida"));
        }

        [Fact]
        public async Task EnviarPlanilhaAsync_QuandoNaoTiverRede_DeveRetornarInternalServerError()
        {
            var escolaCodigo = 41127226;
            var etapas = new List<EtapaEnsino>() { EtapaEnsino.Infantil };
            var descricoesEtapa = string.Join(',', etapas.Select(e => e.AsString(EnumFormat.Description)));
            var planilha = new StringBuilder();
            planilha.AppendLine("Ano do Censo Escolar;ID;Cod. INEP;Nome da Instituição de Ensino;Rede;Porte da Instituição de Ensino;Endereço;CEP;Cidade;UF;Localização;Latitude;Longitude;DDD;Telefone da instituição;Etapas de Ensino Contempladas;Nº de Matrículas Ensino Infantil;Nº de Matrículas 1º ano Ensino Fundamental;Nº de Matrículas 2º ano Ensino Fundamental;Nº de Matrículas 3º ano Ensino Fundamental;Nº de Matrículas 4º ano Ensino Fundamental;Nº de Matrículas 5º ano Ensino Fundamental;Nº de Matrículas 6º ano Ensino Fundamental;Nº de Matrículas 7º ano Ensino Fundamental;Nº de Matrículas 8º ano Ensino Fundamental;Nº de Matrículas 9º ano Ensino Fundamental;Nº de Docentes");
            planilha.AppendLine($"2019;1;{escolaCodigo};ANISIO TEIXEIRA E M EF;;Entre 201 e 500 matrículas de escolarização;RUA JOAO BATISTA SCUCATO, 80 ATUBA. 82860-130 Curitiba - PR.;82860130;Curitiba;PR;Urbana;-25,38443;-49,2011;41;32562393;{descricoesEtapa};;70;90;92;65;73;0;0;0;0;126\r\n");

            var bytes = Encoding.UTF8.GetBytes(planilha.ToString());
            var memoryStream = new MemoryStream(bytes);

            var arquivo = new FormFile(memoryStream, 0, bytes.Length, "planilha", "planilha.csv");
            arquivo.Headers = new HeaderDictionary();
            arquivo.Headers.ContentType = "text/csv";
            var resultado = await escolaController.EnviarPlanilhaAsync(arquivo);

            Assert.IsType<ObjectResult>(resultado);
            var resultadoObjeto = resultado as ObjectResult;
            Assert.Equal(500, resultadoObjeto?.StatusCode);
            Assert.True((resultadoObjeto?.Value as string)?.Contains("rede inválida"));
        }

        [Fact]
        public async Task ExportarRanqueAsync_QuandoExistir_DeveExportar()
        {
            dbContext.PopulaEscolas(2);

            var file = await escolaController.ExportarAsync();

            Assert.NotNull(file);
            Assert.IsType<FileContentResult>(file);
            Assert.True((file as FileContentResult).FileContents.Length > 0);
        }

        internal new void Dispose()
        {
            dbContext.Clear();
        }
    }
}
