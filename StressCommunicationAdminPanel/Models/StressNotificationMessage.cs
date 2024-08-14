using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace StressCommunicationAdminPanel.Models
{
  public class StressNotificationMessage
  {
    [JsonConverter(typeof(StringEnumConverter))]
    public StressEffectCategory currentStressCategory;

    public double stressLevel;

    public StressNotificationMessage(StressEffectCategory currentStressCategory, double stressLevel)
    {
      this.currentStressCategory = currentStressCategory;
      
      this.stressLevel = stressLevel;
    }
  }
}