using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Classification.Core.Models
{
    public class TextInlineSelection : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string propertyName = null)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        public TextInlineSelection(Guid id, string source, string select)
        {
            this.Id = id;
            this.SourceText = source;
            this.SelectedText = select;
            this.Visible = true;
        }
        /// <summary>
        /// Эталонный текст в котором будет производиться выделение области
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
        private Guid _Id { get; set; }
        /// <summary>
        /// Эталонный текст в котором будет производиться выделение области
        /// </summary>
        public string SourceText
        {
            get
            {
                return this._SourceText;
            }
            set
            {
                if (this.SourceText != value)
                {
                    this._SourceText = value;
                    this.OnPropertyChanged();
                }
            }
        }
        private string _SourceText { get; set; }
        /// <summary>
        /// Выделяемый фрагмент текста (искомый кусок текста)
        /// </summary>
        public string SelectedText
        {
            get
            {
                return this._SelectedText;
            }
            set
            {
                if (this.SelectedText != value)
                {
                    this._SelectedText = value;
                    this.OnPropertyChanged();
                }
            }
        }
        private string _SelectedText { get; set; }
        /// <summary>
        /// Фрагмент текста до начала выделения
        /// </summary>
        public string TextBeforeSelect
        {
            get
            {
                return this._TextBeforeSelect;
            }
            set
            {
                if (this.TextBeforeSelect != value)
                {
                    this._TextBeforeSelect = value;
                    this.OnPropertyChanged();
                }
            }
        }

        private string _TextBeforeSelect { get; set; }
        /// <summary>
        /// Подходит ли данный элемент под параметры фильтрации
        /// </summary>
        public bool Visible
        {
            get
            {
                return this._Visible;
            }
            set
            {
                if (this.Visible != value)
                {
                    this._Visible = value;
                    this.OnPropertyChanged();
                }
            }
        }
        private bool _Visible { get; set; }
        public TextInlineSelection() { }


        public override bool Equals(object obj)
        {
            var item = obj as TextInlineSelection;
            if (item == null || item.SourceText == null)
            {
                return false;
            }
            return this.SourceText.ToLower().Trim().Equals(item.SourceText.ToLower().Trim());
        }
        public override int GetHashCode()
        {
            return SourceText.GetHashCode();
        }
        public override string ToString()
        {
            return this.SourceText;
        }
    }
}
