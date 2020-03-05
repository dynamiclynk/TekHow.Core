using System;
using System.Collections.Generic;
using System.Text;

namespace TekHow.Core.Extensions
{
    public static class EnumExtensions
    {
        public static int ToInt(this Enum @enum)
        {
            object val = Convert.ChangeType(@enum, @enum.GetTypeCode());

            return (int)val;
        }
    }
}
