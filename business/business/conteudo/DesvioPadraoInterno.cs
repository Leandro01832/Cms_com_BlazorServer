
   
   namespace business.business.conteudo
{
     public class DesvioPadraoInterno : Pagina
    {
        public DesvioPadraoInterno()
        {
            
        }
        public DesvioPadraoInterno(string segmento)
        {
            segm = segmento;
        }

        private string segm = "teste";
        private string? titulo = "";
        public override string? Titulo
         { 
            get{return titulo;}
             set
             { 
                titulo = value + " - " + segm;
             }
         }
        

        public override string ToString()
        {
            return "Desvio Padrão Int.";
        }  
    }    


}
   