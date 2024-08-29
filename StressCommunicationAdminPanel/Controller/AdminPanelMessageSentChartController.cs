using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.SkiaSharpView;
using SkiaSharp;
using StressCommunicationAdminPanel.Models;
using StressCommunicationAdminPanel.Services;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using StressCommunicationAdminPanel.Interfaces;

namespace StressCommunicationAdminPanel.Controller
{
  public class AdminPanelMessageSentChartController : PropertyChangeHandler, IStressPanelChart<PieSeries<int>, StressEffectCategory>
  {
    private ObservableCollection<PieSeries<int>> _stressEffectMessagesSeriesCollection;

    private bool _hasReplacedDefaultData;

    public ObservableCollection<PieSeries<int>> ChartSeriesCollection
    {
      get => _stressEffectMessagesSeriesCollection;
      set
      {
        _stressEffectMessagesSeriesCollection = value;
        OnPropertyChanged(nameof(ChartSeriesCollection));
      }
    }

    public AdminPanelMessageSentChartController()
    {
      ConfigureChartAttributes();
    }

    public void ConfigureChartAttributes()
    {
      ChartSeriesCollection = new ObservableCollection<PieSeries<int>>
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

      _hasReplacedDefaultData = false;
    }

    public void UpdateChartData(StressEffectCategory effectCategory)
    {
      if (!_hasReplacedDefaultData)
      {
        ChartSeriesCollection.Clear();
        InitializeChartSeries();
        _hasReplacedDefaultData = true;
      }

      string effectTypeName = Enum.GetName(typeof(StressEffectCategory), effectCategory);
      var series = ChartSeriesCollection.FirstOrDefault(s => s.Name == effectTypeName);

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

        OnPropertyChanged(nameof(ChartSeriesCollection));
      }
    }

    public void InitializeChartSeries()
    {
      foreach (StressEffectCategory type in Enum.GetValues(typeof(StressEffectCategory)))
      {
        ChartSeriesCollection.Add(new PieSeries<int>
        {
          Name = type.ToString(),
          Values = new ObservableCollection<int> { 0 },
          Fill = GetColorForCategory(type),
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

    public SolidColorPaint GetColorForCategory(StressEffectCategory type)
    {
      switch (type)
      {
        case StressEffectCategory.Mental:
          return new SolidColorPaint(SKColor.Parse("#f7768e"));
        case StressEffectCategory.Emotional:
          return new SolidColorPaint(SKColor.Parse("#9ece6a"));
        case StressEffectCategory.Physical:
          return new SolidColorPaint(SKColor.Parse("#2ac3de"));
        default:
          return new SolidColorPaint(SKColor.Parse("#1a1b26"));
      }
    }
  } 
}