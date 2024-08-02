using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace StressCommunicationAdminPanel.Model
{
  public class StressNotificationMessage
  {
    [JsonConverter(typeof(StringEnumConverter))]
    public StressEffectType currentStressEffect;

    public float stressLevel;

    public StressNotificationMessage(StressEffectType currentStressEffect, float stressLevel)
    {
      this.currentStressEffect = currentStressEffect;
      
      this.stressLevel = stressLevel;
    }
  }
}