using System.Globalization;
using api;
using api.Polos;
using app.Entidades;
using app.Repositorios;
using app.Repositorios.Interfaces;
using app.Services.Interfaces;
using app.util;

namespace app.Services;

public class PoloService : IPoloService
{
    private readonly IPoloRepositorio _poloRepositorio;
    private readonly IMunicipioRepositorio _municipioRepositorio;
    private readonly IEscolaRepositorio _escolaRepositorio;
    private readonly AppDbContext _dbContext;
    private readonly ModelConverter _modelConverter;

    public PoloService(
        IPoloRepositorio poloRepositorio, 
        IMunicipioRepositorio municipioRepositorio, 
        AppDbContext dbContext, 
        ModelConverter modelConverter, 
        IEscolaRepositorio escolaRepositorio)
    {
        this._poloRepositorio = poloRepositorio;
        _municipioRepositorio = municipioRepositorio;
        _dbContext = dbContext;
        _modelConverter = modelConverter;
        _escolaRepositorio = escolaRepositorio;
    }

    public async Task<Polo> ObterPorIdAsync(int id)
    {
        return await _poloRepositorio.ObterPorIdAsync(id);
    }
    
    public async Task<PoloModel> ObterModelPorIdAsync(int id)
    {
        var polo = await _poloRepositorio.ObterPorIdAsync(id);
        return _modelConverter.ToModel(polo);
    }
    
    public async Task<Polo> CadastrarAsync(CadastroPoloDTO poloDto)
    {
        var municipioId = poloDto.MunicipioId;
        var municipio = await _municipioRepositorio.ObterPorIdAsync(municipioId);

        var poloNovo = _poloRepositorio.Criar(poloDto, municipio);

        // TODO: Analisar uma maneira de otimizar... 
        //Atualmente: Em todo cadastro de polo, calcula-se se o novo polo é mais próximo do que o polo referenciado em escola, para todas as escolas.
        var escolas = await _escolaRepositorio.ListarAsync();
        escolas.ForEach(e => e.SubstituirSeMaisProximo(poloNovo));
        
        await _dbContext.SaveChangesAsync();
        return poloNovo;
    }

    public async Task<ListaPaginada<PoloModel>> ListarPaginadaAsync(PesquisaPoloFiltro filtro)
    {
        var polos = await _poloRepositorio.ListarPaginadaAsync(filtro);
        var poloModels = polos.Items.ConvertAll(_modelConverter.ToModel);
        return new ListaPaginada<PoloModel>(poloModels, polos.Pagina, polos.ItemsPorPagina, polos.Total);
    }

    public async Task AtualizarAsync(Polo poloExistente, CadastroPoloDTO poloDto)
    {
        var municipioId = poloDto.MunicipioId;
        var municipio = await _municipioRepositorio.ObterPorIdAsync(municipioId);

        poloExistente.Endereco = poloDto.Endereco;
        poloExistente.Cep = poloDto.Cep;
        poloExistente.Latitude = poloDto.Latitude;
        poloExistente.Longitude = poloDto.Longitude;
        poloExistente.Nome = poloDto.Nome;
        poloExistente.Municipio = municipio;
        poloExistente.Uf = (UF)poloDto.IdUf;

        // TODO: Analisar uma maneira de otimizar... 
        //Atualmente: Em toda atualizacao de polo, calcula-se se o polo alterado é mais próximo do que o polo referenciado em escola, para todas as escolas.
        var escolas = await _escolaRepositorio.ListarAsync();
        escolas.ForEach(e => e.SubstituirSeMaisProximo(poloExistente));

        await _dbContext.SaveChangesAsync();
    }

    public async Task ExcluirAsync(Polo poloExistente)
    {
        // TODO: Analisar uma maneira de otimizar... 
        //Atualmente: Em toda exclusao de um polo, calcula-se um o novo polo mais próximo para todas as escolas que possuiam o polo deletado como mais próximo.
        var escolasComPoloExcluido = await _escolaRepositorio.ListarAsync(e => e.Polo == poloExistente);
        var polosRestantes = await _poloRepositorio.ListarAsync(p => p != poloExistente);
        
        foreach (var escola in escolasComPoloExcluido)
        {
            var (poloMaisProximo, distancia) = escola.CalcularPoloMaisProximo(polosRestantes);
            escola.Polo = poloMaisProximo;
            escola.DistanciaPolo = distancia.GetValueOrDefault();
        }
        _poloRepositorio.Excluir(poloExistente);

        await _dbContext.SaveChangesAsync();
    }
}
