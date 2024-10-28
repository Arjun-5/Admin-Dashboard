using StressCommunicationAdminPanel.Commands;
using StressCommunicationAdminPanel.Panel_User_Controls;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace StressCommunicationAdminPanel.ViewModels
{
  public class StressCommunicationAppMainViewModel : AppViewModel
  {
    private Button _previousSelectedMenu;

    private object _currentView;
    public object CurrentView
    {
      get => _currentView;

      set
      {
        _currentView = value;

        OnPropertyChanged(nameof(CurrentView));
      }
    }
    public ICommand ShowAdminPanelCommand { get; }

    public ICommand ShowConfigurationControlPanelCommand { get; }

    public ICommand ShowStresMessageInfoContentPanelCommand { get; }

    public ICommand ShowDeviceInfoContentCommand { get; }

    public ICommand ShowSelfReportedStressContentCommand { get; }

    public StressMessageViewModel stressMessageViewModel { get; }

    public StresMessageInfoContentViewModel stresMessageInfoContentViewModel { get; }

    public DeviceInfoContentViewModel deviceInfoContentViewModel { get; }

    public IPAddressConfigurationViewModel ipAddressConfigurationViewModel { get; }

    public SelfReportedStressViewModel selfReportedStressViewModel { get; }

    public StressCommunicationAppMainViewModel(ProgressBar messageProgressBar, Button homePanelButton)
    {
      stresMessageInfoContentViewModel = new StresMessageInfoContentViewModel();

      deviceInfoContentViewModel = new DeviceInfoContentViewModel();

      selfReportedStressViewModel = new SelfReportedStressViewModel();

      stressMessageViewModel = new StressMessageViewModel(messageProgressBar, stresMessageInfoContentViewModel.UpdateStressMessageInfoDataTable, 
        deviceInfoContentViewModel.UpdateControllerInformation, selfReportedStressViewModel.UpdateStressReportStressChart);

      ipAddressConfigurationViewModel = new IPAddressConfigurationViewModel();

      ShowAdminPanelCommand = new RelayCommand(ShowAdminPanel);

      ShowConfigurationControlPanelCommand = new RelayCommand(ShowConfigurationControlPanel);

      ShowStresMessageInfoContentPanelCommand = new RelayCommand(ShowStresMessageInfoContentPanel);

      ShowDeviceInfoContentCommand = new RelayCommand(ShowDeviceInfoPanel);

      ShowSelfReportedStressContentCommand = new RelayCommand(ShowSelfReportedStressContentInfoPanel);

      ShowAdminPanelCommand.Execute(homePanelButton);
    }

    private void ConfigureMenuStyling(object parameter)
    {
      var clickedButton = parameter as Button;

      if (clickedButton != null)
      {
        if (_previousSelectedMenu != null)
        {
          _previousSelectedMenu.Style = Application.Current.Resources["menuButton"] as Style;
        }

        clickedButton.Style = Application.Current.Resources["menuButtonActive"] as Style;

        _previousSelectedMenu = clickedButton;
      }
    }
    private void ShowAdminPanel(object parameter)
    {
      ConfigureMenuStyling(parameter);

      CurrentView = new AdminPanelContent { DataContext = stressMessageViewModel };
    }

    private void ShowConfigurationControlPanel(object parameter)
    {
      ConfigureMenuStyling(parameter);

      CurrentView = new IPAddressConfigurationPanelContent
      (
        () =>
        {
          ipAddressConfigurationViewModel.ReadAndSetConfigDefaultValue();
        }
      )
      { 
        DataContext = ipAddressConfigurationViewModel
      };
    }
    private void ShowStresMessageInfoContentPanel(object parameter)
    {
      ConfigureMenuStyling(parameter);

      CurrentView = new StresMessageInfoContent { DataContext = stresMessageInfoContentViewModel };
    }
    private void ShowDeviceInfoPanel(object parameter)
    {
      ConfigureMenuStyling(parameter);

      CurrentView = new DeviceInfoContent { DataContext = deviceInfoContentViewModel };
    }
    private void ShowSelfReportedStressContentInfoPanel(object parameter)
    {
      ConfigureMenuStyling(parameter);

      CurrentView = new SelfReportedStressInfoContent { DataContext = selfReportedStressViewModel };
    }
  }
}