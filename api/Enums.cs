using System.ComponentModel;
using System.Text.Json.Serialization;

namespace api
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum UF
    {
        [Description("Acre")]
        AC = 1,
        [Description("Alagoas")]
        AL,
        [Description("Amapá")]
        AP,
        [Description("Amazonas")]
        AM,
        [Description("Bahia")]
        BA,
        [Description("Ceará")]
        CE,
        [Description("Espírito Santo")]
        ES,
        [Description("Goiás")]
        GO,
        [Description("Maranhão")]
        MA,
        [Description("Mato Grosso")]
        MT,
        [Description("Mato Grosso do Sul")]
        MS,
        [Description("Minas Gerais")]
        MG,
        [Description("Pará")]
        PA,
        [Description("Paraíba")]
        PB,
        [Description("Paraná")]
        PR,
        [Description("Pernambuco")]
        PE,
        [Description("Piauí")]
        PI,
        [Description("Rio de Janeiro")]
        RJ,
        [Description("Rio Grande do Norte")]
        RN,
        [Description("Rio Grande do Sul")]
        RS,
        [Description("Rondônia")]
        RO,
        [Description("Roraima")]
        RR,
        [Description("Santa Catarina")]
        SC,
        [Description("São Paulo")]
        SP,
        [Description("Sergipe")]
        SE,
        [Description("Tocantins")]
        TO,
        [Description("Distrito Federal")]
        DF
    }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum Localizacao
    {
        Rural = 1,
        Urbana,
    }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum Porte
    {
        [Description("Até 50 matrículas de escolarização")]
        Ate50 = 1,
        [Description("Entre 51 e 200 matrículas de escolarização")]
        Entre51e200 = 4,
        [Description("Entre 201 e 500 matrículas de escolarização")]
        Entre201e500 = 2,
        [Description("Entre 501 e 1000 matrículas de escolarização")]
        Entre501e1000 = 3,
        [Description("Mais de 1000 matrículas de escolarização")]
        Mais1000 = 5,
    }

    public enum Situacao
    {
        [Description("Indicação")]
        Indicacao = 1,
        [Description("Solicitação da escola")]
        SolicitacaoEscola,
        [Description("Jornada de crescimento do professor")]
        Jornada,
        [Description("Escola Crítica")]
        EscolaCritica,
    }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum Rede
    {
        Municipal = 1,
        Estadual,
        Privada,
    }

    public enum EtapaEnsino
    {
        [Description("Educação Infantil")]
        Infantil = 1,
        [Description("Ensino Fundamental")]
        Fundamental,
        [Description("Ensino Médio")]
        Medio,
        [Description("Educação de Jovens Adultos")]
        JovensAdultos,
        [Description("Educação Profissional")]
        Profissional,
    }

    public enum ErrorCodes
    {
        Unknown,
        [Description("Escola não encontrada")]
        EscolaNaoEncontrada,
        [Description("Municipio não encontrado")]
        MunicipioNaoEncontrado,
        [Description("Polo não encontrado")]
        PoloNaoEncontrado,
        [Description("Já tem um ranque sendo calculado, tente novamente mais tarde")]
        RanqueJaSendoCalculado,
        [Description("Formato JSON não reconhecido")]
        FormatoJsonNaoReconhecido,
        [Description("Fator de Priorização não encontrado")]
        FatorNaoEncontrado,
        [Description("Planejamento Macro não encontrado")]
        PlanejamentoMacroNaoEncontrado,
    }

    public enum Mes
    {
        [Description("Janeiro")]
        Janeiro = 1,

        [Description("Fevereiro")]
        Fevereiro,

        [Description("Março")]
        Marco,

        [Description("Abril")]
        Abril,

        [Description("Maio")]
        Maio,

        [Description("Junho")]
        Junho,

        [Description("Julho")]
        Julho,

        [Description("Agosto")]
        Agosto,

        [Description("Setembro")]
        Setembro,

        [Description("Outubro")]
        Outubro,

        [Description("Novembro")]
        Novembro,

        [Description("Dezembro")]
        Dezembro
    }

    public enum Permissao
    {
        [Description("Cadastrar Escola")]
        EscolaCadastrar = 1000,
        [Description("Editar Escola")]
        EscolaEditar = 1001,
        [Description("Remover Escola")]
        EscolaRemover = 1002,
        [Description("Visualizar Escola")]
        EscolaVisualizar = 1003,
        [Description("Exportar Escola")]
        EscolaExportar = 1004,

        [Description("Visualizar Ranking de Escolas")]
        RanqueVisualizar = 5002,
        [Description("Calcular Ranking de Escolas")]
        RanqueCalcular = 5003,
        [Description("Poll ranking em processamento")]
        RanquePollProcessamento = 5004,
        [Description("Exportar Ranking de Escolas")]
        RanqueExportar = 5005,

        [Description("Visualizar solicitação")]
        SolicitacaoVisualizar = 9000,
        
        [Description("Cadastrar Polo")]
        PoloCadastrar = 10000,
        [Description("Editar Polo")]
        PoloEditar = 10001,
        [Description("Remover Polo")]
        PoloRemover = 10002,
        [Description("Visualizar Polo")]
        PoloVisualizar = 10003,

        [Description("Visualizar Planejamento Macro")]
        PlanejamentoVisualizar = 11000,
        [Description("Criar Planejamento Macro")]
        PlanejamentoCriar = 11001,
        [Description("Editar Planejamento Macro")]
        PlanejamentoEditar = 11002,
        [Description("Remover Planejamento Macro")]
        PlanejamentoRemover = 11003,
    }

    public enum PropriedadeCondicao
    {
        Porte = 1,
        Situacao,
        Municipio,
        UF,
        Localizacao,
        TotalAlunos,
        EtapaEnsino,
        Rede
    }

    public enum OperacaoCondicao
    {
        Equals = 1,
        GTE,
        LTE
    }
}
