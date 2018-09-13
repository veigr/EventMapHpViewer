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
            this.List = !string.IsNullOrEmpty(json)
                ? DynamicJson.Parse(json)
                : new ObservableSynchronizedCollection<BossSetting>();
        }

        public static IEnumerable<BossSetting> Parse(IEnumerable<Raw.map_exboss> source)
        {
            return source.Select(x => new BossSetting
            {
                BossHP = x.ship.maxhp,
                IsLast = x.isLast,
            });
        }

        public static BossSettingsWrapper FromSettings
        {
            get => new BossSettingsWrapper(MapHpSettings.BossSettings?.Value);
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

    static class BossSettingsWrapperExtensions
    {
        public static void Save(this BossSettingsWrapper settings)
            => MapHpSettings.BossSettings.Value = DynamicJson.Serialize(settings.List);
    }
}
