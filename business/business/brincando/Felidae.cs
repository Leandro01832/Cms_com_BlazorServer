

namespace business.business.brincando
{
    public class Felidae : Mamifero
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
             + "miaaaauuuuu!!!!!" : $"{palavras}" + "miaaaauuuuu!!!!! ";
             else return "";
        }

        public override string ToString()
        {
            return this.GetType().Name;
        }
    }

}
