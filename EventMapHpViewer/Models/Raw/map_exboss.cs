using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventMapHpViewer.Models.Raw
{
    class map_exboss
    {
        public int mapid { get; set; }
        public int rank { get; set; }
        public int? gauge { get; set; }
        public int maxhp { get; set; }
        public bool last { get; set; }
    }
}
