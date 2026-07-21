

namespace business.business.brincando
{
    public class Felidae : Mamifero
    {
        private Type tipo = typeof(Felidae);

        public override string ExecutarAcao()
        {
            var strBase = base.ExecutarAcao();
            if(!string.IsNullOrEmpty(strBase))
            return strBase
            + $" {Activator.CreateInstance(tipo)!.ToString()} - " + 
            "Gato, Leão, Leopardo, ";
            else return "";
        }

        public override string ToString()
        {
            return tipo.Name;
        }
    }

}
