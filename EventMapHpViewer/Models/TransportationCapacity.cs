using Livet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace EventMapHpViewer.Models
{
    class TransportationCapacity
    {
        public IDictionary<int, double> ShipSettings { get; private set; }

        public IDictionary<int, double> EquipSettings { get; private set; }


        private TransportationCapacity() { }

        public static async Task<TransportationCapacity> Create(RemoteSettingsClient client)
        {
            var settings = await client.GetSettings<CapacitySettings[]>("");
            var instance = new TransportationCapacity();
            instance.ShipSettings = settings.Where(s => s.type == "ship")
                .ToDictionary(x => x.id, x => x.value);
            instance.EquipSettings = settings.Where(s => s.type == "equip")
                .ToDictionary(x => x.id, x => x.value);
            return instance;
        }


        private class CapacitySettings
        {
            public string type { get; set; }
            public int id { get; set; }
            public double value { get; set; }
        }
    }
}
