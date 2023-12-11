namespace api.Polos;

public class PesquisaPoloFiltro
{
    public int Pagina { get; set; } = 1;
    public int TamanhoPagina { get; set; } = 10;
    public string? Nome { get; set; }
    public string? Cep { get; set; }
    public int? IdUf { get; set; }
    public int? IdMunicipio { get; set; }
}
