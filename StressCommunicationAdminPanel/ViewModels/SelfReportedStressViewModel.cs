using LiveChartsCore.Defaults;
using LiveChartsCore.Measure;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Drawing;
using LiveChartsCore.SkiaSharpView.Extensions;
using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.SkiaSharpView.VisualElements;
using LiveChartsCore.VisualElements;
using SkiaSharp;
using StressCommunicationAdminPanel.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Timers;

namespace StressCommunicationAdminPanel.ViewModels
{
  public class SelfReportedStressViewModel : AppViewModel
  {
    private bool _hasReplacedDefaultData;

    private int _sadStatusCount;

    private int _angryStatusCount;

    private int _happyStatusCount;

    private ObservableCollection<SelfReportMessageData> _selfReportMessages = new ObservableCollection<SelfReportMessageData>();

    public ObservableCollection<SelfReportMessageData> SelfReportMessages
    {
      get => _selfReportMessages;

      set
      {
        _selfReportMessages = value;

        OnPropertyChanged(nameof(SelfReportMessages));
      }
    }

    private ObservableCollection<PieSeries<int>> _selfReportStressStatusCollection;

    public ObservableCollection<PieSeries<int>> SelfReportStressStatusCollection
    {

      get => _selfReportStressStatusCollection;

      set
      {
        _selfReportStressStatusCollection = value;

        OnPropertyChanged(nameof(SelfReportStressStatusCollection));
      }
    }


    private ObservableCollection<PieSeries<ObservableValue>> _needleGaugeSeries;

    public ObservableCollection<PieSeries<ObservableValue>> NeedleGaugeSeries
    {

      get => _needleGaugeSeries;

      set
      {
        _needleGaugeSeries = value;

        OnPropertyChanged(nameof(NeedleGaugeSeries));
      }
    }

    public IEnumerable<VisualElement<SkiaSharpDrawingContext>> VisualElements { get; set; }

    public NeedleVisual Needle { get; set; }

    private List<SelfReportStressNeedleGauge> selfReportStressNeedleGauges = new List<SelfReportStressNeedleGauge>();

    public SelfReportedStressViewModel()
    {
      ConfigureDefaultChartAttributes();

      ConfigureNeedleGaugeChartAttributes();
    }
    public void ConfigureDefaultChartAttributes()
    {
      SelfReportStressStatusCollection = new ObservableCollection<PieSeries<int>>
      {
        ConfigureChartStyling(SelfReportStressStatus.None, 1, 22, 3, "#414868",PolarLabelsPosition.Middle)
      };
    }
    public PieSeries<int> ConfigureChartStyling(SelfReportStressStatus typeInfo, int defaultValue, int dataLabelsSize, int strokeThickness, string strokeColor, PolarLabelsPosition labelsPosition)
    {
      return new PieSeries<int>
      {
        Name = typeInfo.ToString(),
        Values = new ObservableCollection<int> { defaultValue },
        Fill = GetColorForCategory(typeInfo),
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
    public RadialGradientPaint GetColorForCategory(SelfReportStressStatus category)
    {
      switch (category)
      {
        case SelfReportStressStatus.Happy:
          {
            return new RadialGradientPaint(SKColor.Parse("#827bff"), SKColor.Parse("#e4bbff"));
          }
        case SelfReportStressStatus.Sad:
          {
            return new RadialGradientPaint(SKColor.Parse("#ff9e64"), SKColors.OrangeRed);
          }
        case SelfReportStressStatus.Angry:
          {
            return new RadialGradientPaint(SKColor.Parse("#2ac3de"), SKColors.BlueViolet);
          }
        default:
          {
            return new RadialGradientPaint(SKColor.Parse("#1a1b26"), SKColor.Parse("#050507"));
          }
      }
    }
    private void ConfigureNeedleGaugeChartAttributes()
    {
      Needle = new NeedleVisual
      {
        Value = 0,
        Fill = new SolidColorPaint(SKColor.Parse("#2ac3de"))
      };

      VisualElements = new VisualElement<SkiaSharpDrawingContext>[]
      {
        new AngularTicksVisual
        {
            LabelsSize = 16,
            LabelsPaint = new SolidColorPaint(SKColor.Parse("#e0af68")),
            LabelsOuterOffset = 5,
            Stroke = new SolidColorPaint(SKColor.Parse("#bb9af7")) { StrokeThickness = 3 },
            OuterOffset = 45,
            TicksLength = 20,
        },
        Needle
      };
      var sectionsOuter = 150;
      var sectionsWidth = 40;

      selfReportStressNeedleGauges.Add(
        new SelfReportStressNeedleGauge(SelfReportStressStatus.Happy, new GaugeItem( value: 100, s => SetStyle(sectionsOuter, sectionsWidth, SKColors.LimeGreen, s)))
      );

      selfReportStressNeedleGauges.Add(
        new SelfReportStressNeedleGauge(SelfReportStressStatus.Sad, new GaugeItem(value: 0, s => SetStyle(sectionsOuter, sectionsWidth, SKColors.Orange, s)))
      );

      selfReportStressNeedleGauges.Add(
        new SelfReportStressNeedleGauge(SelfReportStressStatus.Angry, new GaugeItem(value: 0, s => SetStyle(sectionsOuter, sectionsWidth, SKColors.Red, s)))
      );

      NeedleGaugeSeries = GaugeGenerator.BuildAngularGaugeSections(
        selfReportStressNeedleGauges.Where(data => data.SelfReportStressStatus == SelfReportStressStatus.Angry).Select(data => data.SelfReportStressNeedleGaugeItem).FirstOrDefault(),
        selfReportStressNeedleGauges.Where(data => data.SelfReportStressStatus == SelfReportStressStatus.Sad).Select(data => data.SelfReportStressNeedleGaugeItem).FirstOrDefault(),
        selfReportStressNeedleGauges.Where(data => data.SelfReportStressStatus == SelfReportStressStatus.Happy).Select(data => data.SelfReportStressNeedleGaugeItem).FirstOrDefault()
        );
    }
    private static void SetStyle(double sectionsOuter, double sectionsWidth, SKColor gaugeColor, PieSeries<ObservableValue> series)
    {
      series.OuterRadiusOffset = sectionsOuter;
      series.MaxRadialColumnWidth = sectionsWidth;
      series.Fill = new SolidColorPaint(gaugeColor);
      series.InnerRadius = 50;
    }
    public void UpdateStressReportStressChart(SelfReportMessageData selfReport)
    {
      UpdatePieChartData(selfReport.selfReportStressStatus);

      SelfReportMessages.Add(selfReport);

      switch (selfReport.selfReportStressStatus)
      {
        case SelfReportStressStatus.Sad:
          {
            _sadStatusCount++;

            Needle.Value = CalculatePartitionMidpoint(SelfReportStressStatus.Sad);

            break;
          }
        case SelfReportStressStatus.Angry:
          {
            _angryStatusCount++;

            Needle.Value = CalculatePartitionMidpoint(SelfReportStressStatus.Angry);

            break;
          }
        case SelfReportStressStatus.Happy:
          {
            _happyStatusCount++;

            Needle.Value = CalculatePartitionMidpoint(SelfReportStressStatus.Happy);

            break;
          }
      };

      selfReportStressNeedleGauges.Where(data => data.SelfReportStressStatus == SelfReportStressStatus.Sad)
        .Select(data => data.SelfReportStressNeedleGaugeItem)
        .FirstOrDefault().Value.Value = (_sadStatusCount / GetTotalStressCount()) * 100;

      selfReportStressNeedleGauges.Where(data => data.SelfReportStressStatus == SelfReportStressStatus.Angry)
        .Select(data => data.SelfReportStressNeedleGaugeItem)
        .FirstOrDefault().Value.Value = (_angryStatusCount / GetTotalStressCount()) * 100;

      selfReportStressNeedleGauges.Where(data => data.SelfReportStressStatus == SelfReportStressStatus.Happy)
        .Select(data => data.SelfReportStressNeedleGaugeItem)
        .FirstOrDefault().Value.Value = (_happyStatusCount / GetTotalStressCount()) * 100;
    }
    private float GetTotalStressCount()
    {
      return _sadStatusCount + _angryStatusCount + _happyStatusCount;
    }

    public double CalculatePartitionMidpoint(SelfReportStressStatus stressType)
    {
      int totalCount = _sadStatusCount + _angryStatusCount + _happyStatusCount;

      if (totalCount == 0)
      {
        return 0;
      }

      double sadEnd = (_sadStatusCount / (double)totalCount) * 100;
      
      double angryEnd = sadEnd + (_angryStatusCount / (double)totalCount) * 100;
      
      double happyEnd = angryEnd + (_happyStatusCount / (double)totalCount) * 100;

      switch (stressType)
      {
        case SelfReportStressStatus.Sad:
          return sadEnd / 2;

        case SelfReportStressStatus.Angry:
          return (sadEnd + angryEnd) / 2;

        case SelfReportStressStatus.Happy:
          return (angryEnd + happyEnd) / 2;

        default:
          return 0;
      }
    }
    public void UpdatePieChartData(SelfReportStressStatus stressType)
    {
      if (!_hasReplacedDefaultData)
      {
        SelfReportStressStatusCollection.Clear();

        InitializeChartSeries();

        _hasReplacedDefaultData = true;
      }

      string messageTypeInfo = Enum.GetName(typeof(SelfReportStressStatus), stressType);

      var series = SelfReportStressStatusCollection.FirstOrDefault(s => s.Name == messageTypeInfo);

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

        OnPropertyChanged(nameof(SelfReportStressStatusCollection));
      }
    }
    public void InitializeChartSeries()
    {
      foreach (SelfReportStressStatus type in Enum.GetValues(typeof(SelfReportStressStatus)))
      {
        SelfReportStressStatusCollection.Add(ConfigureChartStyling(type, 0, 22, 3, "#a9b1d6", PolarLabelsPosition.Middle));
      }
    }
  }
}
