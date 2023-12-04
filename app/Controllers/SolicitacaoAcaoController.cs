using api;
using api.Escolas;
using api.Solicitacoes;
using app.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using service.Interfaces;
using System.Net.Mail;

namespace app.Controllers
{
    [ApiController]
    [Route("api/solicitacaoAcao")]
    public class SolicitacaoAcaoController : AppController
    {
        private readonly ISolicitacaoAcaoService solicitacaoAcaoService;
        private readonly AuthService authService;

        public SolicitacaoAcaoController(AuthService authService, ISolicitacaoAcaoService solicitacaoAcaoService)
        {
            this.authService = authService;
            this.solicitacaoAcaoService = solicitacaoAcaoService;
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> EnviarSolicitacaoAcao([FromBody] SolicitacaoAcaoData solicitacaoAcaoDTO)
        {
            try
            {
                solicitacaoAcaoService.EnviarSolicitacaoAcao(solicitacaoAcaoDTO);
                await solicitacaoAcaoService.CriarOuAtualizar(solicitacaoAcaoDTO);
                return Ok();
            }
            catch (SmtpException)
            {
                return StatusCode(500, "Falha no envio do email.");
            }
        }

        [HttpGet]
        [Authorize]
        public async Task<ListaPaginada<SolicitacaoAcaoModel>> ObterSolicitacoesAsync([FromQuery] PesquisaSolicitacaoFiltro filtro)
        {
            authService.Require(Usuario, Permissao.SolicitacaoVisualizar);
            return await solicitacaoAcaoService.ObterSolicitacoesAsync(filtro);
        }

        [HttpGet("escolas")]
        public async Task<IEnumerable<EscolaInep>> ObterEscolas([FromQuery] int municipio)
        {
            var escolas = await solicitacaoAcaoService.ObterEscolas(municipio);
            return escolas;
        }
    }
}
