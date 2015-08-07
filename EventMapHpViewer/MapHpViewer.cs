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
    [ExportMetadata("Version", "2.3.0")]
    [ExportMetadata("Author", "@veigr")]
    public class MapHpViewer : IPlugin, ITool
    {
        private readonly ToolView v = new ToolView { DataContext = new ToolViewModel(new MapInfoProxy()) };

        public void Initialize() {}

        public string Name => "MapHP";

        public object View => this.v;
    }
}
