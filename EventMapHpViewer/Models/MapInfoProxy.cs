using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using EventMapHpViewer.Models.Raw;
using Grabacr07.KanColleWrapper;
using Grabacr07.KanColleWrapper.Models;
using Livet;
using MetroTrilithon.Lifetime;
using MetroTrilithon.Mvvm;

namespace EventMapHpViewer.Models
{
    public class MapInfoProxy : NotificationObject, IDisposableHolder
    {
        #region Maps変更通知プロパティ
        private Maps _Maps;

        public Maps Maps
        {
            get
            { return this._Maps; }
            set
            { 
                if (this._Maps == value)
                    return;
                this._Maps = value;
                this.RaisePropertyChanged();
            }
        }
        #endregion

        public MapInfoProxy()
        {
            this.Maps = new Maps();

            var proxy = KanColleClient.Current.Proxy;

            proxy.ApiSessionSource
                .Where(s => s.Request.PathAndQuery == "/kcsapi/api_start2/getData")
                .TryParse<kcsapi_start2>()
                .Subscribe(x =>
                {
                    Maps.MapAreas = new MasterTable<MapArea>(x.Data.api_mst_maparea.Select(m => new MapArea(m)));
                    Maps.MapInfos = new MasterTable<MapInfo>(x.Data.api_mst_mapinfo.Select(m => new MapInfo(m, Maps.MapAreas)));
                })
                .AddTo(this);

            proxy.ApiSessionSource
                .Where(s => s.Request.PathAndQuery == "/kcsapi/api_get_member/mapinfo")
                .TryParse<mapinfo>()
                .Subscribe(m =>
                {
                    Debug.WriteLine("MapInfoProxy - member_mapinfo");
                    this.Maps.MapList = this.CreateMapList(m.Data.api_map_info);
                    this.RaisePropertyChanged(() => this.Maps);
                })
                .AddTo(this);

            proxy.ApiSessionSource
                .Where(s => s.Request.PathAndQuery == "/kcsapi/api_req_map/select_eventmap_rank")
                .TryParse<map_select_eventmap_rank>()
                .Subscribe(x =>
                {
                    Debug.WriteLine("MapInfoProxy - select_eventmap_rank");
                    this.Maps.MapList = this.UpdateRank(x);
                    this.RaisePropertyChanged(() => this.Maps);
                })
                .AddTo(this);


            proxy.ApiSessionSource
                .Where(x => x.Request.PathAndQuery == "/kcsapi/api_req_map/start")
                .TryParse<map_start_next>()
                .Subscribe(x =>
                {
                    if (x.Data.api_eventmap == null) return;
                    var targetMap = this.Maps.MapList
                                        .FirstOrDefault(m => m.Id.ToString() == x.Data.api_maparea_id.ToString() + x.Data.api_mapinfo_no.ToString());
                    if (targetMap?.Eventmap == null) return;

                    if (targetMap.Eventmap.MaxMapHp.HasValue
                    && targetMap.Eventmap.MaxMapHp != 9999)
                        return;

                    Debug.WriteLine("MapInfoProxy - map_start_next");
                    targetMap.Eventmap.NowMapHp = x.Data.api_eventmap.api_now_maphp;
                    targetMap.Eventmap.MaxMapHp = x.Data.api_eventmap.api_max_maphp;
                    this.RaisePropertyChanged(() => this.Maps);
                })
                .AddTo(this);
        }

        private MapData[] CreateMapList(IEnumerable<member_mapinfo> maps)
        {
            return maps
                .Select(x => new MapData
                {
                    IsCleared = x.api_cleared,
                    DefeatCount = x.api_defeat_count,
                    RequiredDefeatCount = x.api_required_defeat_count,
                    Id = x.api_id,
                    Eventmap = x.api_eventmap != null
                        ? new Eventmap
                        {
                            MaxMapHp = x.api_eventmap.api_max_maphp,
                            NowMapHp = x.api_eventmap.api_now_maphp,
                            SelectedRank = (Rank) x.api_eventmap.api_selected_rank,
                            State = x.api_eventmap.api_state,
                            GaugeType = (GaugeType) x.api_eventmap.api_gauge_type,
                            GaugeNum = x.api_eventmap.api_gauge_num,
                        }
                        : null,
                }).ToArray();
        }

        private MapData[] UpdateRank(SvData<map_select_eventmap_rank> data)
        {
            var rank = 0;
            int.TryParse(data.Request["api_rank"], out rank);
            var areaId = data.Request["api_maparea_id"];
            var mapNo = data.Request["api_map_no"];


            var list = this.Maps.MapList;
            var targetMap = list.FirstOrDefault(m => m.Id.ToString() == areaId + mapNo);
            if (targetMap?.Eventmap == null) return list;

            targetMap.Eventmap.SelectedRank = (Rank) rank;
            if(int.TryParse(data.Data.api_maphp.api_gauge_type, out var gaugeType))
                targetMap.Eventmap.GaugeType = (GaugeType) gaugeType;
            targetMap.Eventmap.MaxMapHp = data.Data.api_maphp.api_max_maphp;
            targetMap.Eventmap.NowMapHp = data.Data.api_maphp.api_now_maphp;
            return list;
        }

        ICollection<IDisposable> IDisposableHolder.CompositeDisposable { get; }
            = new ObservableSynchronizedCollection<IDisposable>();

        #region IDisposable Support
        private bool disposedValue = false; // 重複する呼び出しを検出するには

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: マネージド状態を破棄します (マネージド オブジェクト)。
                    lock (this)
                    {
                        foreach (var disposable in ((IDisposableHolder)this).CompositeDisposable)
                        {
                            disposable.Dispose();
                        }
                    }
                }

                // TODO: アンマネージド リソース (アンマネージド オブジェクト) を解放し、下のファイナライザーをオーバーライドします。
                // TODO: 大きなフィールドを null に設定します。

                disposedValue = true;
            }
        }

        // TODO: 上の Dispose(bool disposing) にアンマネージド リソースを解放するコードが含まれる場合にのみ、ファイナライザーをオーバーライドします。
        // ~MapInfoProxy() {
        //   // このコードを変更しないでください。クリーンアップ コードを上の Dispose(bool disposing) に記述します。
        //   Dispose(false);
        // }

        // このコードは、破棄可能なパターンを正しく実装できるように追加されました。
        public void Dispose()
        {
            // このコードを変更しないでください。クリーンアップ コードを上の Dispose(bool disposing) に記述します。
            Dispose(true);
            // TODO: 上のファイナライザーがオーバーライドされる場合は、次の行のコメントを解除してください。
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}
