using EventMapHpViewer.Models;
using EventMapHpViewer.Models.Settings;
using Livet;
using StatefulModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MetroTrilithon.Mvvm;
using System.Threading;
using Grabacr07.KanColleWrapper;

namespace EventMapHpViewer.ViewModels.Settings
{
    public class TpSettingsViewModel: ViewModel
    {
        #region IsEnabled
        private bool _IsEnabled;
        public bool IsEnabled
        {
            get => this._IsEnabled;
            set
            {
                if (value == this._IsEnabled)
                    return;
                this._IsEnabled = value;
                this.RaisePropertyChanged();
            }
        }
        #endregion
        
        #region UseAutoCalcTpSettings 変更通知プロパティ
        public bool UseAutoCalcTpSettings
        {
            get => MapHpSettings.UseAutoCalcTpSettings.Value;
            set
            {
                if (MapHpSettings.UseAutoCalcTpSettings.Value == value)
                    return;
                MapHpSettings.UseAutoCalcTpSettings.Value = value;
                this.RaisePropertyChanged();
            }
        }
        #endregion

        #region TransportCapacity 変更通知プロパティ
        private TransportCapacity _TransportCapacity;

        public TransportCapacity TransportCapacity
        {
            get => this._TransportCapacity;
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
        public decimal TransportCapacityS
        {
            get => MapHpSettings.TransportCapacityS.Value;
            set
            {
                if (MapHpSettings.TransportCapacityS.Value != value)
                {
                    MapHpSettings.TransportCapacityS.Value = value;
                    this.RaisePropertyChanged();
                }
                this.TransportCapacity = new TransportCapacity(value);
            }
        }
        #endregion

        #region ShipTypeTpSettings 変更通知プロパティ
        private ReadOnlyNotifyChangedCollection<TpSetting> _ShipTypeTpSettings;

        public ReadOnlyNotifyChangedCollection<TpSetting> ShipTypeTpSettings
        {
            get => this._ShipTypeTpSettings;
            set
            {
                if (this._ShipTypeTpSettings == value)
                    return;
                this._ShipTypeTpSettings = value;
                this.RaisePropertyChanged();
            }
        }
        #endregion

        #region SlotItemTpSettings 変更通知プロパティ
        private ReadOnlyNotifyChangedCollection<TpSetting> _SlotItemTpSettings;

        public ReadOnlyNotifyChangedCollection<TpSetting> SlotItemTpSettings
        {
            get => this._SlotItemTpSettings;
            set
            {
                if (this._SlotItemTpSettings == value)
                    return;
                this._SlotItemTpSettings = value;
                this.RaisePropertyChanged();
            }
        }
        #endregion

        #region ShipTpSettings 変更通知プロパティ
        private ReadOnlyNotifyChangedCollection<TpSetting> _ShipTpSettings;

        public ReadOnlyNotifyChangedCollection<TpSetting> ShipTpSettings
        {
            get => this._ShipTpSettings;
            set
            {
                if (this._ShipTpSettings == value)
                    return;
                this._ShipTpSettings = value;
                this.RaisePropertyChanged();
            }
        }
        #endregion

        internal AutoCalcTpSettings Settings { get; }

        public TpSettingsViewModel()
        {
            this.Settings = AutoCalcTpSettings.FromSettings;

            this.Settings.Subscribe(nameof(AutoCalcTpSettings.ShipTypeTp), () =>
            DispatcherHelper.UIDispatcher.Invoke(() =>
            {
                this.ShipTypeTpSettings = this.Settings.ShipTypeTp
                        .ToSyncedSynchronizationContextCollection(SynchronizationContext.Current)
                        .ToSyncedSortedObservableCollection(x => x.TypeId * 10000 + x.SortId)
                        .ToSyncedReadOnlyNotifyChangedCollection();
            }))
            .AddTo(this);

            this.Settings.Subscribe(nameof(AutoCalcTpSettings.SlotItemTp), () =>
            DispatcherHelper.UIDispatcher.Invoke(() =>
            {
                this.SlotItemTpSettings = this.Settings.SlotItemTp
                        .ToSyncedSynchronizationContextCollection(SynchronizationContext.Current)
                        .ToSyncedSortedObservableCollection(x => x.TypeId * 10000 + x.SortId)
                        .ToSyncedReadOnlyNotifyChangedCollection();
            }))
            .AddTo(this);

            this.Settings.Subscribe(nameof(AutoCalcTpSettings.ShipTp), () =>
            DispatcherHelper.UIDispatcher.Invoke(() =>
            {
                this.ShipTpSettings = this.Settings.ShipTp
                        .ToSyncedSynchronizationContextCollection(SynchronizationContext.Current)
                        .ToSyncedSortedObservableCollection(x => x.TypeId * 10000 + x.SortId)
                        .ToSyncedReadOnlyNotifyChangedCollection();
            }))
            .AddTo(this);
        }

        public void Save()
            => this.Settings.Save();

        public void Reset()
            => this.Settings.ResetAndSave();
    }
}
