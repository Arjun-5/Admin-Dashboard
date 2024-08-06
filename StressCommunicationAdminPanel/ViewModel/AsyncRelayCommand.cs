using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace StressCommunicationAdminPanel.ViewModel
{
  public class AsyncRelayCommand : ICommand
  {
    private readonly Func<Task> _execute;

    private readonly Predicate<object> _canExecute;

    public AsyncRelayCommand(Func<Task> execute, Predicate<object> canExecute = null)
    {
      _execute = execute ?? throw new ArgumentNullException(nameof(execute));
    
      _canExecute = canExecute;
    }

    public bool CanExecute(object parameter) => _canExecute == null || _canExecute(parameter);

    public async void Execute(object parameter)
    {
      await _execute();
    }

    public event EventHandler CanExecuteChanged
    {
      add { CommandManager.RequerySuggested += value; }
      
      remove { CommandManager.RequerySuggested -= value; }
    }
  }
}