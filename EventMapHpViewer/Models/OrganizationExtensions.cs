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
            if (org?.Fleets == null || org.Fleets.Count < 1)
                return 0;
            var coefficient = isS ? 1.0 : 0.7;
            var tp = org.BaseTransportationCapacity()
                     + org.DrumTransportationCapacity()
                     + org.DaihatsuTransportationCapacity()
                     + org.RationsTransportationCapacity();
            return (int) Math.Floor(tp * coefficient);
        }

        private static double BaseTransportationCapacity(this Organization org)
        {
            return org.TransportingShips()
                .Sum(x => x.Info.ShipType.TransportationCapacity());
        }

        private static double DrumTransportationCapacity(this Organization org)
            => org.CountSlotitem(75) * 5.0;

        private static double DaihatsuTransportationCapacity(this Organization org)
            => org.CountSlotitem(68) * 8.0 + org.CountSlotitem(166) * 8.0;

        private static double LaunchransportationCapacity(this Organization org)
            => org.CountSlotitem(167) * 2.0;

        private static double RationsTransportationCapacity(this Organization org)
            => org.CountExSlotitem(145) * 1.0;

        private static int CountSlotitem(this Organization org, int slotitemId)
        {
            return org.TransportingShips()
                .Sum(x => x.Slots.Count(y => y.Item.Info.Id == slotitemId));
        }

        private static int CountExSlotitem(this Organization org, int slotitemId)
        {
            return org.TransportingShips()
                .Sum(x => x.ExSlot?.Item.Info.Id == slotitemId ? 1 : 0);
        }

        private static double TransportationCapacity(this ShipType type)
        {
            switch (type.Id)
            {
                case 2:     // 駆逐艦
                    return 5.0;
                case 3:     // 軽巡
                    return 2.0;
                case 6:     // 航空巡洋艦
                    return 4.0;
                case 10:    // 航空戦艦
                    return 7.0;
                case 16:    // 水上機母艦
                    return 9.0;
                case 17:    // 揚陸艦
                    return 12.0;
                case 20:    // 潜水母艦
                    return 7.0;
                case 21:    // 練習巡洋艦
                    return 6.0;
                case 22:    // 補給艦
                    return 15.0;
                default:
                    return 0;   // その他は0
            }
        }

        private static IEnumerable<Ship> TransportingShips(this Organization org)
        {
            var ships = org.Combined
                ? org.CombinedFleet.Fleets.SelectMany(x => x.Ships)
                : org.Fleets[1].Ships;
            return ships.PossibleTransport();
        }

        private static IEnumerable<Ship> PossibleTransport(this IEnumerable<Ship> ships)
        {
            return ships
                .Where(ship => !ship.Situation.HasFlag(ShipSituation.Evacuation))
                .Where(ship => !ship.Situation.HasFlag(ShipSituation.Tow))
                .Where(ship => !ship.Situation.HasFlag(ShipSituation.HeavilyDamaged));
        }
    }
}
