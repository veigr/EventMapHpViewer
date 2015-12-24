using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventMapHpViewer.Models.Raw
{
    class map_exboss
    {
        public bool isLast { get; set; }
        public Ship ship { get; set; }

        public class Ship
        {
            public string shipName { get; set; }
            public int shipId { get; set; }
            public int shipLv { get; set; }
            public int maxhp { get; set; }
            public Slot[] slot { get; set; }
            public int[] param { get; set; }
        }

        public class Slot
        {
            public int slotitemId { get; set; }
            public string slotitemName { get; set; }
        }
    }
}
