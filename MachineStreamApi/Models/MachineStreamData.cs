using System;

namespace MachineStreamApi.Models
{
    public class MachineStreamData
    {
        public string Topic { get; set; }
        public object Ref { get; set; }
        public MachineStreamEventData Payload { get; set; }
        public string Event { get; set; }
    }

    public class MachineStreamEventData
    {
        public Guid Machine_Id { get; set; }
        public Guid Id { get; set; }
        public DateTime Timestamp { get; set; }
        public string Status { get; set; }
    }
}