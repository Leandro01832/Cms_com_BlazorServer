using business.business.conteudo;
using System.Media;
using System.Threading.Tasks;

namespace business.business.brincando
{
    public class Animal : Content
    {


        public string? Pergunta { get; set; } = "Faz qual barulho??? - ";


        public bool ExecutarSom(Type? tipo)
        {
            var name = tipo != null ? tipo.Name : this.GetType().Name;
            string caminhoSom = Path.Combine(AppContext.BaseDirectory,
             "sons", $"{name}.wav");

            if (File.Exists(caminhoSom))
            {
                using (SoundPlayer player = new SoundPlayer(caminhoSom))
                {
                    player.PlaySync(); // PlaySync toca o som e espera ele terminar
                }

                return true;
            }
            else return false;
        }

        public virtual string FazerBarulho(Type mudarComportamento)
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