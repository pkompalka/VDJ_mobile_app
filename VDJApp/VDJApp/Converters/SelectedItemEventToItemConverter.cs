using System;
using System.Globalization;
using Xamarin.Forms;

namespace VDJApp.Converters
{
    public class SelectedItemEventToItemConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var eventArgs = (SelectedItemChangedEventArgs)value;
			return eventArgs.SelectedItem;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
