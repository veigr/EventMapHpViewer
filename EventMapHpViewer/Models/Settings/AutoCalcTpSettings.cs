using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Codeplex.Data;
using Grabacr07.KanColleWrapper;
using Grabacr07.KanColleWrapper.Models;
using Grabacr07.KanColleWrapper.Models.Raw;
using StatefulModel;
using MetroTrilithon.Mvvm;

namespace EventMapHpViewer.Models.Settings
{
    class AutoCalcTpSettings : Livet.NotificationObject
    {
        #region ShipTypeTp

        private ObservableSynchronizedCollection<TpSetting> _ShipTypeTp;
        public ObservableSynchronizedCollection<TpSetting> ShipTypeTp
        {
            get => this._ShipTypeTp;
            private set
            {
                if (this._ShipTypeTp == value)
                    return;
                this._ShipTypeTp = value;
                this.RaisePropertyChanged();
            }
        }

        #endregion

        #region SlotItemTp

        private ObservableSynchronizedCollection<TpSetting> _SlotItemTp;
        public ObservableSynchronizedCollection<TpSetting> SlotItemTp
        {
            get => this._SlotItemTp;
            private set
            {
                if (this._SlotItemTp == value)
                    return;
                this._SlotItemTp = value;
                this.RaisePropertyChanged();
            }
        }

        #endregion

        #region ShipTp

        private ObservableSynchronizedCollection<TpSetting> _ShipTp;
        public ObservableSynchronizedCollection<TpSetting> ShipTp
        {
            get => this._ShipTp;
            private set
            {
                if (this._ShipTp == value)
                    return;
                this._ShipTp = value;
                this.RaisePropertyChanged();
            }
        }

        #endregion

        public AutoCalcTpSettings()
        {
            this.ShipTypeTp = Default.ShipTypeTp;
            this.SlotItemTp = Default.SlotItemTp;
            this.ShipTp = Default.ShipTp;
        }

        private AutoCalcTpSettings(string stypeTp, string slotitemTp, string shipTp)
        {
            try { this.ShipTypeTp = DynamicJson.Parse(stypeTp); }
            catch { this.ShipTypeTp = Default.ShipTypeTp; }

            try { this.SlotItemTp = DynamicJson.Parse(slotitemTp); }
            catch { this.SlotItemTp = Default.SlotItemTp; }

            try { this.ShipTp = DynamicJson.Parse(shipTp); }
            catch { this.ShipTp = Default.ShipTp; }
        }

        private AutoCalcTpSettings(IEnumerable<TpSetting> stypeTp, IEnumerable<TpSetting> slotitemTp, IEnumerable<TpSetting> shipTp)
        {
            this.ShipTypeTp = new ObservableSynchronizedCollection<TpSetting>(new ObservableCollection<TpSetting>(stypeTp));
            this.SlotItemTp = new ObservableSynchronizedCollection<TpSetting>(new ObservableCollection<TpSetting>(slotitemTp));
            this.ShipTp = new ObservableSynchronizedCollection<TpSetting>(new ObservableCollection<TpSetting>(shipTp));
        }

        public void RestoreDefault()
        {
            this.ShipTypeTp = Default.ShipTypeTp;
            this.SlotItemTp = Default.SlotItemTp;
            this.ShipTp = Default.ShipTp;
            this.UpdateFromMaster();
        }

        public void UpdateFromMaster()
        {
            if (!KanColleClient.Current.IsStarted) return;

            var master = KanColleClient.Current.Master;

            this.ShipTypeTp = master.ShipTypes.UpdateSettings(this.ShipTypeTp,
                x => true,
                x => new TpSetting(x.Id, x.SortNumber, x.Name));

            this.SlotItemTp = master.SlotItems.UpdateSettings(this.SlotItemTp,
                x => x.RawData.api_sortno != 0,
                x => new TpSetting(x.Id, x.RawData.api_sortno, x.Name, 0, x.EquipType.Id, x.EquipType.Name));

            this.ShipTp = master.Ships.UpdateSettings(this.ShipTp,
                x => x.SortId != 0,
                x => new TpSetting(x.Id, x.SortId, x.Name, 0, x.ShipType.Id, x.ShipType.Name));

            this.Save();
        }

        public static AutoCalcTpSettings Default { get; } = CreateDefault();

        private static AutoCalcTpSettings CreateDefault()
        {
            var stypTp = new[]
            {
                new TpSetting(2, 2, "駆逐艦", 5),
                new TpSetting(3, 3, "軽巡洋艦", 2),
                new TpSetting(10, 10, "航空戦艦", 7),
                new TpSetting(16, 16, "水上機母艦", 9),
                new TpSetting(14, 14, "潜水空母", 1),
                new TpSetting(21, 21, "練習巡洋艦", 6),
                new TpSetting(6, 6, "航空巡洋艦", 4),
                new TpSetting(22, 22, "補給艦", 15),
                new TpSetting(17, 17, "揚陸艦", 12),
                new TpSetting(20, 20, "潜水母艦", 7),
            };

            var slotitemTp = new[]
            {
                new TpSetting(75, 75, "ドラム缶(輸送用)", 5),
                new TpSetting(68, 68, "大発動艇", 8),
                new TpSetting(193, 193, "特大発動艇", 8),
                new TpSetting(166, 166, "大発動艇(八九式中戦車＆陸戦隊)", 8),
                new TpSetting(230, 230, "特大発動艇＋戦車第11連隊", 8),
                new TpSetting(355, 355, "M4A1 DD", 8),
                new TpSetting(167, 167, "特二式内火艇", 2),
                new TpSetting(145, 145, "戦闘糧食", 1),
                new TpSetting(150, 150, "秋刀魚の缶詰", 1),
                new TpSetting(241, 241, "戦闘糧食(特別なおにぎり)", 1),
            };

            var shipTp = new[]
            {
                new TpSetting(487, 287, "鬼怒改二", 8),
            };

            return new AutoCalcTpSettings(stypTp, slotitemTp, shipTp);
        }

        public static AutoCalcTpSettings FromSettings
        {
            get => new AutoCalcTpSettings(
                MapHpSettings.ShipTypeTpSettings?.Value,
                MapHpSettings.SlotItemTpSettings?.Value,
                MapHpSettings.ShipTpSettings?.Value);
        }
        
    }

    static class AutoCalcTpSettingsExtensions
    {
        public static void Save(this AutoCalcTpSettings settings)
        {
            if (settings.ShipTypeTp.Any())
                MapHpSettings.ShipTypeTpSettings.Value = DynamicJson.Serialize(settings.ShipTypeTp);
            if (settings.SlotItemTp.Any())
                MapHpSettings.SlotItemTpSettings.Value = DynamicJson.Serialize(settings.SlotItemTp);
            if (settings.ShipTp.Any())
                MapHpSettings.ShipTpSettings.Value = DynamicJson.Serialize(settings.ShipTp);
        }

        public static void ResetAndSave(this AutoCalcTpSettings settings)
        {
            MapHpSettings.ShipTypeTpSettings?.Reset();
            MapHpSettings.SlotItemTpSettings?.Reset();
            MapHpSettings.ShipTpSettings?.Reset();
            settings.RestoreDefault();
        }

        public static ObservableSynchronizedCollection<TpSetting> UpdateSettings<T>(this MasterTable<T> master,
            IEnumerable<TpSetting> oldSettings,
            Func<T, bool> filter,
            Func<T, TpSetting> selector)
            where T : class, IIdentifiable
        {
            var newTps = master
                .Select(x => x.Value)
                .Where(filter)
                .Select(selector)
                .ToDictionary(x => x.Id);
            foreach (var oldTp in oldSettings)
            {
                if (newTps.Any(x => x.Key == oldTp.Id))
                {
                    newTps[oldTp.Id].Tp = oldTp.Tp;
                }
                else
                {
                    newTps.Add(oldTp.Id, oldTp);
                }
            }
            return new ObservableSynchronizedCollection<TpSetting>(new ObservableCollection<TpSetting>(newTps.Select(x => x.Value)));
        }
    }
}
