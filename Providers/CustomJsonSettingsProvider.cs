using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using TekHow.Core.Types;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace TekHow.Core.Providers
{
    public class CustomJsonConfigurationProvider : JsonConfigurationProvider
    {
        public CustomJsonConfigurationProvider(CustomJsonConfigurationSource source) : base(source)
        {
            OriginalData = new Dictionary<string, string>();
        }

        public override void Load(Stream stream)
        {
            // Let the base class do the heavy lifting.
            base.Load(stream);

            foreach (var kvp in Data)
            {
                OriginalData.Add(kvp.Key, kvp.Value);
            }

            // Do decryption here, you can tap into the Data property like so:
            Data = Data?.ToDictionary(k => k.Key.ToUpperInvariant(), v => v.Value) as IDictionary<string, string>;

            if (Data == null) return;

            var toUpdate = new Dictionary<string, string>();

            foreach (var kvp in Data)
            {
                var keyAr = kvp.Key?.Split(':');
                if (keyAr == null) continue;
                if (keyAr.Length == 0) continue;

                var valueKey = string.Empty;
                if (keyAr.Length > 2)
                {
                    const string customSetting = "JOBSCUSTOMSETTINGS";
                    if (keyAr[0] == customSetting)
                    {
                        valueKey = $"{keyAr[1]}:{keyAr[2]}:VALUE";
                    }
                }
                if (kvp.Key == valueKey) continue;

                var isEncrypted = Data.FirstOrDefault(kp => kp.Key.ToUpper().EndsWith("ISENCRYPTED") & kp.Value?.ToUpper() == "TRUE").Key != null;

                //var isEncrypted = keyAr.Any(v => v == "ISENCRYPTED");
                if (isEncrypted & Data.ContainsKey(valueKey))
                {
                    var encVal = Data[valueKey];
                    var decVal = Cryptography.DecryptText(encVal);
                    toUpdate.Add(valueKey, decVal);
                }
            }

            //updates
            foreach (var kvp in toUpdate)
            {
                Data[kvp.Key] = kvp.Value;
            }



            //Data["abc:password"] = MyEncryptionLibrary.Decrypt(Data["abc:password"]);

            // But you have to make your own MyEncryptionLibrary, not included here
        }

        public Dictionary<string, string> OriginalData { get; }
    }
}