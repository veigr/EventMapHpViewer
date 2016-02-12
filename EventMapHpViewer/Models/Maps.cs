using System;
using System.Collections.Generic;
using System.Linq;
using Grabacr07.KanColleWrapper;
using System.Threading.Tasks;
using System.Net.Http;
using Nekoxy;
using System.Net;
using Codeplex.Data;

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

        public GaugeType GaugeType => this.Eventmap?.GaugeType ?? GaugeType.Normal;

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
                if (this.IsExBoss == 1) return this.Master.RequiredDefeatCount - this.DefeatCount;  //ゲージ有り通常海域
                return this.Eventmap?.NowMapHp /*イベント海域*/?? 1 /*ゲージ無し通常海域*/;
            }
        }

        private int[] remoteBossHpCache;

        /// <summary>
        /// 残回数。輸送の場合はA勝利の残回数。
        /// </summary>
        public async Task<int> GetRemainingCount(bool useCache = false)
        {
            if (this.IsCleared == 1) return 0;

            if (this.IsExBoss == 1) return this.Current;    //ゲージ有り通常海域

            if (this.Eventmap == null) return 1;    //ゲージ無し通常海域

            if (this.Eventmap.GaugeType == GaugeType.Transport)
            {
                var capacityA = KanColleClient.Current.Homeport.Organization.TransportationCapacity();
                if (capacityA == 0) return int.MaxValue;  //ゲージ減らない
                return (int)Math.Ceiling((double)this.Current / capacityA);
            }

            if (this.Eventmap.SelectedRank == 0) return -1; //難易度未選択

            if (!useCache)
                this.remoteBossHpCache = await GetEventBossHp(this.Id, this.Eventmap.SelectedRank);

            var remoteBossHp = this.remoteBossHpCache;
            if (remoteBossHp != null && remoteBossHp.Any())
                return this.CalculateRemainingCount(remoteBossHp);   //イベント海域(リモートデータ)

            try
            {
                // リモートデータがない場合、ローカルデータを使う
                return this.CalculateRemainingCount(eventBossHpDictionary[this.Eventmap.SelectedRank][this.Id]);   //イベント海域
            }
            catch (KeyNotFoundException)
            {
                return -1;  //未対応
            }
        }

        private int CalculateRemainingCount(int[] bossHPs)
        {
            var lastBossHp = bossHPs.Last();
            var normalBossHp = bossHPs.First();
            if (this.Current <= lastBossHp) return 1;   //最後の1回
            return (int)Math.Ceiling((double)(this.Current - lastBossHp) / normalBossHp) + 1;
        }

        /// <summary>
        /// 輸送ゲージのS勝利時の残回数
        /// </summary>
        public int RemainingCountTransportS
        {
            get
            {
                if (this.Eventmap?.GaugeType != GaugeType.Transport) return -1;
                var capacity = KanColleClient.Current.Homeport.Organization.TransportationCapacity(true);
                if (capacity == 0) return int.MaxValue;  //ゲージ減らない
                return (int)Math.Ceiling((double)this.Current / capacity);
            }
        }

        /// <summary>
        /// 艦これ戦術データ・リンクからボス情報を取得する。
        /// 取得できなかった場合は null を返す。
        /// </summary>
        /// <param name="mapId"></param>
        /// <param name="rank"></param>
        /// <returns></returns>
        private static async Task<int[]> GetEventBossHp(int mapId, int rank)
        {
            using (var client = new HttpClient(GetProxyConfiguredHandler()))
            {
                try {
                    // rank の後ろの"1"はサーバー上手動メンテデータを加味するかどうかのフラグ
                    var response = await client.GetAsync($"https://kctadil.azurewebsites.net/map/exboss/{mapId}/{rank}/1");
                    if (!response.IsSuccessStatusCode)
                    {
                        // 200 じゃなかった
                        return null;
                    }

                    var json = await response.Content.ReadAsStringAsync();
                    Raw.map_exboss[] parsed = DynamicJson.Parse(json);
                    if (parsed == null
                    || !parsed.Any(x => x.isLast)
                    || !parsed.Any(x => !x.isLast))
                    {
                        // データが揃っていない
                        return null;
                    }
                    return parsed
                        .OrderBy(x => x.isLast) // 最終編成が後ろに来るようにする
                        .Select(x => x.ship.maxhp)
                        .ToArray();
                }
                catch (HttpRequestException)
                {
                    // HTTP リクエストに失敗した
                    return null;
                }
            }
        }

        /// <summary>
        /// 本体のプロキシ設定を組み込んだHttpClientHandlerを返す。
        /// </summary>
        /// <returns></returns>
        private static HttpClientHandler GetProxyConfiguredHandler()
        {
            switch (HttpProxy.UpstreamProxyConfig.Type)
            {
                case ProxyConfigType.DirectAccess:
                    return new HttpClientHandler
                    {
                        UseProxy = false
                    };
                case ProxyConfigType.SpecificProxy:
                    var settings = KanColleClient.Current.Proxy.UpstreamProxySettings;
                    var host = settings.IsUseHttpProxyForAllProtocols ? settings.HttpHost : settings.HttpsHost;
                    var port = settings.IsUseHttpProxyForAllProtocols ? settings.HttpPort : settings.HttpsPort;
                    if (string.IsNullOrWhiteSpace(host))
                    {
                        return new HttpClientHandler { UseProxy = false };
                    }
                    else
                    {
                        return new HttpClientHandler
                        {
                            UseProxy = true,
                            Proxy = new WebProxy($"{host}:{port}"),
                        };
                    }
                case ProxyConfigType.SystemProxy:
                    return new HttpClientHandler();
                default:
                    return new HttpClientHandler();
            }
        }

        /// <summary>
        /// 手動メンテデータ用。
        /// いずれ削除される見込み。
        /// </summary>
        private static readonly IReadOnlyDictionary<int, IReadOnlyDictionary<int, int[]>> eventBossHpDictionary
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
                        { 331, new[] { 110 } },
                        { 332, new[] { 600, 380 } },
                        { 333, new[] { 350, 370 } },
                    }
                },
                { //乙
                    2, new Dictionary<int, int[]>
                    {
                        { 331, new[] { 110, 130 } },
                        { 332, new[] { 600, 430 } },
                        { 333, new[] { 350, 380 } },
                    }
                },
                { //甲
                    3, new Dictionary<int, int[]>
                    {
                        { 331, new[] { 130, 160 } },
                        { 332, new[] { 600, 480 } },
                        { 333, new[] { 350, 390 } },
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
        public GaugeType GaugeType { get; set; }

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
