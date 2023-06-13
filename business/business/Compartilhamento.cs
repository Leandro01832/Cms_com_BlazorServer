using System;

namespace 
    business
{
    public class Compartilhamento : BaseModel
    {
        public Compartilhamento()
        {
            Quantidade = 1;
        }
        public DateTime Data { get; set; }
        public string? Livro { get; set; }
        public int Capitulo { get; set; }
        public int Verso { get; set; }
        public int Quantidade { get; set; }
    }

}