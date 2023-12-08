using app.Controllers;
using app.Entidades;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using test.Fixtures;
using test.Stubs;
using Xunit.Abstractions;

namespace test
{
	public class PriorizacaoControllerTest : AuthTest
	{
        private readonly PriorizacaoController controller;
        private readonly AppDbContext db;

        public PriorizacaoControllerTest(ITestOutputHelper testOutputHelper, Base fixture) : base(testOutputHelper, fixture)
        {
            db = fixture.GetService<AppDbContext>(testOutputHelper)!;
            db.PopulaCustosLogisticos(4);
            db.PopulaPriorizacao(4);
            controller = fixture.GetService<PriorizacaoController>(testOutputHelper)!;
        }

        [Fact]
        public async Task ListarCustosLogisticos_QuandoMetodoForChamado_RetornaCustosLogisticos()
        {
            var custosLogisticosDb = db.CustosLogisticos.ToList();
            var result = await controller.ListarCustosLogisticos();
            Assert.Equal(custosLogisticosDb.Count, result.Count);
        }

        [Fact]
        public async Task EditarCustosLogisticos_QuandoMetodoForChamado_RetornaParametrosDeCustoAtualizados()
        {
            var custoLogisticoAtualizado = CustoLogisticoStub.ObterCustoLogisticoAtualizado();
            await controller.EditarCustosLogisticos(custoLogisticoAtualizado);
            var custoLogisticoDb = db.CustosLogisticos.ToList();

            var item = custoLogisticoAtualizado.First();
            var itemDb = custoLogisticoDb.FirstOrDefault(c => c.Custo == item.Custo);

            Assert.NotNull(itemDb);
            Assert.Equal(item.Custo, itemDb.Custo);
            Assert.Equal(item.RaioMin, itemDb.RaioMin);
            Assert.Equal(item.RaioMax, itemDb.RaioMax);
            Assert.Equal(item.Valor, itemDb.Valor);
        }

        [Fact]
        public async Task VisualizarFatorId_QuandoColocarId_DeveRetornarOk()
        {
            var priorizacoes = db.FatorPriorizacoes.ToList();
            var visualizar = await controller.VisualizarFatorId(priorizacoes.First().Id);

            Assert.NotNull(priorizacoes);
            Assert.NotNull(visualizar);
            Assert.Equal(visualizar.Nome, priorizacoes[0].Nome);  
        }

        [Fact]
        public async Task ListarFatores_QuandoMetodoForChamado_DeveRetornarLista()
        {
           var priorizacoes = db.FatorPriorizacoes.ToList();
           var visualizar = await controller.ListarFatores();

            Assert.NotNull(priorizacoes);
            Assert.NotNull(visualizar);
            Assert.Equal(visualizar.Count, priorizacoes.Count);  
        }

        [Fact]
        public async Task DeletarFatorId_QuandoColocadoId_DeveRetornarOk()
        {
            var priorizacao = db.FatorPriorizacoes.FirstOrDefault();
            
            Assert.NotNull(priorizacao);

            var resposta = await controller.DeletarFator(priorizacao.Id);

            Assert.IsType<OkObjectResult>(resposta);  
        }

        [Fact]
        public async Task AdicionarFatorPriorizacao_QuandoFornecidoNovoFator_DeveRetornarFatorAdicionado()
        {
            var novoFator = PriorizacaoStub.ListarFatorPrioriModel(false).First();

            var resposta = await controller.AdicionarFatorPriorizacao(novoFator);

            Assert.NotNull(resposta);
            Assert.Equal(novoFator.Nome, resposta.Nome);
            Assert.Equal(novoFator.Ativo, resposta.Ativo);
            Assert.Equal(novoFator.Primario, resposta.Primario);
            Assert.Equal(novoFator.Peso, resposta.Peso);
        }
    }
}