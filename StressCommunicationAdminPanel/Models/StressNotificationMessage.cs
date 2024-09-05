using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace StressCommunicationAdminPanel.Models
{
  public class StressNotificationMessage
  {
    [JsonConverter(typeof(StringEnumConverter))]
    public StressEffectCategory currentStressCategory;

    public double stressLevel;

    public bool cancellationStatus;

    public StressNotificationMessage(StressEffectCategory currentStressCategory, double stressLevel, bool isCancelled)
    {
      this.currentStressCategory = currentStressCategory;
      
      this.stressLevel = stressLevel;

      this.cancellationStatus = isCancelled;
    }
    public StressNotificationMessage() 
    {

    }
  }
}