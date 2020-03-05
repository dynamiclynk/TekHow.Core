using Microsoft.Extensions.Configuration;
using TekHow.Core.Enums;
using TekHow.Core.Helpers;
using TekHow.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TekHow.Core.Constants;

namespace TekHow.Core.Extensions
{
    public static class JsonConfigurationExtensions
    {
        public static IConfigurationBuilder AddJsonFileCustom(this IConfigurationBuilder builder, string path, bool optional,
            bool reloadOnChange)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }
            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentException("File path must be a non-empty string.");
            }

            var source = new CustomJsonConfigurationSource
            {
                FileProvider = null,
                Path = path,
                Optional = optional,
                ReloadOnChange = reloadOnChange
            };

            source.ResolveFileProvider();
            builder.Add(source);
            return builder;
        }

        public static string GeCustomConnectionString(this IConfigurationRoot root, string key)
        {
            return GetCustomSettingValue<string>(root,"connectionStrings",key);
        }

        public static T GetCustomSettingValue<T>(this IConfigurationRoot root, string section, string key)
        {
            section = section.ToUpperInvariant();
            var keyPattern = $"{StringConstants.CUSTOM_SETTINGS}:{section}:{key?.ToUpperInvariant()}:VALUE";
            var value = root.GetSection(keyPattern)?.Value;
            var typedValue = ConvertAny.Convert<T>(value);

            return typedValue;
        }

        public static T GetSettingValue<T>(this IConfigurationRoot root, AppSettingsSectionEnum appSettingsSection, string key)
        {
            var section = appSettingsSection.ToJsonString().ToUpperInvariant();
            var keyPattern = $"{StringConstants.CUSTOM_SETTINGS}:{section}:{key?.ToUpperInvariant()}:VALUE";
            var value = root.GetSection(keyPattern)?.Value;
            var typedValue = ConvertAny.Convert<T>(value);

            return typedValue;
        }
    }
}
