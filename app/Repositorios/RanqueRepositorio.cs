using api;
using api.Escolas;
using app.Entidades;
using app.Repositorios.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace app.Repositorios
{

    public class RanqueRepositorio : IRanqueRepositorio
    {
        private readonly AppDbContext dbContext;

        public RanqueRepositorio(
            AppDbContext dbContext
        )
        {
            this.dbContext = dbContext;
        }

        public async Task<Ranque?> ObterPorIdAsync(int id)
        {
            return await dbContext.Ranques.FindAsync(id);
        }

        public async Task<Ranque?> ObterUltimoRanqueAsync()
        {
            return await dbContext.Ranques
                .Where(r => r.DataFimUtc != null)
                .OrderByDescending(r => r.DataFimUtc)
                .FirstOrDefaultAsync();
        }

        public async Task<Ranque?> ObterRanqueEmProcessamentoAsync()
        {
            return await dbContext.Ranques
                .OrderByDescending(r => r.DataInicioUtc)
                .FirstOrDefaultAsync();
        }

        public async Task<ListaPaginada<EscolaRanque>> ListarEscolasPaginadaAsync(int ranqueId, PesquisaEscolaFiltro filtro)
        {
            var query = dbContext.EscolaRanques
                .Include(er => er.Ranque)
                .Include(er => er.Escola).ThenInclude(e => e.EtapasEnsino)
                .Include(er => er.Escola).ThenInclude(e => e.Municipio)
                .Include(er => er.Escola).ThenInclude(e => e.Polo).ThenInclude(p => p.Municipio)
                .Include(er => er.Escola).ThenInclude(e => e.Solicitacao)
                .Where(er => er.RanqueId == ranqueId);

            if (filtro.Nome != null)
            {
                query = query.Where(er => er.Escola.Nome.ToLower().Contains(filtro.Nome.Trim().ToLower()));
            }
            if (filtro.IdEtapaEnsino != null)
            {
                var etapas = filtro.IdEtapaEnsino.ConvertAll(e => (EtapaEnsino)e);
                query = query.Where(er => er.Escola.EtapasEnsino!.Any(etapa => etapas.Contains(etapa.EtapaEnsino)));
            }
            if (filtro.IdMunicipio != null)
            {
                query = query.Where(er => er.Escola.MunicipioId == filtro.IdMunicipio);
            }
            if (filtro.IdUf != null)
            {
                query = query.Where(er => er.Escola.Uf == (UF)filtro.IdUf);
            }

            var total = await query.CountAsync();
            var items = await query
                .OrderBy(er => er.Posicao)
                .Skip((filtro.Pagina - 1) * filtro.TamanhoPagina)
                .Take(filtro.TamanhoPagina)
                .ToListAsync();

            return new ListaPaginada<EscolaRanque>(items, filtro.Pagina, filtro.TamanhoPagina, total);
        }

        public async Task<List<EscolaRanque>> ListarEscolasAsync(int ranqueId)
        {
            return await dbContext.EscolaRanques.Where(er => er.RanqueId == ranqueId).OrderByDescending(e => e.Pontuacao).ToListAsync();
        }

        public async Task<EscolaRanque?> ObterEscolaRanquePorIdAsync(Guid escolaId, int ranqueId)
        {
            var escola = await dbContext.EscolaRanques
                .Where(er => er.EscolaId == escolaId && er.RanqueId == ranqueId)
                .Include(e => e.Escola).ThenInclude(e => e.Municipio)
                .Include(e => e.Escola).ThenInclude(e => e.EtapasEnsino)
                .Include(er => er.Escola).ThenInclude(e => e.Polo).ThenInclude(p => p.Municipio)
                .FirstOrDefaultAsync();
            return escola;
        }

        public async Task<ListaPaginada<Ranque>> ListarRanques( PesquisaEscolaFiltro filtro)
        {
            var query = dbContext.Ranques.Include(r => r.EscolaRanques).Where(r => r.DataFimUtc != null).AsQueryable();

            var total = await query.CountAsync();
            var items = await query
                .OrderByDescending(er => er.DataFimUtc)
                .Skip((filtro.Pagina - 1) * filtro.TamanhoPagina)
                .Take(filtro.TamanhoPagina)
                .ToListAsync();

            return new ListaPaginada<Ranque>(items, filtro.Pagina, filtro.TamanhoPagina, total);
        }
        public async Task<List<EscolaRanque>> ListarEscolaRanquesAsync(int ranqueId)
        {
            var query = dbContext.EscolaRanques
                .Include(er => er.Ranque)
                .Include(er => er.Escola).ThenInclude(e => e.EtapasEnsino)
                .Include(er => er.Escola).ThenInclude(e => e.Municipio)
                .Include(er => er.Escola).ThenInclude(e => e.Polo).ThenInclude(p => p.Municipio)
                .Where(er => er.RanqueId == ranqueId);

            var items = await query
                .OrderBy(er => er.Posicao)
                .ToListAsync();

            return items;
        }
    }
}
