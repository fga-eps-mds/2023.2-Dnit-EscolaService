using api;
using api.Escolas;
using app.Entidades;
using app.Repositorios.Interfaces;
using Hangfire;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using service.Interfaces;

namespace app.Services
{
    public class CalcularRanqueJob : ICalcularRanqueJob
    {
        private readonly AppDbContext dbContext;
        private readonly IBackgroundJobClient jobClient;
        private readonly IEscolaRepositorio escolaRepositorio;
        private readonly IRanqueService ranqueService;
        private readonly IOptions<UpsServiceConfig> upsConfig;
        private readonly IUpsService upsService;

        public CalcularRanqueJob(
            AppDbContext dbContext,
            IBackgroundJobClient jobClient,
            IEscolaRepositorio escolaRepositorio,
            IRanqueService ranqueService,
            IOptions<UpsServiceConfig> config,
            IUpsService upsService
        )
        {
            this.dbContext = dbContext;
            this.jobClient = jobClient;
            this.escolaRepositorio = escolaRepositorio;
            this.ranqueService = ranqueService;
            this.upsConfig = config;
        }

        [MaximumConcurrentExecutions(1)]
        public async Task ExecutarAsync(int novoRanqueId, int timeoutMinutos)
        {
            var TamanhoBatelada = 50;

            var totalEscolas = await dbContext.Escolas.CountAsync();
            var filtro = new PesquisaEscolaFiltro { TamanhoPagina = TamanhoBatelada};
            var totalPaginas = Math.Ceiling((double)totalEscolas / TamanhoBatelada);

            var ranque = dbContext.Ranques.Where(r => r.Id == novoRanqueId).First();

            for (int pagina = 1; pagina <= totalPaginas; pagina++)
            {
                filtro.Pagina = pagina;
                var lista = await escolaRepositorio.ListarPaginadaAsync(filtro);
                
                await CalcularUpsEscolas(lista.Items);
                CalcularPontuacaoEscolas(lista.Items, novoRanqueId);
                
                ranque.BateladasEmProgresso = (int)totalPaginas - pagina;
                
                dbContext.Update(ranque);
                dbContext.SaveChanges();
            }

            await ranqueService.ConcluirRanqueamentoAsync(ranque);
            dbContext.SaveChanges();
        }

        public async Task CalcularUpsEscolas(List<Escola> escolas)
        {
            var raio = 2.0D;
            var desde = 2019;

            HttpClient httpClient = new();
            UpsService upsService = new UpsService(
                httpClient,
                upsConfig
            );

            var upss = await upsService.CalcularUpsEscolasAsync(escolas, raio, desde, 20);

            for (int i = 0; i < escolas.Count; i++)
            {
                escolas[i].Ups = upss[i];
            }
            dbContext.UpdateRange(escolas);
            dbContext.SaveChanges();
        }

        private void CalcularPontuacaoEscolas(List<Escola> escolas, int ranqueId)
        {
            var upsMax = dbContext.Escolas.Max(e => e.Ups);

            var fatorUps = dbContext.FatorPriorizacoes
                .Where(f => f.Nome.Contains("UPS") && f.Ativo && f.Primario && f.DeleteTime == null).First();

            var fatorCustoLogisticos = dbContext.FatorPriorizacoes
                .Where(f => f.Nome.Contains("Custo Logistico") && f.Ativo && f.Primario && f.DeleteTime == null).First();

            var custosLogisticos = dbContext.CustosLogisticos.ToList();

            var outrosFatores = dbContext.FatorPriorizacoes
                .Include(f => f.FatorCondicoes)
                .Where(f => !f.Primario && f.DeleteTime == null && f.Ativo)
                .ToList();

            try
            {
                if (fatorUps != null)
                    SalvarFatorRanque(fatorUps, ranqueId);
                
                if (fatorCustoLogisticos != null)
                    SalvarFatorRanque(fatorCustoLogisticos, ranqueId);
                
                foreach(var fator in outrosFatores)
                {
                    SalvarFatorRanque(fator, ranqueId);
                }
            }
            catch{}
            
            foreach(var escola in escolas)
            {
                // Preenche Ups em fatorEscola
                if (fatorUps != null)
                    CalcularESalvarFatorUps(escola, fatorUps, upsMax);

                // Preenche CustoLogistico em fatorEscola
                if (fatorCustoLogisticos != null)
                    CalcularESalvarFatorCustoLogistico(escola, fatorCustoLogisticos, custosLogisticos);

                // Preenche Outros Fatores em fatorEscola
                CalcularESalvarOutrosFatores(escola, outrosFatores);

                dbContext.SaveChanges();

                // Finalmente a pontuação
                CalcularESalvarPontuacao(escola, ranqueId);
            }
        }

        public bool ExisteFatorEscola(FatorPriorizacao fator, Escola escola)
        {
            return dbContext.FatorEscolas.Any(f => f.FatorPriorizacao.Id == fator.Id && f.EscolaId == escola.Id);
        }

        private void CalcularESalvarFatorUps(Escola escola, FatorPriorizacao fatorUps, int UpsMax)
        {
            if (ExisteFatorEscola(fatorUps, escola)) return;

            // valorNormalizado = ((valor - min) / (max - min)) * 100
            // Como min será sempre zero, então 
            // valorNormalizado = (valor / max) * 100
            double valorNormalizado = (double)escola.Ups / UpsMax * 100;

            // Valor de pontucao do ups = valorNormalizado * peso / 100
            double valor = valorNormalizado * fatorUps.Peso / 100;

            var fatorEscola = new FatorEscola
            {
                EscolaId = escola.Id,
                FatorPriorizacaoId = fatorUps.Id,
                Valor = (int)valor
            };
            
            dbContext.Add(fatorEscola);
        }

        private void CalcularESalvarFatorCustoLogistico(Escola escola, FatorPriorizacao fator, List<CustoLogistico> custoLogisticos)
        {
            if (ExisteFatorEscola(fator, escola)) return;

            // Valor de pontuacao do custo logistico = Valor(CL) * peso / 100
            int valor = getCustoLogisticoValue(custoLogisticos, escola.DistanciaPolo);
            
            dbContext.Add(new FatorEscola
                {
                    EscolaId = escola.Id,
                    FatorPriorizacaoId = fator.Id,
                    Valor = valor
                });
        }

        private int getCustoLogisticoValue(List<CustoLogistico> custoLogisticos, double raio)
        {
            foreach(var custo in custoLogisticos)
            {
                if (custo.RaioMax == null)
                {
                    return custo.Valor;
                }

                if (raio >= custo.RaioMin && raio < custo.RaioMax)
                {
                    return custo.Valor;
                }
            }
            return 0;
        }

        private void CalcularESalvarOutrosFatores(Escola escola, List<FatorPriorizacao> fatores)
        {
            foreach(var fator in fatores)
            {
                if (ExisteFatorEscola(fator, escola)) continue;
                
                FatorEscola fatorEscola = new FatorEscola
                {
                    EscolaId = escola.Id,
                    FatorPriorizacaoId = fator.Id,
                    Valor = CalcularCondicoes(escola, fator.FatorCondicoes) * fator.Peso
                };

                dbContext.Add(fatorEscola);
            }
        }

        private int CalcularCondicoes(Escola escola, List<FatorCondicao> condicoes)
        {
            var resultadoBooleano = true;
            foreach(var condicao in condicoes)
            {
                var eval = EvaluateCondicao(escola, condicao);
                resultadoBooleano = resultadoBooleano && eval;
                if (!resultadoBooleano) break;
            }

            return resultadoBooleano ? 1 : 0;
        }

        private bool EvaluateCondicao(Escola escola, FatorCondicao condicao)
        {
            var valores = condicao.Valores.ConvertAll(v => CondicaoValorToInt(v.Valor));

            switch (condicao.Propriedade)
            {
                case PropriedadeCondicao.Porte:
                    return valores.Contains((int)(escola.Porte ?? 0));
                     
                case PropriedadeCondicao.Situacao:
                    return valores.Contains((int)(escola.Situacao ?? 0));
                
                case PropriedadeCondicao.Municipio:
                    return valores.Contains(escola.MunicipioId ?? 0);
                
                case PropriedadeCondicao.UF:
                    return valores.Contains((int)(escola.Uf ?? 0));
                
                case PropriedadeCondicao.Localizacao:
                    return valores.Contains((int)(escola.Localizacao ?? 0));

                case PropriedadeCondicao.TotalAlunos:
                    // Como este campo é numerico, deve haver somente um valor
                    var valor = valores.First();
                    if (condicao.Operador == OperacaoCondicao.GTE)
                    {
                        return valor >= escola.TotalAlunos;
                    }
                    if (condicao.Operador == OperacaoCondicao.LTE)
                    {
                        return valor <= escola.TotalAlunos;
                    }
                    break;
                
                case PropriedadeCondicao.EtapaEnsino:
                    foreach(var etapaEnsino in escola.EtapasEnsino)
                    {
                        if (valores.Contains((int)etapaEnsino.EtapaEnsino)) return true;
                    }
                    break;
                    
                case PropriedadeCondicao.Rede:
                    return valores.Contains((int)escola.Rede);
            }

            return false;
        }

        private int CondicaoValorToInt(string valor)
        {
            try
            {
                return int.Parse(valor);
            }
            catch
            {
                return -1;
            }
        }

        private void SalvarFatorRanque(FatorPriorizacao fator, int ranqueId)
        {
            if (dbContext.FatorRanques.Any(f => f.RanqueId == ranqueId && f.FatorPriorizacaoId == fator.Id)) return;

            var fatorRanque = new FatorRanque
            {
                FatorPriorizacaoId = fator.Id,
                RanqueId = ranqueId
            };
            dbContext.Add(fatorRanque);
        }

        private void CalcularESalvarPontuacao(Escola escola, int ranqueId)
        {
            if (ExisteEscolaRanque(escola, ranqueId)) return;

            List<FatorRanque> fatoresRanque = dbContext.FatorRanques.Where(f => f.RanqueId == ranqueId).ToList();
            int pontuacao = 0;

            foreach(var fatorRanque in fatoresRanque)
            {
                pontuacao += dbContext.FatorEscolas
                    .Where(f => f.EscolaId == escola.Id && f.FatorPriorizacaoId == fatorRanque.FatorPriorizacaoId)
                    .Select(f => f.Valor).First();
            }

            EscolaRanque escolaRanque = new EscolaRanque
            {
                EscolaId = escola.Id,
                RanqueId = ranqueId,
                Pontuacao = pontuacao
            };

            dbContext.Add(escolaRanque);
            dbContext.SaveChanges();
        }

        private bool ExisteEscolaRanque(Escola escola, int ranqueId)
        {
            return dbContext.EscolaRanques.Any(e => e.EscolaId == escola.Id && e.RanqueId == ranqueId);
        }
    }
}