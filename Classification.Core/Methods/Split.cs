using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Classification.Core.Methods
{
   public class Split
    {
        public static readonly char[] splitArray = new char[] {
                 '.',
                 ')',
                 '(',
                 '[',
                 ']',
                 '>',
                 '<',
                 ':',
                 ';',
                 '\n',
                 '\t',
                 '\r',
                 '"',
                 ',',
                 '|',
                 '-',
                 ' '};
    }
}
