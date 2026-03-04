using System;
using System.Globalization;
using Microsoft.Maui.Controls;

namespace TracNghiemLaiXe.Converters
{
    public class IsNotNullConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return value != null;
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class InvertedBoolConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is bool b)
                return !b;
            return false;
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is bool b)
                return !b;
            return false;
        }
    }

    /// <summary>
    /// Converts a string to bool - returns true if string is not null or empty.
    /// Supports ConverterParameter="invert" to return the opposite value.
    /// </summary>
    public class StringToBoolConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            bool hasValue = value is string str
                ? !string.IsNullOrEmpty(str)
                : value != null;

            bool invert = parameter is string p && p == "invert";
            return invert ? !hasValue : hasValue;
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
