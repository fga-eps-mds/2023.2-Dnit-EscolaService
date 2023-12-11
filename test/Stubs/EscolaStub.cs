using api;
using api.Escolas;
using app.Entidades;
using System.Collections.Generic;
using System.Linq;

namespace test.Stubs
{
    public static class EscolaStub
    {
        public static IEnumerable<Escola> ListarEscolas(IEnumerable<Municipio> municipios, bool comEtapas)
        {
            while (true)
            {
                var escola = new Escola
                {
                    Id = Guid.NewGuid(),
                    DataAtualizacao = DateTime.Now,
                    Cep = $"7215436{Random.Shared.Next() % 10}",
                    Endereco = $"Endereço Teste {Random.Shared.Next()}",
                    Codigo = Random.Shared.Next() % 1000,
                    Latitude = Random.Shared.NextDouble().ToString().Truncate(12),
                    Longitude = Random.Shared.NextDouble().ToString().Truncate(12),
                    Localizacao = Enum.GetValues<Localizacao>().TakeRandom(true).FirstOrDefault(),
                    Municipio = municipios.TakeRandom().First(),
                    Nome = $"Escola DNIT {Random.Shared.Next()}",
                    Porte = Enum.GetValues<Porte>().TakeRandom(true).FirstOrDefault(),
                    Rede = Enum.GetValues<Rede>().TakeRandom(true).FirstOrDefault(),
                    Situacao = Enum.GetValues<Situacao>().TakeRandom(true).FirstOrDefault(),
                    Telefone = "52426252",
                    TotalAlunos = Random.Shared.Next() % 100 + 1,
                    TotalDocentes = Random.Shared.Next() % 100 + 1,
                    Uf = Enum.GetValues<UF>().TakeRandom().FirstOrDefault(),
                    Polo = new Polo
                    {
                        Id = Random.Shared.Next(),
                        Nome = $"Polo DNIT {Random.Shared.Next()}",
                        Municipio = municipios.TakeRandom().First(),
                        Cep = $"7215436{Random.Shared.Next() % 10}",
                        Endereco = $"Endereço Teste {Random.Shared.Next()}",
                        Uf = Enum.GetValues<UF>().TakeRandom().FirstOrDefault(),
                        Latitude = Random.Shared.NextDouble().ToString().Truncate(12),
                        Longitude = Random.Shared.NextDouble().ToString().Truncate(12),
                    }
                };
                if (comEtapas)
                {
                    escola.EtapasEnsino = Enum.GetValues<EtapaEnsino>().TakeRandom().ConvertAll(etapa => new EscolaEtapaEnsino
                    {
                        Id = Guid.NewGuid(),
                        EscolaId = escola.Id,
                        Escola = escola,
                        EtapaEnsino = etapa,
                    });
                }
                yield return escola;
            }
        }

        public static IEnumerable<CadastroEscolaData> ListarEscolasDto(IEnumerable<Municipio> municipios, bool comEtapas)
        {
            while (true)
            {
                var escola = new CadastroEscolaData
                {
                    Cep = $"7215436{Random.Shared.Next() % 10}",
                    Endereco = $"Endereço Teste {Random.Shared.Next()}",
                    CodigoEscola = Random.Shared.Next() % 1000,
                    Latitude = Random.Shared.NextDouble().ToString().Truncate(12),
                    Longitude = Random.Shared.NextDouble().ToString().Truncate(12),
                    IdLocalizacao = (int?)Enum.GetValues<Localizacao>().TakeRandom(true).FirstOrDefault(),
                    IdMunicipio = municipios.TakeRandom().First().Id,
                    NomeEscola = $"Escola DNIT {Random.Shared.Next()}",
                    IdPorte = (int?)Enum.GetValues<Porte>().TakeRandom(true).FirstOrDefault(),
                    IdRede = (int)Enum.GetValues<Rede>().TakeRandom(true).FirstOrDefault(),
                    IdSituacao = (int?)Enum.GetValues<Situacao>().TakeRandom(true).FirstOrDefault(),
                    Telefone = "52426252",
                    NumeroTotalDeAlunos = Random.Shared.Next() % 100 + 1,
                    NumeroTotalDeDocentes = Random.Shared.Next() % 100 + 1,
                    IdUf = (short)Enum.GetValues<UF>().TakeRandom().First(),
                    UltimaAtualizacao = DateTime.Now,
                };
                if (comEtapas)
                {
                    escola.IdEtapasDeEnsino = Enum.GetValues<EtapaEnsino>().TakeRandom().ConvertAll(etapa => (int)etapa);
                }
                yield return escola;
            }
        }

        public static IEnumerable<AtualizarDadosEscolaData> ListarAtualizarEscolasDto(IEnumerable<Municipio> municipios, bool comEtapas)
        {
            while (true)
            {
                var escola = new AtualizarDadosEscolaData
                {
                    Latitude = Random.Shared.NextDouble().ToString().Truncate(12),
                    Longitude = Random.Shared.NextDouble().ToString().Truncate(12),
                    IdSituacao = (int?)Enum.GetValues<Situacao>().TakeRandom(true).FirstOrDefault(),
                    Telefone = "52426252",
                    NumeroTotalDeAlunos = Random.Shared.Next() % 100 + 1,
                    NumeroTotalDeDocentes = Random.Shared.Next() % 100 + 1,
                    Observacao = $"Observacao Teste {Random.Shared.Next()}",
                };
                if (comEtapas)
                {
                    escola.IdEtapasDeEnsino = Enum.GetValues<EtapaEnsino>().TakeRandom().ConvertAll(etapa => (int)etapa);
                }
                yield return escola;
            }
        }
    }
}
