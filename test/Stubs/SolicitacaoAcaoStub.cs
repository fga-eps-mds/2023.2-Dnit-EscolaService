using api;
using api.Escolas;
using app.Entidades;

using System.Collections.Generic;
using System.Linq;

namespace test.Stubs
{
    public class SolicitacaoAcaoStub
    {

        public SolicitacaoAcaoData ObterSolicitacaoAcaoDTO()
        {
            return new()
            {
                Escola = "Escola Teste",
                Uf = UF.DF,
                Municipio = "Brasília",
                NomeSolicitante = "João Testador",
                VinculoEscola = "Professor",
                Email = "joao@email.com",
                Telefone = "123123123",
                CiclosEnsino = new[] { "Ensino Médio", "Ensino Fundamental" },
                QuantidadeAlunos = 503,
                Observacoes = "Teste de Solicitação"
            };
        }

        public static IEnumerable<SolicitacaoAcao> ListarSolicitacoes(IEnumerable<Municipio> municipios)
        {
            while (true) {
                var sol = new SolicitacaoAcao ()
                {
                    Id = Guid.NewGuid(),
                    EscolaMunicipio = municipios.TakeRandom().First(),
                    EscolaUf = UF.DF,
                    EscolaNome = "Nome de escola",
                    EscolaId = null,
                    EscolaCodigoInep = Random.Shared.Next(),
                    DataRealizada = DateTime.Now,
                    NomeSolicitante = "João Testador",
                    Vinculo = "Professor",
                    Email = "joao@email.com",
                    Telefone = "123123123",
                    TotalAlunos = Random.Shared.Next() + 1,
                    Observacoes = "Teste de Solicitação",
                };

                yield return sol;
            }
        }
    }
}
