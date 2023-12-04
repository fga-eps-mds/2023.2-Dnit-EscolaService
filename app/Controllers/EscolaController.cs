using app.Services;
using api.Escolas;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using service.Interfaces;
using api;

namespace app.Controllers
{
    [ApiController]
    [Route("api/escolas")]
    public class EscolaController : AppController
    {
        private readonly IEscolaService escolaService;
        private readonly AuthService authService;

        public EscolaController(IEscolaService escolaService, AuthService authService)
        {
            this.escolaService = escolaService;
            this.authService = authService;
        }

        [Authorize]
        [Consumes("multipart/form-data")]
        [HttpPost("cadastrarEscolaPlanilha")]
        public async Task<IActionResult> EnviarPlanilhaAsync(IFormFile arquivo)
        {
            authService.Require(Usuario, Permissao.EscolaCadastrar);
            List<string> escolasNovas;

            try
            {
                if (arquivo == null || arquivo.Length == 0)
                    return BadRequest("Nenhum arquivo enviado.");

                if (arquivo.ContentType.ToLower() != "text/csv")
                {
                    return BadRequest("O arquivo deve estar no formato CSV.");
                }

                using (var memoryStream = new MemoryStream())
                {
                    await arquivo.CopyToAsync(memoryStream);
                    memoryStream.Seek(0, SeekOrigin.Begin);

                    if (escolaService.SuperaTamanhoMaximo(memoryStream))
                    {
                        return StatusCode(406, "Tamanho máximo de arquivo ultrapassado!");
                    }
                }

                using (var memoryStream = new MemoryStream())
                {
                    await arquivo.CopyToAsync(memoryStream);
                    memoryStream.Seek(0, SeekOrigin.Begin);
                    escolasNovas = await escolaService.CadastrarAsync(memoryStream);
                }

                return Ok(escolasNovas);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [Authorize]
        [HttpGet("obter")]
        public async Task<ListaEscolaPaginada<EscolaCorretaModel>> ObterEscolasAsync([FromQuery] PesquisaEscolaFiltro filtro)
        {
            authService.Require(Usuario, Permissao.EscolaVisualizar);

            return await escolaService.ListarPaginadaAsync(filtro);
        }
        

        [Authorize]
        [HttpDelete("excluir")]
        public async Task ExcluirEscolaAsync([FromQuery] Guid id)
        {
            authService.Require(Usuario, Permissao.EscolaRemover);
            await escolaService.ExcluirAsync(id);
        }

        [Authorize]
        [HttpPost("cadastrarEscola")]
        public async Task CadastrarEscolaAsync(CadastroEscolaData cadastroEscolaDTO)
        {
            authService.Require(Usuario, Permissao.EscolaCadastrar);
            await escolaService.CadastrarAsync(cadastroEscolaDTO);
        }

        [Authorize]
        [HttpPost("removerSituacao")]
        public async Task RemoverSituacaoAsync([FromQuery] Guid idEscola)
        {
            authService.Require(Usuario, Permissao.EscolaEditar);
            await escolaService.RemoverSituacaoAsync(idEscola);
        }

        [Authorize]
        [HttpPut("alterarDadosEscola")]
        public async Task AlterarDadosEscolaAsync(AtualizarDadosEscolaData atualizarDadosEscolaDTO)
        {
            authService.Require(Usuario, Permissao.EscolaEditar);

            await escolaService.AlterarDadosEscolaAsync(atualizarDadosEscolaDTO);
        }

        [Authorize]
        [HttpGet("exportar")]
        public async Task<FileResult> ExportarAsync()
        {
            authService.Require(Usuario, Permissao.EscolaExportar);
            return await escolaService.ExportarEscolasAsync();
        }
    }
}
