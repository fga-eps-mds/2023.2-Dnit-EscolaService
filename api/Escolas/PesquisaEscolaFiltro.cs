namespace api.Escolas
{
    public class PesquisaEscolaFiltro
    {
        public string? Nome { get; set; }
        public int Pagina { get; set; } = 1;
        public int TamanhoPagina { get; set; } = 10;
        public int? IdUf { get; set; }
        public int? IdSituacao { get; set; }
        public List<int>? IdEtapaEnsino { get; set; }
        public int? IdMunicipio { get; set; }
    }
}
