﻿using System;

namespace BurnForMoney.Infrastructure.Events
{
    public class ActivityUpdated : DomainEvent
    {
        public Guid ActivityId { get; set; }
        public double DistanceInMeters { get; set; }
        public double MovingTimeInMinutes { get; set; }

        public string ActivityType { get; set; }
        public DateTime StartDate { get; set; }
    }
}