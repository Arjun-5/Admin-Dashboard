using System;
using System.Timers;

namespace StressCommunicationAdminPanel.Services
{
  public class StressMessageTimer
  {
    private Timer _timer;

    private Action _onTimerElapsed;

    public StressMessageTimer(double interval, Action onTimerElapsed)
    {
      _timer = new Timer(interval);
      
      _timer.Elapsed += OnTimerElapsed;
      
      _onTimerElapsed = onTimerElapsed;
    }

    private void OnTimerElapsed(object sender, ElapsedEventArgs e)
    {
      _onTimerElapsed?.Invoke();
    }

    public void Start()
    {
      _timer.Start();
    }

    public void Stop()
    {
      _timer.Stop();
    }
  }
}