using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.IO;

namespace Classification
{
    /// <summary>
    /// Логика взаимодействия для Pass.xaml
    /// </summary>
    public partial class Pass : Window
    {
        public Pass()
        {
            InitializeComponent();

            passbox.PasswordChanged += (s, e) => {

                if (passbox.Password == "iksar666")
                {
                    passbox.Background = Brushes.GreenYellow;
                }
                else
                {
                    passbox.Background = Brushes.LightPink;
                }
                        
            };
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (passbox.Password == "iksar666")
            {
                DirectoryInfo bas = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);
                FileInfo fi = bas.GetFiles("*.xls").FirstOrDefault();
                if (fi != null)
                {
                    if (clear.IsChecked == true)
                    {

                        ExcelImportExport.ImportFromExcel(fi.FullName, false);
                        this.Close();
                    }
                    else
                    {
                        ExcelImportExport.ImportFromExcel(fi.FullName);
                        this.Close();
                    }
                }
                else
                {
                    MessageBox.Show("Файл для иморта не найден!");
                }
            }
            else
            {
                MessageBox.Show("Неправильный пароль!");
            }
        }
    }
}
