using MetroTrilithon.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace EventMapHpViewer.Models.Settings
{
    static class MapHpSettings
    {
        public static readonly string RoamingFilePath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "cat-ears.net", "MapHPViewer", "Settings.xaml");

        private static readonly ISerializationProvider roamingProvider = new FileSettingsProvider(RoamingFilePath);

        public static SerializableProperty<int> TransportCapacityS { get; }
            = new SerializableProperty<int>(GetKey(), roamingProvider) { AutoSave = true };

        public static SerializableProperty<string> BossSettings { get; }
            = new SerializableProperty<string>(GetKey(), roamingProvider) { AutoSave = true };

        public static SerializableProperty<bool> UseLocalBossSettings { get; }
            = new SerializableProperty<bool>(GetKey(), roamingProvider, false) { AutoSave = true };

        private static string GetKey([CallerMemberName] string propertyName = "")
            => $"{nameof(MapHpSettings)}.{propertyName}";
    }
}
