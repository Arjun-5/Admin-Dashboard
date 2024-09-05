using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StressCommunicationAdminPanel.Refactoring
{
  public interface IStressMessageManager
  {
    int MessagesSent { get; set; }
    int MessagesReceived { get; set; }
    string DeviceName { get; set; }

    void ManageServerState();
  }
}
