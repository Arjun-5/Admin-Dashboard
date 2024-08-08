using FontAwesome.Sharp;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Windows.Media;
using StressCommunicationAdminPanel.Models;
using StressCommunicationAdminPanel.Services;
using StressCommunicationAdminPanel.Helpers;
using StressCommunicationAdminPanel.Commands;
using LiveChartsCore.SkiaSharpView;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json;
using System;

namespace StressCommunicationAdminPanel.ViewModels
{
  public class StressMessageViewModel : AppViewModel
  {
    private readonly StressMessageManager _messageManager;

    private readonly StressMessagePieChartHelper _pieChartHelper;

    private ObservableCollection<StressMessage> _messages = new ObservableCollection<StressMessage>();
    public ObservableCollection<StressMessage> Messages
    {
      get 
      { 
        return _messages; 
      }
      
      set 
      { 
        _messages = value; 
        
        OnPropertyChanged(nameof(Messages)); 
      }
    }

    public ObservableCollection<PieSeries<int>> StressEffectMessagesSeriesCollection  => _pieChartHelper.StressEffectMessagesSeriesCollection;

    public int MessagesSent => _messageManager.MessagesSent;

    private string _connectionStatus;
    public string ConnectionStatus
    {
      get 
      { 
        return _connectionStatus; 
      }

      set 
      { 
        _connectionStatus = value; 
        
        OnPropertyChanged(nameof(ConnectionStatus)); 
      }
    }

    private IconChar _connectionStatusIcon;
    public IconChar ConnectionStatusIcon
    {
      get 
      { 
        return _connectionStatusIcon; 
      }

      set 
      { 
        _connectionStatusIcon = value; 
        
        OnPropertyChanged(nameof(ConnectionStatusIcon)); 
      }
    }

    private Brush _connectionStatusIconColor;
    public Brush ConnectionStatusIconColor
    {
      get 
      { 
        return _connectionStatusIconColor; 
      }
      
      set 
      { 
        _connectionStatusIconColor = value; 
        
        OnPropertyChanged(nameof(ConnectionStatusIconColor)); 
      }
    }

    private Brush _connectionStatusColor;
    public Brush ConnectionStatusColor
    {
      get 
      { 
        return _connectionStatusColor; 
      }
      
      set 
      { 
        _connectionStatusColor = value; 
        
        OnPropertyChanged(nameof(ConnectionStatusColor)); 
      }
    }
    public ICommand ToggleServerStateCommand { get; }

    public StressMessageViewModel()
    {
      _messageManager = new StressMessageManager(OnServerStateChanged, OnUpdateAdminPanelCharts);

      _pieChartHelper = new StressMessagePieChartHelper();

      _messageManager.PropertyChanged += (s, e) =>
      {
        if (e.PropertyName == nameof(StressMessageManager.MessagesSent))
        {
          OnPropertyChanged(nameof(MessagesSent));
        }
      };

      _pieChartHelper.PropertyChanged += (s, e) =>
      {
        if (e.PropertyName == nameof(StressMessagePieChartHelper.StressEffectMessagesSeriesCollection))
        {
          OnPropertyChanged(nameof(StressEffectMessagesSeriesCollection));
        }
      };

      ConfigureConnectionStatusDefaults();
      
      ToggleServerStateCommand = new RelayCommand(_messageManager.ManageServerState);
    }

    private void ConfigureConnectionStatusDefaults()
    {
      ConnectionStatus = "Not Connected!!!";

      ConnectionStatusIcon = IconChar.UserSlash;
      
      ConnectionStatusColor = new SolidColorBrush(Colors.Red);
      
      ConnectionStatusIconColor = new SolidColorBrush(Colors.OrangeRed);
    }

    private void OnServerStateChanged(ServerState state, IconChar icon, Brush color, Brush iconColor)
    {
      ConnectionStatus = state.ToString();
      
      ConnectionStatusIcon = icon;
      
      ConnectionStatusColor = color;
      
      ConnectionStatusIconColor = iconColor;
    }

    private void OnStressMessageReceived(StressNotificationMessage message)
    {
      Messages.Add(new StressMessage
      {
        timeSent = DateTime.Now,
        message = JsonConvert.SerializeObject(message, new StringEnumConverter())
      });
    }
    private void OnUpdateAdminPanelCharts(StressNotificationMessage message)
    {
      _pieChartHelper.UpdatePieChartData(message.currentStressEffect);
    }
  }
}