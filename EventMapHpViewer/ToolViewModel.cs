using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grabacr07.KanColleWrapper;
using Livet;

namespace EventMapHpViewer
{
    public class ToolViewModel : ViewModel
    {

        #region MapInfoProxy変更通知プロパティ
        private MapInfoProxy _MapInfoProxy;

        public MapInfoProxy MapInfoProxy
        {
            get
            { return _MapInfoProxy; }
            set
            { 
                if (_MapInfoProxy == value)
                    return;
                _MapInfoProxy = value;
                if (_MapInfoProxy != null)
                {
                    _MapInfoProxy.PropertyChanged += (sender, e) =>
                    {
                        if (e.PropertyName == "Maps")
                        {
                            RaisePropertyChanged(() => NextEventMapHp);
                            RaisePropertyChanged(() => RemainingCount);
                        }
                    };
                }
                RaisePropertyChanged();
            }
        }
        #endregion

        public string NextEventMapHp
        {
            get
            {
                if (MapInfoProxy.Maps == null) return "No Data";
                var map = MapInfoProxy.Maps.api_data.LastOrDefault(x => x.api_eventmap != null);
                if (map == null) return "No Map";
                return map.api_eventmap.api_now_maphp + "/" + map.api_eventmap.api_max_maphp;
            }
        }

        public string RemainingCount
        {
            get
            {
                var shipMaster = KanColleClient.Current.Master.Ships;
                if (MapInfoProxy == null || MapInfoProxy.Maps == null) return "No Data";
                var map = MapInfoProxy.Maps.api_data.LastOrDefault(x => x.api_eventmap != null);
                if (map == null) return "No Map";
                if (!MapInfo.EventBossDictionary.ContainsKey(map.api_id)) return "未対応マップ";
                var lastBossHp = shipMaster
                                .Single(x => x.Key == MapInfo.EventBossDictionary[map.api_id].Last())
                                .Value.HP;
                var normalBossHp = shipMaster
                                .Single(x => x.Key == MapInfo.EventBossDictionary[map.api_id].First())
                                .Value.HP;
                if (map.api_eventmap.api_now_maphp <= lastBossHp) return "1回";
                return (Math.Ceiling((double)(map.api_eventmap.api_now_maphp - lastBossHp) / normalBossHp) + 1) + "回";
            }
        }
    }
}
