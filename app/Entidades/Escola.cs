using api;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace app.Entidades
{
    public class Escola
    {
        [Key]
        public Guid Id { get; set; }

        [Required, MaxLength(200)]
        public string Nome { get; set; }

        [Required]
        public int Codigo { get; set; }

        [Required, MaxLength(8)]
        public string Cep { get; set; }

        [Required, MaxLength(200)]
        public string Endereco { get; set; }

        [Required, MaxLength(12)]
        public string Latitude { get; set; }

        [Required, MaxLength(12)]
        public string Longitude { get; set; }

        [Required]
        public int TotalAlunos { get; set; }

        [Required]
        public int TotalDocentes { get; set; }

        [Required, MaxLength(11)]
        public string Telefone { get; set; }

        [MaxLength(500)]
        public string? Observacao { get; set; }

        [Required]
        public Rede Rede { get; set; }
        
        [Required]
        public double DistanciaSuperintendencia { get; set; }
        
        public int? SuperintendenciaId { get; set; }
        public Superintendencia? Superintendencia { get; set; }
        public UF? Uf { get; set; }

        public Localizacao? Localizacao { get; set; }

        public Porte? Porte { get; set; }

        public Situacao? Situacao { get; set; }

        public int Ups { get; set; }

        public int? MunicipioId { get; set; }
        public Municipio? Municipio { get; set; }

        public List<EscolaEtapaEnsino>? EtapasEnsino { get; set; }

        [NotMapped]
        public DateTimeOffset? DataAtualizacao { get; set; }

        public DateTime? DataAtualizacaoUtc
        {
            get => DataAtualizacao?.UtcDateTime;
            set => DataAtualizacao = value != null ? new DateTimeOffset(value.Value, TimeSpan.Zero) : null;
        }
    }
}
