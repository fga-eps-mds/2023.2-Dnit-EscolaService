namespace api.Escolas
{
    public class SolicitacaoAcaoData
    {
        public int EscolaCodigoInep { get; set; }
        public string Escola { get; set; }
        public UF Uf { get; set; }
        public string Municipio { get; set; }
        public int MunicipioId { get; set; }
        public string NomeSolicitante { get; set; }
        public string VinculoEscola { get; set; }
        public string Email { get; set; }
        public string Telefone { get; set; }
        public string[] CiclosEnsino { get; set; }
        public int QuantidadeAlunos { get; set; }
        public string Observacoes { get; set; }
    }
}
