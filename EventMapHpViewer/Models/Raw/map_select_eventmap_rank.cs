namespace EventMapHpViewer.Models.Raw
{
    public class map_select_eventmap_rank
    {
        public Api_Maphp api_maphp { get; set; }
    }

    public class Api_Maphp
    {
        public int api_now_maphp { get; set; }
        public int api_max_maphp { get; set; }
        public string api_gauge_type { get; set; }
        public int api_gauge_num { get; set; }
    }

}
