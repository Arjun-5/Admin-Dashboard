using LiveChartsCore.Measure;
using LiveChartsCore.SkiaSharpView.Painting;
using System.Collections.ObjectModel;

namespace StressCommunicationAdminPanel.Interfaces
{
  public interface IStressPanelChart<TSeries, TCategory>
  {
    ObservableCollection<TSeries> chartSeriesCollection { get; set; }

    void ConfigureDefaultChartAttributes();

    void InitializeChartSeries();

    void UpdateChartData(TCategory category);

    SolidColorPaint GetColorForCategory(TCategory category);

    TSeries ConfigureChartStyling(TCategory category, int defaultValue, int dataLabelsSize, int strokeThickness, string strokeColor, PolarLabelsPosition labelsPosition);
  }
}
