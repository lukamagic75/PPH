using System;
using System.Globalization;
using Avalonia.Data.Converters;
using PPH.Library.Models;

namespace PPH.Converters;

public class WordToStringConverter : IValueConverter {
    public object Convert(object value, Type targetType, object parameter,
        CultureInfo culture) =>
        value is ObjectWord objectWord
            ? $" {objectWord.Accent}   {objectWord.CnMeaning}"
            : null;

    public object ConvertBack(object value, Type targetType, object parameter,
        CultureInfo culture) =>
        throw new InvalidOperationException();
}