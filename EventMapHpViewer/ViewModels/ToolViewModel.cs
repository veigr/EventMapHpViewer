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

            this.mapInfoProxy.Subscribe(
                nameof(MapInfoProxy.Maps),
                () =>
                {
                    if (this.mapInfoProxy?.Maps?.MapList == null) return;
                    // M の中身は殆ど変更通知してくれないし全部一括作りなおししかしないひどい実装
                    this.Maps = this.mapInfoProxy.Maps.MapList
                        .OrderBy(x => x.Id)
                        .Select(x => new MapViewModel(x))
                        .Where(x => !x.IsCleared)
                        .ToArray();
                    this.IsNoMap = !this.Maps.Any();
                }, false);

            KanColleClient.Current
                .Subscribe(nameof(KanColleClient.IsStarted), Initialize, false);
        }

        public void Initialize()
        {
            Debug.WriteLine("ToolViewModel: Initialize()");
            this.TransportCapacityS = MapHpSettings.TransportCapacityS;
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


        #region TransportCapacityS 変更通知プロパティ
        private int _TransportCapacityS;

        public int TransportCapacityS
        {
            get
            { return this._TransportCapacityS; }
            set
            {
                if (this._TransportCapacityS.Equals(value))
                    return;
                this._TransportCapacityS = value;
                this.RaisePropertyChanged();

                this.TransportCapacity = new TransportCapacity(value);

                if (MapHpSettings.TransportCapacityS != value)
                    MapHpSettings.TransportCapacityS.Value = value;
            }
        }
        #endregion
        

        public bool ExistsTransportGauge
            => this.Maps?.Any(x => x.GaugeType == GaugeType.Transport) ?? false;
    }
}
