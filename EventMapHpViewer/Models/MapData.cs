using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Codeplex.Data;
using Grabacr07.KanColleWrapper;
using Nekoxy;

namespace EventMapHpViewer.Models
{
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

        public int? Max
        {
            get
            {
                if (this.IsExBoss == 1) return this.Master.RequiredDefeatCount;
                return this.Eventmap != null
                    ? this.Eventmap.MaxMapHp
                    : 1;
            }
        }

        public int? Current
        {
            get
            {
                if (this.IsExBoss == 1) return this.Master.RequiredDefeatCount - this.DefeatCount;  //ゲージ有り通常海域
                return this.Eventmap != null
                    ? this.Eventmap.NowMapHp   // イベント海域
                    : 1;    // ゲージ無し通常海域
            }
        }

        private Raw.map_exboss[] remoteBossDataCache;

        /// <summary>
        /// 残回数。輸送の場合はA勝利の残回数。
        /// </summary>
        public async Task<RemainingCount> GetRemainingCount(bool useCache = false)
        {
            if (this.IsCleared == 1) return RemainingCount.Zero;

            if (!this.Current.HasValue) return null;    //難易度切り替え直後

            if (this.IsExBoss == 1) return new RemainingCount(this.Current.Value);    //ゲージ有り通常海域

            if (this.Eventmap == null) return new RemainingCount(1);    //ゲージ無し通常海域

            if (this.Eventmap.GaugeType == GaugeType.Transport)
            {
                var capacityA = await KanColleClient.Current.Homeport.Organization.TransportationCapacity();
                if (capacityA == 0) return RemainingCount.MaxValue;  //ゲージ減らない
                return new RemainingCount((int)Math.Ceiling((double)this.Current / capacityA));
            }

            if (this.Eventmap.SelectedRank == 0) return null; //難易度未選択

            if (!useCache)
            {
                var client = new RemoteSettingsClient();
                this.remoteBossDataCache = await client.GetSettings<Raw.map_exboss[]>($"https://kctadil.azurewebsites.net/map/maphp/v3.2/{this.Id}/{this.Eventmap.SelectedRank}");
                client.CloseConnection();

                if (!this.remoteBossDataCache.Any(x => x.isLast)
                || !this.remoteBossDataCache.Any(x => !x.isLast))
                {
                    this.remoteBossDataCache = null;
                }
            }
            
            if (this.remoteBossDataCache != null && this.remoteBossDataCache.Any())
                return this.CalculateRemainingCount(this.remoteBossDataCache);   //イベント海域(リモートデータ)

            return null;  //未対応
        }

        private RemainingCount CalculateRemainingCount(Raw.map_exboss[] data)
        {
            return new RemainingCount(
                CalculateRemainingCount(
                    data.Where(x => !x.isLast).Max(x => x.ship.maxhp),
                    data.Where(x => x.isLast).Max(x => x.ship.maxhp)
                ),
                CalculateRemainingCount(
                    data.Where(x => !x.isLast).Min(x => x.ship.maxhp),
                    data.Where(x => x.isLast).Min(x => x.ship.maxhp)
                ));
        }

        private int CalculateRemainingCount(int normalBossHp, int lastBossHp)
        {
            if (this.Current <= lastBossHp) return 1;   //最後の1回
            return (int)Math.Ceiling((double)(this.Current - lastBossHp) / normalBossHp) + 1;
        }

        /// <summary>
        /// 輸送ゲージのS勝利時の残回数
        /// </summary>
        public async Task<int> GetRemainingCountTransportS()
        {
            if (this.Eventmap?.GaugeType != GaugeType.Transport) return -1;
            var capacity = await KanColleClient.Current.Homeport.Organization.TransportationCapacity(true);
            if (capacity == 0) return int.MaxValue;  //ゲージ減らない
            return (int)Math.Ceiling((double)this.Current / capacity);
        }
    }
}
