namespace api.Escolas
{
    public class EscolaParaOtimizacao
    {
        public Guid Id;
        public int Ups;
        public double DistanciaPolo;
        public UF UF;

        public EscolaParaOtimizacao(Guid id, int ups, double distanciaPolo, UF uf)
        {
            Id = id;
            Ups = ups;
            DistanciaPolo = distanciaPolo;
            UF = uf;
        }
    }
}