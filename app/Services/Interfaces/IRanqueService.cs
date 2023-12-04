using api;
using api.Escolas;
using api.Ranques;
using app.Entidades;
using Microsoft.AspNetCore.Mvc;

namespace service.Interfaces
{
    public interface IRanqueService
    {
        Task CalcularNovoRanqueAsync();
        Task<ListaPaginada<RanqueEscolaModel>> ListarEscolasUltimoRanqueAsync(PesquisaEscolaFiltro filtro);
        Task<RanqueEmProcessamentoModel> ObterRanqueEmProcessamento();
        Task<DetalhesEscolaRanqueModel> ObterDetalhesEscolaRanque(Guid escolaId);
        Task ConcluirRanqueamentoAsync(Ranque ranqueEmProgresso);
        Task<ListaPaginada<RanqueDetalhesModel>> ListarRanquesAsync(PesquisaEscolaFiltro filtro);
        Task AtualizarRanqueAsync(int id, RanqueUpdateData data);
        Task<FileResult> ExportarRanqueAsync(int id);
    }
}