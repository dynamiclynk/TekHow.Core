using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace TekHow.Core.Extensions
{
    public static class ObjectExtensions
    {
        public static object GetDefault(this Type type)
        {
            return type.IsValueType ? Activator.CreateInstance(type) : null;
        }

        public static bool IsNullEquivalent(this object value)
        {
            return value == null
                   || value is DBNull
                   || string.IsNullOrWhiteSpace(value.ToString());
        }
        public static bool IsDateTime(this object inObj)
        {
            if (inObj == null) return false;
            switch (Type.GetTypeCode(inObj.GetType()))
            {
                case TypeCode.DateTime:
                    {
                        return true;
                    }
                default:
                    return false;
            }
        }

        public static bool IsNumeric(this object inObj)
        {
            if (inObj == null) return false;
            switch (Type.GetTypeCode(inObj.GetType()))
            {
                case TypeCode.Byte:
                case TypeCode.SByte:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.Decimal:
                case TypeCode.Double:
                case TypeCode.Single:
                    return true;
                default:
                    return false;
            }
        }
        public static string ToHash(this object sourceObject)
        {
            //Catch unuseful parameter values
            if (sourceObject == null)
            {
                throw new ArgumentNullException(nameof(sourceObject));
            }
            else
            {
                //We determine if the passed object is really serializable.
                try
                {
                    //Now we begin to do the real work.
                    var hashString = ComputeHash(sourceObject.ToByteArray());
                    return hashString;
                }
                catch (AmbiguousMatchException ame)
                {
                    throw new ApplicationException("Could not definitely decide if object is serializable.Message:" +
                                                   ame.Message);
                }
            }
        }
        public static bool JsonCompare<T1, T2>(this T2 inObj, T2 compareObj)
        {
            if (ReferenceEquals(inObj, compareObj)) return true;
            //var inObjType = inObj.GetType();
            //var compareType = compareObj.GetType();
            var objJson = JsonConvert.SerializeObject(inObj);
            var anotherJson = JsonConvert.SerializeObject(compareObj);

            return objJson.ToMd5String() == anotherJson.ToMd5String();
        }
        public static bool ToBool(this object data, bool? defaultValue = null)
        {
            if (bool.TryParse(ObjectToString(data), out var val))
            {
                return val;
            }
            else
            {
                return defaultValue.GetValueOrDefault(false);
            }
        }
        public static int ToInt(this object data, int? defaultValue = null)
        {
            if (data is decimal dData)
            {
                return (int)dData;
            }
            if (int.TryParse(ObjectToString(data), out var val))
            {
                return val;
            }
            return defaultValue.GetValueOrDefault(0);
        }
        public static uint ToUInt(this object data, uint? defaultValue = null)
        {
            if (data is uint dData)
            {
                return dData;
            }
            if (uint.TryParse(ObjectToString(data), out var val))
            {
                return val;
            }

            return defaultValue.GetValueOrDefault(0);
        }

        public static double ToDouble(this object data, double? defaultValue = null)
        {
            if (data is double dData)
            {
                return dData;
            }
            if (double.TryParse(ObjectToString(data), out var val))
            {
                return val;
            }

            return defaultValue.GetValueOrDefault(0);
        }
        public static short ToShort(this object data, short? defaultValue = null)
        {
            if (data is short dData)
            {
                return dData;
            }
            if (short.TryParse(ObjectToString(data), out var val))
            {
                return val;
            }

            return defaultValue.GetValueOrDefault(0);
        }
        public static ushort ToUShort(this object data, ushort? defaultValue = null)
        {
            if (data is ushort dData)
            {
                return dData;
            }
            if (ushort.TryParse(ObjectToString(data), out var val))
            {
                return val;
            }

            return defaultValue.GetValueOrDefault(0);
        }
        public static byte ToByte(this object data, byte? defaultValue = null)
        {
            if (data is byte dData)
            {
                return dData;
            }
            if (byte.TryParse(ObjectToString(data), out var val))
            {
                return val;
            }

            return defaultValue.GetValueOrDefault(0);
        }

        public static long ToLong(this object data, long? defaultValue = null)
        {
            if (data is long dData)
            {
                return dData;
            }
            if (long.TryParse(ObjectToString(data), out var val))
            {
                return val;
            }
            return defaultValue.GetValueOrDefault(0);
        }
        public static ulong ToULong(this object data, ulong? defaultValue = null)
        {
            if (data is ulong dData)
            {
                return dData;
            }
            if (ulong.TryParse(ObjectToString(data), out var val))
            {
                return val;
            }
            return defaultValue.GetValueOrDefault(0);
        }
        public static decimal ToDecimal(this object data, decimal? defaultValue = null)
        {
            if (data is decimal dData)
            {
                return dData;
            }
            if (decimal.TryParse(ObjectToString(data), out var val))
            {
                return val;
            }
            return defaultValue.GetValueOrDefault(0);
        }

        public static DateTime ToDateTime(this object data, DateTime? defaultValue = null)
        {
            if (DateTime.TryParse(ObjectToString(data), out var result))
            {
                return result;
            }
            return defaultValue.GetValueOrDefault(DateTime.MinValue);
        }
        public static string ToLower(this object data)
        {
            return ObjectToString(data).ToLowerInvariant();
        }
        public static string ToUpper(this object data)
        {
            return ObjectToString(data).ToUpperInvariant();
        }

        public static string ToStringTrim(this object data)
        {
            return ObjectToString(data);
        }

        public static bool IsString(this object data)
        {
            return data is string;
        }
        public static bool IsNullOrEmptyString(this object data)
        {
            return string.IsNullOrEmpty(ObjectToString(data));
        }
        public static bool IsBetween<T>(this T item, T start, T end) where T : IComparable, IComparable<T>
        {
            return Comparer<T>.Default.Compare(item, start) >= 0
                   && Comparer<T>.Default.Compare(item, end) <= 0;
        }

        public static JObject ToJObject(this object obj)
        {
            return JObject.FromObject(obj);
        }

        #region private methods

        private static string ObjectToString(object data)
        {
            var result = string.Empty;
            try
            {
                result = Convert.ToString(data);
            }
            catch { }
            return result;
        }
        private static string ComputeHash(byte[] objectAsBytes)
        {
            var md5 = new MD5CryptoServiceProvider();
            try
            {
                var result = md5.ComputeHash(objectAsBytes);

                // Build the final string by converting each byte
                // into hex and appending it to a StringBuilder
                var sb = new StringBuilder();
                foreach (var t in result)
                {
                    sb.Append(t.ToString("X2"));
                }

                // And return it
                return sb.ToString();
            }
            catch (ArgumentNullException)
            {
                return null;
            }
        }



        #endregion
    }
}
