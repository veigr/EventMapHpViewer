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
                        return "丁";
                    case 2:
                        return "丙";
                    case 3:
                        return "乙";
                    case 4:
                        return "甲";
                    default:
                        return "";
                }
            }
        }
    }
}
