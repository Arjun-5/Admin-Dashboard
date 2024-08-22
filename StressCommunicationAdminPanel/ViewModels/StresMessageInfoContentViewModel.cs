using LiveChartsCore.SkiaSharpView;
using LiveChartsCore;
using StressCommunicationAdminPanel.Models;
using System;
using System.Collections.ObjectModel;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using LiveChartsCore.Defaults;

namespace StressCommunicationAdminPanel.ViewModels
{
  public class StresMessageInfoContentViewModel : AppViewModel
  {
    private ObservableCollection<StressMessage> _messages = new ObservableCollection<StressMessage>();

    private ObservableCollection<ObservablePoint> _mentalStressValues;
    
    private ObservableCollection<ObservablePoint> _physicalStressValues;

    private ObservableCollection<ObservablePoint> _emotionalStressValues;
    
    private LineSeries<ObservablePoint> _mentalStressSeries;

    private LineSeries<ObservablePoint> _physicalStressSeries;

    private LineSeries<ObservablePoint> _emotionalStressSeries;

    public ObservableCollection<StressMessage> StressMessages
    {
      get => _messages;
      
      set
      {
        _messages = value;

        OnPropertyChanged(nameof(StressMessages));
      }
    }

    public ISeries[] StressSeries { get; set; }

    public Axis[] XAxes { get; set; }

    public Axis[] YAxes { get; set; }

    public StresMessageInfoContentViewModel()
    {
      _mentalStressValues = new ObservableCollection<ObservablePoint>();

      _physicalStressValues = new ObservableCollection<ObservablePoint>();

      _emotionalStressValues = new ObservableCollection<ObservablePoint>();

      _mentalStressSeries = new LineSeries<ObservablePoint>
      {
        Name = "Mental Stress",
        Values = _mentalStressValues,
        Stroke = new SolidColorPaint(SKColors.Blue) { StrokeThickness = 2 },
        Fill = null
      };

      _physicalStressSeries = new LineSeries<ObservablePoint>
      {
        Name = "Physical Stress",
        Stroke = new SolidColorPaint(SKColors.Green) { StrokeThickness = 2 },
        Fill = null,
        Values = _physicalStressValues
      };

      _emotionalStressSeries = new LineSeries<ObservablePoint>
      {
        Name = "Emotional Stress",
        Stroke = new SolidColorPaint(SKColors.Red) { StrokeThickness = 2 },
        Fill = null,
        Values = _emotionalStressValues
      };

      StressSeries = new ISeries[] { _mentalStressSeries, _physicalStressSeries, _emotionalStressSeries };

      XAxes = new Axis[]
      {
        new Axis
        {
            Name = "Timestamp",
            Labeler = value => new DateTime((long)value).ToString("HH:mm:ss"),
            NamePadding = new LiveChartsCore.Drawing.Padding(0, 15)
        }
      };

      YAxes = new Axis[]
      {
        new Axis
        {
            Name = "Stress Level",
            NamePadding = new LiveChartsCore.Drawing.Padding(15, 0),
            MinLimit = 0
        }
      };
    }
    public void UpdateStressMessageInfoDataTable(StressNotificationMessage notificationMessage)
    {
      StressMessages.Add(new StressMessage
      {
        stressLevel = notificationMessage.stressLevel,
        currentStressCategory = notificationMessage.currentStressCategory,
        timeSent = DateTime.Now
      });

      var graphPoint = new ObservablePoint(DateTime.Now.Ticks, notificationMessage.stressLevel);

      switch (notificationMessage.currentStressCategory)
      {
        case StressEffectCategory.Mental:
          _mentalStressValues.Add(graphPoint);
          break;
        case StressEffectCategory.Physical:
          _physicalStressValues.Add(graphPoint);
          break;
        case StressEffectCategory.Emotional:
          _emotionalStressValues.Add(graphPoint);
          break;
      }
    }
  }
}
