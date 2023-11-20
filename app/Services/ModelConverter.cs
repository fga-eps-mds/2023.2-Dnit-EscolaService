using api;
using api.Escolas;
using api.Municipios;
using api.Ranques;
using api.Superintendencias;
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
                SuperintendenciaId = value.SuperintendenciaId,
                DistanciaSuperintendencia = value.DistanciaSuperintendencia,
                UfSuperintendencia = value.Superintendencia?.Uf.ToString(),
                NomeMunicipio = value.Municipio?.Nome,
                EtapasEnsino = value.EtapasEnsino?.ConvertAll(e => e.EtapaEnsino),
                EtapaEnsino = value.EtapasEnsino?.ToDictionary(e => (int)e.EtapaEnsino, e => e.EtapaEnsino.AsString(EnumFormat.Description) ?? ""),
            };

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
                    Superintendencia = escolaRanque.Escola.Superintendencia != null ? ToModel(escolaRanque.Escola.Superintendencia): null,
                    DistanciaSuperintendencia = escolaRanque.Escola.DistanciaSuperintendencia,
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
                Superintendencia = ToModel(escola.Superintendencia),
                DistanciaSuperintendencia = escola.DistanciaSuperintendencia,
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

        public SuperintendenciaModel ToModel(Superintendencia superintendencia) =>
            new SuperintendenciaModel
            {
                Id = superintendencia.Id,
                Uf = superintendencia.Uf,
            };
    }
}
