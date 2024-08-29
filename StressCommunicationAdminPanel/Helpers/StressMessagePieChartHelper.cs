using StressCommunicationAdminPanel.Controller;
using StressCommunicationAdminPanel.Services;

namespace StressCommunicationAdminPanel.Helpers
{
  public class StressMessagePieChartHelper : PropertyChangeHandler
  {
    public AdminPanelMessageSentChartController adminPanelMessageSentChartController;

    public AdminPanelMessageReceivedChartController adminPanelMessageReceivedChartController;

    public StressMessagePieChartHelper()
    {
      adminPanelMessageSentChartController = new AdminPanelMessageSentChartController();  

      adminPanelMessageReceivedChartController = new AdminPanelMessageReceivedChartController();
    }
  }
}