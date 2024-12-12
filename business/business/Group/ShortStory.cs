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

        public ShortStory(List<Story> stories, string nome)
        {
            PaginaPadraoLink = stories.Count + 1;
            Nome = nome;
        }

    }
}
