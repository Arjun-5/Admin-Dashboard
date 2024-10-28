using System;
using System.Windows.Input;

namespace StressCommunicationAdminPanel.Commands
{
  public class RelayCommand : ICommand
  {
    private readonly Action _execute;

    private readonly Action<object> _executeWithParameter;

    private readonly Func<bool> _canExecute;

    public RelayCommand(Action execute, Func<bool> canExecute = null)
    {
      _execute = execute;
    
      _canExecute = canExecute;
    }

    public RelayCommand(Action<object> executeWithParameter, Func<bool> canExecute = null)
    {
      _executeWithParameter = executeWithParameter ?? throw new ArgumentNullException(nameof(executeWithParameter));
     
      _canExecute = canExecute;
    }

    public event EventHandler CanExecuteChanged
    {
      add { CommandManager.RequerySuggested += value; }
      
      remove { CommandManager.RequerySuggested -= value; }
    }

    public bool CanExecute(object parameter)
    {
      return _canExecute == null || _canExecute();
    }

    public void Execute(object parameter)
    {
      if (_execute != null)
      {
        _execute();
      }
      else if (_executeWithParameter != null)
      {
        _executeWithParameter(parameter);
      }
    }
  }
}