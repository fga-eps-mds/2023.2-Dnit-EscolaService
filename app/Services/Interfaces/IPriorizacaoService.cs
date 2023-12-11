using api.CustoLogistico;
using api.Fatores;
using app.Entidades;

namespace app.Services.Interfaces
{
    public interface IPriorizacaoService
    {
        Task<FatorPrioriModel> EditarFatorPorId(Guid Id, FatorPrioriModel itemAtualizado);
        Task DeletarFatorId(Guid Id);
        Task <FatorPrioriModel> VisualizarFatorId(Guid Id);
        Task<List<FatorPrioriModel>> ListarFatores();
        Task<FatorPrioriModel> AdicionarFatorPriorizacao(FatorPrioriModel novoFator);
        Task<List<CustoLogisticoItem>> ListarCustosLogisticos();
        Task<List<CustoLogisticoItem>> EditarCustosLogisticos(List<CustoLogisticoItem> custoItems);
        void CalcularFatorUps();
        void CalcularFatorCustoLogistico();
        void CalcularFatorOutro(Guid fatorId);
    }
}
