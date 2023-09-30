
using business.business;
using business.Group;
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
                Data = DateTime.Now;
                SubStoryId = null;
                GrupoId = null;
                SubGrupoId = null;
                SubSubGrupoId = null;
                CamadaSeis = null;
                CamadaSete = null;
                CamadaOito = null;
                CamadaNove = null;
                CamadaDez = null;
                Classificacao = new Classificacao();
            }
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

        public long? Comentario { get; set; } 

        [Display(Name = "Arquivo")]
        public string? ImagemContent { get; set; }

        public string? Content { get; set; }

        public virtual Produto? Produto { get; set; }

        public virtual Classificacao? Classificacao { get; set; }

        [NotMapped]
        public int MostrarDados { get { return mostrarDados; } set { mostrarDados = value; } }

        [NotMapped]
        public string NomeComId { get { return Titulo + " chave - " + Id.ToString(); } }

        public string FlexDirection { get { return flexDirection; } set { flexDirection = value; } }
        
        public string AlignItems { get { return alignItems; } set { alignItems = value; } }

        public string? ContentUser { get; set; }

        [NotMapped]
        public string Conteudo
        {
            get { if (Content != null) return Content; else if (ContentUser != null) return ContentUser; return ""; }
        }
    }
}
