using Newtonsoft.Json.Converters;
using Newtonsoft.Json;

namespace StressCommunicationAdminPanel.Helpers
{
  public class JsonConverter
  {
    public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
    {
      MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
      DateParseHandling = DateParseHandling.None,
      Converters =
      {
          new StringEnumConverter()
      },
    };
  }
}