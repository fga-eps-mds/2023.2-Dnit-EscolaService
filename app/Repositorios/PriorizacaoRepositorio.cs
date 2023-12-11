using api;
using api.CustoLogistico;
using api.Fatores;
using app.Entidades;
using app.Repositorios.Interfaces;
using app.Services;
using Microsoft.EntityFrameworkCore;

namespace app.Repositorios
{
    public class PriorizacaoRepositorio : IPriorizacaoRepositorio
    {
        private readonly AppDbContext dbContext;

        public PriorizacaoRepositorio(
            AppDbContext dbContext
        )
        {
            this.dbContext = dbContext;
        }

        public async Task<FatorPriorizacao>  ObterFatorPrioriPorIdAsync(Guid prioriId)
        {
            return await dbContext.FatorPriorizacoes
                .Include(f => f.FatorCondicoes)
                .FirstOrDefaultAsync(c => c.Id == prioriId && c.DeleteTime == null) ?? throw new ApiException(ErrorCodes.FatorNaoEncontrado); 
        }

        public async Task<FatorCondicao>  ObterFatorCondiPorIdAsync(Guid condicaoId)
        {
            return await dbContext.FatorCondicoes.FirstOrDefaultAsync(c => c.Id == condicaoId) ?? throw new ApiException(ErrorCodes.FatorNaoEncontrado);
        }

        public async Task<FatorEscola> ObterFatorEscolaPorIdAsync(Guid escolaId)
        {
            return await dbContext.FatorEscolas.FirstOrDefaultAsync(c => c.EscolaId == escolaId) ?? throw new ApiException(ErrorCodes.FatorNaoEncontrado);
        }

        public async Task DeletarFatorId(Guid Id)
        {
            FatorPriorizacao item = await ObterFatorPrioriPorIdAsync(Id);
            item.DeleteTime = DateTime.UtcNow;
            item.Ativo = false;
            dbContext.FatorPriorizacoes.Update(item);
            dbContext.SaveChanges();
        }

        public async Task<List<FatorPriorizacao>> ListarFatoresAsync()
        {
            return await dbContext.FatorPriorizacoes
                .Include(f => f.FatorCondicoes)
                .Where(c => c.DeleteTime == null)
                .OrderBy(c => c.Id)
                .ToListAsync();
        }

        public async Task<List<CustoLogistico>> ListarCustosLogisticosAsync()
        {
            return await dbContext.CustosLogisticos
                .OrderBy(c => c.Custo)
                .ToListAsync();
        }

        public FatorCondicao AdicionarFatorCondicao(FatorCondicaoModel fatorCondicao)
        {
            Guid id = Guid.NewGuid();
            FatorCondicao fator = new FatorCondicao
            {
                Id = id,
                Propriedade = (PropriedadeCondicao)fatorCondicao.Propriedade,
                Operador = (OperacaoCondicao)fatorCondicao.Operador,
                Valores = ConverterValoresFatorCondicao(id, fatorCondicao.Valores),
                FatorPriorizacaoId = (Guid)fatorCondicao.FatorPriorizacaoId,
            };

            dbContext.FatorCondicoes.Add(fator);
            return fator;
        }

        private List<CondicaoValor> ConverterValoresFatorCondicao(Guid id, List<string> valores)
        {
            return valores.ConvertAll(v => new CondicaoValor{ FatorCondicaoId = id, Valor = v });
        }

        public FatorPriorizacao AdicionarFatorPriorizacao(FatorPriorizacao novoFator)
        {
            foreach(var condicao in novoFator.FatorCondicoes)
            {
                condicao.FatorPriorizacaoId = novoFator.Id;
            }

            dbContext.FatorPriorizacoes.Add(novoFator);
            return novoFator;
        }

        public async Task<List<CustoLogistico>> EditarCustosLogisticos(List<CustoLogisticoItem> custoItems)
        {
            var custosAtualizados = new List<CustoLogistico>();

            foreach (var item in custoItems)
            {
                var custoLogistico = await dbContext.CustosLogisticos.FirstOrDefaultAsync(c => c.Custo == item.Custo);

                if (custoLogistico != null)
                {
                    custoLogistico.RaioMax = item.RaioMax;
                    custoLogistico.RaioMin = item.RaioMin;
                    custoLogistico.Valor = item.Valor;
                    custosAtualizados.Add(custoLogistico);
                }
            }

            return custosAtualizados;
        }
    }
}
