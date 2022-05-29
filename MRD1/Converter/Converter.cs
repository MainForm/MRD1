using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

using MRD1.ViewModel;

namespace MRD1.Converter
{
    public class BleanToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (bool)value ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not Visibility)
                return false;

            Visibility visibility = (Visibility)value;

            if (visibility == Visibility.Visible)
                return true;

            return false;
        }
    }

    public class NotBleanToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return !(bool)value ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not Visibility)
                return false;

            Visibility visibility = (Visibility)value;

            if (visibility != Visibility.Visible)
                return true;

            return false;
        }
    }

    public class DateTimeToYYYYMMDDConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            DateTime date = (DateTime)value;

            return date.ToString("yyyy-MM-dd");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string datetime = (string)value;

            return System.Convert.ToDateTime(datetime);
        }
    }

    public class DateTimeToYYYYMMDD_HHMMSSConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            DateTime date = (DateTime)value;

            return date.ToString("yyyy-MM-dd HH:mm:ss");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string datetime = (string)value;

            return System.Convert.ToDateTime(datetime);
        }
    }

    public class MeasureButtonConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            MeasureStatus measureStatus = (MeasureStatus)value;

            switch (measureStatus)
            {
                case MeasureStatus.None:
                    return "준비";
                case MeasureStatus.Ready:
                    return "시작";
                case MeasureStatus.Start:
                    return "측정 중...";
            }

            return "Error";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class LedPoisitionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            LedPosition ledPosition = (LedPosition)value;

            return (int)ledPosition;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (LedPosition)(int)value;
        }
    }

    public class GenderToIntConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Gender ledPosition = (Gender)value;

            return (int)ledPosition;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (Gender)(int)value;
        }
    }
}
