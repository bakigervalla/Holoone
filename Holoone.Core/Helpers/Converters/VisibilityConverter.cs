using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Markup;

namespace Holoone.Core.Helpers.Converters
{
    public class VisibilityConverter : MarkupExtension, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {

            bool IsReversed = false;
            if (parameter != null)
                IsReversed = bool.Parse(parameter.ToString());

            if ((bool)value == true)
                return IsReversed ? "Collapsed" : "Visible";
            else
                return IsReversed ? "Visible" : "Collapsed";

        }
        public object ConvertBack(object value, Type targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
    }

}