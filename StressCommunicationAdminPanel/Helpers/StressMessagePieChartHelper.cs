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

    public ObservableCollection<PieSeries<int>> StressEffectMessagesSeriesCollection
    {
      get { return _stressEffectMessagesSeriesCollection; }

      set { _stressEffectMessagesSeriesCollection = value; OnPropertyChanged(nameof(StressEffectMessagesSeriesCollection)); }
    }

    private bool _hasReplacedDefaultData;
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
          Name = "No Valid data",
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
    }
    public void UpdatePieChartData(StressEffectType effectType)
    {
      if (_hasReplacedDefaultData)
      {
        InitializePieChartSeries();

        _hasReplacedDefaultData = true;
      }
      string effectTypeName = Enum.GetName(typeof(StressEffectType), effectType);

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

    private void InitializePieChartSeries()
    {
      foreach (StressEffectType type in Enum.GetValues(typeof(StressEffectType)))
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
    private SolidColorPaint GetColorForStressType(StressEffectType type)
    {
      switch (type)
      {
        case StressEffectType.Mental:
          return new SolidColorPaint(SKColor.Parse("#f7768e"));
        case StressEffectType.Emotional:
          return new SolidColorPaint(SKColor.Parse("#9ece6a"));
        case StressEffectType.Physical:
          return new SolidColorPaint(SKColor.Parse("#2ac3de"));
        default:
          return new SolidColorPaint(SKColor.Parse("#1a1b26"));
      }
    }

  }
}