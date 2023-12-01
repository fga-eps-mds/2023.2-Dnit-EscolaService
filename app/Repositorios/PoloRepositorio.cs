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
        return await dbContext.Polos.FirstOrDefaultAsync(x => x.Id == id)
            ?? throw new ApiException(ErrorCodes.PoloNaoEncontrado);
    }

    public async Task<List<Polo>> ListarAsync()
    {
        return await dbContext.Polos.ToListAsync();
    }

    public Polo Criar(CadastroPoloDTO poloDto, Municipio municipio)
    {
        var polo = new Polo
        {
            Id = poloDto.Id,
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

        if (filtro.idMunicipio != null)
        {
            query = query.Where(p => p.MunicipioId == filtro.idMunicipio);
        }

        if (filtro.idUf != null)
        {
            query = query.Where(p => p.Uf == (UF)filtro.idUf);
        }

        var total = await query.CountAsync();
        var items = await query
            .OrderBy(p => p.Nome)
            .Skip((filtro.Pagina - 1) * filtro.TamanhoPagina)
            .Take(filtro.TamanhoPagina)
            .ToListAsync();

        return new ListaPaginada<Polo>(items, filtro.Pagina, filtro.TamanhoPagina, total);
    }
}
