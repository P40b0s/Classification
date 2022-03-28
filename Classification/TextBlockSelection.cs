using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Classification
{
    public class TextBlockSelection
    {
        public TextBlockSelection(string before, string after)
        {
            this.TextBefore = before;
            this.TextSelect = after;
        }
        public string TextBefore { get; set; }
        public string TextSelect { get; set; }
        public string TextBeforeSelect { get; set; }
        public TextBlockSelection() { }
    }
}
