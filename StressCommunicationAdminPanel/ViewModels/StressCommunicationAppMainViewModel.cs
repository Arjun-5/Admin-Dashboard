using StressCommunicationAdminPanel.Commands;
using StressCommunicationAdminPanel.Panel_User_Controls;
using System.Windows.Controls;
using System.Windows.Input;

namespace StressCommunicationAdminPanel.ViewModels
{
  public class StressCommunicationAppMainViewModel : AppViewModel
  {
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
    
    public StressMessageViewModel stressMessageViewModel { get; }

    public StresMessageInfoContentViewModel stresMessageInfoContentViewModel { get; }

    public IPAddressConfigurationViewModel ipAddressConfigurationViewModel { get; }

    public StressCommunicationAppMainViewModel(ProgressBar messageProgressBar)
    {
      stresMessageInfoContentViewModel = new StresMessageInfoContentViewModel();

      stressMessageViewModel = new StressMessageViewModel(messageProgressBar, stresMessageInfoContentViewModel.UpdateStressMessageInfoDataTable);

      ipAddressConfigurationViewModel = new IPAddressConfigurationViewModel();

      ShowAdminPanelCommand = new RelayCommand(ShowAdminPanel);

      ShowConfigurationControlPanelCommand = new RelayCommand(ShowConfigurationControlPanel);

      ShowStresMessageInfoContentPanelCommand = new RelayCommand(ShowStresMessageInfoContentPanel);

      CurrentView = new AdminPanelContent { DataContext = stressMessageViewModel };
    }

    private void ShowAdminPanel()
    {
      CurrentView = new AdminPanelContent { DataContext = stressMessageViewModel };
    }

    private void ShowConfigurationControlPanel()
    {
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
    private void ShowStresMessageInfoContentPanel()
    {
      CurrentView = new StresMessageInfoContent { DataContext = stresMessageInfoContentViewModel };
    }
  }
}