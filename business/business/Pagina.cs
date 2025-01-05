
using business.business;
using business.Group;
using Newtonsoft.Json;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace business
{
    public class Pagina : Content
    {
        public Pagina()
        {
            if(Pagina.entity)
            {
                Comentario = null;   
                Data = DateTime.Now;
            }
        }

        public Pagina(Story story)
        {
                Data = DateTime.Now;
                StoryId = story.Id;
                Versiculo = story.Pagina.Count + 1;
            
        } 
        
        public Pagina(int count)
        {
            Data = DateTime.Now;
            Versiculo = count;
            
        }           

        public int Versiculo { get; set; }        

        [NotMapped]
        public string NomeComId { get { return Titulo + " chave - " + Id.ToString(); } }       
    }
}
