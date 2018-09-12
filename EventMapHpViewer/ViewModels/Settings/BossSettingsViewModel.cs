using EventMapHpViewer.Models.Settings;
using Grabacr07.KanColleWrapper;
using Livet;
using StatefulModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MetroTrilithon.Mvvm;
using System.Threading;
using System.Runtime.CompilerServices;

namespace EventMapHpViewer.ViewModels.Settings
{
    public class BossSettingsViewModel: ViewModel
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

        #region UseLocalBossSettings
        public bool UseLocalBossSettings
        {
            get => MapHpSettings.UseLocalBossSettings.Value;
            set
            {
                if (value == MapHpSettings.UseLocalBossSettings.Value)
                    return;
                MapHpSettings.UseLocalBossSettings.Value = value;
                this.RaisePropertyChanged();
            }
        }
        #endregion

        #region Id
        private int _Id;
        public int Id
        {
            get => this._Id;
            set
            {
                if (value == this._Id)
                    return;
                this._Id = value;
                this.RaisePropertyChanged();
            }
        }
        #endregion

        #region MapItemsSource
        private IEnumerable<KeyValuePair<int, string>> _MapItemsSource;
        public IEnumerable<KeyValuePair<int, string>> MapItemsSource
        {
            get => this._MapItemsSource;
            set
            {
                if (value == this._MapItemsSource)
                    return;
                this._MapItemsSource = value;
                this.RaisePropertyChanged();
            }
        }
        #endregion

        #region Rank
        private int _Rank;
        public int Rank
        {
            get => this._Rank;
            set
            {
                if (value == this._Rank)
                    return;
                this._Rank = value;
                this.RaisePropertyChanged();
            }
        }
        #endregion

        #region RankItemsSource

        public IEnumerable<KeyValuePair<int, string>> RankItemsSource { get; }
            = Enum.GetValues(typeof(Models.Rank))
            .Cast<Models.Rank>()
            .Select(x => new KeyValuePair<int, string>((int)x, x.ToString()))
            .ToArray();

        #endregion

        #region GaugeNum
        private int _GaugeNum;
        public int GaugeNum
        {
            get => this._GaugeNum;
            set
            {
                if (value == this._GaugeNum)
                    return;
                this._GaugeNum = value;
                this.RaisePropertyChanged();
            }
        }
        #endregion

        #region BossHP
        private int _BossHP;
        public int BossHP
        {
            get => this._BossHP;
            set
            {
                if (value == this._BossHP)
                    return;
                this._BossHP = value;
                this.RaisePropertyChanged();
            }
        }
        #endregion

        #region IsLast
        private bool _IsLast;
        public bool IsLast
        {
            get => this._IsLast;
            set
            {
                if (value == this._IsLast)
                    return;
                this._IsLast = value;
                this.RaisePropertyChanged();
            }
        }
        #endregion

        #region IsAddEnabled
        public bool IsAddEnabled
        {
            get => !this.Settings.List.Any(
                x => x.Id == this.Id
                    && x.Rank == this.Rank
                    && x.GaugeNum == this.GaugeNum
                    && x.BossHP == this.BossHP
                    && x.IsLast == this.IsLast
                )
                && this.Id != default
                && this.Rank != default
                && this.GaugeNum != default
                && this.BossHP != default;
        }
        #endregion

        #region IsModifyRemoveEnabled
        public bool IsModifyRemoveEnabled
        {
            get => this.SelectedBossSetting != null;
        }
        #endregion

        #region SelectedBossSetting
        private BossSetting _SelectedBossSetting;
        public BossSetting SelectedBossSetting
        {
            get => this._SelectedBossSetting;
            set
            {
                this._SelectedBossSetting = value;
                if(value != null)
                {
                    this.Id = value.Id;
                    this.Rank = value.Rank;
                    this.GaugeNum = value.GaugeNum;
                    this.BossHP = value.BossHP;
                    this.IsLast = value.IsLast;
                }
                else
                {
                    this.Id = default;
                    this.Rank = default;
                    this.GaugeNum = default;
                    this.BossHP = default;
                    this.IsLast = default;
                }
                this.RaisePropertyChanged();
            }
        }
        #endregion

        #region BossSettings
        private ReadOnlyNotifyChangedCollection<BossSetting> _BossSettings;
        public ReadOnlyNotifyChangedCollection<BossSetting> BossSettings
        {
            get => this._BossSettings;
            set
            {
                this._BossSettings = value;
                this.RaisePropertyChanged();
            }
        }
        #endregion

        private BossSettings Settings { get; }

        public BossSettingsViewModel()
        {
            this.Settings = new BossSettings();

            this.BossSettings = this.Settings.List
                .ToSyncedSortedObservableCollection(x => $"{x.Id:D4}{x.Rank:D2}{x.GaugeNum:D2}{(x.IsLast ? 1 : 0)}{x.BossHP:D4}")
                .ToSyncedSynchronizationContextCollection(SynchronizationContext.Current)
                .ToSyncedReadOnlyNotifyChangedCollection();
        }

        public void Add()
        {
            var newValue = new BossSetting
            {
                Id = this.Id,
                Rank = this.Rank,
                GaugeNum = this.GaugeNum,
                BossHP = this.BossHP,
                IsLast = this.IsLast
            };
            this.Settings.List.Add(newValue);
            this.Settings.Save();
            this.SelectedBossSetting = newValue;
            this.UpdateButtonState();
        }

        public void Modify()
        {
            this.Settings.List.Remove(this.SelectedBossSetting);
            this.Add();
        }

        public void Remove()
        {
            this.Settings.List.Remove(this.SelectedBossSetting);
            this.Settings.Save();
            this.UpdateButtonState();
        }

        private void UpdateButtonState()
        {
            this.RaisePropertyChanged(nameof(this.IsAddEnabled));
            this.RaisePropertyChanged(nameof(this.IsModifyRemoveEnabled));
        }

        protected override void RaisePropertyChanged([CallerMemberName] string propertyName = "")
        {
            base.RaisePropertyChanged(propertyName);

            if(propertyName != nameof(this.IsAddEnabled)
            && propertyName != nameof(this.IsModifyRemoveEnabled))
                this.UpdateButtonState();
        }
    }
}
