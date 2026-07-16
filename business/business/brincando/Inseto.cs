
namespace business.business.brincando
{
    public class Inseto : Animal
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
             + "zzzzz zzzzz zzzzzz!!!!!" : $"{palavras}" + "zzzzz zzzzzz zzzzzz!!!!! ";
             else return "";
        }

        public override string ToString()
        {
            return this.GetType().Name;
        }
    }

}
