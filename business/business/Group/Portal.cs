    
namespace business.business.Group
{

    
    public class Portal : BaseModel
    {
        // portal para Biologo - pronto,
        // portal para veterinario
        // portal para ramster - petshop
        // portal para cachorro - petshop
        // portal para gato - petshop
        // portal para peixe - petshop
        // portal para aves - petshop

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

        public Int64 StoryId { get; set; }
        public virtual Story? Story { get; set; }
        public virtual List<Filtro>? Filtro { get; set; }
    }
}
