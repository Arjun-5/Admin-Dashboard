using Newtonsoft.Json;
using StressCommunicationAdminPanel.Models;
using System;
using System.IO;

namespace StressCommunicationAdminPanel.Helpers
{
    public class ConfigHandler
    {
    public static StressMessageAppConfig LoadConfig()
    {
      string configFileLocation = Path.Combine(Environment.CurrentDirectory, "StressMessageConfig.json");

      try
      {
        var processedConfigData = File.ReadAllText(configFileLocation);

        return JsonConvert.DeserializeObject<StressMessageAppConfig>(processedConfigData);
      }
      catch (Exception ex)
      {
        Console.WriteLine($"Error loading config: {ex.Message}");

        return null;
      }
    }
  }
}