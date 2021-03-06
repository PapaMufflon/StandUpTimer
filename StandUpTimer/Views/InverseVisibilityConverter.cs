using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace StandUpTimer.Views
{
    public class InverseVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var visibility = value as Visibility? ?? Visibility.Visible;

            return visibility == Visibility.Visible ? Visibility.Hidden : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}