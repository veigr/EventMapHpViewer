using System;
using System.Collections.Generic;
using System.Linq;
using Grabacr07.KanColleWrapper;

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

            public Grabacr07.KanColleWrapper.Models.MapInfo Master
            {
                get { return KanColleClient.Current.Master.MapInfos[this.api_id]; }
            }

            public string MapNumber
            {
                get
                {
                    return this.Master.MapAreaId + "-" + this.Master.IdInEachMapArea;
                }
            }

            public string Name
            {
                get { return this.Master.Name; }
            }

            public string AreaName
            {
                get { return this.Master.MapArea.Name; }
            }

            public int Max
            {
                get
                {
                    if (this.api_exboss_flag == 1)
                        return this.Master.RequiredDefeatCount;
                    if (this.api_eventmap != null) return this.api_eventmap.api_max_maphp;
                    return 1;
                }
            }

            public int Current
            {
                get
                {
                    if (this.api_exboss_flag == 1)
                        return this.Master.RequiredDefeatCount - this.api_defeat_count;
                    if (this.api_eventmap != null) return this.api_eventmap.api_now_maphp;
                    return 0;
                }
            }

            public int RemainingCount
            {
                get
                {
                    if (this.api_exboss_flag == 1)
                    {
                        return this.Current;
                    }

                    if (this.api_eventmap == null) return -1;

                    var shipMaster = KanColleClient.Current.Master.Ships;
                    var lastBossHp = shipMaster[EventBossDictionary[this.api_eventmap.api_selected_rank][this.api_id].Last()].HP;
                    var normalBossHp = shipMaster[EventBossDictionary[this.api_eventmap.api_selected_rank][this.api_id].First()].HP;
                    if (this.Current <= lastBossHp) return 1;
                    return (int)Math.Ceiling((double) (this.Current - lastBossHp) / normalBossHp) + 1;
                }
            }
        }

        public class Api_Eventmap
        {
            public int api_now_maphp { get; set; }
            public int api_max_maphp { get; set; }
            public int api_state { get; set; }
            public int api_selected_rank { get; set; }
        }

        public static readonly IReadOnlyDictionary<int, IReadOnlyDictionary<int, int[]>> EventBossDictionary
            = new Dictionary<int, IReadOnlyDictionary<int, int[]>>
            {
                {   //難易度未選択
                    0, new Dictionary<int, int[]>
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
                        {284, new[] {599, 600}},
                    }
                },
                {   //丙
                    1, new Dictionary<int, int[]>
                    {
                        {291, new[] {570, 571}},
                        {292, new[] {528, 565}},
                        {293, new[] {601}},
                        {294, new[] {586}},
                        {295, new[] {603}},
                    }
                },
                {   //乙
                    2, new Dictionary<int, int[]>
                    {
                        {291, new[] {571}},
                        {292, new[] {528, 565}},
                        {293, new[] {601, 602}},
                        {294, new[] {586}},
                        {295, new[] {604}},
                    }
                },
                {    //甲
                    3, new Dictionary<int, int[]>
                    {
                        {291, new[] {571, 572}},
                        {292, new[] {579, 565}},
                        {293, new[] {602}},
                        {294, new[] {586}},
                        {295, new[] {604}},
                    }
                },
            };
    }
}
