using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Classification.Core.Models
{
    public class AutoClassificationModel
    {
        public AutoClassificationModel()
        {
            Rubrics = new List<Guid>();
            IdentifyKeys = new List<string>();
        }

        public bool IsRegex { get; set; }
        public string ContainsType { get; set; }
        public string SearchModel { get; set; }
        public List<Guid> Rubrics { get; set; }
        public List<string> IdentifyKeys { get; set; }
    }
}
