using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Media;
using EventMapHpViewer.Models;
using Livet;

namespace EventMapHpViewer.ViewModels
{
    public class MapViewModel : ViewModel
    {

        private static readonly SolidColorBrush red;
        private static readonly SolidColorBrush green;

        static MapViewModel()
        {
            red = new SolidColorBrush(Color.FromRgb(255, 32, 32));
            red.Freeze();
            green = new SolidColorBrush(Color.FromRgb(64, 200, 32));
            green.Freeze();
        }

        #region MapNumber変更通知プロパティ
        private string _MapNumber;

        public string MapNumber
        {
            get
            { return this._MapNumber; }
            set
            {
                if (this._MapNumber == value)
                    return;
                this._MapNumber = value;
                this.RaisePropertyChanged();
            }
        }
        #endregion


        #region Name変更通知プロパティ
        private string _Name;

        public string Name
        {
            get
            { return this._Name; }
            set
            {
                if (this._Name == value)
                    return;
                this._Name = value;
                this.RaisePropertyChanged();
            }
        }
        #endregion


        #region AreaName変更通知プロパティ
        private string _AreaName;

        public string AreaName
        {
            get
            { return this._AreaName; }
            set
            {
                if (this._AreaName == value)
                    return;
                this._AreaName = value;
                this.RaisePropertyChanged();
            }
        }
        #endregion


        #region Current変更通知プロパティ
        private int _Current;

        public int Current
        {
            get
            { return this._Current; }
            set
            {
                if (this._Current == value)
                    return;
                this._Current = value;
                this.RaisePropertyChanged();
            }
        }
        #endregion


        #region Max変更通知プロパティ
        private int _Max;

        public int Max
        {
            get
            { return this._Max; }
            set
            {
                if (this._Max == value)
                    return;
                this._Max = value;
                this.RaisePropertyChanged();
            }
        }
        #endregion


        #region SelectedRank変更通知プロパティ
        private string _SelectedRank;

        public string SelectedRank
        {
            get
            { return this._SelectedRank; }
            set
            {
                if (this._SelectedRank == value)
                    return;
                this._SelectedRank = value;
                this.RaisePropertyChanged();
            }
        }
        #endregion

        public Visibility SelectedRankVisibility
            => string.IsNullOrEmpty(this.SelectedRank) ? Visibility.Collapsed : Visibility.Visible;

        #region RemainingCountMin 変更通知プロパティ
        private string _RemainingCountMin;

        public string RemainingCountMin
        {
            get
            { return this._RemainingCountMin; }
            set
            {
                if (this._RemainingCountMin == value)
                    return;
                this._RemainingCountMin = value;
                this.RaisePropertyChanged();
                this.RaisePropertyChanged(nameof(this.IsSingleRemainingCount));
            }
        }
        #endregion

        #region RemainingCountMax 変更通知プロパティ
        private string _RemainingCountMax;

        public string RemainingCountMax
        {
            get
            { return this._RemainingCountMax; }
            set
            {
                if (this._RemainingCountMax == value)
                    return;
                this._RemainingCountMax = value;
                this.RaisePropertyChanged();
                this.RaisePropertyChanged(nameof(this.IsSingleRemainingCount));
            }
        }
        #endregion

        public bool IsSingleRemainingCount
            => this.RemainingCountMin == this.RemainingCountMax;


        #region RemainingCountTransportS変更通知プロパティ
        private string _RemainingCountTransportS;

        public string RemainingCountTransportS
        {
            get
            { return this._RemainingCountTransportS; }
            set
            {
                if (this._RemainingCountTransportS == value)
                    return;
                this._RemainingCountTransportS = value;
                this.RaisePropertyChanged();
            }
        }
        #endregion


        #region IsCleared変更通知プロパティ
        private bool _IsCleared;

        public bool IsCleared
        {
            get
            { return this._IsCleared; }
            set
            {
                if (this._IsCleared == value)
                    return;
                this._IsCleared = value;
                this.RaisePropertyChanged();
            }
        }
        #endregion


        #region GaugeColor変更通知プロパティ
        private SolidColorBrush _GaugeColor;

        public SolidColorBrush GaugeColor
        {
            get
            { return this._GaugeColor; }
            set
            {
                if (Equals(this._GaugeColor, value))
                    return;
                this._GaugeColor = value;
                this.RaisePropertyChanged();
            }
        }
        #endregion


        #region IsRankSelected変更通知プロパティ
        private bool _IsRankSelected;

        public bool IsRankSelected
        {
            get
            { return this._IsRankSelected; }
            set
            {
                if (this._IsRankSelected == value)
                    return;
                this._IsRankSelected = value;
                this.RaisePropertyChanged();
            }
        }
        #endregion


        #region IsLoading変更通知プロパティ
        private bool _IsLoading;

        public bool IsLoading
        {
            get
            { return this._IsLoading; }
            set
            {
                if (this._IsLoading == value)
                    return;
                this._IsLoading = value;
                this.RaisePropertyChanged();
                this.RaiseVisibilityChanged();
            }
        }
        #endregion


        #region IsSupported変更通知プロパティ
        private bool _IsSupported;

        public bool IsSupported
        {
            get
            { return this._IsSupported; }
            set
            {
                if (this._IsSupported == value)
                    return;
                this._IsSupported = value;
                this.RaisePropertyChanged();
                this.RaiseVisibilityChanged();
            }
        }
        #endregion

        public Visibility IsUnSupportedVisibility
            => !this.IsLoading && !this.IsSupported ? Visibility.Visible : Visibility.Collapsed;

        #region IsInfinity変更通知プロパティ
        private bool _IsInfinity;

        public bool IsInfinity
        {
            get
            { return this._IsInfinity; }
            set
            {
                if (this._IsInfinity == value)
                    return;
                this._IsInfinity = value;
                this.RaisePropertyChanged();
                this.RaiseVisibilityChanged();
            }
        }
        #endregion

        public Visibility IsInfinityVisibility
            => !this.IsLoading && this.IsInfinity ? Visibility.Visible : Visibility.Collapsed;

        public Visibility IsCountVisibility
            => !this.IsLoading && this.IsSupported && !this.IsInfinity ? Visibility.Visible : Visibility.Collapsed;

        #region GaugeType変更通知プロパティ
        private GaugeType _GaugeType;

        public GaugeType GaugeType
        {
            get
            { return this._GaugeType; }
            set
            {
                if (this._GaugeType == value)
                    return;
                this._GaugeType = value;
                this.RaisePropertyChanged();
            }
        }
        #endregion

        private MapData _source;

        public MapViewModel(MapData info)
        {
            this._source = info;
            this.MapNumber = info.MapNumber;
            this.Name = info.Name;
            this.AreaName = info.AreaName;
            this.Current = info.Current;
            this.Max = info.Max;
            this.SelectedRank = info.Eventmap?.SelectedRankText ?? "";
            this.RemainingCountTransportS = info.RemainingCountTransportS.ToString();
            this.IsCleared = info.IsCleared == 1;
            this.IsRankSelected = info.Eventmap == null
                || info.Eventmap.SelectedRank != 0
                || info.Eventmap.NowMapHp != 9999;
            this.GaugeType = info.GaugeType;

            this.GaugeColor = green;
            this.IsSupported = true;
            this.IsInfinity = false;
            this.IsLoading = true;

            this.UpdateRemainingCount(info);
        }

        public void UpdateTransportCapacity()
        {
            this.UpdateRemainingCount(this._source, true);
        }


        private void UpdateRemainingCount(MapData info, bool useCache = false)
        {
            try
            {
                info.GetRemainingCount(useCache)
                    .ContinueWith(t => this.Update(t.Result, useCache));
            }
            catch (AggregateException e)
            {
                Debug.WriteLine(e);
            }
        }

        private void Update(RemainingCount remainingCount, bool useCache)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                if (!useCache)
                {
                    this.IsLoading = false;
                }
                this.IsSupported = remainingCount != null;
                if (!this.IsSupported)
                {
                    this.GaugeColor = red;
                    return;
                }

                this.RemainingCountMin = remainingCount.Min.ToString();
                this.RemainingCountMax = remainingCount.Max.ToString();
                this.RemainingCountTransportS = this._source.RemainingCountTransportS.ToString();
                this.IsInfinity = remainingCount == RemainingCount.MaxValue;
                this.GaugeColor = remainingCount.Min < 2 ? red : green;
            });
        }

        private void RaiseVisibilityChanged()
        {
            this.RaisePropertyChanged(nameof(this.IsCountVisibility));
            this.RaisePropertyChanged(nameof(this.IsUnSupportedVisibility));
            this.RaisePropertyChanged(nameof(this.IsInfinityVisibility));
        }
    }
}
