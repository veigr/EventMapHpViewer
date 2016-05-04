namespace EventMapHpViewer.Models
{
    public class Eventmap
    {
        public int? NowMapHp { get; set; }
        public int? MaxMapHp { get; set; }
        public int State { get; set; }
        public int SelectedRank { get; set; }
        public GaugeType GaugeType { get; set; }

        public string SelectedRankText
        {
            get
            {
                switch (this.SelectedRank)
                {
                    case 1:
                        return "•¸";
                    case 2:
                        return "‰³";
                    case 3:
                        return "b";
                    default:
                        return "";
                }
            }
        }
    }
}
