using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TekHow.Core.Extensions
{
    public static class GuidExtensions
    {
        public static bool IsGuid(this string inString)
        {
            return Guid.TryParse(inString, out var resultGuid);
        }

        public static bool IsEmpty(this Guid guid)
        {
            return guid.Equals(Guid.Empty);
        }
    }
}
