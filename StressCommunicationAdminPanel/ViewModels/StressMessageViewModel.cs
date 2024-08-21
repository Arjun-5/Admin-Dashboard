using FontAwesome.Sharp;
using System.Collections.ObjectModel;
using System.Windows.Media;
using StressCommunicationAdminPanel.Models;
using StressCommunicationAdminPanel.Services;
using StressCommunicationAdminPanel.Helpers;
using LiveChartsCore.SkiaSharpView;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json;
using System;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Input;
using StressCommunicationAdminPanel.Commands;

namespace StressCommunicationAdminPanel.ViewModels
{
  public class StressMessageViewModel : AppViewModel
  {
    private readonly StressMessageManager _messageManager;

    private readonly StressMessagePieChartHelper _pieChartHelper;

    private readonly StressMessageStatusBarHelper _statusBarHelper;

    private ObservableCollection<StressMessage> _messages = new ObservableCollection<StressMessage>();

    private string _connectionStatus;

    private IconChar _connectionStatusIcon;

    private Brush _connectionStatusIconColor;

    private Brush _connectionStatusColor;
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
    public ICommand ToggleServerStateCommand { get; }
    public StressMessageManager MessageManager => _messageManager;
    public ObservableCollection<PieSeries<int>> StressEffectMessagesSeriesCollection => _pieChartHelper.StressEffectMessagesSeriesCollection;

    public int MessagesSent => _messageManager.MessagesSent;

    public IconChar StatusBarConnectionIcon => _statusBarHelper.StatusBarConnectionIcon;

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
    public StressMessageViewModel(ProgressBar messageProgressBar)
    {
      _messageManager = new StressMessageManager(OnServerStateChanged, OnUpdateAdminPanelCharts, OnUpdateStatusBarContent);

      _pieChartHelper = new StressMessagePieChartHelper();

      _statusBarHelper = new StressMessageStatusBarHelper(messageProgressBar);

      _messageManager.PropertyChanged += (s, e) => OnMessagesSentPropertyChanged(s, e);

      _pieChartHelper.PropertyChanged += (s, e) => OnStressEffectMessageSeriesCollectionUpdated(s, e);

      _statusBarHelper.PropertyChanged += (s, e) => OnStatusBarPropertyUpdated(s, e);

      ConfigureConnectionStatusDefaults();

      ToggleServerStateCommand = new RelayCommand(_messageManager.ManageServerState);
    }
    private void OnMessagesSentPropertyChanged(object obj, PropertyChangedEventArgs property)
    {
      if (property.PropertyName == nameof(StressMessageManager.MessagesSent))
      {
        OnPropertyChanged(nameof(MessagesSent));
      }
    }
    private void OnStressEffectMessageSeriesCollectionUpdated(object obj, PropertyChangedEventArgs property)
    {
      if (property.PropertyName == nameof(StressMessagePieChartHelper.StressEffectMessagesSeriesCollection))
      {
        OnPropertyChanged(nameof(StressEffectMessagesSeriesCollection));
      }
    }
    private void OnStatusBarPropertyUpdated(object obj, PropertyChangedEventArgs property)
    {
      if (property.PropertyName == nameof(StressMessageStatusBarHelper.StatusBarConnectionIcon))
      {
        OnPropertyChanged(nameof(StatusBarConnectionIcon));
      }
    }
    private void ConfigureConnectionStatusDefaults()
    {
      ConnectionStatus = "Not Connected ! ! !";

      ConnectionStatusIcon = IconChar.UserSlash;

      ConnectionStatusColor = new SolidColorBrush(Colors.OrangeRed);

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
      _pieChartHelper.UpdatePieChartData(message.currentStressCategory);
    }
    private void OnUpdateStatusBarContent(IconChar icon, bool progressBarAnimationState)
    {
      _statusBarHelper.UpdateStatusBarData(icon, progressBarAnimationState);
    }
  }
}