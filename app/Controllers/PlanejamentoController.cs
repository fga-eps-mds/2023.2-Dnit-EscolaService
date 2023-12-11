using api;
using api.Planejamento;
using app.Entidades;
using app.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Migrations;
using service.Interfaces;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace app.Controllers
{
    [ApiController]
    [Route("api/planejamento")]
    public class PlanejamentoController : AppController
    {
        private readonly IPlanejamentoService planejamentoService;
        private readonly AuthService authService;
        private readonly ModelConverter modelConverter;

        public PlanejamentoController(
            IPlanejamentoService planejamentoService,
            AuthService authService,
            ModelConverter modelConverter
            
        )
        {
            this.planejamentoService = planejamentoService;
            this.authService = authService;
            this.modelConverter = modelConverter;
        }

        [Authorize]    
        [HttpGet]
        public async Task<ListaPaginada<PlanejamentoMacroDetalhadoModel>> ObterPlanejamentosAsync([FromQuery] PesquisaPlanejamentoFiltro filtro){
            authService.Require(Usuario, Permissao.PlanejamentoVisualizar);
            
            return await planejamentoService.ListarPaginadaAsync(filtro);
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> ObterPlanejamentoMacro(Guid id)
        {
            authService.Require(Usuario, Permissao.PlanejamentoVisualizar);
            var plan = await planejamentoService.ObterPlanejamentoMacroAsync(id);
            return Ok(modelConverter.ToModel(plan));
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> ExcluirPlanejamentoMacro(Guid id)
        {
            authService.Require(Usuario, Permissao.PlanejamentoRemover);
            await planejamentoService.ExcluirPlanejamentoMacro(id);

            return Ok();
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CriarPlanejamentoMacro([FromBody] PlanejamentoMacroDTO planejamentoMacro)
        {
            authService.Require(Usuario, Permissao.PlanejamentoCriar);
            var recomendacao = await planejamentoService.GerarRecomendacaoDePlanejamento(planejamentoMacro); //retorna entidade sem id
            var planejamentoMacroCriado = planejamentoService.CriarPlanejamentoMacro(recomendacao);
            var planejamentoMacroDetalhadoModel = modelConverter.ToModel(planejamentoMacroCriado);

            return Ok(planejamentoMacroDetalhadoModel);
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> EditarPlanejamentoMacro(Guid id, [FromBody] PlanejamentoMacroDetalhadoDTO dto)
        {
            authService.Require(Usuario, Permissao.PlanejamentoEditar);

            try{
                //Edita o planejamento macro
                var planejamento = await planejamentoService.EditarPlanejamentoMacro(id, dto.Nome, dto.PlanejamentoMacroMensal);
                return Ok(modelConverter.ToModel(planejamento));
            }
            catch(KeyNotFoundException)
            {
                return NotFound("Planejamento Macro não encontrado.");
            }
            catch(DbUpdateException){
                return UnprocessableEntity("Erro ao editar Planejamento.");
            }
            catch(InvalidOperationException){
                return UnprocessableEntity("Erro ao editar Planejamento.");
            }
            catch(Exception ex){
                return StatusCode(500, $"Houve um erro interno no servidor. {ex.Message}");
            }
        }
    }
}