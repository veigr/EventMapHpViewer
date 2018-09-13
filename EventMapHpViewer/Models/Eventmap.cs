namespace EventMapHpViewer.Models
{
    public class Eventmap
    {
        public int? NowMapHp { get; set; }
        public int? MaxMapHp { get; set; }
        public int State { get; set; }
        public Rank SelectedRank { get; set; }
        public GaugeType GaugeType { get; set; }
        public int? GaugeNum { get; set; }
    }
}
