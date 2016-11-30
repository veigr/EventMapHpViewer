using Livet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace EventMapHpViewer.Models
{
    class TransportationCapacitySettings
    {
        public IDictionary<int, double> ShipSettings { get; private set; }

        public IDictionary<int, double> EquipSettings { get; private set; }


        private TransportationCapacitySettings() { }

        public static async Task<TransportationCapacitySettings> Create(RemoteSettingsClient client)
        {
            var settings = await client.GetSettings<TpSettingsJson[]>("https://shatteredskies.blob.core.windows.net/kancolle/tp.json");
            var instance = new TransportationCapacitySettings();
            instance.ShipSettings = settings.Where(s => s.type == "ship")
                .ToDictionary(x => x.id, x => x.value);
            instance.EquipSettings = settings.Where(s => s.type == "equip")
                .ToDictionary(x => x.id, x => x.value);
            return instance;
        }


        private class TpSettingsJson
        {
            public string type { get; set; }
            public int id { get; set; }
            public double value { get; set; }
        }
    }
}
