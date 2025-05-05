using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace SudokuSolverWPF.Converters
{
	public class IntToVisibilityConverter : IValueConverter
	{
		// متد تبدیل مقدار به Visibility
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return value is int num && num > 0 ? Visibility.Visible : Visibility.Collapsed;
		}

		// متد تبدیل معکوس (غیرفعال در این پیاده‌سازی)
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}