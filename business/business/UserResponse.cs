﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace business.business
{
    public class UserResponse : BaseModel
    {
        public string user { get; set; }
        public int capitulo { get; set; }
        public int pasta { get; set; }
        public int Pergunta { get; set; }     

        public int resposta1 { get; set; }
        public bool exempoloR1 { get; set; }
        public int resposta2 { get; set; }
        public bool exempoloR2 { get; set; }
        public int resposta3 { get; set; }
        public bool exempoloR3 { get; set; }
        public int resposta4 { get; set; }
        public bool exempoloR4 { get; set; }
        public int resposta5 { get; set; }
        public bool exempoloR5 { get; set; }
        public int resposta6 { get; set; }
        public bool exempoloR6 { get; set; }
        public int resposta7 { get; set; }
        public bool exempoloR7 { get; set; }
        public int resposta8 { get; set; }
        public bool exempoloR8 { get; set; }
        public int resposta9 { get; set; }
        public bool exempoloR9 { get; set; }
        public int resposta10 { get; set; }
        public bool exempoloR10 { get; set; }
    }
}