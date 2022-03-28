using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Classification.Core.Models
{
    public class KeysAndRubricsModel : INotifyPropertyChanged
    {
        #region INotifyPropertyChanged
        [field: NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName]string propertyName = null)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion

        private Guid _ItemId { get; set; }
        /// <summary>
        /// Заполнено ли минимальное количество рубрик у данного документа
        /// </summary>
        public Guid ItemId
        {
            get
            {
                return this._ItemId;
            }
            set
            {
                if (this.ItemId != value)
                {
                    this._ItemId = value;
                    this.OnPropertyChanged();
                }
            }
        }

        private string _Item { get; set; }
        /// <summary>
        /// Заполнено ли минимальное количество рубрик у данного документа
        /// </summary>
        public string Item
        {
            get
            {
                return this._Item;
            }
            set
            {
                if (this.Item != value)
                {
                    this._Item = value;
                    this.OnPropertyChanged();
                }
            }
        }

        private int _Weight { get; set; }
        /// <summary>
        /// Заполнено ли минимальное количество рубрик у данного документа
        /// </summary>
        public int Weight
        {
            get
            {
                return this._Weight;
            }
            set
            {
                if (this.Weight != value)
                {
                    this._Weight = value;
                    this.OnPropertyChanged();
                }
            }
        }

        private int _InCount { get; set; }
        /// <summary>
        /// Заполнено ли минимальное количество рубрик у данного документа
        /// </summary>
        public int InCount
        {
            get
            {
                return this._InCount;
            }
            set
            {
                if (this.InCount != value)
                {
                    this._InCount = value;
                    this.OnPropertyChanged();
                }
            }
        }

        public override bool Equals(object obj)
        {
            var item = obj as KeysAndRubricsModel;
            if (item == null)
            {
                return false;
            }
            return this.ItemId.Equals(item.ItemId);
        }
        public override int GetHashCode()
        {
            return ItemId.GetHashCode();
        }
        public override string ToString()
        {
            return this.ItemId.ToString("D");
        }
    }
}
