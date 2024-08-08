using FontAwesome.Sharp;
using StressCommunicationAdminPanel.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace StressCommunicationAdminPanel.Helpers
{
  public class StressMessageStatusBarHelper : PropertyChangeHandler
  {
    private IconChar _statusBarConnectionIcon;
    public IconChar StatusBarConnectionIcon
    {
      get => _statusBarConnectionIcon;

      set
      {
        if (_statusBarConnectionIcon != value)
        {
          _statusBarConnectionIcon = value;

          OnPropertyChanged(nameof(StatusBarConnectionIcon));
        }
      }
    }
    private ProgressBar _messageProgressBar;

    private DoubleAnimation _progressBarAnimation;
    public StressMessageStatusBarHelper(ProgressBar messageProgressBar)
    {
      StatusBarConnectionIcon = IconChar.PlugCircleExclamation;

      _messageProgressBar = messageProgressBar;

      Duration duration = new Duration(TimeSpan.FromSeconds(10));

      _progressBarAnimation = new DoubleAnimation(0.0, 100.0, duration);
    }
    public void UpdateStatusBarData(IconChar statusBarIcon, bool shouldActivateAnimation)
    {
      StatusBarConnectionIcon = statusBarIcon;

      switch (shouldActivateAnimation)
      {
        //From = "0" To = "100" Duration = "0:0:30"
        case true:
          _messageProgressBar.BeginAnimation(ProgressBar.ValueProperty, _progressBarAnimation);
          break;
        case false:
          _messageProgressBar.Value = 0.0;
          break;
      }
    }
  }
}