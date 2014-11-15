using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Grabacr07.KanColleViewer.Composition;
using Grabacr07.KanColleWrapper;

namespace EventMapHpViewer
{
    [Export(typeof(IToolPlugin))]
    [ExportMetadata("Title", "EventMapHpViewer")]
    [ExportMetadata("Description", "Map HP を表示します。")]
    [ExportMetadata("Version", "1.0.0")]
    [ExportMetadata("Author", "@veigr")]
    public class EventMapHpViewer : IToolPlugin
    {
        private readonly ToolViewModel _vm = new ToolViewModel
        {
            MapInfoProxy = new MapInfoProxy()
        };

        public object GetToolView()
        {
            return new ToolView {DataContext = _vm};
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
