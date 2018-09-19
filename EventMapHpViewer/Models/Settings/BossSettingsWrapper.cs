using Codeplex.Data;
using StatefulModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MetroTrilithon.Mvvm;

namespace EventMapHpViewer.Models.Settings
{
    class BossSettingsWrapper: Livet.NotificationObject
    {
        private ObservableSynchronizedCollection<BossSetting> _List;
        public ObservableSynchronizedCollection<BossSetting> List
        {
            get => this._List;
            private set
            {
                if (this._List == value)
                    return;
                this._List = value;
                this.RaisePropertyChanged();
            }
        }

        public BossSettingsWrapper(string json = "")
        {
            try
            {
                BossSettingForParse[] parsed = DynamicJson.Parse(json);
                this.List = new ObservableSynchronizedCollection<BossSetting>(parsed.Select(x => x.ToValue()));
            }
            catch
            {
                this.List = new ObservableSynchronizedCollection<BossSetting>();
            }
        }

        public static IEnumerable<BossSetting> Parse(IEnumerable<Raw.map_exboss> source)
        {
            return source.Select(x => new BossSetting
            {
                MapId = x.mapid,
                Rank = x.rank,
                GaugeNum = x.gauge,
                BossHP = x.maxhp,
                IsLast = x.last,
            });
        }

        public static BossSettingsWrapper FromSettings
        {
            get => new BossSettingsWrapper(MapHpSettings.BossSettings?.Value);
        }
    }

    public class BossSetting
    {
        public int MapId { get; set; }
        public int Rank { get; set; }
        public int? GaugeNum { get; set; }
        public int BossHP { get; set; }
        public bool IsLast { get; set; }
    }

    public class BossSettingForParse
    {
        public int MapId { get; set; }
        public int Rank { get; set; }
        public string GaugeNum { get; set; }    // DynamicJson は int? に Parse できない
        public int BossHP { get; set; }
        public bool IsLast { get; set; }
        public BossSetting ToValue()
            => new BossSetting
            {
                MapId = this.MapId,
                Rank = this.Rank,
                GaugeNum = int.TryParse(this.GaugeNum, out var num) ? num : (int?)null,
                BossHP = this.BossHP,
                IsLast = this.IsLast,
            };
    }

    static class BossSettingsWrapperExtensions
    {
        public static void Save(this BossSettingsWrapper settings)
            => MapHpSettings.BossSettings.Value = DynamicJson.Serialize(settings.List);
    }
}
