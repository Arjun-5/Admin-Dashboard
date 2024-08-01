using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StressCommunicationAdminPanel
{
  public class StressMessageViewModel
  {
    public ObservableCollection<StressNotificationMessage> stressNotificationMessages { get; set; }

    public StressMessageViewModel() 
    {
      stressNotificationMessages = new ObservableCollection<StressNotificationMessage>();
    }
  }
}
