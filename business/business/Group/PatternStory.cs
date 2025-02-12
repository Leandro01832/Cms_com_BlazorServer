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

       

        public PatternStory(string nome, List<Story> stories, Story padrao) : base(nome, stories, padrao)
        {
        }

       


    }
}
