using app.Entidades;
using app.Repositorios.Interfaces;
using app.Services.Interfaces;

namespace app.Services;

public class SuperintendenciaService : ISuperintendenciaService
{
    private readonly ISuperintendenciaRepositorio superintendenciaRepositorio;

    public SuperintendenciaService(ISuperintendenciaRepositorio superintendenciaRepositorio)
    {
        this.superintendenciaRepositorio = superintendenciaRepositorio;
    }

    public async Task<Superintendencia> ObterPorIdAsync(int id)
    {
        return await superintendenciaRepositorio.ObterPorIdAsync(id);
    }
}
