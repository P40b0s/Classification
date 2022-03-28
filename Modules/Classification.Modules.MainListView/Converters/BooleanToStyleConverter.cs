using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace Classification.Modules.MainListViewModule.Converters
{
    public class BooleanToStyleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Uri resourceLocator = new Uri("/MainListViewModule;component/Templates/ListView/ItemStyle.xaml", System.UriKind.Relative);
            ResourceDictionary resourceDictionary = (ResourceDictionary)Application.LoadComponent(resourceLocator);
            if((bool)value)
            {
                return resourceDictionary["ListViewItemStyleIsReadyVisible"] as Style;
            }
            else
            {
                return resourceDictionary["ListViewItemStyle"] as Style;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
