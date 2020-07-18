using Codeplex.Data;
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

        #region TpSettings

        public static SerializableProperty<decimal> TransportCapacityS { get; }
            = new SerializableProperty<decimal>(GetKey(), roamingProvider) { AutoSave = true };

        public static SerializableProperty<string> ShipTypeTpSettings { get; }
            = new SerializableProperty<string>(GetKey(), roamingProvider,
                DynamicJson.Serialize(AutoCalcTpSettings.Default.ShipTypeTp.ToArray()
                    )) { AutoSave = true };

        public static SerializableProperty<string> SlotItemTpSettings { get; }
            = new SerializableProperty<string>(GetKey(), roamingProvider,
                DynamicJson.Serialize(AutoCalcTpSettings.Default.SlotItemTp.ToArray()
                    )) { AutoSave = true };

        public static SerializableProperty<string> ShipTpSettings { get; }
            = new SerializableProperty<string>(GetKey(), roamingProvider,
                DynamicJson.Serialize(AutoCalcTpSettings.Default.ShipTp.ToArray()
                    )) { AutoSave = true };

        public static SerializableProperty<bool> UseAutoCalcTpSettings { get; }
            = new SerializableProperty<bool>(GetKey(), roamingProvider,
                true
                ) { AutoSave = true };

        #endregion

        #region BossHpSettings

        public static SerializableProperty<string> BossSettings { get; }
            = new SerializableProperty<string>(GetKey(), roamingProvider) { AutoSave = true };

        public static SerializableProperty<bool> UseLocalBossSettings { get; }
            = new SerializableProperty<bool>(GetKey(), roamingProvider,
                false
                ) { AutoSave = true };

        public static SerializableProperty<string> RemoteBossSettingsUrl { get; }
            = new SerializableProperty<string>(GetKey(), roamingProvider,
                Properties.Settings.Default.RemoteBossSettings
                ) { AutoSave = true };

        #endregion

        private static string GetKey([CallerMemberName] string propertyName = "")
            => $"{nameof(MapHpSettings)}.{propertyName}";
    }
}
