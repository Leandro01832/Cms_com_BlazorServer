    
namespace business.business.Group
{

    
    public class Portal : BaseModel
    {
        public Portal()
        {
            
        }
        public string? Nome { get; set; }
        public string? Descricao { get; set; }
        public string? Logo { get; set; }
        public string? CorPrimaria { get; set; }
        public string? CorSecundaria { get; set; }
        public string? CorTerciaria { get; set; }
        public string? CorQuaternaria { get; set; }

        public string? Html { get; set; }

        public Int64 StoryId { get; set; }
        public virtual Story? Story { get; set; }
        public virtual List<Filtro>? Filtro { get; set; }
        public virtual List<UserProfile>? UserProfile { get; set; }
    }
}
