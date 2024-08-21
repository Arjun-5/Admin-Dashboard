using System;
using System.Collections.Generic;
using System.Linq;
using StressCommunicationAdminPanel.Commands;
using System.ComponentModel;
using System.IO;
using System.Text.Json;
using System.Windows.Input;

namespace StressCommunicationAdminPanel.Panel_User_Controls
{
  public class IpAddressConfigViewModel : INotifyPropertyChanged
  {
    private string _ipAddress;
    private string _portNumber;
    private string _timeout;

    public string IpAddress
    {
      get => _ipAddress;
      set { _ipAddress = value; OnPropertyChanged(nameof(IpAddress)); }
    }

    public string PortNumber
    {
      get => _portNumber;
      set { _portNumber = value; OnPropertyChanged(nameof(PortNumber)); }
    }

    public string Timeout
    {
      get => _timeout;
      set { _timeout = value; OnPropertyChanged(nameof(Timeout)); }
    }

    public ICommand SaveCommand { get; }
    public ICommand ClearCommand { get; }

    public IpAddressConfigViewModel()
    {
      SaveCommand = new RelayCommand(SaveSettings);
      ClearCommand = new RelayCommand(ClearSettings);
    }

    private void SaveSettings()
    {
      var config = new
      {
        IpAddress = this.IpAddress,
        PortNumber = this.PortNumber,
        Timeout = this.Timeout
      };

      string json = JsonSerializer.Serialize(config);
      File.WriteAllText("config.json", json);
    }

    private void ClearSettings()
    {
      IpAddress = string.Empty;
      PortNumber = string.Empty;
      Timeout = string.Empty;
    }

    public event PropertyChangedEventHandler PropertyChanged;
    protected virtual void OnPropertyChanged(string propertyName)
    {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
  }
}