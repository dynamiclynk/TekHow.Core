using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using TekHow.Core.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TekHow.Core.Types
{
    public class CustomJsonConfigurationSource : JsonConfigurationSource
    {
        public override IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            EnsureDefaults(builder);
            return new CustomJsonConfigurationProvider(this);
        }
    }
}
