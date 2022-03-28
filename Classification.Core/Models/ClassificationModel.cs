using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Classification.Core.Interfaces;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using NLog;
using System.Collections.ObjectModel;

namespace Classification.Core.Models
{
    public class ClassificationModel : IClassification, INotifyPropertyChanged
    {
        public ClassificationModel() { }
        public ClassificationModel(Guid id, string actname, string actnumber, DateTime signdate, DateTime? changeTime, bool isready)
        {
            this.Id = id;
            this.ActName = actname;
            this.ActNumber = actnumber;
            this.SignDate = signdate;
            this.ChangeTime = changeTime;
            IsReady = isready;
            Keys = new AsyncObservableCollection<KeysAndRubricsModel>();
            Rubrics = new AsyncObservableCollection<KeysAndRubricsModel>();
            Keys.DefaultComparer = new KeysAndRubricsComparer();
            Rubrics.DefaultComparer = new KeysAndRubricsComparer();
            Keys.CollectionChanged += Keys_CollectionChanged;
            Rubrics.CollectionChanged += Rubrics_CollectionChanged;
        }

        public ClassificationModel(Guid id, string actname, string actnumber, DateTime signdate, DateTime uploadTime)
        {
            this.Id = id;
            this.ActName = actname;
            this.ActNumber = actnumber;
            this.SignDate = signdate;
            this.PortionUploadTime = uploadTime;
            Keys = new AsyncObservableCollection<KeysAndRubricsModel>();
            Rubrics = new AsyncObservableCollection<KeysAndRubricsModel>();
            Keys.DefaultComparer = new KeysAndRubricsComparer();
            Rubrics.DefaultComparer = new KeysAndRubricsComparer();
            Keys.CollectionChanged += Keys_CollectionChanged;
            Rubrics.CollectionChanged += Rubrics_CollectionChanged;
        }

        private void Rubrics_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (Rubrics.Count >= 1)
            {
                IsRubricsReady = true;
            }
            else
            {
                IsRubricsReady = false;
            }
        }

        private void Keys_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (Keys.Count >= 3)
            {
                IsKeywordsReady = true;
            }
            else
            {
                IsKeywordsReady = false;
            }
        }

        [NonSerialized]
        public readonly Logger logger = LogManager.GetCurrentClassLogger();

        private string _ActName { get; set; }
        /// <summary>
        /// Название правового акта
        /// </summary>
        public string ActName
        {
            get
            {
                return this._ActName;
            }
            private set
            {
                if (this.ActName != value)
                {
                    this._ActName = value;
                    this.OnPropertyChanged();
                }
            }
        }

        private string _ActNumber { get; set; }
        /// <summary>
        /// Номер правового акта
        /// </summary>
        public string ActNumber
        {
            get
            {
                return this._ActNumber;
            }
            private set
            {
                if (this.ActNumber != value)
                {
                    this._ActNumber = value;
                    this.OnPropertyChanged();
                }
            }
        }

        private Guid _Id { get; set; }
        /// <summary>
        /// Идентификатор правового акта
        /// </summary>
        public Guid Id
        {
            get
            {
                return this._Id;
            }
            private set
            {
                if (this.Id != value)
                {
                    this._Id = value;
                    this.OnPropertyChanged();
                }
            }
        }

        private bool _IsNotSaved { get; set; }
        /// <summary>
        /// Выбран ли правовой акт для выгрузки документа в эксель файл
        /// </summary>
        public bool IsNotSaved
        {
            get
            {
                return this._IsNotSaved;
            }
            set
            {
                if (this.IsNotSaved != value)
                {
                    this._IsNotSaved = value;
                    this.OnPropertyChanged();
                }
            }
        }

        private bool _IsKeywordsReady { get; set; }
        /// <summary>
        /// Заполнено ли минимальное количество ключевых слов у данного документа
        /// </summary>
        public bool IsKeywordsReady
        {
            get
            {
                return this._IsKeywordsReady;
            }
            private set
            {
                if (this.IsKeywordsReady != value)
                {
                    this._IsKeywordsReady = value;
                    this.OnPropertyChanged();
                    if (value && IsRubricsReady)
                        IsNotSaved = true;
                    else
                        IsNotSaved = false;
                }
            }
        }

        private bool _IsReady { get; set; }
        /// <summary>
        /// Заполнены ли ключевые слова и рубрики у данного документа
        /// </summary>
        public bool IsReady
        {
            get
            {
                return this._IsReady;
            }
            set
            {
                if (this.IsReady != value)
                {
                    this._IsReady = value;
                    this.OnPropertyChanged();
                }
            }
        }

        private bool _IsRubricsReady { get; set; }
        /// <summary>
        /// Заполнено ли минимальное количество рубрик у данного документа
        /// </summary>
        public bool IsRubricsReady
        {
            get
            {
                return this._IsRubricsReady;
            }
            private set
            {
                if (this.IsRubricsReady != value)
                {
                    this._IsRubricsReady = value;
                    this.OnPropertyChanged();
                    if (value && IsKeywordsReady)
                        IsNotSaved = true;
                    else
                        IsNotSaved = false;
                }
            }
        }

      
       
        private AsyncObservableCollection<KeysAndRubricsModel> _Keys { get; set; }
        /// <summary>
        /// Коллекция ключевых слов данного документа
        /// </summary>
        public AsyncObservableCollection<KeysAndRubricsModel> Keys
        {
            get
            {
                return this._Keys;
            }
            set
            {
                if (this.Keys != value)
                {
                    this._Keys = value;
                    this.OnPropertyChanged();
                    if (value.Count >= 3)
                        this.IsKeywordsReady = true;
                    else
                        this.IsKeywordsReady = false;
                }
            }
        }

        private AsyncObservableCollection<KeysAndRubricsModel> _Rubrics { get; set; }
        /// <summary>
        /// Коллекция ключевых слов данного документа
        /// </summary>
        public AsyncObservableCollection<KeysAndRubricsModel> Rubrics
        {
            get
            {
                return this._Rubrics;
            }
            set
            {
                if (this.Rubrics != value)
                {
                    this._Rubrics = value;
                    this.OnPropertyChanged();
                    if (value.Count >= 1)
                        this.IsRubricsReady = true;
                    else
                        this.IsRubricsReady = false;
                }
            }
        }

        private DateTime _PortionUploadTime { get; set; }
        /// <summary>
        /// Заполнено ли минимальное количество рубрик у данного документа
        /// </summary>
        public DateTime PortionUploadTime
        {
            get
            {
                return this._PortionUploadTime;
            }
            private set
            {
                if (this.PortionUploadTime != value)
                {
                    this._PortionUploadTime = value;
                    this.OnPropertyChanged();
                }
            }
        }

        private DateTime _SignDate { get; set; }
        /// <summary>
        /// Заполнено ли минимальное количество рубрик у данного документа
        /// </summary>
        public DateTime SignDate
        {
            get
            {
                return this._SignDate;
            }
            private set
            {
                if (this.SignDate != value)
                {
                    this._SignDate = value;
                    this.OnPropertyChanged();
                }
            }
        }

        private DateTime? _ExcelExportTime { get; set; }
        /// <summary>
        /// Заполнено ли минимальное количество рубрик у данного документа
        /// </summary>
        public DateTime? ExcelExportTime
        {
            get
            {
                return this._ExcelExportTime;
            }
            set
            {
                if (this.ExcelExportTime != value)
                {
                    this._ExcelExportTime = value;
                    this.OnPropertyChanged();
                }
            }
        }

        private DateTime? _ChangeTime { get; set; }
        /// <summary>
        /// Заполнено ли минимальное количество рубрик у данного документа
        /// </summary>
        public DateTime? ChangeTime
        {
            get
            {
                return this._ChangeTime;
            }
            set
            {
                if (this.ChangeTime != value)
                {
                    this._ChangeTime = value;
                    this.OnPropertyChanged();
                }
            }
        }

        private int _ItemNumber { get; set; }
        /// <summary>
        /// Заполнено ли минимальное количество рубрик у данного документа
        /// </summary>
        public int ItemNumber
        {
            get
            {
                return this._ItemNumber;
            }
            set
            {
                if (this.ItemNumber != value)
                {
                    this._ItemNumber = value;
                    this.OnPropertyChanged();
                }
            }
        }


        public void Update(IClassification model)
        {
            this.ExcelExportTime = model.ExcelExportTime;
            this.IsNotSaved = model.IsNotSaved;
            this.ChangeTime = model.ChangeTime;
            //this.Keys = model.Keys;
            //this.Rubrics = model.Rubrics;
            if (Keys.Count >= 3)
                this.IsKeywordsReady = true;
            else
                this.IsKeywordsReady = false;

            if (Rubrics.Count >= 1)
                this.IsRubricsReady = true;
            else
                this.IsRubricsReady = false;
        }

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

        public override bool Equals(object obj)
        {
            var item = obj as ClassificationModel;
            if (item == null)
            {
                return false;
            }
            return this.Id.Equals(item.Id);
        }
        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
        public override string ToString()
        {
            return this.Id.ToString("D");
        }
    }
}
