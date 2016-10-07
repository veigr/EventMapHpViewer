namespace EventMapHpViewer.Models.Raw
{
    public class mapinfo
    {
        public member_mapinfo[] api_map_info { get; set; }
    }

    public class member_mapinfo
    {
        public int api_id { get; set; }
        public int api_cleared { get; set; }
        public int api_exboss_flag { get; set; }
        public int api_defeat_count { get; set; }
        public Api_Eventmap api_eventmap { get; set; }
    }
}
