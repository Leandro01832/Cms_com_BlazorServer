using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using business.Group;

namespace business.business.Group
{
    public class ShortStory : Story
    {

        // No maximo 3 algarismos e não possui um padrão
        // Necessario compartilhar capitulo 
        public ShortStory()
        {
            
        }

        public ShortStory(string nome, Story padrao) : base(nome, padrao)
        {

        }

        public ShortStory(string nome, int quantidade, Story padrao) : base(nome, quantidade, padrao)
        {

        }

        public ShortStory(string nome, List<Story> stories, Story padrao) : base(nome, stories, padrao)
        {
        }

        public ShortStory(string nome, List<Story> stories, List<Content> conteudos, Story padrao) : base(nome, stories, conteudos, padrao)
        {

        }

        public ShortStory(string nome, List<Content> conteudos, Story padrao) : base(nome, conteudos, padrao)
        {

        }

    }
}
