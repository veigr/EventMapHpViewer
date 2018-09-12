using Grabacr07.KanColleWrapper;
using Livet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MetroTrilithon.Mvvm;
using StatefulModel;

namespace EventMapHpViewer.ViewModels.Settings
{
    public class SettingsViewModel : ViewModel
    {
        public BossSettingsViewModel BossSettings { get; }
            = new BossSettingsViewModel();
        public TpSettingsViewModel TpSettings { get; }
            = new TpSettingsViewModel();

        public SettingsViewModel()
        {
            KanColleClient.Current.Subscribe(nameof(KanColleClient.Current.IsStarted), () =>
            DispatcherHelper.UIDispatcher.Invoke(() =>
            {
                this.Initialize();
            })
            , false)
            .AddTo(this);
        }

        private void Initialize()
        {
            this.BossSettings.MapItemsSource
                = Models.Maps.MapInfos
                .Where(x => 20 < x.Value.MapAreaId)
                .Select(x => x.Value)
                .Select(x => new KeyValuePair<int, string>(x.Id, $"{x.MapAreaId}-{x.IdInEachMapArea} : {x.Name} - {x.OperationName}"))
                .ToArray();
            this.BossSettings.IsEnabled = true;

            this.TpSettings.Settings.UpdateFromMaster();
            this.TpSettings.IsEnabled = true;
        }
    }
}
