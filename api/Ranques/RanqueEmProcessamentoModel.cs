namespace api.Ranques
{
    public class RanqueEmProcessamentoModel
    {
        public int Id { get; set; }
        public bool EmProgresso { get; set; }
        public DateTimeOffset DataInicio { get; set; }
        public DateTimeOffset? DataFim { get; set; }
    }
}