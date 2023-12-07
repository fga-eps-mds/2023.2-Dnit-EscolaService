using api.Planejamento;
using app.Entidades;

namespace service.Interfaces
{
    public interface IPlanejamentoService
    {
        // Definir os métodos a serem implementados
        Task<PlanejamentoMacro> GerarRecomendacaoDePlanejamento(PlanejamentoMacroDTO planejamento);
        PlanejamentoMacro CriarPlanejamentoMacro(PlanejamentoMacro planejamento);

        Task<PlanejamentoMacroDetalhadoModel> ObterPlanejamentoMacroDetalhado(Guid id);
        Task ExcluirPlanejamentoMacro(Guid id);
    }
}