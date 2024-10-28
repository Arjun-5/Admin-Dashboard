using StressCommunicationAdminPanel.Helpers;
using System;

namespace StressCommunicationAdminPanel.Models
{
  public class DevicePhysicsData
  {
    public DeviceType deviceType { get; set; }

    public SerializableVector3 deviceVelocity { get; set; }

    public SerializableVector3 deviceAcceleration { get; set; }

    public DateTime timeSent { get; set; }
  }
  public class PhysicsInfoDataTable
  {
    public DeviceType deviceType { get; set; }

    public string deviceVelocity { get; set; }

    public string deviceAcceleration { get; set; }

    public string timeSent { get; set; }
  }
}
