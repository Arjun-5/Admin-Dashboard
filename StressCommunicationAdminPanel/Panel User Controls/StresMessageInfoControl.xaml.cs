using LiveChartsCore.SkiaSharpView;
using LiveChartsCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace StressCommunicationAdminPanel.Panel_User_Controls
{
  public class StressMessage
  {
    public string StressType { get; set; }
    public int StressValue { get; set; }
    public DateTime Timestamp { get; set; }
  }

  public class StressDataViewModel : INotifyPropertyChanged
  {
    public ObservableCollection<StressMessage> StressMessages { get; set; }
    public ObservableCollection<ISeries> StressChartSeries { get; set; }
    public ObservableCollection<string> XLabels { get; set; } = new ObservableCollection<string>();

    public StressDataViewModel()
    {
      StressMessages = new ObservableCollection<StressMessage>();
      StressChartSeries = new ObservableCollection<ISeries>
        {
            new LineSeries<int>
            {
                Values = new ObservableCollection<int>(),
                Name = "Stress Level"
            }
        };

      // Add some sample data
      AddSampleData();
    }

    private void AddSampleData()
    {
      var now = DateTime.Now;
      StressMessages.Add(new StressMessage { StressType = "Type A", StressValue = 5, Timestamp = now });
      StressMessages.Add(new StressMessage { StressType = "Type B", StressValue = 7, Timestamp = now.AddMinutes(-5) });

      foreach (var message in StressMessages)
      {
        /*(StressChartSeries[0] as LineSeries<int>).Values.Add(message.StressValue);
        XLabels.Add(message.Timestamp.ToString("HH:mm:ss"));*/
      }
    }

    public event PropertyChangedEventHandler PropertyChanged;
    protected virtual void OnPropertyChanged(string propertyName)
    {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

  }
}