using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using business.Group;

namespace business.business.Group
{
    public class PatternStory : Story
    {
        public PatternStory()
        {
            
        }

        public PatternStory(string nome, Story padrao) : base(nome, padrao) 
        {
            
        }

        public PatternStory(string nome, int quantidade, Story padrao) : base(nome, quantidade, padrao) 
        {
            
        }

        public PatternStory(string nome, List<Story> stories, Story padrao) : base(nome, stories, padrao)
        {
        }

        public PatternStory(string nome, List<Story> stories, List<Content> conteudos, Story padrao) : base(nome, stories, conteudos, padrao)
        {

        }

        public PatternStory(string nome, List<Content> conteudos, Story padrao) : base(nome, conteudos, padrao) 
        {
            
        }

    }
}
