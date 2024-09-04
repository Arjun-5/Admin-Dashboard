using System;
using System.Globalization;
using System.Windows.Data;

namespace StressCommunicationAdminPanel.Helpers
{
  /*  
   *  For more info refer this stackoverflow page
   *  
   *  https://stackoverflow.com/questions/3361362/wpf-radiobutton-two-binding-to-boolean-value
   */
  public class BoolRadioConverter : IValueConverter
  {
    public bool inverse {  get; set; }
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      bool boolValue = (bool)value;

      return inverse ? !boolValue : boolValue;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      bool boolValue = (bool)value;

      if (!boolValue)
      {
        return null;
      }

      return !inverse;
    }
  }
}
