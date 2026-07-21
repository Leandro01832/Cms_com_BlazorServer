namespace business.business.brincando
{
    public class Aves : Animal
    {
        private Type tipo = typeof(Aves);
        public override string ExecutarAcao()
        {
            var strBase = base.ExecutarAcao();
            if(!string.IsNullOrEmpty(strBase))
            return strBase
            + $" {Activator.CreateInstance(tipo)!.ToString()} - " + 
             "Gaivota, bem-te-vi, ";
            else return "";
        }

        public override string ToString()
        {
            return tipo.Name;
        }
    }
}
