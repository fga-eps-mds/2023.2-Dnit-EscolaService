using api;
using api.Polos;
using app.Entidades;
using app.Services;
using app.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace app.Controllers;

[ApiController]
[Route("api/polos")]
public class PolosController : AppController
{
    private readonly IPoloService _poloService;
    private readonly AuthService authService;

    public PolosController(
        IPoloService poloService,
        AuthService authService)
    {
        this._poloService = poloService;
        this.authService = authService;
    }
    
    [Authorize]
    [HttpGet("{id}")]
    public async Task<PoloModel> Obter(int id)
    {
        authService.Require(Usuario, Permissao.PoloVisualizar);
        return await _poloService.ObterModelPorIdAsync(id);
    }

    [Authorize]
    [HttpGet("paginado")]
    public async Task<ListaPaginada<PoloModel>> ObterPolosAsync([FromQuery] PesquisaPoloFiltro filtro)
    {
        authService.Require(Usuario, Permissao.PoloVisualizar);
        return await _poloService.ListarPaginadaAsync(filtro);
    }
    
    [Authorize]
    [HttpPost]
    public async Task CriarPolo(CadastroPoloDTO poloDto)
    {
        authService.Require(Usuario, Permissao.PoloCadastrar);
        await _poloService.CadastrarAsync(poloDto);
    }

    [Authorize]
    [HttpPut("{id}")]
    public async Task<IActionResult> AtualizarPolo(int id, CadastroPoloDTO poloDto)
    {
        authService.Require(Usuario, Permissao.PoloEditar);

        var poloExistente = await _poloService.ObterPorIdAsync(id);
        await _poloService.AtualizarAsync(poloExistente, poloDto);

        return NoContent();
    }

    [Authorize]
    [HttpDelete("{id}")]
    public async Task<IActionResult> ExcluirPolo(int id)
    {
        authService.Require(Usuario, Permissao.PoloRemover);

        var poloExistente = await _poloService.ObterPorIdAsync(id);
        await _poloService.ExcluirAsync(poloExistente);

        return NoContent();
    }


    
}
