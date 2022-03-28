using Classification.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Classification.Core
{
    public class KeysAndRubricsComparer : IComparer<KeysAndRubricsModel>
    {
        public int Compare(KeysAndRubricsModel x, KeysAndRubricsModel y)
        {
            return string.Compare(x.Item, y.Item);
        }
    }
}
