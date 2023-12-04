namespace api.Escolas
{
    public class PesquisaSolicitacaoFiltro
    {
        public int Pagina { get; set; } = 1;
        public int TamanhoPagina { get; set; } = 10;
        public string? Nome { get; set; }
        public UF? Uf { get; set; }
        public int? IdMunicipio { get; set; }
        public int? QuantidadeAlunosMin {get; set;}
        public int? QuantidadeAlunosMax {get; set;}
    }
}
