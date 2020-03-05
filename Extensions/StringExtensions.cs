using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TekHow.Core.Extensions
{
    public static class StringExtensions
    {
        public static string ToMd5String(this string inString)
        {
           try
            {
                var md5 = new MD5CryptoServiceProvider();
                var result = md5.ComputeHash(Encoding.UTF8.GetBytes(inString));

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

        public static string ToJsonString(this object inObject)
        {
            if (inObject == null) return null;
            return JsonConvert.SerializeObject(inObject);
        }

        public static bool IsValidJson(this string strInput)
        {
            strInput = strInput.Trim();
            if ((strInput.StartsWith("{") && strInput.EndsWith("}")) || //For object
                (strInput.StartsWith("[") && strInput.EndsWith("]"))) //For array
            {
                try
                {
                    var obj = JToken.Parse(strInput);
                    return true;
                }
#pragma warning disable 0168 
                catch (JsonReaderException jex)
                {
                    //Exception in parsing json
                    return false;
                }
                catch (Exception ex) //some other exception
                {
                    return false;
                }
#pragma warning restore 0168 
            }
            else
            {
                return false;
            }
        }
        public static string ToFormattedJson(this string json)
        {
            if (!json.IsValidJson()) { return json; }
            if (string.IsNullOrEmpty(json)) { return string.Empty; }

            var parsedJson = JsonConvert.DeserializeObject(json);
            return JsonConvert.SerializeObject(parsedJson, Formatting.Indented);
        }
        public static TReturn ToObjectFromJson<TReturn>(this string json)
        {
            return !json.IsValidJson() ? default(TReturn) : JsonConvert.DeserializeObject<TReturn>(json);
        }
        public static bool IsBase64String(this string inString)
        {
            return !string.IsNullOrEmpty(inString) && Regex.IsMatch(inString, @"^[a-zA-Z0-9\+/]*={0,2}$");
        }
        public static string ToBase64(this string inString, bool removePadding = false)
        {
            var base64 = string.IsNullOrEmpty(inString) ? string.Empty : Convert.ToBase64String(Encoding.UTF8.GetBytes(inString));
            return removePadding ? base64.Replace("=", "") : base64;
        }
        public static string FromBase64(this string encodedString)
        {
            var len = encodedString.Length;

            if (len >= 4)
            {
                var padMod = len % 4;
                if (padMod == 2)
                {
                    var str = new string('=', 2);
                    encodedString += str;
                }
                if (padMod == 3)
                {
                    var str = new string('=', 1);
                    encodedString += str;
                }
            }

            return string.IsNullOrEmpty(encodedString) ? string.Empty : Encoding.UTF8.GetString(Convert.FromBase64String(encodedString));
        }
        public static string ToSafeGuid(this string guid)
        {
            if (string.IsNullOrEmpty(guid)) { return string.Empty; }
            return guid.ToUpper().Replace("-", "").Replace("{", "").Replace("}", "");
        }
        public static Guid ToGuid(this string guid)
        {
            Guid.TryParse(guid, out var createdGuid);

            return createdGuid;
        }
        public static string ConcatString(this string[] stringArray, string delimiter = "\r\n")
        {
            var sb = new StringBuilder(stringArray.Length);
            var count = 0;
            foreach (var s in stringArray)
            {
                count++;
                sb.Append(count < stringArray.Length ? $"{s}{delimiter}" : $"{s}");
            }

            return sb.ToString();
        }
        public static string TrimAndRemoveWhitespace(this string input)
        {
            input = input.Trim();
            return new string(input.ToCharArray()
                .Where(c => !char.IsWhiteSpace(c))
                .ToArray());
        }
        public static string ConcatString(this IEnumerable<string> strings, string delimiter = "\r\n",
            bool preprendIndex = false)
        {
            var enumerable = strings as string[] ?? strings.ToArray();
            var sb = new StringBuilder(enumerable.Length);
            var index = 0;
            foreach (var s in enumerable)
            {
                var sVal = s;
                if (sVal == null)
                {
                    sVal = "";
                }
                sb.Append($"{(preprendIndex ? index.ToString() : "")}{sVal}{delimiter}");
                index++;
            }

            return sb.ToString();
        }
        public static string Str(this int number, int length = 0)
        {
            return length > 0 ? new string(' ', length - number.ToString().Length) + number.ToString() : number.ToString();
        }
  
        public static char ToChar(this string value)
        {
            char.TryParse(value, out var result);

            return result;
        }
        public static bool IsUpper(this string value)
        {
            // Consider string to be uppercase if it has no lowercase letters.
            foreach (var t in value)
            {
                if (char.IsLower(t))
                {
                    return false;
                }
            }

            return true;
        }

        public static bool IsLower(this string value)
        {
            // Consider string to be lowercase if it has no uppercase letters.
            foreach (var t in value)
            {
                if (char.IsUpper(t))
                {
                    return false;
                }
            }

            return true;
        }

        public static string[] RegexSplit(this string inString, string pattern, RegexOptions regexOpts)
        {
            return Regex.Split(inString, pattern, regexOpts);
        }

        public static bool ContainsX(this string inString, string search, bool ignoreCase = true)
        {
            return Regex.IsMatch(inString, search, ignoreCase ? RegexOptions.IgnoreCase : RegexOptions.None);
        }

        public static string LeftToRightSelect(this string inString, int count)
        {
            return inString.Length >= count ? inString.Substring(0, count) : inString;
        }

        public static string RightToLeftSelect(this string inString, int count)
        {
            return inString.Length >= count ?
                inString.Substring(inString.Length - count, count) : inString;
        }
    }
}
