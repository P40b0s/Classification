using Classification.Core.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Classification.Core.Interfaces
{
    public interface IClassification : IClassificationBase
    {

        #region Ключи готовности
        bool IsNotSaved { get; set; }
        #endregion

        #region Реквизиты документа
        string ActNumber { get; }
        string ActName { get; }
        DateTime SignDate { get; }
        AsyncObservableCollection<KeysAndRubricsModel> Keys { get; set; }
        AsyncObservableCollection<KeysAndRubricsModel> Rubrics { get; set; }
        
        #endregion
    }
}
