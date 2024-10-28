using LiveChartsCore;
using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.SkiaSharpView;
using SkiaSharp;
using StressCommunicationAdminPanel.Models;
using StressCommunicationAdminPanel.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace StressCommunicationAdminPanel.ViewModels
{
  public class DeviceInfoContentViewModel : AppViewModel
  {
    private RightControllerHandler _rightControllerHandler;

    private LeftControllerHandler _leftControllerHandler;

    private HeadsetHandler _headsetHandler;

    public ObservableCollection<PhysicsInfoDataTable> RightControllerPhysicsData  => _rightControllerHandler.RightControllerPhysicsData;

    public ISeries[] RightControllerVelocitySeries => _rightControllerHandler.RightControllerVelocitySeries;

    public ISeries[] RightControllerAccelerationSeries => _rightControllerHandler.RightControllerAccelerationSeries;

    public ObservableCollection<PhysicsInfoDataTable> LeftControllerPhysicsData => _leftControllerHandler.LeftControllerPhysicsData;

    public ISeries[] LeftControllerVelocitySeries => _leftControllerHandler.LeftControllerVelocitySeries;

    public ISeries[] LeftControllerAccelerationSeries => _leftControllerHandler.LeftControllerAccelerationSeries;

    public ObservableCollection<PhysicsInfoDataTable> HeadsetPhysicsData => _headsetHandler.HeadsetPhysicsData;

    public ISeries[] HeadsetVelocitySeries => _headsetHandler.HeadsetVelocitySeries;

    public ISeries[] HeadsetAccelerationSeries => _headsetHandler.HeadsetAccelerationSeries;

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
      SKTypeface = SKTypeface.FromFamilyName("Perpetua", SKFontStyle.Bold)
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

    public DeviceInfoContentViewModel()
    {
      _rightControllerHandler = new RightControllerHandler();

      _leftControllerHandler = new LeftControllerHandler();

      _headsetHandler = new HeadsetHandler();

      XAxes = new Axis[]
      {
        new Axis
        {
          Labeler = value => new DateTime((long)value).ToString("HH:mm:ss"),
          NamePadding = new LiveChartsCore.Drawing.Padding(0, 2),
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
          NamePadding = new LiveChartsCore.Drawing.Padding(2, 0),
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
    public void UpdateControllerInformation(List<DevicePhysicsData> controllerPhysicsInformation)
    {
      foreach (var controllerInfo in controllerPhysicsInformation)
      {
        switch (controllerInfo.deviceType)
        {
          case DeviceType.OculusRightHandController:
            _rightControllerHandler.ProcessControllerInformation(controllerInfo);
            break;
          case DeviceType.OculusLeftHandController:
            _leftControllerHandler.ProcessControllerInformation(controllerInfo);
            break;
          case DeviceType.OculusHeadset:
            _headsetHandler.ProcessControllerInformation(controllerInfo);
            break;
        }
      }
    }
  }
}