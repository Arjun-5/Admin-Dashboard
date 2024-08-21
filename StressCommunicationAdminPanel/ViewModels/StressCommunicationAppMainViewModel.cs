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
    public ICommand ShowConfigurationPanelCommand { get; }
    public StressMessageViewModel StressMessageViewModel { get; }

    public StressCommunicationAppMainViewModel(ProgressBar messageProgressBar)
    {
      StressMessageViewModel = new StressMessageViewModel(messageProgressBar);

      ShowAdminPanelCommand = new RelayCommand(ShowAdminPanel);
      
      ShowConfigurationPanelCommand = new RelayCommand(ShowConfigurationPanel);

      CurrentView = new AdminPanelContent { DataContext = StressMessageViewModel };
    }

    private void ShowAdminPanel()
    {
      CurrentView = new AdminPanelContent { DataContext = StressMessageViewModel };
    }

    private void ShowConfigurationPanel()
    {
      CurrentView = new ConfigurationPanelContent { DataContext = StressMessageViewModel };
    }
  }
}
