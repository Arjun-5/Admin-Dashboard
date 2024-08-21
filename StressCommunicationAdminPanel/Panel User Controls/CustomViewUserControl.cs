using System.ComponentModel;
using System.Windows.Controls;

namespace StressCommunicationAdminPanel.Panel_User_Controls
{
  public class CustomViewUserControl : UserControl, INotifyPropertyChanged
  {
    public event PropertyChangedEventHandler PropertyChanged;

    protected void OnPropertyChanged(string propertyName)
    {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
  }
}