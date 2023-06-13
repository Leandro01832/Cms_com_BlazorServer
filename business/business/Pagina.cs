
using business.div;
using business.Elementos.texto;
using business.Group;
using business.Join;
using Newtonsoft.Json;
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
                Div = null;              
                Music = false;
                Tempo = 11000;
                Data = DateTime.Now;
                ArquivoMusic = "";
                Sobreescrita = null;
                SubStoryId = null;
                GrupoId = null;
                SubGrupoId = null;
                SubSubGrupoId = null;
                CamadaSeis = null;
                CamadaSete = null;
                CamadaOito = null;
                CamadaNove = null;
                CamadaDez = null;
            }
        }
        
        public Pagina(int quant)
        {
            Div = new List<PaginaContainer>
            {
                new PaginaContainer{Container = new Container()}, 
                new PaginaContainer
                {
                     Container = new Container(quant){Content = true}
                }

            };
            Tempo = 11000;
        }
       
        public Pagina(int quant, int quantContainers)
        {
            Div = new List<PaginaContainer>
            {
                new PaginaContainer{Container = new Container()},
                new PaginaContainer
                {
                     Container = new Container(quant){Content = true}
                }

            };
            for (int i = 0; i < quantContainers; i++)
            Div.Add(new PaginaContainer());
            Tempo = 11000;
        }

        private int mostrarDados = 0;
        private DateTime data = DateTime.Now;
        private Int64? camadaDezId = null;
        private Int64? camadaNoveId = null;
        private Int64? camadaOitoId = null;
        private Int64? camadaSeteId = null;
        private Int64? camadaSeisId = null;
        private Int64? subSubGrupoId = null;
        private Int64? subGrupoId = null;
        private Int64? grupoId = null;
         private Int64? subStoryId = null;
         private string flexDirection = "row";
         private string alignItems = "flex-start";

        public DateTime Data { get { return data; } set { data = value; } }

        [Display(Name ="Story")]
        public Int64 StoryId { get; set; }
        public virtual Story? Story { get; set; }

        [Display(Name ="Sub-Story")]
        public Int64? SubStoryId 
        {
            get
            {
                if (subStoryId == 0) return null;                
                return subStoryId;
            }
            set
            {   
                subStoryId = value;
            }
        }

        public virtual SubStory? SubStory { get; set; }

        [Display(Name ="Grupo")]
        public Int64? GrupoId 
        {
            get
            {
                if (grupoId == 0) return null;                
                return grupoId;
            }
            set
            {   
                grupoId = value;
            }
        }
        public virtual Grupo? Grupo { get; set; }

         [Display(Name ="Sub-Grupo")]
        public Int64? SubGrupoId
        {
            get
            {
                if (subGrupoId == 0) return null;                
                return subGrupoId;
            }
            set
            {   
                subGrupoId = value;
            }
        }
        public virtual SubGrupo? SubGrupo{get; set;}
        

         [Display(Name ="Sub-Sub-Grupo")]
        public Int64? SubSubGrupoId
        {
            get
            {
                if (subSubGrupoId == 0) return null;                
                return subSubGrupoId;
            }
            set
            {   
                subSubGrupoId = value;
            }
        }

        public virtual SubSubGrupo? SubSubGrupo { get; set; }

         [Display(Name ="Camada nº 6")]
        public Int64? CamadaSeisId
        {
            get
            {
                if (camadaSeisId == 0) return null;                
                return camadaSeisId;
            }
            set
            {   
                camadaSeisId = value;
            }
        }
        public virtual CamadaSeis? CamadaSeis { get; set; }

         [Display(Name ="Camada nº 7")]
        public Int64? CamadaSeteId
        {
            get
            {
                if (camadaSeteId == 0) return null;                
                return camadaSeteId;
            }
            set
            {   
                camadaSeteId = value;
            }
        }
        public virtual CamadaSete? CamadaSete { get; set; }

         [Display(Name ="Camada nº 8")]
        public Int64? CamadaOitoId
        {
            get
            {
                if (camadaOitoId == 0) return null;                
                return camadaOitoId;
            }
            set
            {   
                camadaOitoId = value;
            }
        }
        public virtual CamadaOito? CamadaOito { get; set; }
         
         [Display(Name ="Camada nº 9")]
        public Int64? CamadaNoveId
        {
            get
            {
                if (camadaNoveId == 0) return null;                
                return camadaNoveId;
            }
            set
            {   
                camadaNoveId = value;
            }
        }
        public virtual CamadaNove? CamadaNove { get; set; }

         [Display(Name ="Camada nº 10")]
        public Int64? CamadaDezId
        {
            get
            {
                if (camadaDezId == 0) return null;                
                return camadaDezId;
            }
            set
            {   
                camadaDezId = value;
            }
        }
        public virtual CamadaDez? CamadaDez { get; set; }

        [Required(ErrorMessage = "O titulo é necessário")]
        [Display(Name = "Titulo da pagina")]
        public string? Titulo { get; set; }      

        public string? Sobreescrita { get; set; }      

        [Display(Name = "Arquivo")]
        public string? ArquivoMusic { get; set; }       

        public bool Music { get; set; }      

        public long? Comentario { get; set; } 

        [Display(Name = "Arquivo")]
        public string? ImagemContent { get; set; }  
        
        [JsonIgnore]
        public virtual List<PaginaContainer>? Div { get; set; }
      
        public virtual Produto? Produto { get; set; }                         

        [NotMapped]
        public int MostrarDados { get { return mostrarDados; } set { mostrarDados = value; } }

        [NotMapped]
        public string NomeComId { get { return Titulo + " chave - " + Id.ToString(); } }

        //Para Programador
        public bool Layout { get; set; }

        public string FlexDirection { get { return flexDirection; } set { flexDirection = value; } }
        
        public string AlignItems { get { return alignItems; } set { alignItems = value; } }

        public int Tempo { get; set; }       
        
        public void IncluiDiv(Container container)
        {
            this.Div!.Add(new PaginaContainer { Container = container, Pagina = this });
        }
        
        public void setarElemento(Texto elemento)
        {
            this.Div!.First(d => d.Container!.Content).Container!.Div!
            .First(d => d.Div!.Content).Div!.Elemento = new List<DivElemento>();
            this.Div!.First(d => d.Container!.Content).Container!.Div!
            .First(d => d.Div!.Content).Div!.Elemento!.Add(new DivElemento
            {
                Div = this.Div!.First(d => d.Container!.Content).Container!.Div!
                .First(d => d.Div!.Content).Div,
                Elemento = elemento                
            });
        }
       
    }
}
