﻿using api;
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

        public async Task<Ranque?> ObterUltimoRanqueAsync()
        {
            return await dbContext.Ranques
                .Where(r => r.DataFimUtc != null)
                .OrderByDescending(r => r.DataFimUtc)
                .FirstOrDefaultAsync();
        }

        public async Task<ListaPaginada<EscolaRanque>> ListarEscolasAsync(int ranqueId, PesquisaEscolaFiltro filtro)
        {
            var query = dbContext.EscolaRanques
                .Include(er => er.Ranque)
                .Include(er => er.Escola).ThenInclude(e => e.EtapasEnsino)
                .Include(er => er.Escola).ThenInclude(e => e.Municipio)
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
                .OrderByDescending(er => er.Pontuacao)
                .Skip((filtro.Pagina - 1) * filtro.TamanhoPagina)
                .Take(filtro.TamanhoPagina)
                .ToListAsync();

            return new ListaPaginada<EscolaRanque>(items, filtro.Pagina, filtro.TamanhoPagina, total);
        }
    }
}