

namespace business.business.brincando
{
    public class Mamifero : Animal
    {
        private Type tipo = typeof(Mamifero);

        public override string ExecutarAcao()
        {
            var strBase = base.ExecutarAcao();
            if(!string.IsNullOrEmpty(strBase))
            return strBase
            + $" {Activator.CreateInstance(tipo)!.ToString()} - " + 
            "Tigre, rinoceronte, girafa, ";
            else return "";
        }

        public override string ToString()
        {
            return tipo.Name;
        }
    }

}
