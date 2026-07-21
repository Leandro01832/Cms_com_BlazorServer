namespace business.business.brincando
{
    public class Baleia : Mamifero
    {
        private Type tipo = typeof(Baleia);
        public override string ExecutarAcao()
        {
            var strBase = base.ExecutarAcao();
            if(!string.IsNullOrEmpty(strBase))
            return strBase 
            + $" {Activator.CreateInstance(tipo)!.ToString()} - " + 
            "Baleia cinzenta, baleia azul, baleia cachalote.";
            return "";
        }

        public override string ToString()
        {
            return tipo.Name;
        }
    }
}
