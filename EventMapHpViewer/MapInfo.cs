using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grabacr07.KanColleWrapper.Models;

namespace EventMapHpViewer
{
    public class MapInfo
    {
        public int api_result { get; set; }
        public string api_result_msg { get; set; }
        public Api_Data[] api_data { get; set; }

        public class Api_Data
        {
            public int api_id { get; set; }
            public int api_cleared { get; set; }
            public int api_exboss_flag { get; set; }
            public int api_defeat_count { get; set; }
            public Api_Eventmap api_eventmap { get; set; }
        }

        public class Api_Eventmap
        {
            public int api_now_maphp { get; set; }
            public int api_max_maphp { get; set; }
            public int api_state { get; set; }
        }

        public static readonly IDictionary<int, int[]> EventBossDictionary = new Dictionary<int, int[]>
        {
            {271, new[] {566}},
            {272, new[] {581, 582}},
            {273, new[] {585}},
            {274, new[] {583, 584}},
            {275, new[] {586}},
            {276, new[] {557}},
            {281, new[] {595}},
            {282, new[] {597, 598}},
            {283, new[] {557}},
            {284, new[] {599, 600}}
        };
    };
}
