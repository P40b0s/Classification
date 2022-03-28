using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using System.ComponentModel;
using System.Windows.Controls;

namespace Classification
{
    public class ReadyListBoxClass : INotifyPropertyChanged
    {
        public ReadyListBoxClass() { }
        static IconsResources ir = new IconsResources();
        public ReadyListBoxClass(int _itemNumber, string _date, string _number, bool check, List<string> rub, List<string> keys)
        {
            this.IndexNum = _itemNumber;
            this._Number = _number;
            this._SignDate = _date;
            this._Rubriki = rub;
            this._Keywordd = keys;
            this._Check = check;

        }
        private string _Number { get; set; }
        public string Number
        {
            get
            {
                return this._Number;
            }
            set
            {
                if (this.Number != value)
                {
                    this._Number = value;
                    this.OnPropertyChanged("Number");
                }
            }
        }
        private string _SignDate { get; set; }
        public string SignDate
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
                    this.OnPropertyChanged("SignDate");
                }
            }
        }
        private string _ActName { get; set; }
        public string ActName
        {
            get
            {
                return this._ActName;
            }
            set
            {
                if (this.ActName != value)
                {
                    this._ActName = value;
                    this.OnPropertyChanged("ActName");
                }
            }
        }
        private List<string> _Rubriki { get; set; }
        public List<string> Rubriki
        {
            get
            {
                return this._Rubriki;
            }
            set
            {
                if (this.Rubriki != value)
                {
                    this._Rubriki = value;
                    this.OnPropertyChanged("Rubriki");
                }
            }
        }
        private List<string> _Keywordd { get; set; }
        public List<string> Keywordd
        {
            get
            {                
                return this._Keywordd;
            }
            set
            {
                if (this.Keywordd != value)
                {
                    this._Keywordd = value;
                    this.OnPropertyChanged("Keywordd");
                }
            }
        }
        public int IndexNum { get; set; }
        private bool _Check { get; set; }
        public bool Check
        {
            get
            {
                return this._Check;
            }
            set
            {
                if (this.Check != value)
                {
                    this._Check = value;
                    this.OnPropertyChanged("Check");
                }
            }
        }
        public double Opacity
        {
            get
            {
                double d = 1;
                if (Keywordd.Count >= 3 && Rubriki.Count >= 1)
                {
                    d = 0.6;
                }
                return d;
            }
        }
       
        public static VisualBrush icon
        {
            get
            {               
               Visual c =  ir.Icons["appbar_sleep"] as Canvas;
               VisualBrush vb = new VisualBrush(c);
               return vb;
            }
        }
      
        public Brush Col
        {
            get
            {
                if (Check == true)
                {
                    SolidColorBrush cb = new SolidColorBrush(Color.FromRgb(10, 241, 115));
                    return cb;
                }
                else
                {
                    SolidColorBrush cb = new SolidColorBrush(Color.FromRgb(251, 123, 102));
                    return cb;
                }
            }
            set
            {

            }
        }
        [XmlIgnore]
        public Brush BorderColor
        {
            get
            {
                if (Keywordd.Count >= 3 && Rubriki.Count >= 1)
                {
                    SolidColorBrush cb = new SolidColorBrush(Color.FromRgb(10, 241, 115));
                    return cb;
                }
                else
                {
                    SolidColorBrush cb = new SolidColorBrush(Color.FromRgb(251, 123, 102));
                    return cb;
                }
            }
            set
            {

            }

        }
        [XmlIgnore]
        public Visibility rubVis
        {
            get
            {
                if (Rubriki.Count >= 1)
                {
                    return Visibility.Visible;
                }
                else
                {
                    return Visibility.Hidden;
                }
            }
            set { }
        }
        [XmlIgnore]
        public Visibility keyVis
        {
            get
            {
                if (Keywordd.Count >= 1)
                {
                    return Visibility.Visible;
                }
                else
                {
                    return Visibility.Hidden;
                }
            }
            set { }

        }
        public static System.Globalization.CultureInfo RuCult = new System.Globalization.CultureInfo("ru");
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }


    }

    class IconsResources
    {
        public IconsResources()
        {
       
                ResourceDictionary rd = new ResourceDictionary();
                rd.Source = new System.Uri("pack://application:,,,/MahApps.Metro.Resources;component/Icons.xaml");
                this.Icons = rd;
        }
        public ResourceDictionary Icons { get; set; }
    }
}
