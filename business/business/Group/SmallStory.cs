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

        

        public SmallStory(string nome, List<Story> stories, Story padrao) : base(nome, stories, padrao)
        {
        }



    }
}
