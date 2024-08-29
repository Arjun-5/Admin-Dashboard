using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.SkiaSharpView;
using SkiaSharp;
using StressCommunicationAdminPanel.Models;
using StressCommunicationAdminPanel.Services;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using StressCommunicationAdminPanel.Interfaces;
using LiveChartsCore.Measure;

namespace StressCommunicationAdminPanel.Controller
{
  public class AdminPanelMessageSentChartController : PropertyChangeHandler, IStressPanelChart<PieSeries<int>, StressEffectCategory>
  {
    private ObservableCollection<PieSeries<int>> _stressEffectMessagesSeriesCollection;

    private bool _hasReplacedDefaultData;

    public ObservableCollection<PieSeries<int>> chartSeriesCollection
    {
      get => _stressEffectMessagesSeriesCollection;

      set
      {
        _stressEffectMessagesSeriesCollection = value;

        OnPropertyChanged(nameof(chartSeriesCollection));
      }
    }

    public AdminPanelMessageSentChartController()
    {
      ConfigureDefaultChartAttributes();

      _hasReplacedDefaultData = false;
    }

    public void ConfigureDefaultChartAttributes()
    {
      chartSeriesCollection = new ObservableCollection<PieSeries<int>>
      {
        ConfigureChartStyling(StressEffectCategory.None, 1, 22, 3, "#414868", PolarLabelsPosition.Middle)
      };
    }

    public void UpdateChartData(StressEffectCategory effectCategory)
    {
      if (!_hasReplacedDefaultData)
      {
        chartSeriesCollection.Clear();
        
        InitializeChartSeries();
        
        _hasReplacedDefaultData = true;
      }

      string effectTypeName = Enum.GetName(typeof(StressEffectCategory), effectCategory);

      var series = chartSeriesCollection.FirstOrDefault(s => s.Name == effectTypeName);

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

        OnPropertyChanged(nameof(chartSeriesCollection));
      }
    }

    public void InitializeChartSeries()
    {
      foreach (StressEffectCategory type in Enum.GetValues(typeof(StressEffectCategory)))
      {
        chartSeriesCollection.Add(ConfigureChartStyling(type, 0, 22, 3, "#a9b1d6", PolarLabelsPosition.Middle));
      }
    }

    public RadialGradientPaint GetColorForCategory(StressEffectCategory type)
    {
      switch (type)
      {
        case StressEffectCategory.Mental:
          {
            return new RadialGradientPaint(SKColor.Parse("#f7768e"), SKColor.Parse("#e4bbff"));
          }
        case StressEffectCategory.Physical:
          {
            return new RadialGradientPaint(SKColor.Parse("#9ece6a"), SKColors.OrangeRed);
          }
        case StressEffectCategory.Emotional:
          {
            return new RadialGradientPaint(SKColor.Parse("#2ac3de"), SKColors.BlueViolet);
          }
        default:
          {
            return new RadialGradientPaint(SKColor.Parse("#1a1b26"), SKColor.Parse("#050507"));
          }
      }
    }
    public PieSeries<int> ConfigureChartStyling(StressEffectCategory category, int defaultValue, int dataLabelsSize, int strokeThickness, string strokeColor, PolarLabelsPosition labelsPosition)
    {
      return new PieSeries<int>
      {
        Name = category.ToString(),
        Values = new ObservableCollection<int> { defaultValue },
        Fill = GetColorForCategory(category),
        DataLabelsSize = dataLabelsSize,
        Stroke = new SolidColorPaint(SKColor.Parse(strokeColor)) { StrokeThickness = strokeThickness },
        DataLabelsPosition = labelsPosition,
        DataLabelsPaint = new SolidColorPaint
        {
          FontFamily = "Perpetua",
          SKFontStyle = new SKFontStyle(SKFontStyleWeight.ExtraBold, SKFontStyleWidth.Normal, SKFontStyleSlant.Italic),
          Color = SKColors.White
        }
      };
    }
  } 
}