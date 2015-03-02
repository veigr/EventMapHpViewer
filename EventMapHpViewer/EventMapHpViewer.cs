using System.ComponentModel.Composition;
using Grabacr07.KanColleViewer.Composition;

namespace EventMapHpViewer
{
    [Export(typeof(IToolPlugin))]
    [ExportMetadata("Title", "MapHPViewer")]
    [ExportMetadata("Description", "Map HPを表示します。")]
    [ExportMetadata("Version", "2.0.0")]
    [ExportMetadata("Author", "@veigr")]
    public class EventMapHpViewer : IToolPlugin
    {
        private readonly ToolViewModel _vm = new ToolViewModel(new MapInfoProxy());

        public object GetToolView()
        {
            return new ToolView {DataContext = this._vm};
        }

        public string ToolName
        {
            get { return "MapHP"; }
        }

        public object GetSettingsView()
        {
            return null;
        }
    }
}
