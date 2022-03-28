using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace KwywordsUniqueSetter
{
   public class Model : INotifyPropertyChanged
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

        private Guid _Id { get; set; }
        /// <summary>
        /// Название правового акта
        /// </summary>
        public Guid Id
        {
            get
            {
                return this._Id;
            }
            set
            {
                if (this.Id != value)
                {
                    this._Id = value;
                    this.OnPropertyChanged();
                }
            }
        }

        private string _Key { get; set; }
        /// <summary>
        /// Название правового акта
        /// </summary>
        public string Key
        {
            get
            {
                return this._Key;
            }
            set
            {
                if (this.Key != value)
                {
                    this._Key = value;
                    this.OnPropertyChanged();
                }
            }
        }

        private int  _Weight { get; set; }
        /// <summary>
        /// Номер правового акта
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
    }
}
