using api;
using api.Escolas;
using app.Entidades;
using Microsoft.AspNetCore.Mvc;

namespace service.Interfaces
{
    public interface IEscolaService
    {
        Task CadastrarAsync(CadastroEscolaData cadastroEscolaDTO);
        Task<List<string>> CadastrarAsync(MemoryStream planilha);
        Task AtualizarAsync(Escola escola, EscolaModel data);
        Task<ListaEscolaPaginada<EscolaCorretaModel>> ListarPaginadaAsync(PesquisaEscolaFiltro filtro);
        Task ExcluirAsync(Guid id);
        bool SuperaTamanhoMaximo(MemoryStream planilha);
        Task RemoverSituacaoAsync(Guid id);
        Task AlterarDadosEscolaAsync(AtualizarDadosEscolaData dados);
        Task<FileResult> ExportarEscolasAsync();
    }
}