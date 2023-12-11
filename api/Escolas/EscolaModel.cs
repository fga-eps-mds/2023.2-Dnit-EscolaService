using api.Solicitacoes;

namespace api.Escolas
{
    public class EscolaModel
    {
        public Guid IdEscola { get; set; }
        public int CodigoEscola { get; set; }
        public string NomeEscola { get; set; }
        public int? IdRede { get; set; }
        public string? DescricaoRede { get; set; }
        public string Cep { get; set; }
        public int? IdUf { get; set; }
        public UF? Uf { get; set; }
        public string? DescricaoUf { get; set; }
        public string Endereco { get; set; }
        public int? IdMunicipio { get; set; }
        public string? NomeMunicipio { get; set; }
        public int? IdLocalizacao { get; set; }
        public string? DescricaoLocalizacao { get; set; }
        public string? Longitude { get; set; }
        public string? Latitude { get; set; }
        public int? IdEtapasDeEnsino { get; set; }
        public string DescricaoEtapasEnsino { get; set; }
        public int? NumeroTotalDeAlunos { get; set; }
        public int? IdSituacao { get; set; }
        public string DescricaoSituacao { get; set; }
        public int? IdPorte { get; set; }
        public string? DescricaoPorte { get; set; }
        public string Telefone { get; set; }
        public int NumeroTotalDeDocentes { get; set; }
        public string? SiglaUf { get; set; }
        public string? Observacao { get; set; }
        public Rede? Rede { get; set; }
        public Porte? Porte { get; set; }
        public Localizacao? Localizacao { get; set; }
        public List<EtapaEnsino>? EtapasEnsino { get; set; }
        public Situacao? Situacao { get; set; }
        public double DistanciaPolo { get; set; }
        public int? PoloId { get; set; }
        public string? UfPolo { get; set; }
        public bool TemSolicitacao { get; set; } = false;
    }
}
