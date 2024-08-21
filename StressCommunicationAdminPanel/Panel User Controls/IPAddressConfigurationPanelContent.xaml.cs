using System;
using System.Windows.Controls;

namespace StressCommunicationAdminPanel.Panel_User_Controls
{
  public partial class IPAddressConfigurationPanelContent : UserControl
  {
    public IPAddressConfigurationPanelContent(Action onCompleteInitialization)
    {
      InitializeComponent();

      onCompleteInitialization?.Invoke();
    }
  }
}