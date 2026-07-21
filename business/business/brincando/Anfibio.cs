namespace business.business.brincando
{
    public class Anfibio : Animal
    {
        private Type tipo = typeof(Anfibio);

        public override string ExecutarAcao()
        {
            var strBase = base.ExecutarAcao();
            if(!string.IsNullOrEmpty(strBase))
            return strBase 
            + $" {Activator.CreateInstance(tipo)!.ToString()} - " + 
             " Sapo, perereca, rã, ";
            else return "";
        }

        public override string ToString()
        {
            return tipo.Name;
        }
    }
}
