using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace Core.Shared {
    public class Utilities {

        public static bool isAlphaNumeric(string str) {
            if (string.IsNullOrEmpty(str))
                return false;
            Regex r = new Regex("^(?=.+\\d)(?=.+[a-zA-Z]).*$");
            return r.IsMatch(str);
        }
        public static bool isMD5(string str) {
            if (string.IsNullOrEmpty(str))
                return false;
            Regex r = new Regex("^[a-f0-9]{32}$");
            return r.IsMatch(str);
        }
        public static bool isHexStr(string str) {
            if (string.IsNullOrEmpty(str))
                return false;
            Regex r = new Regex("^[a-fA-F0-9]+$");
            return r.IsMatch(str);
        }
        public static bool passwordCase(string str) {
            if (string.IsNullOrEmpty(str))
                return false;
            Regex r = new Regex("^(?=.*\\d)(?=.*[A-Z])(?=.*[_.@$!#%^&()_+=]).*$");
            return r.IsMatch(str);
        }

        public static bool isPhone(string phone) {
            if (string.IsNullOrEmpty(phone))
                return false;
            Regex r = new Regex(@"^[07981]{3}[0-9]{8}|234[7981]{2}[0-9]{8}$");
            return r.IsMatch(phone);
        }

        public static long getTimeStamp(
                        int year, int month, int day,
                        int hour, int minute, int second, int milliseconds) {
            DateTime value = new DateTime(year, month, day);
            var date = new DateTime(1970, 1, 1, 0, 0, 0, value.Kind);
            var unixTimestamp = System.Convert.ToInt64((value - date).TotalSeconds);
            return unixTimestamp;
        }
        public static (DateTime modernDate, long unixTimestamp, long unixTimestampMS) getTodayDate() {
            long totalSeconds = (long)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
            long totalmSecs = (long)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalMilliseconds;
            System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(totalSeconds).ToLocalTime();
            return (modernDate: dtDateTime, unixTimestamp: totalSeconds, unixTimestampMS: totalmSecs);
        }
        public static DateTime unixTimeStampToDateTime(double unixTimeStamp, bool dateOnly = false) {
            try {
                System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
                dtDateTime = dtDateTime.AddSeconds(unixTimeStamp).ToLocalTime();
                if (!dateOnly)
                    return dtDateTime;
                return dtDateTime.Date;
            } catch (Exception) {
                return new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            }
        }
        
        public static JObject asJObject(object obj) {
            JObject json = JObject.Parse(JsonConvert.SerializeObject(obj));
            return json;
        }
        public static string findString(JObject json, string needle) {
            try {
                return json[needle].ToString();
            } catch {
                return null;
            }
        }
        public static JArray findArray(JObject json, string needle) {
            try {
                return JArray.Parse(json[needle].ToString());
            } catch {
                return null;
            }
        }
        public static JObject findObj(JObject json, string needle) {
            try {
                return JObject.Parse(json[needle].ToString());
            } catch {
                return null;
            }
        }
        public static double? findNumber(JObject json, string needle) {
            try {
                return double.Parse(json[needle].ToString());
            } catch {
                return null;
            }
        }

        public static string validBase64(string raw) {
            int pads = raw.Length % 4;
            if (pads > 0) {
                raw += new string('=', 4 - pads);
            }
            return raw;
        }

        public static bool noNullValue(object obj) {            
            foreach (PropertyInfo prop in obj.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance)) {
                var data = prop.GetValue(obj);
                if (data == null)
                    return false;
            }
            return true;
        }

        public static bool noNullValue(object obj, List<string> properties) {
            try {
                foreach (string propertyName in properties) {
                    var data = obj.GetType().GetProperty(propertyName).GetValue(obj, null);
                    if (data == null)
                        return false;
                }
                return true;
            } catch {
                return false;
            }
        }
    }
}
