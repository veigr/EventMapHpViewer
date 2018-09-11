using Livet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventMapHpViewer.ViewModels.Settings
{
    public class SettingsViewModel : ViewModel
    {
        public BossSettingsViewModel BossSettings { get; }
            = new BossSettingsViewModel();
        public TpSettingsViewModel TpSettings { get; }
            = new TpSettingsViewModel();
    }
}
