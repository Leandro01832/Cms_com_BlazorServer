using business.business;
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
        public Pedido(long clienteId)
        {
            ClienteId = clienteId;
        }
        public List<ItemPedido> Itens { get; private set; } = new List<ItemPedido>();
        [ForeignKey("ClienteId")]
        public virtual Cliente Cliente { get; set; }
        [Required]
        public long ClienteId { get; set; }
        public string? Status { get; set; }

    }
}