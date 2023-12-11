using api.Escolas;
using app.Entidades;

namespace service.Interfaces
{
    public interface ICalcularRanqueJob
    {
        Task ExecutarAsync(int novoRanqueId, int timeoutMinutos);
        Task CalcularUpsEscolas(List<Escola> escolas);

        bool ExisteFatorEscola(FatorPriorizacao fator, Escola escola);
        // public Task FinalizarCalcularRanqueJob(int ranqueId);
    }
}