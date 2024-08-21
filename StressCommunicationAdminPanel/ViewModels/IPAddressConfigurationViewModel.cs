using Newtonsoft.Json;
using StressCommunicationAdminPanel.Commands;
using StressCommunicationAdminPanel.Helpers;
using StressCommunicationAdminPanel.Models;
using System.IO;
using System.Windows.Input;

namespace StressCommunicationAdminPanel.ViewModels
{
  public class IPAddressConfigurationViewModel : AppViewModel
  {
    private string _ipAddress;

    private int _portNumber;

    private int _timeInterval;

    public string IpAddress
    {
      get => _ipAddress;

      set { _ipAddress = value; OnPropertyChanged(nameof(IpAddress)); }
    }

    public int PortNumber
    {
      get => _portNumber;

      set { _portNumber = value; OnPropertyChanged(nameof(PortNumber)); }
    }

    public int TimeInterval
    {
      get => _timeInterval;

      set { _timeInterval = value; OnPropertyChanged(nameof(TimeInterval)); }
    }

    public ICommand SaveCommand { get; }
    public ICommand ClearCommand { get; }

    public IPAddressConfigurationViewModel()
    {
      SaveCommand = new RelayCommand(SaveConfigSettingsToFile);

      ClearCommand = new RelayCommand(ClearTextBoxes);
    }
    private void SaveConfigSettingsToFile()
    {
      var config = new StressMessageConfig
      {
        ipAddress = this.IpAddress,
        stressMessageSendingPort = this.PortNumber,
        messageTimeInterval = this.TimeInterval
      };

      string json = JsonConvert.SerializeObject(config,Formatting.Indented);
      
      File.WriteAllText("StressMessageConfig.json", json);
    }

    private void ClearTextBoxes()
    {
      IpAddress = string.Empty;

      PortNumber = 0;
      
      TimeInterval = 0;
    }
    public void ReadAndSetConfigDefaultValue()
    {
      var config = ConfigHandler.LoadConfig();

      if (config != null) 
      {
        IpAddress = config.ipAddress;

        PortNumber = config.stressMessageSendingPort;

        TimeInterval = config.messageTimeInterval;
      }
    }
  }
}
