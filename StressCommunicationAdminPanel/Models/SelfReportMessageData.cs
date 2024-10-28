using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;

namespace StressCommunicationAdminPanel.Models
{
  public class SelfReportMessageData
  {
    public DateTime timeSent { get; set; }

    [JsonConverter(typeof(StringEnumConverter))]
    public SelfReportStressStatus selfReportStressStatus { get; set; }
  }
}
