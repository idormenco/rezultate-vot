﻿using System.Collections.Generic;

namespace ElectionResults.Core.Models
{
    public class VoteMonitoringStats
    {
        public long Timestamp { get; set; }

        public List<MonitoringInfo> Statistics { get; set; }
    }
}