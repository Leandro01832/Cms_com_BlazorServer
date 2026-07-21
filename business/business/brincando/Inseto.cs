
namespace business.business.brincando
{
    public class Inseto : Animal
    {
        private Type tipo = typeof(Inseto);

        public override string ExecutarAcao()
        {
            var strBase = base.ExecutarAcao();
            if(!string.IsNullOrEmpty(strBase))
            return strBase 
            + $" {Activator.CreateInstance(tipo)!.ToString()} - " + 
            "Abelha, Zangão, pernilongo, mosca, ";
            else return "";
        }

        public override string ToString()
        {
            return tipo.Name;
        }
    }

}
