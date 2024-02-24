using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows;

namespace RRuleGenerator
{
    public class EqualityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Check if value and parameter are equal
            bool areEqual = value.Equals(parameter);
            return areEqual ? TrueValue : FalseValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        // Define properties for true and false values
        public object TrueValue { get; set; }
        public object FalseValue { get; set; }
    }

    public class InverseBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolean)
            {
                return !boolean;
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolean)
            {
                return !boolean;
            }
            return value;
        }
    }

    public sealed class EnumToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            { return null; }

            return Strings.ResourceManager.GetString(value.ToString());
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string str = (string)value;

            foreach (object enumValue in Enum.GetValues(targetType))
            {
                if (str == Strings.ResourceManager.GetString(enumValue.ToString()))
                { return enumValue; }
            }

            throw new ArgumentException(null, nameof(value));
        }
    }

    public sealed class WeekEnumConverter : IValueConverter
    {
        private WeeklyWeekDays _selectedWeekDays;
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is WeeklyWeekDays selectedWeekDays &&
                parameter is WeeklyWeekDays parameterWeekDays)
            {
                _selectedWeekDays = selectedWeekDays;
                return selectedWeekDays.HasFlag(parameterWeekDays);
            }
            return Binding.DoNothing;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is bool isChecked &&
                parameter is WeeklyWeekDays parameterWeekDays)
            {
                return isChecked ? _selectedWeekDays | parameterWeekDays : _selectedWeekDays & ~parameterWeekDays;
            }
            return Binding.DoNothing;
        }
    }
}
