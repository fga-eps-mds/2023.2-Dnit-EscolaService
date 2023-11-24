using api;
using app.Entidades;
using app.Services;
using app.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace app.Controllers;

[ApiController]
[Route("api/[controller]")]
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

    [HttpGet("{id}")]
    public async Task<Polo> Obter(int id)
    {
        authService.Require(Usuario, Permissao.EscolaVisualizar);
        return await _poloService.ObterPorIdAsync(id);
    }
}
