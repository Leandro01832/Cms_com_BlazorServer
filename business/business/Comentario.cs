namespace  business
{

    public class Comentario : BaseModel
    {
        public long IdPagina { get; set; }
        public int Capitulo { get; set; }
        public int Verso { get; set; }
    }

}