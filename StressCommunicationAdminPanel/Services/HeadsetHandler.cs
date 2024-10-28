using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.SkiaSharpView;
using SkiaSharp;
using System.Collections.ObjectModel;
using StressCommunicationAdminPanel.Models;
using LiveChartsCore;
using System;
using StressCommunicationAdminPanel.ViewModels;

namespace StressCommunicationAdminPanel.Services
{
  public class HeadsetHandler : AppViewModel
  {
    private ObservableCollection<PhysicsInfoDataTable> _headsetPhysicsData = new ObservableCollection<PhysicsInfoDataTable>();

    public ObservableCollection<PhysicsInfoDataTable> HeadsetPhysicsData
    {
      get => _headsetPhysicsData;

      set
      {
        _headsetPhysicsData = value;

        OnPropertyChanged(nameof(HeadsetPhysicsData));
      }
    }

    public ObservableCollection<ObservablePoint> VelocityX { get; private set; }
    
    public ObservableCollection<ObservablePoint> VelocityY { get; private set; }
    
    public ObservableCollection<ObservablePoint> VelocityZ { get; private set; }

    public ObservableCollection<ObservablePoint> AccelerationX { get; private set; }
    
    public ObservableCollection<ObservablePoint> AccelerationY { get; private set; }

    public ObservableCollection<ObservablePoint> AccelerationZ { get; private set; }

    public ISeries[] HeadsetVelocitySeries { get; private set; }

    public ISeries[] HeadsetAccelerationSeries { get; private set; }
    public HeadsetHandler()
    {
      VelocityX = new ObservableCollection<ObservablePoint>();
      
      VelocityY = new ObservableCollection<ObservablePoint>();
      
      VelocityZ = new ObservableCollection<ObservablePoint>();

      AccelerationX = new ObservableCollection<ObservablePoint>();
      
      AccelerationY = new ObservableCollection<ObservablePoint>();

      AccelerationZ = new ObservableCollection<ObservablePoint>();

      HeadsetVelocitySeries = InitializeVelocitySeries();

      HeadsetAccelerationSeries = InitializeAccelerationSeries();
    }
    public LineSeries<ObservablePoint>[] InitializeVelocitySeries()
    {
      return new LineSeries<ObservablePoint>[]
      {
        new LineSeries<ObservablePoint>
        {
          Name = "Velocity X",
          Values = VelocityX,
          Stroke = new SolidColorPaint(SKColor.Parse("#2ac3de")) { StrokeThickness = 1.25f },
          DataLabelsPaint = new SolidColorPaint(SKColor.Parse("#cfc9c2"))
          {
              SKTypeface = SKTypeface.FromFamilyName("Perpetua", SKFontStyle.Bold)
          },
          Fill = null
        },
        new LineSeries<ObservablePoint>
        {
          Name = "Velocity Y",
          Values = VelocityY,
          Stroke = new SolidColorPaint(SKColor.Parse("#9ece6a")) { StrokeThickness = 1.5f },
          DataLabelsPaint = new SolidColorPaint(SKColor.Parse("#cfc9c2"))
          {
              SKTypeface = SKTypeface.FromFamilyName("Perpetua", SKFontStyle.Bold)
          },
          Fill = null
        },
        new LineSeries<ObservablePoint>
        {
          Name = "Velocity Z",
          Values = VelocityZ,
          Stroke = new SolidColorPaint(SKColor.Parse("#9ece6a")) { StrokeThickness = 1f },
          DataLabelsPaint = new SolidColorPaint(SKColor.Parse("#cfc9c2"))
          {
              SKTypeface = SKTypeface.FromFamilyName("Perpetua", SKFontStyle.Bold)
          },
          Fill = null
        }
      };
    }

    public LineSeries<ObservablePoint>[] InitializeAccelerationSeries()
    {
      return new LineSeries<ObservablePoint>[]
      {
        new LineSeries<ObservablePoint>
        {
          Name = "Acceleration X",
          Values = AccelerationX,
          Stroke = new SolidColorPaint(SKColor.Parse("#2ac3de")) { StrokeThickness = 1.25f },
          DataLabelsPaint = new SolidColorPaint(SKColor.Parse("#cfc9c2"))
          {
              SKTypeface = SKTypeface.FromFamilyName("Perpetua", SKFontStyle.Bold)
          },
          Fill = null
        },
        new LineSeries<ObservablePoint>
        {
          Name = "Acceleration Y",
          Values = AccelerationY,
          Stroke = new SolidColorPaint(SKColor.Parse("#9ece6a")) { StrokeThickness = 1.5f },
          DataLabelsPaint = new SolidColorPaint(SKColor.Parse("#cfc9c2"))
          {
              SKTypeface = SKTypeface.FromFamilyName("Perpetua", SKFontStyle.Bold)
          },
          Fill = null
        },
        new LineSeries<ObservablePoint>
        {
          Name = "Acceleration Z",
          Values = AccelerationZ,
          Stroke = new SolidColorPaint(SKColor.Parse("#9ece6a")) { StrokeThickness = 1f },
          DataLabelsPaint = new SolidColorPaint(SKColor.Parse("#cfc9c2"))
          {
              SKTypeface = SKTypeface.FromFamilyName("Perpetua", SKFontStyle.Bold)
          },
          Fill = null
        }
      };
    }
    public void ProcessControllerInformation(DevicePhysicsData data)
    {
      HeadsetPhysicsData.Add(new PhysicsInfoDataTable 
      {
        deviceType = data.deviceType,
        deviceVelocity = $"( X = {data.deviceVelocity.X}, Y = {data.deviceVelocity.Y}, Z = {data.deviceVelocity.Z} )",
        deviceAcceleration = $"( X = {data.deviceAcceleration.X}, Y = {data.deviceAcceleration.Y}, Z = {data.deviceAcceleration.Z} )",
        timeSent = data.timeSent.ToString()
      });

      VelocityX.Add(new ObservablePoint(data.timeSent.Ticks, data.deviceVelocity.X));

      VelocityY.Add(new ObservablePoint(data.timeSent.Ticks, data.deviceVelocity.Y));

      VelocityZ.Add(new ObservablePoint(data.timeSent.Ticks, data.deviceVelocity.Z));

      AccelerationX.Add(new ObservablePoint(data.timeSent.Ticks, data.deviceAcceleration.X));

      AccelerationY.Add(new ObservablePoint(data.timeSent.Ticks, data.deviceAcceleration.Y));

      AccelerationZ.Add(new ObservablePoint(data.timeSent.Ticks, data.deviceAcceleration.Z));
    }
  }
}
