using System;
using System.Collections.Generic;
using System.Linq;
using Grabacr07.KanColleWrapper;

namespace EventMapHpViewer.Models
{
    public class Maps
    {
        public MapData[] MapList { get; set; }
        public static MasterTable<MapArea> MapAreas { get; set; }
        public static MasterTable<MapInfo> MapInfos { get; set; }
    }

    public class MapData
    {
        public int Id { get; set; }
        public int IsCleared { get; set; }
        public int IsExBoss { get; set; }
        public int DefeatCount { get; set; }
        public Eventmap Eventmap { get; set; }

        public MapInfo Master => Maps.MapInfos[this.Id];

        public string MapNumber => this.Master.MapAreaId + "-" + this.Master.IdInEachMapArea;

        public string Name => this.Master.Name;

        public string AreaName => this.Master.MapArea.Name;

        public int Max
        {
            get
            {
                if (this.IsExBoss == 1) return this.Master.RequiredDefeatCount;
                return this.Eventmap?.MaxMapHp ?? 1;
            }
        }

        public int Current
        {
            get
            {
                if (this.IsExBoss == 1)　return this.Master.RequiredDefeatCount - this.DefeatCount;  //ゲージ有り通常海域
                return this.Eventmap?.NowMapHp /*イベント海域*/?? 1 /*ゲージ無し通常海域*/;
            }
        }

        public int RemainingCount
        {
            get
            {
                if (this.IsExBoss == 1)
                {
                    return this.Current;    //ゲージ有り通常海域
                }

                if (this.Eventmap == null) return 1;    //ゲージ無し通常海域

                try
                {
                    var lastBossHp = EventBossHpDictionary[this.Eventmap.SelectedRank][this.Id].Last();
                    var normalBossHp = EventBossHpDictionary[this.Eventmap.SelectedRank][this.Id].First();
                    if (this.Current <= lastBossHp) return 1;   //最後の1回
                    return (int)Math.Ceiling((double)(this.Current - lastBossHp) / normalBossHp) + 1;   //イベント海域
                }
                catch (KeyNotFoundException)
                {
                    return -1;  //未対応
                }
            }
        }

        public static readonly IReadOnlyDictionary<int, IReadOnlyDictionary<int, int[]>> EventBossHpDictionary
            = new Dictionary<int, IReadOnlyDictionary<int, int[]>>
            {
                { //難易度未選択
                    0, new Dictionary<int, int[]>
                    {
                    }
                },
                { //丙
                    1, new Dictionary<int, int[]>
                    {
                        { 311, new[] { 150 } },
                        { 312, new[] { 210 } },
                        { 313, new[] { 350 } },
                        { 314, new[] { 500 } },
                        { 315, new[] { 400 } },
                        { 316, new[] { 350 } },
                        { 317, new[] { 255 } },
                    }
                },
                { //乙
                    2, new Dictionary<int, int[]>
                    {
                        { 311, new[] { 150, 190 } },
                        { 312, new[] { 210 } },
                        { 313, new[] { 350 } },
                        { 314, new[] { 500 } },
                        { 315, new[] { 400 } },
                        { 316, new[] { 350 } },
                        { 317, new[] { 255 } },
                    }
                },
                { //甲
                    3, new Dictionary<int, int[]>
                    {
                        { 311, new[] { 150, 190 } },
                        { 312, new[] { 270 } },
                        { 313, new[] { 350 } },
                        { 314, new[] { 500 } },
                        { 315, new[] { 400 } },
                        { 316, new[] { 350 } },
                        { 317, new[] { 255 } },
                    }
                },
            };
    }

    public class Eventmap
    {
        public int NowMapHp { get; set; }
        public int MaxMapHp { get; set; }
        public int State { get; set; }
        public int SelectedRank { get; set; }

        public string SelectedRankText
        {
            get
            {
                switch (this.SelectedRank)
                {
                    case 1:
                        return "丙";
                    case 2:
                        return "乙";
                    case 3:
                        return "甲";
                    default:
                        return "";
                }
            }
        }
    }
}
