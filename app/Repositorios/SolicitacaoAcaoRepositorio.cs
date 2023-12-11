using api;
using api.Escolas;
using app.Entidades;
using app.Repositorios.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace app.Repositorios
{
    public class SolicitacaoAcaoRepositorio : ISolicitacaoAcaoRepositorio
    {
        private readonly AppDbContext dbContext;

        public SolicitacaoAcaoRepositorio(AppDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<SolicitacaoAcao> CriarOuAtualizar(SolicitacaoAcaoData s, Escola? escolaCadastrada, SolicitacaoAcao? solicitacaoAcaoExistente)
        {
            if (solicitacaoAcaoExistente == null)
            {
                var solicitacao = new SolicitacaoAcao
                {
                    EscolaCodigoInep = s.EscolaCodigoInep,
                    EscolaId = escolaCadastrada?.Id,
                    EscolaUf = s.Uf,
                    EscolaMunicipioId = s.MunicipioId,
                    Email = s.Email,
                    EscolaNome = s.Escola,
                    Telefone = s.Telefone,
                    NomeSolicitante = s.NomeSolicitante,
                    DataRealizada = DateTimeOffset.Now,
                    Observacoes = s.Observacoes,
                    TotalAlunos = s.QuantidadeAlunos,
                    Vinculo = s.VinculoEscola,
                };
                await dbContext.Solicitacoes.AddAsync(solicitacao);
                return solicitacao;
            }
            else
            {
                solicitacaoAcaoExistente.DataRealizada = DateTimeOffset.Now;
            }
            return solicitacaoAcaoExistente;
        }

        public async Task<SolicitacaoAcao?> ObterPorCodigoInepdAsync(int codigoInep)
        {
            return await dbContext.Solicitacoes
                .Where(e => e.EscolaCodigoInep == codigoInep)
                .FirstOrDefaultAsync();
        }

        public async Task<ListaPaginada<SolicitacaoAcao>> ObterSolicitacoesAsync(PesquisaSolicitacaoFiltro filtro)
        {
            var query = dbContext.Solicitacoes
                .Include(s => s.Escola!).ThenInclude(e => e.Municipio)
                .Include(s => s.Escola!).ThenInclude(e => e.EtapasEnsino)
                .Include(s => s.EscolaMunicipio)
                .AsQueryable();

            if (filtro.Nome != null)
                query = query.Where(s => s.EscolaNome.ToLower().Contains(filtro.Nome.ToLower()));

            if (filtro.QuantidadeAlunosMin != null)
            {
                query = query.Where(s => s.TotalAlunos >= filtro.QuantidadeAlunosMin);
                if (filtro.QuantidadeAlunosMax != null)
                    query = query.Where(s => s.TotalAlunos <= filtro.QuantidadeAlunosMax);
            }

            if (filtro.Uf != null)
                query = query.Where(s => s.EscolaUf == filtro.Uf);

            if (filtro.IdMunicipio != null)
                query = query.Where(s => s.EscolaMunicipioId == filtro.IdMunicipio);

            var total = await query.CountAsync();
            var sols = await query
                .OrderBy(s => s.Escola!.DistanciaPolo)
                .Skip((filtro.Pagina - 1) * filtro.TamanhoPagina)
                .Take(filtro.TamanhoPagina)
                .ToListAsync();
            return new ListaPaginada<SolicitacaoAcao>(sols, filtro.Pagina, filtro.TamanhoPagina, total);
        }
    }
}