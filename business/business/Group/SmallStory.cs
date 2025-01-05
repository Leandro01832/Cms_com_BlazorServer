using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using business.Group;

namespace business.business.Group
{
    public class SmallStory : Story
    {
        // No maximo 4 algarismos e não possui um padrão
        // Necessario compartilhar capitulo 
        public SmallStory()
        {

        }

        public SmallStory(string nome, Story padrao) : base(nome, padrao)
        {

        }

        public SmallStory(string nome, int quantidade, Story padrao) : base(nome, quantidade, padrao)
        {

        }

        public SmallStory(string nome, List<Story> stories, Story padrao) : base(nome, stories, padrao)
        {
        }

        public SmallStory(string nome, List<Story> stories, List<Content> conteudos, Story padrao) : base(nome, stories, conteudos, padrao)
        {

        }

        public SmallStory(string nome, List<Content> conteudos, Story padrao) : base(nome, conteudos, padrao)
        {

        }

    }
}
