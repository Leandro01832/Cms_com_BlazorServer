
using business.business;
using business.Group;
using Newtonsoft.Json;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace business
{
    public class Pagina : BaseModel
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
                Comentario = null;
                Data = DateTime.Now;
                StoryId = story.Id;
                Versiculo = story.Pagina.Count + 1;
            
        }



        private DateTime data = DateTime.Now;
        private string content;

        public DateTime Data { get { return data; } set { data = value; } }

        [Display(Name ="Story")]
        public Int64 StoryId { get; set; }
        public virtual Story? Story { get; set; }
  
        public virtual List<FiltroPagina>? Filtro { get; set; }

        [Required(ErrorMessage = "O titulo é necessário")]
        [Display(Name = "Titulo da pagina")]
        public string? Titulo { get; set; }        

        public long? Comentario { get; set; } 

             

        public string? Content
        {
            get { return content; }
            set 
            { 
                content = value;
                if(value != null)
                Data = DateTime.Now;
            }
        }

        public int Versiculo { get; set; }

        public string? Rotas { get; set; }

        public virtual Produto? Produto { get; set; }

        [NotMapped]
        public string NomeComId { get { return Titulo + " chave - " + Id.ToString(); } }


        

        public void IncluiFiltro(Filtro fil)
        {
            if (this.Filtro == null) this.Filtro = new List<FiltroPagina>();

            if(this.Filtro.FirstOrDefault(f => f.FiltroId == fil.Id) == null)
            {
                if (fil is SubStory)
                    this.Filtro!.Add(new FiltroPagina { Filtro = fil, Pagina = this,
                     QuantidadePorType = this.Filtro.Where(f => f.Filtro is SubStory).ToList().Count + 1  }) ;

                if (fil is Grupo)
                    this.Filtro!.Add(new FiltroPagina
                    {
                        Filtro = fil,
                        Pagina = this,
                        QuantidadePorType = this.Filtro.Where(f => f.Filtro is Grupo).ToList().Count + 1
                    });
            
                if (fil is SubGrupo)
                    this.Filtro!.Add(new FiltroPagina
                    {
                        Filtro = fil,
                        Pagina = this,
                        QuantidadePorType = this.Filtro.Where(f => f.Filtro is SubGrupo).ToList().Count + 1
                    });
            
                if (fil is SubSubGrupo)
                    this.Filtro!.Add(new FiltroPagina
                    {
                        Filtro = fil,
                        Pagina = this,
                        QuantidadePorType = this.Filtro.Where(f => f.Filtro is SubSubGrupo).ToList().Count + 1
                    });
            
                if (fil is CamadaSeis)
                    this.Filtro!.Add(new FiltroPagina
                    {
                        Filtro = fil,
                        Pagina = this,
                        QuantidadePorType = this.Filtro.Where(f => f.Filtro is CamadaSeis).ToList().Count + 1
                    });
            
                if (fil is CamadaSete)
                    this.Filtro!.Add(new FiltroPagina
                    {
                        Filtro = fil,
                        Pagina = this,
                        QuantidadePorType = this.Filtro.Where(f => f.Filtro is CamadaSete).ToList().Count + 1
                    });
            
                if (fil is CamadaOito)
                    this.Filtro!.Add(new FiltroPagina
                    {
                        Filtro = fil,
                        Pagina = this,
                        QuantidadePorType = this.Filtro.Where(f => f.Filtro is CamadaOito).ToList().Count + 1
                    });
            
                if (fil is CamadaNove)
                    this.Filtro!.Add(new FiltroPagina
                    {
                        Filtro = fil,
                        Pagina = this,
                        QuantidadePorType = this.Filtro.Where(f => f.Filtro is CamadaNove).ToList().Count + 1
                    });
            
                if (fil is CamadaDez)
                    this.Filtro!.Add(new FiltroPagina
                    {
                        Filtro = fil,
                        Pagina = this,
                        QuantidadePorType = this.Filtro.Where(f => f.Filtro is CamadaDez).ToList().Count + 1
                    });

            }


        }
    }
}
