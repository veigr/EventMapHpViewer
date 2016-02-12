using System.ComponentModel.Composition;
using EventMapHpViewer.Models;
using EventMapHpViewer.ViewModels;
using Grabacr07.KanColleViewer.Composition;

namespace EventMapHpViewer
{
    [Export(typeof(IPlugin))]
    [Export(typeof(ITool))]
    [ExportMetadata("Guid", "101436F4-9308-4892-A88A-19EFBDF2ED5F")]
    [ExportMetadata("Title", "MapHPViewer")]
    [ExportMetadata("Description", "Map HPを表示します。")]
    [ExportMetadata("Version", "3.0.3")]
    [ExportMetadata("Author", "@veigr")]
    public class MapHpViewer : IPlugin, ITool
    {
        private ToolViewModel vm;

        public void Initialize()
        {
            this.vm = new ToolViewModel(new MapInfoProxy());
        }

        public string Name => "MapHP";

        // タブ表示するたびに new されてしまうが、今のところ new しないとマルチウィンドウで正常に表示されない
        public object View => new ToolView { DataContext = this.vm };
    }
}
