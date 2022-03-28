using System;
using System.Collections.Generic;
using System.Linq;
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
using MahApps.Metro.Controls;
using System.Reflection;
using System.IO;
using NLog;
using Prism.Events;

namespace Classification.Shell.Views
{

    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class StartWindow : MetroWindow
    {
        private string LaunchDir { get; set; }
        private readonly Logger logger = LogManager.GetCurrentClassLogger();
        IEventAggregator eventAggregator;
        public StartWindow(IEventAggregator _eventAggregator)
        {
            InitializeComponent();
            try
            {
                eventAggregator = _eventAggregator;
                //Core.Collections.InitializeStaticCollections(_eventAggregator);
                getTitle();
            }
            catch (System.Exception ex)
            {
                logger.Fatal(ex);
            }
        }

        private void getTitle()
        {
            try
            {
                Assembly exe = Assembly.Load(File.ReadAllBytes("Classification.Shell.exe"));
                LaunchDir = new Uri(exe.CodeBase).LocalPath.Replace("Classification.Shell.exe", null);
                this.Title = string.Format("Проект \"Классификация нормативных актов Москвы\" {0}", exe.FullName.Split(',')[1].Replace("Version=", ""));
                exe = null;
            }
            catch (System.Exception ex)
            {
                logger.Fatal(ex);
            }
        }
    }
}
