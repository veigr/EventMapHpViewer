﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventMapHpViewer.Models
{
    public enum GaugeType
    {
        Normal = 0,
        // 1, 2 は実際には返ってこないので暫定。逆かも。
        Extra = 1,
        Event = 2,
        Transport = 3,
    }
}
