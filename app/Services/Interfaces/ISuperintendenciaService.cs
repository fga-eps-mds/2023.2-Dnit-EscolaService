using app.Entidades;

namespace app.Services.Interfaces;

public interface ISuperintendenciaService
{
    Task<Superintendencia> ObterPorIdAsync(int id);
}
