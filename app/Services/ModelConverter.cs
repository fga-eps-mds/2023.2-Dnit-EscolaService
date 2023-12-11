using api;
using api.CustoLogistico;
using api.Escolas;
using api.Fatores;
using api.Municipios;
using api.Planejamento;
using api.Polos;
using api.Ranques;
using api.Solicitacoes;
using app.Entidades;
using EnumsNET;

namespace app.Services
{
    public class ModelConverter
    {
        public EscolaCorretaModel ToModel(Escola value) =>
            new EscolaCorretaModel()
            {
                IdEscola = value.Id,
                CodigoEscola = value.Codigo,
                NomeEscola = value.Nome,
                Telefone = value.Telefone,
                UltimaAtualizacao = value.DataAtualizacao?.LocalDateTime,
                Cep = value.Cep,
                Endereco = value.Endereco,
                Uf = value.Uf,
                IdUf = (int?)value.Uf,
                SiglaUf = value.Uf?.ToString(),
                DescricaoUf = value.Uf?.AsString(EnumFormat.Description),
                IdSituacao = (int?)value.Situacao,
                Situacao = value.Situacao,
                DescricaoSituacao = value.Situacao?.AsString(EnumFormat.Description),
                IdRede = (int?)value.Rede,
                Rede = value.Rede,
                DescricaoRede = value.Rede.AsString(EnumFormat.Description),
                IdPorte = (int?)value.Porte,
                Porte = value.Porte,
                Observacao = value.Observacao,
                IdLocalizacao = (int?)value.Localizacao,
                Localizacao = value.Localizacao,
                DescricaoLocalizacao = value.Localizacao?.ToString(),
                Latitude = value.Latitude,
                Longitude = value.Longitude,
                NumeroTotalDeDocentes = value.TotalDocentes,
                NumeroTotalDeAlunos = value.TotalAlunos,
                IdMunicipio = value.MunicipioId,
                PoloId = value.PoloId,
                DistanciaPolo = value.DistanciaPolo,
                UfPolo = value.Polo?.Uf.ToString(),
                NomeMunicipio = value.Municipio?.Nome,
                EtapasEnsino = value.EtapasEnsino?.ConvertAll(e => e.EtapaEnsino),
                EtapaEnsino = value.EtapasEnsino?.ToDictionary(e => (int)e.EtapaEnsino, e => e.EtapaEnsino.AsString(EnumFormat.Description) ?? ""),
                TemSolicitacao = value.Solicitacao == null,
            };

        private SolicitacaoAcaoModel _ToModel(SolicitacaoAcao solicitacao)
        {
            return new()
            {
                Id = solicitacao.Id,
                Email = solicitacao.Email,
                Observacoes = solicitacao.Observacoes,
                Nome = solicitacao.EscolaNome,
                NomeSolicitante = solicitacao.NomeSolicitante,
                Vinculo = solicitacao.Vinculo,
                Telefone = solicitacao.Telefone,
                QuantidadeAlunos = solicitacao.TotalAlunos,
                Uf = solicitacao.EscolaUf,
            };
        }

        public UfModel ToModel(UF uf) =>
            new UfModel
            {
                Id = (int)uf,
                Sigla = uf.ToString(),
                Nome = uf.AsString(EnumFormat.Description)!,
            };

        public EtapasdeEnsinoModel ToModel(EtapaEnsino value) =>
            new EtapasdeEnsinoModel
            {
                Id = (int)value,
                Descricao = value.AsString(EnumFormat.Description)!,
            };

        public MunicipioModel ToModel(Municipio value) =>
            new MunicipioModel
            {
                Id = value.Id,
                Nome = value.Nome,
            };

        public SituacaoModel ToModel(Situacao value) =>
            new SituacaoModel
            {
                Id = (int)value,
                Descricao = value.AsString(EnumFormat.Description)!,
            };

        public RanqueEscolaModel ToModel(EscolaRanque escolaRanque) =>
            new RanqueEscolaModel
            {
                RanqueId = escolaRanque.RanqueId,
                Pontuacao = escolaRanque.Pontuacao,
                Posicao = escolaRanque.Posicao,
                Escola = new EscolaRanqueInfo
                {
                    Id = escolaRanque.Escola.Id,
                    Nome = escolaRanque.Escola.Nome,
                    EtapaEnsino = escolaRanque.Escola.EtapasEnsino?.ConvertAll(e => ToModel(e.EtapaEnsino)),
                    Municipio = escolaRanque.Escola.Municipio != null ? ToModel(escolaRanque.Escola.Municipio) : null,
                    Uf = escolaRanque.Escola.Uf.HasValue ? ToModel(escolaRanque.Escola.Uf.Value) : null,
                    Polo = escolaRanque.Escola.Polo != null ? ToModel(escolaRanque.Escola.Polo): null,
                    DistanciaPolo = escolaRanque.Escola.DistanciaPolo,
                    TemSolicitacao = escolaRanque.Escola.Solicitacao != null,
                    Ups = escolaRanque.Escola.Ups
                }
            };

        public DetalhesEscolaRanqueModel ToModel(Escola escola, RanqueInfo ranque)
            => new DetalhesEscolaRanqueModel
            {
                RanqueInfo = ranque,
                Id = escola.Id,
                Nome = escola.Nome,
                Telefone = escola.Telefone,
                Cep = escola.Cep,
                Codigo = escola.Codigo,
                Longitude = escola.Longitude,
                Latitude = escola.Latitude,
                Endereco = escola.Endereco,
                TotalAlunos = escola.TotalAlunos,
                TotalDocentes = escola.TotalDocentes,
                Uf = escola.Uf.HasValue ? ToModel(escola.Uf.Value) : null,
                Municipio = escola.Municipio != null ? ToModel(escola.Municipio) : null,
                Porte = escola.Porte.HasValue ? ToModel(escola.Porte.Value) : null,
                Rede = ToModel(escola.Rede),
                Localizacao = escola.Localizacao.HasValue ? ToModel(escola.Localizacao.Value) : null,
                Situacao = escola.Situacao.HasValue ? ToModel(escola.Situacao.Value) : null,
                EtapasEnsino = escola.EtapasEnsino?.ConvertAll(e => ToModel(e.EtapaEnsino)),
                Polo = ToModel(escola.Polo),
                DistanciaPolo = escola.DistanciaPolo,
                TemSolicitacao = escola.Solicitacao != null,
            };

        public PorteModel ToModel(Porte porte) =>
            new PorteModel
            {
                Id = porte,
                Descricao = porte.AsString(EnumFormat.Description)!,
            };

        public RedeModel ToModel(Rede rede) =>
            new RedeModel
            {
                Id = rede,
                Descricao = rede.AsString(EnumFormat.Description)!,
            };

        public LocalizacaoModel ToModel(Localizacao localizacao) =>
            new LocalizacaoModel
            {
                Id = localizacao,
                Descricao = localizacao.ToString(),
            };

        public PoloModel ToModel(Polo polo) =>
            new PoloModel
            {
                Id = polo.Id,
                Uf = ToModel(polo.Uf), 
                Nome = polo.Nome,
                Municipio = ToModel(polo.Municipio),
                Cep = polo.Cep,
                Endereco = polo.Endereco,
                Latitude = polo.Latitude,
                Longitude = polo.Longitude,
            };

        public CustoLogisticoItem ToModel(CustoLogistico custoLogistico) =>
            new CustoLogisticoItem
            {
                Custo = custoLogistico.Custo,
                RaioMin = custoLogistico.RaioMin,
                RaioMax = custoLogistico.RaioMax,
                Valor = custoLogistico.Valor,
            };
        
        public FatorPrioriModel ToModel(FatorPriorizacao fatorPriorizacao) =>
            new FatorPrioriModel
            {
                Id = fatorPriorizacao.Id,
                Nome = fatorPriorizacao.Nome,
                Peso = fatorPriorizacao.Peso,
                Ativo = fatorPriorizacao.Ativo,
                Primario = fatorPriorizacao.Primario,
                FatorCondicoes = fatorPriorizacao.FatorCondicoes.ConvertAll(ToModel)
            };

        public FatorPriorizacao ToModel(FatorPrioriModel fator) =>
            new FatorPriorizacao
            {
                Id = fator.Id,
                Nome = fator.Nome,
                Peso = fator.Peso,
                Ativo = fator.Ativo,
                Primario = fator.Primario,
                FatorCondicoes = fator.FatorCondicoes.ConvertAll(ToModel)
            };

        public FatorCondicaoModel ToModel(FatorCondicao fatorCondicao) =>   
            new FatorCondicaoModel
            {
                Id = fatorCondicao.Id,
                Propriedade = fatorCondicao.Propriedade,
                Operador = fatorCondicao.Operador,
                Valores = fatorCondicao.Valores.ConvertAll(v => v.Valor),
                FatorPriorizacaoId = fatorCondicao.FatorPriorizacaoId
            };

        public FatorCondicao ToModel(FatorCondicaoModel fatorCondicaoModel) => 
            new FatorCondicao
            {
                Id = fatorCondicaoModel.Id,
                Propriedade = (PropriedadeCondicao)fatorCondicaoModel.Propriedade,
                Operador = (OperacaoCondicao)fatorCondicaoModel.Operador,
                Valores = fatorCondicaoModel.Valores.ConvertAll(v => new CondicaoValor{ FatorCondicaoId = fatorCondicaoModel.Id, Valor = v }),
            };
        
        public SolicitacaoAcaoModel ToModel(SolicitacaoAcao solicitacao)
        {
            return new()
            {
                Id = solicitacao.Id,
                Email = solicitacao.Email,
                Observacoes = solicitacao.Observacoes,
                Nome = solicitacao.EscolaNome,
                NomeSolicitante = solicitacao.NomeSolicitante,
                Vinculo = solicitacao.Vinculo,
                Telefone = solicitacao.Telefone,
                QuantidadeAlunos = solicitacao.TotalAlunos,
                Uf = solicitacao.EscolaUf,
                Municipio = ToModel(solicitacao.EscolaMunicipio!),
                CodigoEscola = solicitacao.EscolaCodigoInep,
                Escola = solicitacao.Escola == null
                    ? null
                    : _ToModel(solicitacao.Escola!),
            };
        }

        private static EscolaCorretaModel _ToModel(Escola value) =>
            new()
            {
                IdEscola = value.Id,
                CodigoEscola = value.Codigo,
                NomeEscola = value.Nome,
                Telefone = value.Telefone,
                UltimaAtualizacao = value.DataAtualizacao?.LocalDateTime,
                Cep = value.Cep,
                Endereco = value.Endereco,
                Uf = value.Uf,
                IdUf = (int?)value.Uf,
                SiglaUf = value.Uf?.ToString(),
                DescricaoUf = value.Uf?.AsString(EnumFormat.Description),
                IdSituacao = (int?)value.Situacao,
                Situacao = value.Situacao,
                DescricaoSituacao = value.Situacao?.AsString(EnumFormat.Description),
                IdRede = (int?)value.Rede,
                Rede = value.Rede,
                DescricaoRede = value.Rede.AsString(EnumFormat.Description),
                IdPorte = (int?)value.Porte,
                Porte = value.Porte,
                Observacao = value.Observacao,
                IdLocalizacao = (int?)value.Localizacao,
                Localizacao = value.Localizacao,
                DescricaoLocalizacao = value.Localizacao?.ToString(),
                Latitude = value.Latitude,
                Longitude = value.Longitude,
                NumeroTotalDeDocentes = value.TotalDocentes,
                NumeroTotalDeAlunos = value.TotalAlunos,
                IdMunicipio = value.MunicipioId,
                PoloId = value.PoloId,
                DistanciaPolo = value.DistanciaPolo,
                UfPolo = value.Polo?.Uf.ToString(),
                NomeMunicipio = value.Municipio?.Nome,
                EtapasEnsino = value.EtapasEnsino?.ConvertAll(e => e.EtapaEnsino),
                EtapaEnsino = value.EtapasEnsino?.ToDictionary(e => (int)e.EtapaEnsino, e => e.EtapaEnsino.AsString(EnumFormat.Description) ?? ""),
                TemSolicitacao = true,
            };
        public RanqueDetalhesModel ToModel(Ranque ranque, FatorModel[] fatores)
        {
            return new RanqueDetalhesModel{
                Id = ranque.Id,
                Data = ranque.DataFim!.Value.LocalDateTime,
                NumEscolas = ranque.EscolaRanques.Count(),
                Descricao = ranque.Descricao,
                Fatores = fatores
            };
        }

        public PropriedadeCondicaoModel ToModel(PropriedadeCondicao propriedadeCondicao) =>
            new PropriedadeCondicaoModel
            {
                Id = (int)propriedadeCondicao,
                Rotulo = propriedadeCondicao.ToString()
            };
        public PlanejamentoMacroMensalModel ToModel(List<PlanejamentoMacroEscola> planejamentoMacroEscola)
        {
            if(planejamentoMacroEscola == null || planejamentoMacroEscola.Any(i => i==null))
            {
                throw new InvalidOperationException();
            }

            List<DetalhesEscolaMensal> detalhesEscolaMensal = planejamentoMacroEscola
                .Select(pme => new DetalhesEscolaMensal
                {
                    Id = pme.EscolaId,
                    UPS = pme.Escola.Ups,
                    Nome = pme.Escola.Nome,
                    UF = pme.Escola.Uf,
                    QuantidadeAlunos = pme.Escola.TotalAlunos,
                    DistanciaPolo = pme.Escola.DistanciaPolo
                })
                .ToList();

            List<DetalhesPorUF> detalhesPorUFs = detalhesEscolaMensal
                .GroupBy(dem => dem.UF)
                .Select(gp => new DetalhesPorUF 
                {
                    UF = gp.Key,
                    QuantidadeEscolasTotal = gp.Count()
                })
                .ToList();

            var planejamentoMacroMensalModel = new PlanejamentoMacroMensalModel
            {
                Mes = planejamentoMacroEscola[0].Mes,
                Ano = planejamentoMacroEscola[0].Ano,
                UPSTotal = detalhesEscolaMensal.Sum(dem => dem.UPS),
                QuantidadeAlunosTotal = detalhesEscolaMensal.Sum(dem => dem.QuantidadeAlunos),
                QuantidadeEscolasTotal = planejamentoMacroEscola.Count,
                Escolas = detalhesEscolaMensal,
                DetalhesPorUF = detalhesPorUFs
            };

            return planejamentoMacroMensalModel;
        }

        public PlanejamentoMacroDetalhadoModel ToModel(PlanejamentoMacro planejamentoMacro)
        {
            var listaPorMes = planejamentoMacro.Escolas
                .GroupBy(g => g.Mes)
                .Select(m => new { l = m.ToList()})
                .ToList();
            
            var planejamentoMacroMensalModels = new List<PlanejamentoMacroMensalModel>();
            
            listaPorMes.ForEach(lista => planejamentoMacroMensalModels.Add(ToModel(lista.l)));

            return new PlanejamentoMacroDetalhadoModel
            {
                Id = planejamentoMacro.Id,
                Nome = planejamentoMacro.Nome,
                Responsavel = planejamentoMacro.Responsavel,
                MesInicio = planejamentoMacro.MesInicio,
                MesFim = planejamentoMacro.MesFim,
                AnoInicio = planejamentoMacro.AnoInicio,
                AnoFim = planejamentoMacro.AnoFim,
                QuantidadeAcoes = planejamentoMacro.QuantidadeAcoes,
                PlanejamentoMacroMensal = planejamentoMacroMensalModels
            };
        }
    }
}
