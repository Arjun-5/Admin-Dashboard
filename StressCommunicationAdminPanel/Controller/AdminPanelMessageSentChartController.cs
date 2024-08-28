using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.SkiaSharpView;
using SkiaSharp;
using StressCommunicationAdminPanel.Models;
using StressCommunicationAdminPanel.Services;
using System;
using System.Collections.ObjectModel;

namespace StressCommunicationAdminPanel.Controller
{
  public class AdminPanelMessageSentChartController : PropertyChangeHandler
  {
    private ObservableCollection<PieSeries<int>> _stressEffectMessagesSeriesCollection;

    public ObservableCollection<PieSeries<int>> StressEffectMessagesSeriesCollection
    {
      get => _stressEffectMessagesSeriesCollection;

      set
      {
        _stressEffectMessagesSeriesCollection = value;

        OnPropertyChanged(nameof(StressEffectMessagesSeriesCollection));
      }
    }

    public AdminPanelMessageSentChartController() 
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
    }
    private void InitializeMessageSentPieChartSeries()
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
  }
}
