using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using Fiddler;
using Grabacr07.KanColleWrapper;
using Grabacr07.KanColleWrapper.Models;
using Livet;
using System.Runtime.Serialization.Json;
using System.IO;

namespace EventMapHpViewer
{
    public class MapInfoProxy : NotificationObject
    {

        #region Maps変更通知プロパティ
        private MapInfo _Maps;

        public MapInfo Maps
        {
            get
            { return _Maps; }
            set
            { 
                if (_Maps == value)
                    return;
                _Maps = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        public MapInfoProxy()
        {
            KanColleClient.Current.Proxy.ApiSessionSource
                .Where(s => s.PathAndQuery.StartsWith("/kcsapi/api_get_member/mapinfo"))
                .TryParseMapInfo()
                .Subscribe(m => Maps = m);
        }
    }

    static class SessionExtensions
    {
        public static IObservable<MapInfo> TryParseMapInfo(this IObservable<Session> sessions)
        {
            return sessions.Select(s =>
                {
                    MapInfo mapinfo;
                    s.TryParseMapInfo(out mapinfo);
                    return mapinfo;
                })
                .Where(m => m != null);
        }

        public static MapInfo ParseMapInfo(this Session session)
        {
            var json = session.GetResponseBodyAsString().Replace("svdata=", "");
            var bytes = Encoding.UTF8.GetBytes(json);
            var serializer = new DataContractJsonSerializer(typeof(MapInfo));
            using (var stream = new MemoryStream(bytes))
            {
                return serializer.ReadObject(stream) as MapInfo;
            }
        }

        public static bool TryParseMapInfo(this Session session, out MapInfo result)
        {
            try
            {
                result = ParseMapInfo(session);
                return true;
            }
            catch (Exception)
            {
                result = null;
                return false;
            }
        }
    }
}
