using business.Group;
using System;

namespace business
{
    public  class Filtro : BaseModel
    {
        public string? Nome { get; set; }
        public string? Rotas { get; set; }
        public Int64 StoryId { get; set; }
        public virtual Story? Story { get; set; }
        public long? SubStoryId { get; set; }
        public virtual SubStory? SubStory { get; set; }
        public long? GrupoId { get; set; }
        public virtual Grupo? Grupo { get; set; }
        public long? SubGrupoId { get; set; }
        public SubGrupo? Subgrupo { get; set; }
        public long? SubSubGrupoId { get; set; }
        public virtual SubSubGrupo? SubSubGrupo { get; set; }
        public long? CamadaSeisId { get; set; }
        public virtual CamadaSeis? CamadaSeis { get; set; }
        public long? CamadaSeteId { get; set; }
        public virtual CamadaSete? CamadaSete { get; set; }
        public long? CamadaOitoId { get; set; }
        public CamadaOito? CamadaOito { get; set; }
        public long? CamadaNoveId { get; set; }
        public virtual CamadaNove? CamadaNove { get; set; }
        public long? CamadaDezId { get; set; }
        public virtual CamadaDez? CamadaDez { get; set; }
    }
}