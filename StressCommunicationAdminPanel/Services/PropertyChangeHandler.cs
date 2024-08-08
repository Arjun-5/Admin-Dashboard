using System.ComponentModel;

namespace StressCommunicationAdminPanel.Services
{
  public class PropertyChangeHandler : INotifyPropertyChanged
  {
    public event PropertyChangedEventHandler PropertyChanged;

    protected void OnPropertyChanged(string propertyName)
    {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
  }
}