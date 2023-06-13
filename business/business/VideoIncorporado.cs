using System.ComponentModel.DataAnnotations;

namespace business
{
   public class VideoIncorporado : BaseModel
   {
        public int? Tamanho { get; set; }

        [Display(Name = "Arquivo")]
        public string? ArquivoVideoIncorporado { get; set; }

        public string Nome 
        {
          get
          { 
            if(Tamanho != null)
            return "Videos - " + Id.ToString() + " Quant: " + Tamanho.ToString();
            else
            return "Videos - " + Id.ToString();
          }
        }
   }
}