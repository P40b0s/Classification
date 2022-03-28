using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Classification
{
    class KeyRingsClass
    {
        public KeyRingsClass(List<string> KeysList)
        {
            this.Keyrings = KeysList;
        }


        public List<string> Keyrings { get; set; }
    }

    class RubricsClass
    {
        public RubricsClass(List<string> RubricsList)
        {
            this.Rubrics = RubricsList;
        }

        public List<string> Rubrics { get; set; }
    }

   
}
