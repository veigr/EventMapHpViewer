using System.Linq;
using EventMapHpViewer.Models;
using Livet;
using Livet.EventListeners;
using Grabacr07.KanColleWrapper;
using MetroTrilithon.Mvvm;
using System.Collections.Generic;
using Grabacr07.KanColleWrapper.Models;
using System;
using System.Reactive.Linq;
using EventMapHpViewer.Models.Raw;
using System.Diagnostics;

namespace EventMapHpViewer.ViewModels
{
    public class ToolViewModel : ViewModel
    {
        private readonly MapInfoProxy mapInfoProxy;

        public ToolViewModel(MapInfoProxy proxy)
        {
            this.mapInfoProxy = proxy;

            if (this.mapInfoProxy == null) return;

            this.CompositeDisposable.Add(new PropertyChangedEventListener(this.mapInfoProxy)
            {
                {
                    () => this.mapInfoProxy.Maps, (sender, args) =>
                    {
                        // M の中身は殆ど変更通知してくれないし全部一括作りなおししかしないひどい実装
                        this.Maps = this.mapInfoProxy.Maps.MapList
                            .OrderBy(x => x.Id)
                            .Select(x => new MapViewModel(x))
                            .Where(x => !x.IsCleared)
                            .ToArray();
                        this.IsNoMap = !this.Maps.Any();
                        this.FleetsUpdated();
                    }
                }
            });
        }

        public void Loaded()
        {
            // 変更検知はあまり深く考えないでやってしまっているのでマズいところあるかも (そもそもVMでやることではない)
            KanColleClient.Current.Homeport.Organization
                .Subscribe(nameof(Organization.Fleets), this.FleetsUpdated)
                .Subscribe(nameof(Organization.Combined), this.RaiseTransportCapacityChanged)
                .Subscribe(nameof(Organization.Ships), () => this.handledShips.Clear())
                .AddTo(this);
            KanColleClient.Current.Proxy.ApiSessionSource
                .Where(s => s.Request.PathAndQuery == "/kcsapi/api_req_map/next")
                .TryParse<map_start_next>()
                .Subscribe(x =>
                {
                    if (x.Data.api_event_id == 9)
                        this.fixedTransportCapacity = true;
                })
                .AddTo(this);
            KanColleClient.Current.Proxy.api_port
                .Subscribe(_ => this.fixedTransportCapacity = false)
                .AddTo(this);
        }

        #region Maps変更通知プロパティ
        private MapViewModel[] _Maps;

        public MapViewModel[] Maps
        {
            get
            { return this._Maps; }
            set
            { 
                if (this._Maps == value)
                    return;
                this._Maps = value;
                this.RaisePropertyChanged();
                this.RaisePropertyChanged(nameof(this.ExistsTransportGauge));
            }
        }
        #endregion


        #region IsNoMap変更通知プロパティ
        private bool _IsNoMap;

        public bool IsNoMap
        {
            get
            { return this._IsNoMap; }
            set
            { 
                if (this._IsNoMap == value)
                    return;
                this._IsNoMap = value;
                this.RaisePropertyChanged();
            }
        }
        #endregion

        public bool ExistsTransportGauge
            => this.Maps?.Any(x => x.GaugeType == GaugeType.Transport) ?? false;

        public int TransportCapacity
            => KanColleClient.Current.Homeport.Organization.TransportationCapacity();

        public int TransportCapacityS
            => KanColleClient.Current.Homeport.Organization.TransportationCapacity(true);

        private readonly HashSet<Ship> handledShips = new HashSet<Ship>();

        private readonly List<IDisposable> fleetHandlers = new List<IDisposable>();

        private bool fixedTransportCapacity;

        private void FleetsUpdated()
        {
            foreach (var handler in fleetHandlers)
            {
                handler.Dispose();
            }
            this.fleetHandlers.Clear();
            foreach (var fleet in KanColleClient.Current.Homeport.Organization.Fleets.Values)
            {
                this.fleetHandlers.Add(fleet.Subscribe(nameof(fleet.Ships), this.RaiseTransportCapacityChanged));
                foreach (var ship in fleet.Ships)
                {
                    if (this.handledShips.Contains(ship)) return;
                    this.fleetHandlers.Add(ship.Subscribe(nameof(ship.Slots), this.RaiseTransportCapacityChanged));
                    this.fleetHandlers.Add(ship.Subscribe(nameof(ship.Situation), this.RaiseTransportCapacityChanged));
                    this.handledShips.Add(ship);
                }
            }
        }

        private void RaiseTransportCapacityChanged()
        {
            if (this.fixedTransportCapacity) return;    // 揚陸地点到達後は更新しない

            Debug.WriteLine(nameof(this.RaiseTransportCapacityChanged));
            this.RaisePropertyChanged(nameof(this.TransportCapacity));
            this.RaisePropertyChanged(nameof(this.TransportCapacityS));
            if (this.Maps == null) return;
            foreach (var map in this.Maps)
            {
                map.UpdateTransportCapacity();
            }
        }
    }
}
