using System.Numerics;
using api;
using api.Escolas;
using api.Planejamento;
using app.Entidades;
using app.Repositorios.Interfaces;
using OptimizationPSO;
using OptimizationPSO.Swarm;
using service.Interfaces;

namespace app.Services
{
    public class PlanejamentoService : IPlanejamentoService
    {
        private readonly IPlanejamentoRepositorio planejamentoRepositorio;
        private readonly IRanqueService ranqueService;
        private readonly ModelConverter modelConverter;
        private readonly IEscolaRepositorio escolaRepositorio;
        private readonly AppDbContext dbContext;

        public PlanejamentoService
        (
            IPlanejamentoRepositorio planejamentoRepositorio,
            IEscolaRepositorio escolaRepositorio,
            ModelConverter modelConverter,
            IRanqueService ranqueService,
            AppDbContext dbContext
        )
        {
            this.planejamentoRepositorio = planejamentoRepositorio;
            this.modelConverter = modelConverter;
            this.dbContext = dbContext;
            this.ranqueService = ranqueService;
            this.escolaRepositorio = escolaRepositorio;
        }

        public async Task<PlanejamentoMacro> ObterPlanejamentoMacroAsync(Guid id)
        {
            return await planejamentoRepositorio.ObterPlanejamentoMacroAsync(id);
        }
            
        public async Task ExcluirPlanejamentoMacro(Guid id)
        {
            var planejamento = await planejamentoRepositorio.ObterPlanejamentoMacroAsync(id);
            planejamento.Escolas.ForEach(e => planejamentoRepositorio.ExcluirPlanejamentoMacroEscola(e));
            planejamentoRepositorio.ExcluirPlanejamentoMacro(planejamento);
            await dbContext.SaveChangesAsync();
        }        

        public async Task<PlanejamentoMacro> GerarRecomendacaoDePlanejamento(PlanejamentoMacroDTO planejamento)
        {            
            var q = planejamento.QuantidadeAcoes;
            var n = q * 3;

            var filtro = new PesquisaEscolaFiltro()
            {
                TamanhoPagina = n
            };

            var escolas = await ranqueService.ListarEscolasUltimoRanqueAsync(filtro);
            var numeroMeses = Math.Abs(planejamento.MesFim - planejamento.MesInicio) + 1;
            var acoesPorMes = (int) Math.Ceiling((double) q / numeroMeses);

            var escolaParaOtimizacao = new List<EscolaParaOtimizacao>();
            escolas.Items.ForEach(e => escolaParaOtimizacao.Add(e.Escola.ParaOtimizacao()));

            var lista = new List<PlanejamentoMacroEscola>();

            var otimizador = new Otimizador(q, escolaParaOtimizacao);
            var listaOtimizada = otimizador.Solve();

            int i = 0, mes = (int) planejamento.MesInicio;
            foreach(var e in listaOtimizada)
            {
                if(i == acoesPorMes)
                {
                    i = 0;
                    mes = mes == 12 ? 1 : mes + 1;
                }

                var planejamentoMacroEscola = new PlanejamentoMacroEscola()
                {
                    Mes = (Mes) mes,
                    Ano = planejamento.AnoInicio,
                    EscolaId = e    
                };

                lista.Add(planejamentoMacroEscola);
                i++;
            }

            var planejamentoMacroGerado = new PlanejamentoMacro()
            {
                Nome = planejamento.Nome,
                Responsavel = planejamento.Responsavel,
                MesInicio = planejamento.MesInicio,
                MesFim = planejamento.MesFim,
                AnoInicio = planejamento.AnoInicio,
                AnoFim = planejamento.AnoFim,
                QuantidadeAcoes = planejamento.QuantidadeAcoes,
                Escolas = lista                
            };          

            //algoritmo de recomendação:    
            //pegar um vetor V com as n primeiras escolas do ranking
            //normalizar V para objetos com {Id, Ups, CustoLogistico}
            //implemetar a função-custo como: [1]
            //rodar uma otimização por enxame de partículas ou um algoritmo genético
            //usar q dimensões num S = [0, n]
            //captar o vetor R com o resultado da otimização
            //R é composto pelos indices das escolas escolhidas em V
            //dividir q igualmente pela quantidade de meses
            //tentar agrupar por UF as escolas selecionadas no mesmo Mês
            //Montar um objeto PlanejamentoMacro com o resultado

            /* [1]
            rascunho de função-custo:
        
                X -> vetor q-dimensional
                custo = 0;
                for(int i = 0, i < q, i++)
                {
                    custo += -V[X[i]].Ups + V[X[i]].custologistico
                }
            */

            return planejamentoMacroGerado;
        }

        public PlanejamentoMacro CriarPlanejamentoMacro(PlanejamentoMacro planejamento)
        {
            var planejamentoMacro = planejamentoRepositorio.RegistrarPlanejamentoMacro(planejamento);
            dbContext.SaveChanges();
            return planejamentoMacro;
        }

        public async Task<PlanejamentoMacro> EditarPlanejamentoMacro(Guid id, string nome, List<PlanejamentoMacroMensalDTO> planejamentoMacroMensal)
        {
            var planejamentoMacroAtualizar = await planejamentoRepositorio
                .ObterPlanejamentoMacroAsync(id) ?? 
                throw new KeyNotFoundException("Planejamento não encontrado");

            planejamentoMacroAtualizar.Nome = nome;

            foreach(var mes in planejamentoMacroMensal)
            {
                mes.Escolas.ForEach(async e => {
                    var plan = planejamentoMacroAtualizar.Escolas
                        .Find(pme => pme.EscolaId == e);
                    
                    Console.WriteLine(plan == null);

                    if(plan == null)
                    {                        
                        var novoPlanejamentoMacroEscola = new PlanejamentoMacroEscola
                        {
                            Mes = mes.Mes,
                            Ano = mes.Ano,
                            PlanejamentoMacroId = planejamentoMacroAtualizar.Id,
                            EscolaId = e
                        };

                        //registra novo PlanejamentoMacroEscola
                        planejamentoRepositorio.RegistrarPlanejamentoMacroMensal(novoPlanejamentoMacroEscola);

                        //planejamentoMacroAtualizar.Escolas.Add(novoPlanejamentoMacroEscola);
                    }
                    else{
                        plan.Ano = mes.Ano;
                        plan.Mes = mes.Mes;
                    }
                });
            }

            foreach(var plan in planejamentoMacroAtualizar.Escolas)
            {
                var mes = planejamentoMacroMensal.FirstOrDefault(p => p.Mes == plan.Mes && p.Ano == plan.Ano);
                
                if(!mes!.Escolas.Contains(plan.EscolaId))
                {
                    planejamentoRepositorio.ExcluirPlanejamentoMacroEscola(plan);
                    //planejamentoMacroAtualizar.Escolas.Remove(plan);
                }
            }

            dbContext.SaveChanges();

            foreach(var pme in planejamentoMacroAtualizar.Escolas)
            {
                pme.Escola ??= await escolaRepositorio.ObterPorIdAsync(pme.EscolaId);
            }

            return planejamentoMacroAtualizar;
        }

        public async Task<ListaPaginada<PlanejamentoMacroDetalhadoModel>> ListarPaginadaAsync(PesquisaPlanejamentoFiltro filtro)
        {
            var planejamentos = await planejamentoRepositorio.ListarPaginadaAsync(filtro);
            var planejamentosCorretos = planejamentos.Items.ConvertAll(modelConverter.ToModel);
            return new ListaPaginada<PlanejamentoMacroDetalhadoModel>(planejamentosCorretos, planejamentos.Pagina, planejamentos.ItemsPorPagina, planejamentos.Total);
        }
    }

    internal class Otimizador
    {
        private int NumDim;
        private List<EscolaParaOtimizacao> EspacoDeBusca;

        internal Otimizador(int numDim, List<EscolaParaOtimizacao> espacoDeBusca)
        {
            NumDim = numDim;
            EspacoDeBusca = espacoDeBusca;
        }

        private int FuncaoCusto(double[] x)
        {
            if(x.Select(Convert.ToInt32).Distinct().Count() != NumDim)
            {
                return int.MaxValue;
            }

            int custo = 0;
            for(int i = 0; i < NumDim; i++)
            {
                int index = (int) x[i];
                custo += (-2 * EspacoDeBusca[index].Ups) + (int) Math.Ceiling(EspacoDeBusca[index].DistanciaPolo);
                custo /= 3;
            }

            return custo;
        }

        internal List<Guid> Solve()
        {   
            var lb = new double[NumDim];
            var ub = new double[NumDim];
            Array.Fill<double>(lb, 0);
            Array.Fill<double>(ub, EspacoDeBusca.Count - 1);

            var solveConfig = PSOSolverConfig.CreateDefault(                
                numberParticles: 50,
                maxEpochs: 400,
                lowerBound: lb,
                upperBound: ub,
                isStoppingCriteriaEnabled: true
            );

            Func<double[], double> func = x => FuncaoCusto(x);
            var solver = new ParticleSwarmMinimization(func, solveConfig);

            double[] resultado;
            
            do
            {
                resultado = solver.Solve().BestPosition;
            }while(resultado.Select(Convert.ToInt32).Distinct().Count() != NumDim);            

            var listaOtimizada = new List<Guid>();

            foreach(var r in resultado)
            {
                listaOtimizada.Add(EspacoDeBusca[(int) r].Id);
            }

            return listaOtimizada;
        }
    }
}