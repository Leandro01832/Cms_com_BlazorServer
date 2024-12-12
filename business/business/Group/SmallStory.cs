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

        public SmallStory(List<Story> stories, string nome)
        {
            PaginaPadraoLink = stories.Count + 1;
            Nome = nome;
        }
    }
}
