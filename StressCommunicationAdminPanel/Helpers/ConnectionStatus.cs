using Newtonsoft.Json;

namespace StressCommunicationAdminPanel.Helpers
{
  public class ConnectionStatus
  {
    public static bool DeserializeData(string json) => JsonConvert.DeserializeObject<bool>(json);
  }
}
