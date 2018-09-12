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

namespace EventMapHpViewer.Models.Settings
{
    class AutoCalcTpSettings : Livet.NotificationObject
    {
        public ObservableSynchronizedCollection<TpSetting> ShipTypeTp { get; private set; }
        public ObservableSynchronizedCollection<TpSetting> SlotItemTp { get; private set; }

        public AutoCalcTpSettings()
        {
            this.LoadFromLocal();
        }

        private AutoCalcTpSettings(IEnumerable<TpSetting> stypeTp, IEnumerable<TpSetting> slotitemTp)
        {
            this.ShipTypeTp = new ObservableSynchronizedCollection<TpSetting>(new ObservableCollection<TpSetting>(stypeTp));
            this.SlotItemTp = new ObservableSynchronizedCollection<TpSetting>(new ObservableCollection<TpSetting>(slotitemTp));
        }

        public void Save()
        {
            if(this.ShipTypeTp.Any())
                MapHpSettings.ShipTypeTpSettings.Value = DynamicJson.Serialize(this.ShipTypeTp);
            if (this.SlotItemTp.Any())
                MapHpSettings.SlotItemTpSettings.Value = DynamicJson.Serialize(this.SlotItemTp);
        }

        private void LoadFromLocal()
        {
            var stypeJson = MapHpSettings.ShipTypeTpSettings?.Value;
            if (!string.IsNullOrWhiteSpace(stypeJson))
            {
                this.ShipTypeTp = DynamicJson.Parse(stypeJson);
            }

            var slotItemJson = MapHpSettings.SlotItemTpSettings?.Value;
            if (!string.IsNullOrWhiteSpace(slotItemJson))
            {
                this.SlotItemTp = DynamicJson.Parse(slotItemJson);
            }

            this.RaiseSettingsPropertyChanged();
        }

        public void ResetAndSave()
        {
            MapHpSettings.ShipTypeTpSettings?.Reset();
            MapHpSettings.SlotItemTpSettings?.Reset();
            this.LoadFromLocal();
        }

        public void UpdateFromMaster()
        {
            if (!KanColleClient.Current.IsStarted) return;

            var master = KanColleClient.Current.Master;

            IDictionary<int, TpSetting> newStypeTp = master.ShipTypes
                .Select(x => new TpSetting(x.Value.Id, x.Value.Name))
                .ToDictionary(x => x.Id);
            foreach (var oldStyleTp in this.ShipTypeTp)
            {
                if (newStypeTp.Any(x => x.Key == oldStyleTp.Id))
                {
                    newStypeTp[oldStyleTp.Id].Name = oldStyleTp.Name;
                    newStypeTp[oldStyleTp.Id].Tp = oldStyleTp.Tp;
                }
                else
                {
                    newStypeTp.Add(oldStyleTp.Id, oldStyleTp);
                }
            }
            this.ShipTypeTp = new ObservableSynchronizedCollection<TpSetting>(new ObservableCollection<TpSetting>(newStypeTp.Select(x => x.Value)));

            var newSlotitemTp = master.SlotItems
                .Where(x => x.Key <= 500)
                .Select(x => new TpSetting(x.Value.Id, x.Value.Name, 0))
                .ToDictionary(x => x.Id);
            foreach (var oldSltitemTp in this.SlotItemTp)
            {
                if (newSlotitemTp.Any(x => x.Key == oldSltitemTp.Id))
                {
                    newSlotitemTp[oldSltitemTp.Id].Name = oldSltitemTp.Name;
                    newSlotitemTp[oldSltitemTp.Id].Tp = oldSltitemTp.Tp;
                }
                else
                {
                    newSlotitemTp.Add(oldSltitemTp.Id, oldSltitemTp);
                }
            }
            this.SlotItemTp = new ObservableSynchronizedCollection<TpSetting>(new ObservableCollection<TpSetting>(newSlotitemTp.Select(x => x.Value)));

            this.RaiseSettingsPropertyChanged();

            this.Save();
        }

        private void RaiseSettingsPropertyChanged()
        {
            this.RaisePropertyChanged(nameof(this.ShipTypeTp));
            this.RaisePropertyChanged(nameof(this.SlotItemTp));
        }

        public static AutoCalcTpSettings Default { get; } = CreateDefault();

        private static AutoCalcTpSettings CreateDefault()
        {
            var stypTp = new[]
            {
                new TpSetting(2, "駆逐艦", 5),
                new TpSetting(3, "軽巡洋艦", 2),
                new TpSetting(10, "航空戦艦", 7),
                new TpSetting(16, "水上機母艦", 9),
                new TpSetting(14, "潜水空母", 1),
                new TpSetting(21, "練習巡洋艦", 6),
                new TpSetting(6, "航空巡洋艦", 4),
                new TpSetting(22, "補給艦", 15),
                new TpSetting(17, "揚陸艦", 12),
                new TpSetting(20, "潜水母艦", 7),
            };

            var slotitemTp = new[]
            {
                new TpSetting(75, "ドラム缶(輸送用)", 5),
                new TpSetting(68, "大発動艇", 8),
                new TpSetting(193, "特大発動艇", 8),
                new TpSetting(166, "大発動艇(八九式中戦車＆陸戦隊)", 8),
                new TpSetting(230, "特大発動艇＋戦車第11連隊", 8),
                new TpSetting(167, "特二式内火艇", 2),
                new TpSetting(145, "戦闘糧食", 1),
                new TpSetting(150, "秋刀魚の缶詰", 1),
                new TpSetting(241, "戦闘糧食(特別なおにぎり)", 1),
            };

            return new AutoCalcTpSettings(stypTp, slotitemTp);
        }

        //private static KeyValuePair<ShipType, int> NewShipTypeTp(int id, string name, int tp)
        //{
        //    return new KeyValuePair<ShipType, int>(NewShipType(id, name), tp);
        //}

        //private static ShipType NewShipType(int id, string name)
        //{
        //    return new ShipType(new kcsapi_mst_stype()
        //    {
        //        api_id = id,
        //        api_name = name,
        //    });
        //}

        //private static TpSetting NewSlotItemTp(int id, string name, int tp)
        //{
        //    return new TpSetting(NewSlotItem(id, name), tp);
        //}

        //private static SlotItemInfo NewSlotItem(int id, string name)
        //{
        //    return typeof(SlotItemInfo).GetConstructor(
        //        BindingFlags.NonPublic | BindingFlags.Instance,
        //        null,
        //        new[] { typeof(kcsapi_mst_slotitem), typeof(MasterTable<SlotItemEquipType>) },
        //        null)
        //        .Invoke(new object[]
        //        {
        //            new kcsapi_mst_slotitem
        //            {
        //                api_id = id,
        //                api_name = name,
        //            },
        //            new MasterTable<SlotItemEquipType>()
        //        }) as SlotItemInfo;
        //}
    }

    public class TpSetting: Livet.NotificationObject
    {
        public int Id { get; set; }
        public string Name { get; set; }

        private decimal _Tp;
        public decimal Tp
        {
            get => this._Tp;
            set
            {
                if (this._Tp == value)
                    return;
                this._Tp = value;
                this.RaisePropertyChanged();
            }
        }

        public TpSetting() { }

        public TpSetting(int id, string name, decimal tp = 0)
        {
            this.Id = id;
            this.Name = name;
            this.Tp = tp;
        }
    }
}
