using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace RudycommerceWPF
{
    /// <summary>
    /// Can be used to convert values from wpf controls.    
    /// </summary>
    public class PercentageConverter : IValueConverter
    {
        // For example: If something has a variable width (for example stackpanel)
        // You can make a Child its width equal to a percentage of the parent's (stackpanel) ActualWidth (in XAML)

        // https://stackoverflow.com/questions/717299/wpf-setting-the-width-and-height-as-a-percentage-value

        public object Convert(object value,
            Type targetType,
            object parameter,
            System.Globalization.CultureInfo culture)
        {
            return System.Convert.ToDouble(value) *
                   System.Convert.ToDouble(parameter);
        }

        public object ConvertBack(object value,
            Type targetType,
            object parameter,
            System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
