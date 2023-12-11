using api;
using app.Services;
using EnumsNET;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace app.Entidades
{
    public class Escola : ISerializable
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
        public double DistanciaPolo { get; set; }
        
        public int? PoloId { get; set; }
        public Polo? Polo { get; set; }
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
        public SolicitacaoAcao? Solicitacao { get; set; }

        public List<object?> Serialize()
        {
            return new ()
            {
                Id, Nome, Latitude, Longitude, TotalAlunos, TotalDocentes, Uf?.ToString(),
                Rede.ToString(), Porte?.AsString(EnumFormat.Description), Localizacao?.AsString(EnumFormat.Description),
                Situacao?.AsString(EnumFormat.Description), string.Join("_", EtapasEnsino!.Select(e => e.EtapaEnsino.AsString(EnumFormat.Description))),
                Codigo, $"({Telefone.Substring(0, 2)}){Telefone.Substring(2)}", PoloId
            };
        }

        public static List<string> SerializeHeaders()
        {
            return new() {
                "Id", "Nome", "Latitude", "Longitude",
                "TotalAlunos", "TotalDocentes", "Uf",
                "Rede", "Porte", "Localização", "Situação", "EtapasEnsino",
                "Codigo", "Telefone", "PoloId", "DistânciaPolo",
            };
        }
    }
}
