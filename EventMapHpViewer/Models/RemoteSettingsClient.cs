using Codeplex.Data;
using Grabacr07.KanColleWrapper;
using Nekoxy;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EventMapHpViewer.Models
{
    class RemoteSettingsClient
    {
        private HttpClient client;

        private ConcurrentDictionary<string, DateTimeOffset> lastModified;

        private ConcurrentDictionary<string, object> caches;

        private TimeSpan cacheTtl;

        private bool updating;

#if DEBUG
        public RemoteSettingsClient() : this(TimeSpan.FromSeconds(10)) { }
#else
        public RemoteSettingsClient() : this(TimeSpan.FromHours(1)) { }
#endif

        public RemoteSettingsClient(TimeSpan cacheTtl)
        {
            this.lastModified = new ConcurrentDictionary<string, DateTimeOffset>();
            this.caches = new ConcurrentDictionary<string, object>();
            this.cacheTtl = cacheTtl;
        }

        /// <summary>
        /// 艦これ戦術データ・リンクから設定情報を取得する。
        /// 取得できなかった場合は null を返す。
        /// </summary>
        /// <param name="mapId"></param>
        /// <param name="rank"></param>
        /// <returns></returns>
        public async Task<T> GetSettings<T>(string url)
            where T : class
        {
            DateTimeOffset lm;
            lock (this.lastModified)
            {
                while (this.updating)
                {
                    Thread.Sleep(100);
                }

                lm = this.lastModified.GetOrAdd(url, DateTimeOffset.MinValue);

                if (DateTimeOffset.Now - lm < this.cacheTtl)
                {
                    object value;
                    if (this.caches.TryGetValue(url, out value)
                    && value is T)
                    {
                        return (T)value;
                    }
                }
                this.updating = true;
            }

            if (this.client == null)
            {
                this.client = new HttpClient(GetProxyConfiguredHandler());
                this.client.DefaultRequestHeaders
                    .TryAddWithoutValidation("User-Agent", $"{MapHpViewer.title}/{MapHpViewer.version}");
            }
            try
            {
                var response = await client.GetAsync(url);
                if (!response.IsSuccessStatusCode)
                {
                    // 200 じゃなかった
                    return null;
                }

                var json = await response.Content.ReadAsStringAsync();
                T parsed = DynamicJson.Parse(json);
                this.lastModified.TryUpdate(url, DateTimeOffset.Now, lm);
                this.caches.AddOrUpdate(url, parsed, (_, __) => parsed);
                return parsed;
            }
            catch (HttpRequestException)
            {
                // HTTP リクエストに失敗した
                return null;
            }
            finally
            {
                this.updating = false;
            }
        }

        public void CloseConnection()
        {
            this.client?.Dispose();
            this.client = null;
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
    }
}
