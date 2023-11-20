using api;
using app.Entidades;
using app.Services;
using app.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace app.Controllers;

[ApiController]
[Route("api/superintendencias")]
public class SuperintendenciaController : AppController
{
    private readonly ISuperintendenciaService superintendenciaService;
    private readonly AuthService authService;

    public SuperintendenciaController(
        ISuperintendenciaService superintendenciaService,
        AuthService authService)
    {
        this.superintendenciaService = superintendenciaService;
        this.authService = authService;
    }

    [HttpGet("{id}")]
    public async Task<Superintendencia> Obter(int id)
    {
        authService.Require(Usuario, Permissao.EscolaVisualizar);
        return await superintendenciaService.ObterPorIdAsync(id);
    }
}
