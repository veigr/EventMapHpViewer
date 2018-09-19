using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Codeplex.Data;
using EventMapHpViewer.Models.Settings;
using Grabacr07.KanColleWrapper;
using Nekoxy;

namespace EventMapHpViewer.Models
{
    public class MapData
    {
        private readonly RemoteSettingsClient client = new RemoteSettingsClient();
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

        /// <summary>
        /// 残回数。輸送の場合はA勝利の残回数。
        /// </summary>
        public async Task<RemainingCount> GetRemainingCount()
        {
            if (this.IsCleared == 1) return RemainingCount.Zero;

            if (!this.Current.HasValue) return null;    //難易度切り替え直後

            if (this.IsExBoss == 1) return new RemainingCount(this.Current.Value);    //ゲージ有り通常海域

            if (this.Eventmap == null) return new RemainingCount(1);    //ゲージ無し通常海域

            if (this.Eventmap.GaugeType == GaugeType.Transport)
            {
                var capacity = KanColleClient.Current.Homeport.Organization.TransportationCapacity();
                if (capacity.A == 0) return RemainingCount.MaxValue;  //ゲージ減らない
                return new RemainingCount((int)Math.Ceiling((decimal)this.Current / capacity.A));
            }

            if (this.Eventmap.SelectedRank == 0) return null; //難易度未選択

            if (MapHpSettings.UseLocalBossSettings)
            {
                var settings = BossSettingsWrapper.FromSettings.List
                    .Where(x => x.MapId == this.Id)
                    .Where(x => x.Rank == (int)this.Eventmap.SelectedRank)
                    .Where(x => x.GaugeNum == this.Eventmap.GaugeNum)
                    .ToArray();
                if (settings.Any())
                    return this.CalculateRemainingCount(settings);
            }
            else
            {
                var remoteBossData = await client.GetSettings<Raw.map_exboss[]>(
                    RemoteSettingsClient.BuildBossSettingsUrl(
                        MapHpSettings.RemoteBossSettingsUrl,
                        this.Id,
                        (int)this.Eventmap.SelectedRank,
                        this.Eventmap.GaugeNum ?? 0));  // GaugeNum がない場合 0 とみなす(リモート設定は空にしても 0 になるので)
                client.CloseConnection();

                if (remoteBossData == null)
                    return null;

                if (!remoteBossData.Any(x => x.last))
                    return null;

                if(!remoteBossData.Any(x => !x.last))
                    return null;

                return this.CalculateRemainingCount(BossSettingsWrapper.Parse(remoteBossData));   //イベント海域(リモートデータ)
            }

            return null;  //未対応
        }

        private RemainingCount CalculateRemainingCount(IEnumerable<BossSetting> settings)
        {
            var normals = settings.Where(x => !x.IsLast);
            var lasts = settings.Where(x => x.IsLast);
            if (!normals.Any() || !lasts.Any())
                return null;
            return new RemainingCount(
                CalculateRemainingCount(
                    normals.Max(x => x.BossHP),
                    lasts.Max(x => x.BossHP)
                ),
                CalculateRemainingCount(
                    normals.Min(x => x.BossHP),
                    lasts.Min(x => x.BossHP)
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
        public int GetRemainingCountTransportS()
        {
            if (this.Eventmap?.GaugeType != GaugeType.Transport) return -1;

            var capacity = KanColleClient.Current.Homeport.Organization.TransportationCapacity();
            if (capacity.S == 0) return int.MaxValue;  //ゲージ減らない
            return (int)Math.Ceiling((decimal)this.Current / capacity.S);
        }
    }
}
