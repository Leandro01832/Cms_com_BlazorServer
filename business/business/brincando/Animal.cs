using business.business.conteudo;
using System.Media;
using System.Threading.Tasks;

namespace business.business.brincando
{
    public class Animal : Content
    {
        private Type tipo = typeof(Animal);

        public List<Comportamento> Comportamentos { get; set; }

        public bool ExecutarSom()
        {
            var name =  this.GetType().Name;
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

        public virtual string ExecutarAcao()
        {
            bool c = ExecutarSom();
            var retorno =  $"Correção: {Comportamentos.First().Acao}. " ;
            if (c)
                return retorno
            + $" {Activator.CreateInstance(tipo)!.ToString()}. - ";
            else return "";
        }





        public override string ToString()
        {
           return tipo.Name;
        }
    }



}