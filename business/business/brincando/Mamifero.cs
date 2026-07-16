

namespace business.business.brincando
{
    public class Mamifero : Animal
    {
        public override string FazerBarulho(Type mudarComportamento)
        {
            var className = Activator.CreateInstance(this.GetType())!.ToString();
            if(mudarComportamento != this.GetType())  
            Pergunta = $"faz";
            var palavras = $" {className} {Pergunta} ";
            bool c = ExecutarSom(mudarComportamento);
            if(c)
            return $"{palavras}";
            else return "";
        }



        public override string ToString()
        {
            return this.GetType().Name;
        }
    }

}
