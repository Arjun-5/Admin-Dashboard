using System;

namespace StressCommunicationAdminPanel.Models
{
  public class StressMessage
  {
    public DateTime timeSent { get; set; }

    public StressEffectCategory currentStressCategory { get; set; }

    public double stressLevel { get; set; }
  }
}
