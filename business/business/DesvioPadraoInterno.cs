using business;
    public class DesvioPadraoInterno : Pagina
    {
        public DesvioPadraoInterno()
        {
            
        }
        public DesvioPadraoInterno(string segmento)
        {
            Titulo += $" - {segmento}";
        }
        

        public override string ToString()
        {
            return "Desvio Padrao Interno";
        }  
    }    

