using System;

namespace business
{
    public class Compartilhante : BaseModel
    {
        public DateTime Data { get; set; }
        public string? Livro { get; set; }
        public string? Admin { get; set; }
        public int Comissao { get; set; }
    }

}