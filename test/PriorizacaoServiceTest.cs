using app.Entidades;
using app.Repositorios.Interfaces;
using app.Services.Interfaces;
using service.Interfaces;
using System.Linq;
using System.Threading.Tasks;
using test.Fixtures;
using test.Stubs;
using Xunit.Abstractions;
using Xunit.Microsoft.DependencyInjection.Abstracts;

namespace test
{
	public class PriorizacaoServiceTest : TestBed<Base>
	{
		private readonly IPriorizacaoService priorizacaoService;
		private readonly IPriorizacaoRepositorio priorizacaoRepositorio;
		private readonly AppDbContext db;

		public PriorizacaoServiceTest(ITestOutputHelper testOutputHelper, Base fixture) : base(testOutputHelper, fixture)
        {
            db = fixture.GetService<AppDbContext>(testOutputHelper)!;
            db.PopulaCustosLogisticos(4);
            priorizacaoService = fixture.GetService<IPriorizacaoService>(testOutputHelper)!;
			priorizacaoRepositorio = fixture.GetService<IPriorizacaoRepositorio>(testOutputHelper)!;
        }

        [Fact]
        public async Task EditarCustosLogisticos_QuandoCustoForValido_RetornaParametrosDeCustoLogisticoItemAtualizados()
        {
            var custoValido = CustoLogisticoStub.ObterCustoLogisticoAtualizado();
            var custoAtualizado = await priorizacaoService.EditarCustosLogisticos(custoValido);
            
            foreach (var itemAtualizado in custoAtualizado)
            {
                var itemValido = custoValido.Where(c => c.Custo == itemAtualizado.Custo).FirstOrDefault();
                Assert.NotNull(itemValido);
                Assert.Equal(itemValido.Custo, itemAtualizado.Custo);
                Assert.Equal(itemValido.RaioMin, itemAtualizado.RaioMin);
                Assert.Equal(itemValido.RaioMax, itemAtualizado.RaioMax);
                Assert.Equal(itemValido.Valor, itemAtualizado.Valor);
            }
        }

        [Fact]
		public async Task EditarCustosLogisticos_QuandoTiverTamanhoIncorreto_DeveRetornarExcecaoComMensagem()
		{
            var custoTamanhoInvalido = CustoLogisticoStub.ObterCustoLogisticoTamanhoInvalido();
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(
                () => priorizacaoService.EditarCustosLogisticos(custoTamanhoInvalido));
            Assert.Equal("Operação Inválida: Deve conter as 4 categorias de custo logístico", exception.Message);
		}

        [Fact]
        public async Task EditarCustosLogisticos_QuandoRaioMinForDiferenteDoRaioMaxAnterior_DeveRetornarExcecaoComMensagem()
        {
            var custoRaioInvalido = CustoLogisticoStub.ObterCustoLogisticoComRaioMinDiferenteDoRaioMaxAnterior();
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(
                () => priorizacaoService.EditarCustosLogisticos(custoRaioInvalido));
            Assert.Equal("Operação Inválida: O RaioMin deve ser igual ao RaioMax anterior", exception.Message);
        }

        [Fact]
        public async Task EditarCustosLogisticos_QuandoRaioMinForMaiorOuIgualAoRaioMax_DeveRetornarExcecaoComMensagem()
        {
            var custoRaioInvalido = CustoLogisticoStub.ObterCustoLogisticoComRaioMinMaiorOuIgualAoRaioMax();
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(
                () => priorizacaoService.EditarCustosLogisticos(custoRaioInvalido));
            Assert.Equal("Operação Inválida: O RaioMin deve ser menor que o RaioMax", exception.Message);
        }

        [Fact]
        public async Task EditarCustosLogisticos_QuandoTiverDuplicacaoDeCusto_DeveRetornarExcecaoComMensagem()
        {
            var custoDuplicado = CustoLogisticoStub.ObterCustoLogisticoComDuplicacao();
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(
                () => priorizacaoService.EditarCustosLogisticos(custoDuplicado));
            Assert.Equal("Operação Inválida: Categorias de custo logístico repetidas", exception.Message);
        }

        [Fact]
        public async Task EditarCustosLogisticos_QuandoTiverCustoInvalido_DeveRetornarExcecaoComMensagem()
        {
            var custoInvalido= CustoLogisticoStub.ObterCustoLogisticoComCustoInvalido();
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(
                () => priorizacaoService.EditarCustosLogisticos(custoInvalido));
            Assert.Equal("Operação Inválida: Deve conter categorias de 1 a 4", exception.Message);
        }
    }
}