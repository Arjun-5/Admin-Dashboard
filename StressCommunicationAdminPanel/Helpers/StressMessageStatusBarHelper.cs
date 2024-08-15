using FontAwesome.Sharp;
using StressCommunicationAdminPanel.Services;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace StressCommunicationAdminPanel.Helpers
{
  public class StressMessageStatusBarHelper : PropertyChangeHandler
  {
    private IconChar _statusBarConnectionIcon;

    private Storyboard _statusBarStoryboard;
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
    public StressMessageStatusBarHelper(ProgressBar messageProgressBar)
    {
      StatusBarConnectionIcon = IconChar.PlugCircleExclamation;

      _messageProgressBar = messageProgressBar;

      var animation = new DoubleAnimation
      {
        From = 0,
        To = 100,
        Duration = new Duration(TimeSpan.FromSeconds(5)),
        RepeatBehavior = RepeatBehavior.Forever
      };

      if (_statusBarStoryboard == null)
      {
        _statusBarStoryboard = new Storyboard();

        Storyboard.SetTarget(animation, messageProgressBar);
        
        Storyboard.SetTargetProperty(animation, new PropertyPath("Value"));
        
        _statusBarStoryboard.Children.Add(animation);
      }

      
    }
    public void UpdateStatusBarData(IconChar statusBarIcon, bool shouldActivateAnimation)
    {
      StatusBarConnectionIcon = statusBarIcon;

      switch (shouldActivateAnimation)
      {
        case true:
          _statusBarStoryboard.Begin();
          break;
        case false:
          _statusBarStoryboard.Stop();
          _messageProgressBar.Value = 0;
          break;
      }
    }
  }
}