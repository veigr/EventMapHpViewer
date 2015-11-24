using System.Linq;
using EventMapHpViewer.Models;
using Livet;
using Livet.EventListeners;
using Grabacr07.KanColleWrapper;
using MetroTrilithon.Mvvm;
using System.Collections.Generic;
using Grabacr07.KanColleWrapper.Models;

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
            KanColleClient.Current.Homeport.Organization
                .Subscribe(nameof(Organization.Fleets), this.FleetsUpdated)
                .Subscribe(nameof(Organization.Combined), this.RaiseTransportCapacityChanged)
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

        public bool ExistsTransportGauge => this.Maps?.Any(x => x.GaugeType == GaugeType.Transport) ?? false;

        public int TransportCapacity => KanColleClient.Current.Homeport.Organization.TransportationCapacity();

        public int TransportCapacityS => KanColleClient.Current.Homeport.Organization.TransportationCapacity(true);

        private HashSet<Ship> handledShips = new HashSet<Ship>();

        private void FleetsUpdated()
        {
            foreach (var fleet in KanColleClient.Current.Homeport.Organization.Fleets.Values)
            {
                fleet.Subscribe(nameof(fleet.Ships), this.RaiseTransportCapacityChanged);
                foreach (var ship in fleet.Ships)
                {
                    if (handledShips.Contains(ship)) return;
                    ship.Subscribe(nameof(ship.Slots), this.RaiseTransportCapacityChanged);
                    handledShips.Add(ship);
                }
            }
        }

        private void RaiseTransportCapacityChanged()
        {
            this.RaisePropertyChanged(nameof(this.TransportCapacity));
            this.RaisePropertyChanged(nameof(this.TransportCapacityS));
            if (this.Maps == null) return;
            foreach (var map in this.Maps)
            {
                map.CalcTransportCapacityChanged();
            }
        }
    }
}
