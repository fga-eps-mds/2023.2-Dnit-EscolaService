using api;
using api.Escolas;
using api.Ranques;
using app.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using service.Interfaces;

namespace app.Controllers
{
    [ApiController]
    [Route("api/ranque")]
    public class RanqueController : AppController
    {
        private readonly IRanqueService ranqueService;
        private readonly AuthService authService;

        public RanqueController(
            IRanqueService ranqueService,
            AuthService authService
        )
        {
            this.ranqueService = ranqueService;
            this.authService = authService;
        }

        [HttpGet("escolas")]
        [Authorize]
        public async Task<ListaPaginada<RanqueEscolaModel>> ListarEscolasUltimoRanque([FromQuery] PesquisaEscolaFiltro filtro)
        {
            authService.Require(Usuario, Permissao.RanqueVisualizar);
            return await ranqueService.ListarEscolasUltimoRanqueAsync(filtro);
        }

        [HttpGet("escolas/{escolaId}")]
        [Authorize]
        public async Task<DetalhesEscolaRanqueModel> ObterDetalhesEscolaRanque([FromRoute] Guid escolaId)
        {
            authService.Require(Usuario, Permissao.RanqueVisualizar);
            return await ranqueService.ObterDetalhesEscolaRanque(escolaId);
        }

        [Authorize]
        [HttpPost("escolas/novo")]
        public async Task CalcularRanque()
        {
            authService.Require(Usuario, Permissao.RanqueCalcular);
            await ranqueService.CalcularNovoRanqueAsync();
        }

        [Authorize]
        [HttpGet("processamento")]
        public async Task<RanqueEmProcessamentoModel> RanqueProcessamento()
        {
            authService.Require(Usuario, Permissao.RanquePollProcessamento);
            return await ranqueService.ObterRanqueEmProcessamento();
        }
    }
}
