using Grabacr07.KanColleWrapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EventMapHpViewer.Models.Settings;
using Grabacr07.KanColleWrapper.Models;

namespace EventMapHpViewer.Models
{
    static class TpExtensions
    {
        public static TransportCapacity TransportationCapacity(this Organization org)
        {
            if (!MapHpSettings.UseAutoCalcTpSettings.Value)
                return new TransportCapacity(MapHpSettings.TransportCapacityS.Value);

            var settings = AutoCalcTpSettings.FromSettings;
            var tp = org.TransportingShips()
                .PossibleTransport()
                .Sum(x => x.CalcTp(settings));
            return new TransportCapacity(tp);
        }

        public static decimal CalcTp(this Ship ship, AutoCalcTpSettings settings)
        {
            var stypeTp = settings.ShipTypeTp.FirstOrDefault(x => x.Id == ship.Info.ShipType.Id)?.Tp ?? 0;

            var slotTp = ship.Slots
                .Concat(new[] { ship.ExSlot })
                .Where(x => x.Equipped)
                .Select(x => x.Item.Info.Id)
                .Sum(x => settings.SlotItemTp.FirstOrDefault(y => y.Id == x)?.Tp ?? 0);

            return stypeTp + slotTp;
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
