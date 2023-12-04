using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using api;

namespace app.Entidades
{
    public class SolicitacaoAcao
    {
        [Key]
        public Guid Id { get; set; }
        public UF EscolaUf { get; set; }
        public int EscolaMunicipioId { get; set; }
        public Municipio? EscolaMunicipio { get; set; }

        /// <summary>
        /// Essa coluna sempre deve ser preenchida, mas não deve ser chave 
        /// estrangeira porque nem toda solicitação vai apontar para um escola
        /// cadastrada no sistema.
        /// </summary>
        [Required]
        public int EscolaCodigoInep { get; set; }
        [Required, MaxLength(200)]
        public string EscolaNome { get; set; }
        [Required]
        public int TotalAlunos { get; set; }
        public Guid? EscolaId { get; set; }
        public Escola? Escola { get; set; }
        [Required, MaxLength(150)]
        public string NomeSolicitante { get; set; }
        [Required, MaxLength(100)]
        public string Email { get; set; }

        [Required, MaxLength(20)]
        public string Vinculo { get; set; }
        [Required, MaxLength(20)]
        public string Telefone { get; set; }

        [Required, MaxLength(200)]
        public string Observacoes { get; set; }

        [NotMapped]
        public DateTimeOffset? DataRealizada { get; set; }

        public DateTime? DataRealizadaUtc
        {
            get => DataRealizada?.UtcDateTime;
            set => DataRealizada = value != null ? new DateTimeOffset(value.Value, TimeSpan.Zero) : null;
        }
    }
}
