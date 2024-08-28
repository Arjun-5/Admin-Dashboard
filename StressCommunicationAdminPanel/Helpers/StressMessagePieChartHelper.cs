using System;
using System.Collections.ObjectModel;
using System.Linq;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using StressCommunicationAdminPanel.Models;
using StressCommunicationAdminPanel.Services;

namespace StressCommunicationAdminPanel.Helpers
{
  public class StressMessagePieChartHelper : PropertyChangeHandler
  {
    private ObservableCollection<PieSeries<int>> _stressEffectMessagesSeriesCollection;

    private ObservableCollection<PieSeries<int>> _statusMessagesSeriesCollection;

    //Debug setup - for demo recording should be updated later
    private bool _hasReplacedDefaultData;

    private bool _hasReplacedDefaultDataReceivedMessage;

    public ObservableCollection<PieSeries<int>> StressEffectMessagesSeriesCollection
    {
      get => _stressEffectMessagesSeriesCollection;

      set
      {
        _stressEffectMessagesSeriesCollection = value;

        OnPropertyChanged(nameof(StressEffectMessagesSeriesCollection));
      }
    }

    public ObservableCollection<PieSeries<int>> StatusMessagesSeriesCollection
    {
      get => _statusMessagesSeriesCollection;

      set
      {
        _statusMessagesSeriesCollection = value;

        OnPropertyChanged(nameof(StatusMessagesSeriesCollection));
      }
    }

    public StressMessagePieChartHelper()
    {
      ConfigureStressMessagePieChartAttributes();
    }

    private void ConfigureStressMessagePieChartAttributes()
    {
      StressEffectMessagesSeriesCollection = new ObservableCollection<PieSeries<int>>
      {
        new PieSeries<int>
        {
          Name = Enum.GetName(StressEffectCategory.None),
          Values = new ObservableCollection<int> { 1 },
          Stroke = new SolidColorPaint(SKColor.Parse("#414868")) { StrokeThickness = 3 },
          Fill = new SolidColorPaint(SKColor.Parse("#1a1b26")),
          DataLabelsPaint = new SolidColorPaint
          {
            FontFamily = "Perpetua",
            SKFontStyle = new SKFontStyle(SKFontStyleWeight.ExtraBold, SKFontStyleWidth.Normal, SKFontStyleSlant.Italic),
            Color = SKColors.White
          }
        }
      };

      StatusMessagesSeriesCollection = new ObservableCollection<PieSeries<int>>
      {
        new PieSeries<int>
        {
          Name = Enum.GetName(MessageTypeInfo.None),
          Values = new ObservableCollection<int> { 1 },
          Stroke = new SolidColorPaint(SKColor.Parse("#414868")) { StrokeThickness = 3 },
          Fill = new SolidColorPaint(SKColor.Parse("#1a1b26")),
          DataLabelsPaint = new SolidColorPaint
          {
            FontFamily = "Perpetua",
            SKFontStyle = new SKFontStyle(SKFontStyleWeight.ExtraBold, SKFontStyleWidth.Normal, SKFontStyleSlant.Italic),
            Color = SKColors.White
          }
        }
      };

      _hasReplacedDefaultData = false;

      _hasReplacedDefaultDataReceivedMessage = false;
    }
    public void UpdatePieChartData(StressEffectCategory effectCategory)
    {
      if (!_hasReplacedDefaultData)
      {
        StressEffectMessagesSeriesCollection.Clear();

        InitializePieChartSeries();

        _hasReplacedDefaultData = true;
      }
      string effectTypeName = Enum.GetName(typeof(StressEffectCategory), effectCategory);

      var series = StressEffectMessagesSeriesCollection.FirstOrDefault(s => s.Name == effectTypeName);

      if (series != null)
      {
        var valuesCollection = series.Values as ObservableCollection<int>;

        if (valuesCollection != null)
        {
          valuesCollection[0]++;
        }
        else
        {
          series.Values = new ObservableCollection<int> { series.Values.First() + 1 };
        }

        OnPropertyChanged(nameof(StressEffectMessagesSeriesCollection));
      }
    }

    public void UpdateReceivedPieChartData(MessageTypeInfo messageType)
    {
      if (!_hasReplacedDefaultDataReceivedMessage)
      {
        StatusMessagesSeriesCollection.Clear();

        InitializePieChartSeriesReceived();

        _hasReplacedDefaultDataReceivedMessage = true;
      }
      string messageTypeInfo = Enum.GetName(typeof(MessageTypeInfo), messageType);

      var series = StatusMessagesSeriesCollection.FirstOrDefault(s => s.Name == messageTypeInfo);

      if (series != null)
      {
        var valuesCollection = series.Values as ObservableCollection<int>;

        if (valuesCollection != null)
        {
          valuesCollection[0]++;
        }
        else
        {
          series.Values = new ObservableCollection<int> { series.Values.First() + 1 };
        }

        OnPropertyChanged(nameof(StatusMessagesSeriesCollection));
      }
    }

    private void InitializePieChartSeries()
    {
      foreach (StressEffectCategory type in Enum.GetValues(typeof(StressEffectCategory)))
      {
        StressEffectMessagesSeriesCollection.Add(new PieSeries<int>
        {
          Name = type.ToString(),
          Values = new ObservableCollection<int> { 0 },
          Fill = GetColorForStressType(type),
          DataLabelsSize = 22,
          Stroke = new SolidColorPaint(SKColor.Parse("#a9b1d6")) { StrokeThickness = 3 },
          DataLabelsPosition = LiveChartsCore.Measure.PolarLabelsPosition.Middle,
          DataLabelsPaint = new SolidColorPaint
          {
            FontFamily = "Perpetua",
            SKFontStyle = new SKFontStyle(SKFontStyleWeight.ExtraBold, SKFontStyleWidth.Normal, SKFontStyleSlant.Italic),
            Color = SKColors.White
          }
        });
      }
    }
    private void InitializePieChartSeriesReceived()
    {
      foreach (MessageTypeInfo type in Enum.GetValues(typeof(MessageTypeInfo)))
      {
        StatusMessagesSeriesCollection.Add(new PieSeries<int>
        {
          Name = type.ToString(),
          Values = new ObservableCollection<int> { 0 },
          Fill = GetColorForMessageType(type),
          DataLabelsSize = 22,
          Stroke = new SolidColorPaint(SKColor.Parse("#a9b1d6")) { StrokeThickness = 3 },
          DataLabelsPosition = LiveChartsCore.Measure.PolarLabelsPosition.Middle,
          DataLabelsPaint = new SolidColorPaint
          {
            FontFamily = "Perpetua",
            SKFontStyle = new SKFontStyle(SKFontStyleWeight.ExtraBold, SKFontStyleWidth.Normal, SKFontStyleSlant.Italic),
            Color = SKColors.White
          }
        });
      }
    }
    private SolidColorPaint GetColorForStressType(StressEffectCategory type)
    {
      switch (type)
      {
        case StressEffectCategory.Mental:
          {
            return new SolidColorPaint(SKColor.Parse("#f7768e"));
          }
        case StressEffectCategory.Emotional:
          {
            return new SolidColorPaint(SKColor.Parse("#9ece6a"));
          }
        case StressEffectCategory.Physical:
          {
            return new SolidColorPaint(SKColor.Parse("#2ac3de"));
          }
        default:
          {
            return new SolidColorPaint(SKColor.Parse("#1a1b26"));
          }
      }
    }
    private SolidColorPaint GetColorForMessageType(MessageTypeInfo type)
    {
      switch (type)
      {
        case MessageTypeInfo.DeviceInfo:
          {
            return new SolidColorPaint(SKColor.Parse("#ff9e64"));
          }
        case MessageTypeInfo.TaskInfo:
          {
            return new SolidColorPaint(SKColor.Parse("#9ece6a"));
          }
        case MessageTypeInfo.SelfReportStressInfo:
          {
            return new SolidColorPaint(SKColor.Parse("#2ac3de"));
          }
        case MessageTypeInfo.PhysicsInfo:
          {
            return new SolidColorPaint(SKColor.Parse("#bb9af7"));
          }
        default:
          {
            return new SolidColorPaint(SKColor.Parse("#1a1b26"));
          }
      }
    }
  }
}