using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Controls;
using EventMapHpViewer.Models.Settings;

namespace EventMapHpViewer.Views.Controls
{
    public class BossSettingsUrlRule : ValidationRule
    {
        public bool AllowsEmpty { get; set; }

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            var urlAsString = value as string;
            if (string.IsNullOrEmpty(urlAsString))
            {
                return this.AllowsEmpty
                    ? new ValidationResult(true, null)
                    : new ValidationResult(false, "値を入力してください。");
            }

            var url = RemoteSettingsClient.BuildBossSettingsUrl(urlAsString, 0, 0, 0);

            if(Uri.TryCreate(url, UriKind.Absolute, out var _))
            {
                return new ValidationResult(true, null);
            }
            else
            {
                return new ValidationResult(false, "URL 形式を入力して下さい。");
            }
        }
    }
}
