using Codeplex.Data;
using StatefulModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventMapHpViewer.Models.Settings
{
    class BossSettings
    {
        public static ObservableSynchronizedCollection<BossSetting> Settings { get; }
        
        static BossSettings()
        {
            if(string.IsNullOrWhiteSpace(MapHpSettings.BossSettings))
                Settings = new ObservableSynchronizedCollection<BossSetting>();
            else
                Settings = DynamicJson.Parse(MapHpSettings.BossSettings.Value);
        }

        public static void Save()
            => MapHpSettings.BossSettings.Value = DynamicJson.Serialize(Settings);

        public static IEnumerable<BossSetting> Parse(IEnumerable<Raw.map_exboss> source)
        {
            return source.Select(x => new BossSetting
            {
                BossHP = x.ship.maxhp,
                IsLast = x.isLast,
            });
        }
    }

    public class BossSetting
    {
        public int Id { get; set; }
        public int Rank { get; set; }
        public int GaugeNum { get; set; }
        public int BossHP { get; set; }
        public bool IsLast { get; set; }
    }
}
