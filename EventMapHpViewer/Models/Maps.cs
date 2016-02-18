using Grabacr07.KanColleWrapper;
using System.Reflection;

namespace EventMapHpViewer.Models
{
    public class Maps
    {
        public MapData[] MapList { get; set; }
        public static MasterTable<MapArea> MapAreas { get; set; }
        public static MasterTable<MapInfo> MapInfos { get; set; }
    }
}
