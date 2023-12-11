namespace api.Planejamento
{
  public class PesquisaPlanejamentoFiltro
  {
    public int Pagina { get; set; } = 1;
    public int TamanhoPagina { get; set; } = 10;
    public string? Nome { get; set; }
    public string? Periodo { get; set; }
    public string? Responsavel { get; set; }
    public int? QuantidadeAcoes { get; set; }
  }
}