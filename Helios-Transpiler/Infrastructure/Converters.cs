using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Helios_Transpiler.Infrastructure
{
    /// <summary>
    /// Returns a pin emoji depending on whether the item is pinned.
    /// true  → 📌  (pinned, gold)
    /// false → 📍  (unpinned, muted)
    /// </summary>
    [ValueConversion(typeof(bool), typeof(string))]
    public class PinIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            => value is true ? "📌" : "📍";

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotSupportedException();
    }

    /// <summary>
    /// Collapses a panel when count == 0, shows it otherwise.
    /// </summary>
    [ValueConversion(typeof(int), typeof(Visibility))]
    public class CountToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            => value is int n && n > 0 ? Visibility.Visible : Visibility.Collapsed;

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotSupportedException();
    }
}
