using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Classification.Core.Interfaces
{
    public interface IClassificationBase
    {
        Guid Id { get;}
        DateTime PortionUploadTime {get;}
        DateTime? ExcelExportTime { get; set; }
        DateTime? ChangeTime { get; set; }
        void Update(IClassification cdoc);
    }
}
