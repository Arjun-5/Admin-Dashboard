using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StressCommunicationAdminPanel.Interfaces
{
  public interface IStressPanelChart<TSeries, TCategory>
  {
    ObservableCollection<TSeries> ChartSeriesCollection { get; set; }

    void UpdateChartData(TCategory category);

    void InitializeChartSeries();

    void ConfigureChartAttributes();

    SolidColorPaint GetColorForCategory(TCategory category);
  }
}
