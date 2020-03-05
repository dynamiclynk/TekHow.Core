using Microsoft.Extensions.Configuration;
using TekHow.Core.Enums;
using TekHow.Core.Extensions;
using TekHow.Core.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TekHow.Core.Constants;

namespace TekHow.Core.Types
{
    /// <summary>
    /// Custom library to provide access to appsettings.json in any .Net project type.
    /// </summary>
    public class AppSettings
    {
        private IConfigurationRoot _configRoot = null;
        private ConfigurationBuilder _builder = null;
        /// <summary>
        /// Custom library to provide access to appsettings.json in any .Net project type.
        /// </summary>
        /// <param name="environmentName">Indicates to use appsettings.{environmentName}.json by default it uses the value from the ASPNETCORE_ENVIRONMENT environment parameter if an environmentName is not specified.</param>
        public AppSettings(string environmentName = "")
        {
            if (string.IsNullOrEmpty(environmentName.Trim()))
            {
                environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "";
            }

            _builder = new ConfigurationBuilder();

            string path = Path.Combine(Directory.GetCurrentDirectory(), "appsettings.json");
            if (!string.IsNullOrEmpty(environmentName))
            {
                path = Path.Combine(Directory.GetCurrentDirectory(), $"appsettings.{environmentName}.json");
            }

            if (!File.Exists(path))
            {
                path = Path.Combine(Directory.GetCurrentDirectory(), "appsettings.json");
            }

            if (File.Exists(path))
            {

                _builder.AddJsonFileCustom(path, optional: true, reloadOnChange: true);

                var cutomSource = (CustomJsonConfigurationSource)_builder.Sources.FirstOrDefault(v => v is CustomJsonConfigurationSource);

                if (cutomSource != null)
                {
                    var internalBuilder = new ConfigurationBuilder();
                    internalBuilder.Add(cutomSource);

                    _configRoot = internalBuilder.Build();
                }
            }
        }

        public ConfigurationBuilder GetBuilder()
        {
            return _builder;
        }

        public string GetConnectionString(string key)
        {
            return GetSettingValue<string>(AppSettingsSectionEnum.ConnectionStrings, key);
        }

        public T GetSettingValue<T>(string section, string key)
        {
            var sectionUpper = section.ToUpper();

            if (_configRoot == null) return default;

            var rootSection = _configRoot.GetSection(StringConstants.CUSTOM_SETTINGS);

            var value = rootSection?.GetSection($"{sectionUpper}:{key?.ToUpperInvariant()}:VALUE")?.Value;

            if (!string.IsNullOrEmpty(value))
            {
                return ConvertAny.Convert<T>(value);
            }

            return default;
        }

        public T GetSettingValue<T>(AppSettingsSectionEnum appSettingsSection, string key)
        {
            var sectionUpper = appSettingsSection.ToUpper();

            if (_configRoot == null) return default;

            var rootSection = _configRoot.GetSection(StringConstants.CUSTOM_SETTINGS);
            
            var value = rootSection?.GetSection($"{sectionUpper}:{key?.ToUpperInvariant()}:VALUE")?.Value;
            if (!string.IsNullOrEmpty(value))
            {
                return ConvertAny.Convert<T>(value);
            }


            return default;
        }
    }
}
