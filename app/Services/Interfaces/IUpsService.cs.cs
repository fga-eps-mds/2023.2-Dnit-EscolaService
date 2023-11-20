using app.Entidades;

namespace service.Interfaces
{
    public interface IUpsService
    {
        public Task<List<int>> CalcularUpsEscolasAsync(List<Escola> escolas, double raioKm, int desdeAno, int expiracaoMinutos);
    }
}