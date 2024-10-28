using FontAwesome.Sharp;
using System.Collections.ObjectModel;
using System.Windows.Media;
using StressCommunicationAdminPanel.Models;
using StressCommunicationAdminPanel.Services;
using StressCommunicationAdminPanel.Helpers;
using LiveChartsCore.SkiaSharpView;
using System;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Input;
using StressCommunicationAdminPanel.Commands;
using System.Windows;
using System.Windows.Threading;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace StressCommunicationAdminPanel.ViewModels
{
  public class StressMessageViewModel : AppViewModel
  {
    private readonly StressMessageManager _messageManager;

    private readonly StressMessagePieChartHelper _pieChartHelper;

    private readonly StressMessageStatusBarHelper _statusBarHelper;

    private string _connectionStatus;

    private IconChar _connectionStatusIcon;

    private Brush _connectionStatusIconColor;

    private Brush _connectionStatusColor;

    private Action<StressNotificationMessage> updateStressMessageDataTable;

    private Action<List<DevicePhysicsData>> updateDevicePhysicsInfoView;

    private Action<SelfReportMessageData> updateSelfReportMessageView;
    
    public ICommand ToggleServerStateCommand { get; }
    
    public StressMessageManager MessageManager => _messageManager;
    
    public ObservableCollection<PieSeries<int>> StressEffectMessagesSeriesCollection => _pieChartHelper.adminPanelMessageSentChartController.chartSeriesCollection;

    public ObservableCollection<PieSeries<int>> StatusMessagesSeriesCollection => _pieChartHelper.adminPanelMessageReceivedChartController.chartSeriesCollection;

    public StresMessageInfoContentViewModel stresMessageInfoContentViewModel { get; }

    public int MessagesSent => _messageManager.MessagesSent;

    public int MessagesReceived => _messageManager.MessagesReceived;

    public string DeviceName => _messageManager.DeviceName;

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
    public StressMessageViewModel(ProgressBar messageProgressBar, Action<StressNotificationMessage> onStressMessageSent, Action<List<DevicePhysicsData>> updateDevicePhysicsInfoView, Action<SelfReportMessageData> updateSelfReportMessageView)
    {
      _messageManager = new StressMessageManager(OnServerStateChanged, OnUpdateAdminPanelCharts, OnUpdateStatusBarContent, OnUpdateReceivedDataChart);

      _pieChartHelper = new StressMessagePieChartHelper();

      _statusBarHelper = new StressMessageStatusBarHelper(messageProgressBar);

      stresMessageInfoContentViewModel = new StresMessageInfoContentViewModel();

      updateStressMessageDataTable = onStressMessageSent;

      this.updateDevicePhysicsInfoView = updateDevicePhysicsInfoView;

      this.updateSelfReportMessageView = updateSelfReportMessageView;

      _messageManager.PropertyChanged += (s, e) => OnMessagesSentPropertyChanged(s, e);

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

      if (property.PropertyName == nameof(StressMessageManager.MessagesReceived))
      {
        OnPropertyChanged(nameof(MessagesReceived));
      }

      if (property.PropertyName == nameof(StressMessageManager.DeviceName))
      {
        OnPropertyChanged(nameof(DeviceName));
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
    private void OnUpdateAdminPanelCharts(StressNotificationMessage message)
    {
      Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => updateStressMessageDataTable?.Invoke(message)));

      _pieChartHelper.adminPanelMessageSentChartController.UpdateChartData(message.currentStressCategory);
    }
    private void OnUpdateReceivedDataChart(ReceivedMessageInfo message)
    {
      _pieChartHelper.adminPanelMessageReceivedChartController.UpdateChartData(message.MessageTypeInfo);

      switch(message.MessageTypeInfo)
      {
        case MessageTypeInfo.PhysicsInfo:
          Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => {
            updateDevicePhysicsInfoView?.Invoke(JsonConvert.DeserializeObject<List<DevicePhysicsData>>(message.messageContent));
            }));
          break;
        case MessageTypeInfo.SelfReportStressInfo:
          Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => {
            updateSelfReportMessageView?.Invoke(JsonConvert.DeserializeObject<SelfReportMessageData>(message.messageContent));
          }));
          break;
      }
    }
    private void OnUpdateStatusBarContent(IconChar icon, bool progressBarAnimationState)
    {
      _statusBarHelper.UpdateStatusBarData(icon, progressBarAnimationState);
    }
  }
}