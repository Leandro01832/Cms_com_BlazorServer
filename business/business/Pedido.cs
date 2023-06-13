using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace business
{
    public class Pedido : BaseModel
    {
        public Pedido()
        {

        }       
        public Pedido(string clienteId)
        {
            ClienteId = clienteId;
        }
        public List<ItemPedido> ItensPedido { get; private set; } = new List<ItemPedido>();  
        [Required]
        public string? ClienteId { get; set; }
    }
}