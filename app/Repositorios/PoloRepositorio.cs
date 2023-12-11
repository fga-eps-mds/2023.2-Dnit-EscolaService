using System.Linq.Expressions;
using api;
using api.Polos;
using app.Entidades;
using app.Repositorios.Interfaces;
using app.Services;
using Microsoft.EntityFrameworkCore;

namespace app.Repositorios;

public class PoloRepositorio : IPoloRepositorio
{

    private readonly AppDbContext dbContext;

    public PoloRepositorio(AppDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public async Task<Polo> ObterPorIdAsync(int id)
    {
        return await dbContext.Polos.Include(p => p.Municipio)
                   .FirstOrDefaultAsync(x => x.Id == id)
            ?? throw new ApiException(ErrorCodes.PoloNaoEncontrado);
    }

    public async Task<List<Polo>> ListarAsync(Expression<Func<Polo, bool>>? filter = null)
    {
        return await dbContext.Polos.Where(filter ?? (p => true)).AsQueryable().ToListAsync();
    }

    public Polo Criar(CadastroPoloDTO poloDto, Municipio municipio)
    {
        var polo = new Polo
        {
            Nome = poloDto.Nome,
            Municipio = municipio,
            Longitude = poloDto.Longitude,
            Latitude = poloDto.Latitude,
            Uf = (UF)poloDto.IdUf,
            Endereco = poloDto.Endereco,
            Cep = poloDto.Cep,
        };
        dbContext.Add(polo);
        return polo;
    }

    public async Task<ListaPaginada<Polo>> ListarPaginadaAsync(PesquisaPoloFiltro filtro)
    {
        var query = dbContext.Polos
            .Include(p => p.Municipio)
            .AsQueryable();

        if (!string.IsNullOrEmpty(filtro.Nome))
        {
            var nome = filtro.Nome.ToLower().Trim();
            query = query.Where(p => p.Nome.ToLower() == nome 
                                     || p.Nome.ToLower().Contains(nome));
        }

        if (filtro.Cep != null)
        {
            var cep = filtro.Cep.Trim();
            query = query.Where(p => p.Cep == cep 
                                     || p.Cep.Contains(cep));
        }

        if (filtro.IdMunicipio != null)
        {
            query = query.Where(p => p.MunicipioId == filtro.IdMunicipio);
        }

        if (filtro.IdUf != null)
        {
            query = query.Where(p => p.Uf == (UF)filtro.IdUf);
        }

        var total = await query.CountAsync();
        var items = await query
            .OrderBy(p => p.Nome)
            .Skip((filtro.Pagina - 1) * filtro.TamanhoPagina)
            .Take(filtro.TamanhoPagina)
            .ToListAsync();

        return new ListaPaginada<Polo>(items, filtro.Pagina, filtro.TamanhoPagina, total);
    }

    public void Excluir(Polo polo)
    {
        dbContext.Remove(polo);

        dbContext.SaveChanges();
    }
}
