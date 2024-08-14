using System.ComponentModel;

namespace StressCommunicationAdminPanel.Models
{
  public class StressEffectMessageData : INotifyPropertyChanged
  {
    private int _messageCount;
    public StressEffectCategory effectType { get; set; }

    public int messageCount
    {
      get => _messageCount;

      set
      {
        if (_messageCount != value)
        {
          _messageCount = value;

          OnPropertyChanged(nameof(messageCount));
        }
      }
    }
    public event PropertyChangedEventHandler PropertyChanged;

    protected void OnPropertyChanged(string propertyName)
    {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
  }
}