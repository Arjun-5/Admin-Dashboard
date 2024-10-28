using LiveChartsCore.SkiaSharpView.Extensions;

namespace StressCommunicationAdminPanel.Models
{
  public class SelfReportStressNeedleGauge
  {
    public SelfReportStressStatus SelfReportStressStatus;

    public GaugeItem SelfReportStressNeedleGaugeItem;

    public SelfReportStressNeedleGauge(SelfReportStressStatus selfReportStressStatus, GaugeItem selfReportStressNeedleGaugeItem)
    {
      SelfReportStressStatus = selfReportStressStatus;
      
      SelfReportStressNeedleGaugeItem = selfReportStressNeedleGaugeItem;
    }
  }
}
