using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace KwywordsUniqueSetter
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
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
        DB dbase;
        public MainWindow()
        {
            InitializeComponent();
            collection = new ObservableCollection<Model>();
            DataContext = this;
            dbase = new DB();
            LOAD();
        }

        private ObservableCollection<Model> _collection { get; set; }
        /// <summary>
        /// Название правового акта
        /// </summary>
        public ObservableCollection<Model> collection
        {
            get
            {
                return this._collection;
            }
            set
            {
                if (this.collection != value)
                {
                    this._collection = value;
                    this.OnPropertyChanged();
                }
            }
        }

        private string _ActNumber { get; set; }
        /// <summary>
        /// Название правового акта
        /// </summary>
        public string ActNumber
        {
            get
            {
                return this._ActNumber;
            }
            set
            {
                if (this.ActNumber != value)
                {
                    this._ActNumber = value;
                    this.OnPropertyChanged();
                }
            }
        }

        private DateTime _SignDate { get; set; }
        /// <summary>
        /// Название правового акта
        /// </summary>
        public DateTime SignDate
        {
            get
            {
                return this._SignDate;
            }
            set
            {
                if (this.SignDate != value)
                {
                    this._SignDate = value;
                    this.OnPropertyChanged();
                }
            }
        }


        private async void LOAD()
        {
            await dbase.GetKeywords(collection);
            SignDate = DateTime.Now;
        }
        
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            dbase.SaveRecords(collection);
        }

        private void DelAct(object sender, RoutedEventArgs e)
        {
            dbase.CancelReadyDocument(ActNumber, SignDate);
        }
    }
}
