using System.Linq.Expressions;
using api;
using api.Planejamento;
using app.Entidades;
using app.Repositorios.Interfaces;
using app.Services;
using EnumsNET;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;

namespace app.Repositorios
{
    public class PlanejamentoRepositorio : IPlanejamentoRepositorio
    {
        private readonly AppDbContext dbContext;

        public PlanejamentoRepositorio(
            AppDbContext dbContext
        )
        {
            this.dbContext = dbContext;
        }

        public void ExcluirPlanejamentoMacro(PlanejamentoMacro pm)
        {
            dbContext.PlanejamentoMacro.Remove(pm);
        }

        // Implemetar os metodos da inteface

        public void ExcluirPlanejamentoMacroEscola(PlanejamentoMacroEscola pm)
        {
            dbContext.PlanejamentoMacroEscola.Remove(pm);
        }

        public async Task<List<PlanejamentoMacro>> ListarAsync(Expression<Func<PlanejamentoMacro, bool>>? filter = null)
        {
            return await dbContext.PlanejamentoMacro
                .Include(e => e.Nome)
                .Where(filter ?? (e => true))
                .AsQueryable()
                .ToListAsync();
        }

        public async Task<ListaPaginada<PlanejamentoMacro>> ListarPaginadaAsync(PesquisaPlanejamentoFiltro filtro)
        {
            var query = dbContext.PlanejamentoMacro
                .Include(e => e.Escolas)
                .ThenInclude(e => e.Escola)
                .AsQueryable();
            
            if(filtro.Nome != null)
            {
                var nome = filtro.Nome.ToLower().Trim();
                query = query.Where(e => e.Nome.ToLower() == nome || e.Nome.ToLower().Contains(nome));
            }
            if(filtro.Responsavel != null)
            {
                var responsavel = filtro.Responsavel.ToLower().Trim();
                query = query.Where(e => e.Responsavel.ToLower() == responsavel || e.Responsavel.ToLower().Contains(responsavel));
            }
            if(filtro.Periodo != null){
                var periodo = filtro.Periodo.ToLower().Trim();
                query = query.Where(e => e.MesInicio != null || e.MesFim != null || e.AnoInicio != null || e.AnoFim != null);
            }


            var total = await query.CountAsync();
            var items = await query
                .OrderBy(e => e.Nome)
                .Skip((filtro.Pagina - 1) * filtro.TamanhoPagina)
                .Take(filtro.TamanhoPagina)
                .ToListAsync();
            
            return new ListaPaginada<PlanejamentoMacro>(items, filtro.Pagina, filtro.TamanhoPagina, total);
        }

        // public async Task<List<PlanejamentoMacro>> ListarAsync()
        // {
        //     throw new NotImplementedException();
        // }

        public async Task<PlanejamentoMacro> ObterPlanejamentoMacroAsync(Guid id)
        {
            return await dbContext.PlanejamentoMacro
                .Include(p => p.Escolas)
                .ThenInclude(e => e.Escola)
                .FirstOrDefaultAsync(x => x.Id==id) ?? throw new ApiException(api.ErrorCodes.PlanejamentoMacroNaoEncontrado);
        }

        public  PlanejamentoMacro RegistrarPlanejamentoMacro(PlanejamentoMacro pm)
        {
            dbContext.PlanejamentoMacro.Add(pm);
            return pm;
        }

        public void RegistrarPlanejamentoMacroMensal(PlanejamentoMacroEscola pme)
        {
            dbContext.PlanejamentoMacroEscola.Add(pme);            
        }
    }
}