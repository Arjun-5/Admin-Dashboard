namespace StressCommunicationAdminPanel.Models
{
  public class StressMessageAppConfig
  {
    public string ipAddress { get; set; }

    public int messageTimeInterval { get; set; }

    public int stressMessageSendingPort { get; set; }

    public bool shouldUseDebugSetup {  get; set; } 

    public bool shouldUseSplashScreen { get; set; }
  }
}