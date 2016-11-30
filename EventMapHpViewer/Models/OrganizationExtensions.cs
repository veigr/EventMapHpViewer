using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grabacr07.KanColleWrapper;
using Grabacr07.KanColleWrapper.Models;

namespace EventMapHpViewer.Models
{
    static class OrganizationExtensions
    {
        private static RemoteSettingsClient client = new RemoteSettingsClient();

        public static async Task<int> TransportationCapacity(this Organization org, bool isS = false)
        {
            if (org?.Fleets == null || org.Fleets.Count < 1)
                return 0;
            var coefficient = isS ? 1.0 : 0.7;

            var settings = await TransportationCapacitySettings.Create(client);

            var tp = org.BaseTp(settings.ShipSettings) + org.SlotitemTp(settings.EquipSettings);
            return (int)Math.Floor(tp * coefficient);
        }

        private static double BaseTp(this Organization org, IDictionary<int, double> tpSettings)
        {
            return org.TransportingShips()
                .Sum(x =>
                {
                    double tp;
                    return tpSettings.TryGetValue(x.Info.ShipType.Id, out tp) ? tp : 0;
                });
        }

        private static double SlotitemTp(this Organization org, IDictionary<int, double> tpSettings)
        {
            return org.TransportingShips()
                .SelectMany(x => x.Slots)
                .Concat(org.TransportingShips().Select(x => x.ExSlot))
                .Select(x => x.Item.Info.Id)
                .Sum(x =>
                {
                    double tp;
                    return tpSettings.TryGetValue(x, out tp) ? tp : 0;
                });

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
