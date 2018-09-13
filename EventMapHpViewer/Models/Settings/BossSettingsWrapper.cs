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
    class BossSettingsWrapper
    {
        public ObservableSynchronizedCollection<BossSetting> List { get; }
        
        public BossSettingsWrapper()
        {
            if(string.IsNullOrWhiteSpace(MapHpSettings.BossSettings.Value))
                this.List = new ObservableSynchronizedCollection<BossSetting>();
            else
                this.List = DynamicJson.Parse(MapHpSettings.BossSettings?.Value);
        }

        public void Save()
            => MapHpSettings.BossSettings.Value = DynamicJson.Serialize(this.List);

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
