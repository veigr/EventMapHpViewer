using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventMapHpViewer.Models
{
    public enum GaugeType
    {
        // 1, 2 は実際には返ってこないので暫定。逆かも。
        Event = 1,
        Extra = 2,
        Transport = 3,
    }
}
