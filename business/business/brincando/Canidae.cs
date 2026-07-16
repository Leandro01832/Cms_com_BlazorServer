
namespace business.business.brincando
{
    public class Canidae : Mamifero
    {

        public override string FazerBarulho(Type mudarComportamento)
        {
            var className = Activator.CreateInstance(this.GetType())!.ToString();
            if(mudarComportamento != this.GetType()) 
            Pergunta = $"faz";
            var palavras = $" {className} {Pergunta} ";
            bool c = ExecutarSom(mudarComportamento);
            if(c)
            return mudarComportamento != null ? $"Correção:{palavras}"
             + "au au au... au au au!!!!!" : $"{palavras}" + "au au au... au au au!!!!!";
             else return "";
        }

        public override string ToString()
        {
            return this.GetType().Name;
        }
    }

}
