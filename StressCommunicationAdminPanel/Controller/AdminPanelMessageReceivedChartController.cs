using LiveChartsCore.Measure;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using StressCommunicationAdminPanel.Interfaces;
using StressCommunicationAdminPanel.Models;
using StressCommunicationAdminPanel.Services;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace StressCommunicationAdminPanel.Controller
{
  public class AdminPanelMessageReceivedChartController : PropertyChangeHandler, IStressPanelChart<PieSeries<int>, MessageTypeInfo>
  {
    private bool _hasReplacedDefaultData;

    private ObservableCollection<PieSeries<int>> _statusMessagesSeriesCollection;

    public ObservableCollection<PieSeries<int>> chartSeriesCollection { 
      
      get => _statusMessagesSeriesCollection; 
      
      set 
      {
        _statusMessagesSeriesCollection = value;

        OnPropertyChanged(nameof(chartSeriesCollection));
      } 
    }

    public AdminPanelMessageReceivedChartController() 
    {
      ConfigureDefaultChartAttributes();
    }
    public void ConfigureDefaultChartAttributes()
    {
      chartSeriesCollection = new ObservableCollection<PieSeries<int>>
      {
        ConfigureChartStyling(MessageTypeInfo.None, 1, 22, 3, "#414868",PolarLabelsPosition.Middle)
      };
    }

    public SolidColorPaint GetColorForCategory(MessageTypeInfo category)
    {
      switch (category)
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
        case MessageTypeInfo.Exit:
          {
            return new SolidColorPaint(SKColors.OrangeRed);
          }
        default:
          {
            return new SolidColorPaint(SKColor.Parse("#1a1b26"));
          }
      }
    }

    public void InitializeChartSeries()
    {
      foreach (MessageTypeInfo type in Enum.GetValues(typeof(MessageTypeInfo)))
      {
        chartSeriesCollection.Add(ConfigureChartStyling(type, 0, 22, 3, "#a9b1d6",PolarLabelsPosition.Middle));
      }
    }

    public void UpdateChartData(MessageTypeInfo category)
    {
      if (!_hasReplacedDefaultData)
      {
        chartSeriesCollection.Clear();

        InitializeChartSeries();

        _hasReplacedDefaultData = true;
      }

      string messageTypeInfo = Enum.GetName(typeof(MessageTypeInfo), category);

      var series = chartSeriesCollection.FirstOrDefault(s => s.Name == messageTypeInfo);

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

    public PieSeries<int> ConfigureChartStyling(MessageTypeInfo typeInfo, int defaultValue, int dataLabelsSize, int strokeThickness, string strokeColor, PolarLabelsPosition labelsPosition)
    {
      return new PieSeries<int>
      {
        Name = typeInfo.ToString(),
        Values = new ObservableCollection<int> {  defaultValue },
        Fill = GetColorForCategory(typeInfo),
        DataLabelsSize = dataLabelsSize,
        Stroke = new SolidColorPaint(SKColor.Parse(strokeColor)) { StrokeThickness = strokeThickness },
        DataLabelsPosition = labelsPosition,
        DataLabelsPaint = new SolidColorPaint
        {
          FontFamily = "Perpetua",
          SKFontStyle = new SKFontStyle(SKFontStyleWeight.ExtraBold, SKFontStyleWidth.Normal,SKFontStyleSlant.Italic),
          Color = SKColors.White
        }
      };
    }
  }
}