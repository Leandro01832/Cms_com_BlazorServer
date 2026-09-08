

namespace business.business.Group
{
    public class Nicho : BaseModel
    {
        // Ex: Natureza - floricultura, loja de peixe de aquarios, viveiro de mudas;
       //  entretenimento - loja de brinquedos, cinema;
       //   Alimentos - pizzaria, restaurante, lanchonete etc.
        public string Nome { get; set; } 
        public long StoryId { get; set; }
        public virtual Story Story { get; set; }
    }
    
}
