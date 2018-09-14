using System.ComponentModel.Composition;
using EventMapHpViewer.Models;
using EventMapHpViewer.ViewModels;
using EventMapHpViewer.ViewModels.Settings;
using EventMapHpViewer.Views;
using Grabacr07.KanColleViewer.Composition;

namespace EventMapHpViewer
{
    [Export(typeof(IPlugin))]
    [Export(typeof(ITool))]
    [Export(typeof(ISettings))]
    [ExportMetadata("Guid", "101436F4-9308-4892-A88A-19EFBDF2ED5F")]
    [ExportMetadata("Title", title)]
    [ExportMetadata("Description", "Map HPを表示します。")]
    [ExportMetadata("Version", version)]
    [ExportMetadata("Author", "@veigr")]
    public class MapHpViewer : IPlugin, ITool, ISettings
    {
        internal const string title = "MapHPViewer";
        internal const string version = "3.4.0";
        private ToolViewModel toolVm;
        private SettingsViewModel settingsVm;

        public void Initialize()
        {
            this.toolVm = new ToolViewModel(new MapInfoProxy());
            this.settingsVm = new SettingsViewModel();
        }

        public string Name => "MapHP";

        // タブ表示するたびに new されてしまうが、今のところ new しないとマルチウィンドウで正常に表示されない
        object ITool.View => new ToolView { DataContext = this.toolVm };

        private SettingsView settingsViewCache;
        object ISettings.View
        {
            get
            {
                // なぜかViewを使い回さずVMだけ使い回し、Viewを作り直すとUseAutoCalcTpSettingsのRadioButtonでStackOverFlowが発生する。
                if (settingsViewCache == null)
                    this.settingsViewCache = new SettingsView { DataContext = this.settingsVm };
                return this.settingsViewCache;
            }
        }
        
    }
}
