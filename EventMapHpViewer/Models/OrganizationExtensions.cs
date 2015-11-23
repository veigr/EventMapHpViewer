using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grabacr07.KanColleWrapper;
using Grabacr07.KanColleWrapper.Models;

namespace EventMapHpViewer.Models
{
    public static class OrganizationExtensions
    {
        public static int TransportationCapacity(this Organization org, bool isS = false)
        {
            if (org == null || org.Fleets == null || org.Fleets.Count < 1)
                return 0;
            var coefficient = isS ? 1.45 : 1;
            return (int)Math.Floor(org.BaseTransportationCapacity() * coefficient)
                + (int)Math.Floor(org.DrumTransportationCapacity() * coefficient)
                + (int)Math.Floor(org.DaihatsuTransportationCapacity() * coefficient)
                ;
        }

        private static double BaseTransportationCapacity(this Organization org)
        {
            var ships = org.Combined ? org.CombinedFleet.Fleets.SelectMany(x => x.Ships) : org.Fleets[1].Ships;
            return ships.Sum(x => x.Info.ShipType.TransportationCapacity());
        }

        private static double DrumTransportationCapacity(this Organization org)
        {
            var ships = org.Combined ? org.CombinedFleet.Fleets.SelectMany(x => x.Ships) : org.Fleets[1].Ships;
            var countDrum = ships.Sum(x => x.Slots.Count(y => y.Item.Info.Id == 75));
            return countDrum * 3.5;
        }

        private static double DaihatsuTransportationCapacity(this Organization org)
        {
            var ships = org.Combined ? org.CombinedFleet.Fleets.SelectMany(x => x.Ships) : org.Fleets[1].Ships;
            var countDaihatsu = ships.Sum(x => x.Slots.Count(y => y.Item.Info.Id == 68));
            return countDaihatsu * 5.5;
        }

        private static double TransportationCapacity(this ShipType type)
        {
            switch (type.Id)
            {
                case 2:
                    return 3.5;
                case 3:
                    return 1.2;
                case 6:
                    return 3;
                case 16:
                    return 6.4;
                case 17:
                    return 8.5;
                case 22:
                    return 10.5;
                default:
                    return 0;   //わからないのは0
            }
        }
    }
}
