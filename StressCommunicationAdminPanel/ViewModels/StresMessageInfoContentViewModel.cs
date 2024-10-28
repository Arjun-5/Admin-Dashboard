using LiveChartsCore.SkiaSharpView;
using LiveChartsCore;
using StressCommunicationAdminPanel.Models;
using System;
using System.Collections.ObjectModel;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using LiveChartsCore.Defaults;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.IO;
using Microsoft.Win32;
using StressCommunicationAdminPanel.Interfaces;
using LiveChartsCore.Measure;

namespace StressCommunicationAdminPanel.ViewModels
{
  public class StresMessageInfoContentViewModel : AppViewModel, IStressPanelChart<StressMessage,StressMessage>
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

    public SolidColorPaint LegendTextPaint => new SolidColorPaint 
    {
      Color = SKColor.Parse("#ff9e64"),
      SKTypeface = SKTypeface.FromFamilyName("Perpetua")
    };

    public SolidColorPaint LegendBackgroundPaint => new SolidColorPaint(SKColor.Parse("#ff9e64"));

    public SolidColorPaint TooltipTextPaint => new SolidColorPaint
    {
      Color = SKColor.Parse("#f7768e"),
      SKTypeface = SKTypeface.FromFamilyName("Perpetua",SKFontStyle.Bold)
    };
    public DrawMarginFrame MarginFrame => new DrawMarginFrame()
    {
      Fill = null,
      Stroke = new SolidColorPaint
      {
        Color = SKColor.Parse("#9ece6a"),
        StrokeThickness = 2
      }
    };

    public SolidColorPaint TooltipBackgroundPaint => new SolidColorPaint(SKColor.Parse("#565f89"));

    public ObservableCollection<StressMessage> chartSeriesCollection { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    public StresMessageInfoContentViewModel()
    {
      _mentalStressValues = new ObservableCollection<ObservablePoint>();

      _physicalStressValues = new ObservableCollection<ObservablePoint>();

      _emotionalStressValues = new ObservableCollection<ObservablePoint>();

      _mentalStressSeries = new LineSeries<ObservablePoint>
      {
        Name = "Mental Stress",
        Values = _mentalStressValues,
        Stroke = new SolidColorPaint(SKColor.Parse("#2ac3de")) { 
          StrokeThickness = 1.25f
        },
        DataLabelsPaint = new SolidColorPaint(SKColor.Parse("#cfc9c2"))
        {
          SKTypeface = SKTypeface.FromFamilyName("Perpetua", SKFontStyle.Bold)
        },
        Fill = null
      };

      _physicalStressSeries = new LineSeries<ObservablePoint>
      {
        Name = "Physical Stress",
        Stroke = new SolidColorPaint(SKColor.Parse("#9ece6a")) { 
          StrokeThickness = 1.5f
        },
        Fill = null,
        DataLabelsPaint = new SolidColorPaint(SKColor.Parse("#cfc9c2"))
        {
          SKTypeface = SKTypeface.FromFamilyName("Perpetua", SKFontStyle.Bold)
        },
        Values = _physicalStressValues
      };

      _emotionalStressSeries = new LineSeries<ObservablePoint>
      {
        Name = "Emotional Stress",
        Stroke = new SolidColorPaint(SKColor.Parse("#9ece6a")) {
          StrokeThickness = 1
        },
        Fill = null,
        DataLabelsPaint = new SolidColorPaint(SKColor.Parse("#cfc9c2"))
        {
          SKTypeface = SKTypeface.FromFamilyName("Perpetua", SKFontStyle.Bold)
        },
        Values = _emotionalStressValues
      };

      StressSeries = new ISeries[] { _mentalStressSeries, _physicalStressSeries, _emotionalStressSeries };

      XAxes = new Axis[]
      {
        new Axis
        {
          Name = "Timestamp",
          Labeler = value => new DateTime((long)value).ToString("HH:mm:ss"),
          NamePadding = new LiveChartsCore.Drawing.Padding(0, 15),
          NamePaint = new SolidColorPaint(SKColor.Parse("#73daca"))
          {
            StrokeThickness = 1,
            SKTypeface = SKTypeface.FromFamilyName("Perpetua", SKFontStyle.Bold)
          },
          LabelsPaint = new SolidColorPaint(SKColor.Parse("#73daca")) 
          {
            StrokeThickness = 1,
            SKTypeface = SKTypeface.FromFamilyName("Perpetua", SKFontStyle.Bold)
          },
        }
      };

      YAxes = new Axis[]
      {
        new Axis
        {
          Name = "Stress Level",
          NamePadding = new LiveChartsCore.Drawing.Padding(15, 0),
          NamePaint = new SolidColorPaint(SKColor.Parse("#e0af68"))
          {
            StrokeThickness = 2,
            SKTypeface = SKTypeface.FromFamilyName("Perpetua", SKFontStyle.Bold)
          },
          MinLimit = 0,
          LabelsPaint = new SolidColorPaint(SKColor.Parse("#e0af68"))
          {
            StrokeThickness = 1,
            SKTypeface = SKTypeface.FromFamilyName("Perpetua", SKFontStyle.Bold)
          },
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
    private void ExportData_Click(object sender, RoutedEventArgs e)
    {
      var data = (IEnumerable<StressMessage>)StressMessages;

      var saveFileDialog = new SaveFileDialog
      {
        Filter = "CSV files (*.csv)|*.csv|JSON files (*.json)|*.json",
        Title = "Export Data"
      };

      if (saveFileDialog.ShowDialog() == true)
      {
        var filePath = saveFileDialog.FileName;

        if (filePath.EndsWith(".csv"))
        {
          ExportToCsv(data, filePath);
        }
        else if (filePath.EndsWith(".json"))
        {
          ExportToJson(data, filePath);
        }
      }
    }

    private void ExportToCsv(IEnumerable<StressMessage> data, string filePath)
    {
      var csvData = new StringBuilder();
      csvData.AppendLine("Stress Type,Stress Value,Timestamp");

      foreach (var item in data)
      {
        csvData.AppendLine($"{item.currentStressCategory},{item.stressLevel},{item.timeSent}");
      }

      File.WriteAllText(filePath, csvData.ToString());
    }

    private void ExportToJson(IEnumerable<StressMessage> data, string filePath)
    {
      var jsonData = JsonConvert.SerializeObject(data, Formatting.Indented);
      File.WriteAllText(filePath, jsonData);
    }

    public void ConfigureDefaultChartAttributes()
    {
      throw new NotImplementedException();
    }

    public void InitializeChartSeries()
    {
      throw new NotImplementedException();
    }

    public void UpdateChartData(StressMessage category)
    {
      throw new NotImplementedException();
    }

    public RadialGradientPaint GetColorForCategory(StressMessage category)
    {
      throw new NotImplementedException();
    }

    public StressMessage ConfigureChartStyling(StressMessage category, int defaultValue, int dataLabelsSize, int strokeThickness, string strokeColor, PolarLabelsPosition labelsPosition)
    {
      throw new NotImplementedException();
    }
  }
}
