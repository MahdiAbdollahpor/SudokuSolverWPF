using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace SudokuSolverWPF.Converters
{
	public class InverseBooleanConverter : IValueConverter
	{
		// متد تبدیل مقدار boolean به معکوس آن
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value is bool boolValue)
				return !boolValue;
			return true;
		}

		// متد تبدیل معکوس (غیرفعال در این پیاده‌سازی)
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}