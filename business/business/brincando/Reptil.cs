namespace business.business.brincando
{
    public class Reptil : Animal
    {
        private Type tipo = typeof(Canidae);
        public override string ExecutarAcao()
        {
            var strBase = base.ExecutarAcao();
            if(!string.IsNullOrEmpty(strBase))
            return strBase 
            + $" {Activator.CreateInstance(tipo)!.ToString()} - " + 
            " Lagarto, cobra, lagartixa, tartaruga.";
            else return "";
        }

        public override string ToString()
        {
            return tipo.Name;
        }
    }
}
