using api;
using app.Entidades;
using app.Repositorios.Interfaces;
using app.Services;
using Microsoft.EntityFrameworkCore;

namespace app.Repositorios;

public class SuperIntendenciaRepositorio : ISuperintendenciaRepositorio
{

    private readonly AppDbContext dbContext;

    public SuperIntendenciaRepositorio(AppDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public async Task<Superintendencia> ObterPorIdAsync(int id)
    {
        return await dbContext.Superintendencias.FirstOrDefaultAsync(x => x.Id == id)
            ?? throw new ApiException(ErrorCodes.SuperIntendenciaNaoEncontrada);
    }

    public async Task<List<Superintendencia>> ListarAsync()
    {
        return await dbContext.Superintendencias.ToListAsync();
    }
}
