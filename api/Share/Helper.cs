using System.Globalization;
using api.Models;

namespace api.Share {
    public static class Helper {

        public static bool IsNullOrEmpty(string str) {
            return string.IsNullOrEmpty(str);
        }

        public static string Truncate(string str, int length) {
            return str.Length > length ? str.Substring(0, length) : str;
        }

        public static bool IsBlank(string str) {
            return string.IsNullOrWhiteSpace(str);
        }


        public static string ToSlug(string str) {
            return str.Replace(" ", "-").ToLower();
        }


        public static string ToTitleCase(string str) {
            return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(str);
        }
        
    }
}
