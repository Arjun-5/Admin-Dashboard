using StressCommunicationAdminPanel.Models;
using System;
using System.Collections.ObjectModel;

namespace StressCommunicationAdminPanel.ViewModels
{
  public class StresMessageInfoContentViewModel : AppViewModel
  {
    private ObservableCollection<StressMessage> _messages = new ObservableCollection<StressMessage>();

    public ObservableCollection<StressMessage> StressMessages
    {
      get
      {
        return _messages;
      }

      set
      {
        _messages = value;

        OnPropertyChanged(nameof(StressMessages));
      }
    }
    public void UpdateStressMessageInfoDataTable(StressNotificationMessage notificationMessage)
    {
      StressMessages .Add(new StressMessage
      {
        stressLevel = notificationMessage.stressLevel,
        currentStressCategory = notificationMessage.currentStressCategory,
        timeSent = DateTime.Now
      });
    }
  }
}
