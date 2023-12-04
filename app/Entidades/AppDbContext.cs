using api;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic.FileIO;

namespace app.Entidades
{
    public class AppDbContext : DbContext
    {
        public DbSet<Municipio> Municipios { get; set; }
        public DbSet<Escola> Escolas { get; set; }
        public DbSet<SolicitacaoAcao> Solicitacoes { get; set; }
        public DbSet<EscolaEtapaEnsino> EscolaEtapaEnsino { get; set; }
        public DbSet<Ranque> Ranques { get; set; }
        public DbSet<EscolaRanque> EscolaRanques { get; set; }
        public DbSet<Superintendencia> Superintendencias { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Escola>().HasMany(escola => escola.EtapasEnsino).WithOne(e => e.Escola);
            modelBuilder.Entity<Ranque>()
                .Property(r => r.Id).ValueGeneratedOnAdd();
            modelBuilder.Entity<EscolaRanque>()
                .Property(r => r.Id).ValueGeneratedOnAdd();
        }

        public void Popula()
        {
            PopulaMunicipiosPorArquivo(null, Path.Join(".", "Migrations", "Data", "municipios.csv"));

            PopulaSuperintendenciasPorArquivo(null, Path.Join(".", "Migrations", "Data", "superintendencias.csv"));
        }

        public List<Municipio>? PopulaMunicipiosPorArquivo(int? limit, string caminho)
        {
            var hasMunicipio = Municipios.Any();
            var municipios = new List<Municipio>();

            if (hasMunicipio)
            {
                return null;
            }

            using (var fs = File.OpenRead(caminho))
            using (var parser = new TextFieldParser(fs))
            {
                parser.TextFieldType = FieldType.Delimited;
                parser.SetDelimiters(",");

                var columns = new Dictionary<string, int> { { "id", 0 }, { "name", 1 }, { "uf", 2 } };

                while (!parser.EndOfData)
                {
                    var row = parser.ReadFields()!;
                    var municipio = new Municipio
                    {
                        Id = int.Parse(row[columns["id"]]),
                        Nome = row[columns["name"]],
                        Uf = (UF)int.Parse(row[columns["uf"]]),
                    };

                    municipios.Add(municipio);
                    if (limit.HasValue && municipios.Count >= limit.Value)
                    {
                        break;
                    }
                }
            }

            AddRange(municipios);
            SaveChanges();
            return municipios;
        }

        public List<Superintendencia>? PopulaSuperintendenciasPorArquivo(int? limit, string caminho)
        {
            var hasSuperintendencias = Superintendencias.Any();
            var superintendencias = new List<Superintendencia>();

            if (hasSuperintendencias)
                return null;

            using (var fs = File.OpenRead(caminho))
            using (var parser = new TextFieldParser(fs))
            {
                parser.TextFieldType = FieldType.Delimited;
                parser.SetDelimiters(";");

                var columns = new Dictionary<string, int>
                {
                    { "id", 0 }, { "endereco", 1 }, { "cep", 2 }, { "latitude", 3 }, { "longitude" , 4}, { "uf" , 5}
                };

                while (!parser.EndOfData)
                {
                    var row = parser.ReadFields()!;
                    var superintendencia = new Superintendencia
                    {
                        Id = int.Parse(row[columns["id"]]),
                        Endereco = row[columns["endereco"]],
                        Cep = row[columns["cep"]],
                        Latitude = row[columns["latitude"]],
                        Longitude = row[columns["longitude"]],
                        Uf = (UF)int.Parse(row[columns["uf"]]),
                    };

                    superintendencias.Add(superintendencia);
                    limit--;

                    if (limit == 0)
                        break;
                }
            }

            AddRange(superintendencias);
            SaveChanges();
            return superintendencias;

        }

    }
}