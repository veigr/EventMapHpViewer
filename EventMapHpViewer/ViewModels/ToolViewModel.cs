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
using EventMapHpViewer.Models.Settings;

namespace EventMapHpViewer.ViewModels
{
    public class ToolViewModel : ViewModel
    {
        private readonly MapInfoProxy mapInfoProxy;

        public ToolViewModel(MapInfoProxy proxy)
        {
            this.mapInfoProxy = proxy;
            this.CompositeDisposable.Add(proxy);

            if (this.mapInfoProxy == null) return;

            this.mapInfoProxy.Subscribe(
                nameof(MapInfoProxy.Maps),
                () =>
                {
                    if (this.mapInfoProxy?.Maps?.MapList == null) return;
                    // 雑
                    this.Maps = this.mapInfoProxy.Maps.MapList
                        .OrderBy(x => x.Id)
                        .Select(x => new MapViewModel(x))
                        .Where(x => !x.IsCleared)
                        .ToArray();
                    this.IsNoMap = !this.Maps.Any();
                }, false)
                .AddTo(this);

            KanColleClient.Current
                .Subscribe(nameof(KanColleClient.IsStarted), Initialize, false)
                .AddTo(this);

            MapHpSettings.UseLocalBossSettings.Subscribe(_ => this.UpdateRemainingCount()).AddTo(this);
            MapHpSettings.BossSettings.Subscribe(_ => this.UpdateRemainingCount()).AddTo(this);

            MapHpSettings.UseAutoCalcTpSettings.Subscribe(_ => this.UpdateTransportCapacity()).AddTo(this);
            MapHpSettings.TransportCapacityS.Subscribe(_ => this.UpdateTransportCapacity()).AddTo(this);
            MapHpSettings.ShipTypeTpSettings.Subscribe(_ => this.UpdateTransportCapacity()).AddTo(this);
            MapHpSettings.SlotItemTpSettings.Subscribe(_ => this.UpdateTransportCapacity()).AddTo(this);
        }

        public void Initialize()
        {
            Debug.WriteLine("ToolViewModel: Initialize()");
            KanColleClient.Current.Homeport.Organization
                .Subscribe(nameof(Organization.Fleets), this.UpdateFleets, false)
                .Subscribe(nameof(Organization.Combined), this.UpdateTransportCapacity, false)
                .Subscribe(nameof(Organization.Ships), () => this.handledShips.Clear(), false)
                .AddTo(this);
            KanColleClient.Current.Proxy.ApiSessionSource
                .Where(s => s.Request.PathAndQuery == "/kcsapi/api_req_map/next")
                .TryParse<map_start_next>()
                .Subscribe(x =>
                {
                    if (x.Data.api_event_id == 9)
                    {
                        Debug.WriteLine("ToolViewModel: fixedTransportCapacity = true");
                        this.fixedTransportCapacity = true;
                    }
                })
                .AddTo(this);
            KanColleClient.Current.Proxy.api_port
                .Subscribe(_ =>
                {
                    if (fixedTransportCapacity)
                    {
                        Debug.WriteLine("ToolViewModel: fixedTransportCapacity = false");
                        this.fixedTransportCapacity = false;
                    }
                    this.UpdateTransportCapacity();
                })
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
        

        #region TransportCapacity 変更通知プロパティ
        private TransportCapacity _TransportCapacity;

        public TransportCapacity TransportCapacity
        {
            get
            { return this._TransportCapacity; }
            set
            {
                if (this._TransportCapacity.Equals(value))
                    return;
                this._TransportCapacity = value;
                this.RaisePropertyChanged();
            }
        }
        #endregion

        public bool ExistsTransportGauge
            => this.Maps?.Any(x => x.GaugeType == GaugeType.Transport) ?? false;

        private readonly HashSet<Ship> handledShips = new HashSet<Ship>();

        private readonly List<IDisposable> fleetHandlers = new List<IDisposable>();

        private bool fixedTransportCapacity;

        private void UpdateFleets()
        {
            foreach (var handler in fleetHandlers)
            {
                handler.Dispose();
            }
            this.fleetHandlers.Clear();
            foreach (var fleet in KanColleClient.Current.Homeport.Organization.Fleets.Values)
            {
                this.fleetHandlers.Add(fleet.Subscribe(nameof(fleet.Ships), this.UpdateTransportCapacity, false));
                foreach (var ship in fleet.Ships)
                {
                    if (this.handledShips.Contains(ship)) return;
                    this.fleetHandlers.Add(ship.Subscribe(nameof(ship.Slots), this.UpdateTransportCapacity, false));
                    this.fleetHandlers.Add(ship.Subscribe(nameof(ship.Situation), this.UpdateTransportCapacity, false));
                    this.handledShips.Add(ship);
                }
            }
        }

        private void UpdateTransportCapacity()
        {
            if (this.fixedTransportCapacity) return;    // 揚陸地点到達後は更新しない

            if (KanColleClient.Current.Homeport?.Organization?.Fleets.Any() != true) return;

            Debug.WriteLine(nameof(this.UpdateTransportCapacity));
            this.TransportCapacity = KanColleClient.Current.Homeport.Organization.TransportationCapacity();
            this.UpdateRemainingCount();
        }

        private void UpdateRemainingCount()
        {
            if (this.fixedTransportCapacity) return;    // 揚陸地点到達後は更新しない

            if (this.Maps == null) return;
            foreach (var map in this.Maps)
            {
                map.UpdateRemainingCount();
            }
        }
    }
}
