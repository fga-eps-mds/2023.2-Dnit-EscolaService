using api;
using app.Entidades;

namespace app.Repositorios.Interfaces;

public interface ISuperintendenciaRepositorio
{
    Task<Superintendencia> ObterPorIdAsync(int id);
    Task<List<Superintendencia>> ListarAsync();
}
