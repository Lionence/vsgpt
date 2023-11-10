using System;
using System.Globalization;
using System.Windows.Data;

namespace Lionence.VSGPT.Controls.Converters
{
    public class WidthConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double actualWidth && parameter is string offsetString)
            {
                if (double.TryParse(offsetString, out double offset))
                {
                    return Math.Max(0, actualWidth - offset);
                }
            }

            return Binding.DoNothing;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
