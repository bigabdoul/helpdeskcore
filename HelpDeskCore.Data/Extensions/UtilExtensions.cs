using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace HelpDeskCore.Data.Extensions
{
    public static class UtilExtensions
    {
        const double ONE_MINUTE_IN_SECONDS = 60d;
        const double TWO_MINUTES_IN_SECONDS = 2 * ONE_MINUTE_IN_SECONDS;
        const double ONE_HOUR_IN_SECONDS = 3600d;
        const double TWO_HOURS_IN_SECONDS = 2 * ONE_HOUR_IN_SECONDS;
        const double ONE_DAY_IN_SECONDS = 86400d;
        const double TWO_DAYS_IN_SECONDS = 2 * 86400d;
        const double ONE_YEAR_IN_SECONDS = 365 * ONE_DAY_IN_SECONDS;
        const double TWO_YEARS_IN_SECONDS = 2 * ONE_YEAR_IN_SECONDS;

        internal const string FULL_DATE_TIME = "dddd, d MMMM yyyy @ HH:mm";
        internal const string LONG_DATE_TIME = "dd/MM/yyyy @ HH:mm";

        const string EMAIL_PATTERN = @"(?<email>([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3}))";

        /// <summary>
        /// Regular expression pattern that matches emails in the following format: "Abdul R. Kaba"&lt;abdulkaba@example.com>.
        /// Used to extract the 'From' name and email address parts from a string.
        /// </summary>
        const string NAME_EMAIL_PATTERN = @"""?(?<name>[\w\.\-'\s]*)""?\s*\<\s*" + EMAIL_PATTERN + @"\s*>";

        public static string EmailRecipient(string email, string name) => $"\"{name}\" <{email}>";

        public static T Deserialize<T>(this string value)
          => Newtonsoft.Json.JsonConvert.DeserializeObject<T>(value);

        public static byte[] ComputeHash(this byte[] data)
        {
            using (var sha = new SHA256Managed())
            {
                return sha.ComputeHash(data);
            }
        }

        public static StringBuilder ReplaceLineBreaks(this StringBuilder sb) => sb.Replace(Environment.NewLine, "<br/>").Replace("\n", "<br/>");

        public static string GetDateString(DateTime? startDate, string fmt = LONG_DATE_TIME) => startDate.HasValue ? startDate.Value.ToString(fmt) : string.Empty;

        public static string PriorityName(short priority)
        {
            switch (priority)
            {
                case -1: return "Basse";
                case 0: return "Normale";
                case 1: return "Haute";
                case 2: return "Critique";
                default: return string.Empty;
            };
        }

        public static int? CountDays(DateTime? date)
        {
            if (date == null) return null;
            var diff = DateTime.UtcNow - date.Value;
            return Convert.ToInt32(diff.TotalDays);
        }

        public static string TranslateDays(DateTime? date)
        {
            var count = CountDays(date);
            if (count == null) return string.Empty;
            if (count == 0) return "aujourd'hui";
            if (count == 1) return "hier";
            if (count > 1) return $"il y a {count} jours";
            if (count == -1) return "demain";
            return $"dans {-count} jours";
        }

        public static string TranslateTime(DateTime? date)
        {
            if (date == null) return null;
            var diff = DateTime.UtcNow - date.Value;
            var seconds = diff.TotalSeconds;

            if (Math.Floor(seconds) == 0d) return "maintenant";

            var intval = string.Empty;
            var absTime = Math.Abs(seconds);

            if (absTime < ONE_MINUTE_IN_SECONDS)
            {
                intval = "secondes";
            }
            else if (absTime < TWO_MINUTES_IN_SECONDS)
            {
                absTime = 1d;
                intval = "minute+";
            }
            else if (absTime < ONE_HOUR_IN_SECONDS)
            {
                intval = "minutes";
                absTime = Math.Abs(diff.TotalMinutes);
            }
            else if (absTime < TWO_HOURS_IN_SECONDS)
            {
                absTime = 1d;
                intval = "heure+";
            }
            else if (absTime < ONE_DAY_IN_SECONDS)
            {
                intval = "heures";
                absTime = Math.Abs(diff.TotalHours);
            }
            else if (absTime < TWO_DAYS_IN_SECONDS)
            {
                absTime = 1d;
                intval = "jour";
            }
            else if (absTime < ONE_YEAR_IN_SECONDS)
            {
                intval = "jours";
                absTime = Math.Abs(diff.TotalDays);
            }
            else if (absTime < TWO_YEARS_IN_SECONDS)
            {
                absTime = 1d;
                intval = "an+";
            }
            else
            {
                intval = "ans";
                absTime = Math.Abs(diff.TotalDays) / 365;
            }

            return string.Format("{0}{1:N0} {2}", seconds < 0d ? "dans " : string.Empty, Math.Floor(absTime), intval);
        }

        public static bool ExtractNameAndAddress(this string nameAndEmail, out string name, out string address)
        {
            name = null;
            address = null;
            bool success;
            var match = Regex.Match(nameAndEmail, NAME_EMAIL_PATTERN, RegexOptions.Compiled);
            if (success = match.Success)
            {
                name = match.Groups["name"].Value?.Trim();
                address = match.Groups["email"].Value;
            }
            return success;
        }

        public static bool IsEmailAddress(this string input) =>
            !string.IsNullOrWhiteSpace(input) && Regex.IsMatch(input, EMAIL_PATTERN, RegexOptions.Compiled);

        /// <summary>
        /// Performs the specified action on each element of the <see cref="IEnumerable{T}"/>.
        /// </summary>
        /// <typeparam name="T">The type of the elements of the collection.</typeparam>
        /// <param name="collection">The collection on which to perform an action on each element.</param>
        /// <param name="action">The <see cref="Action{T}"/> delegate to perform on each element of the <see cref="IEnumerable{T}"/>.</param>
        public static void ForEach<T>(this IEnumerable<T> collection, Action<T> action)
        {
            foreach (var item in collection) action(item);
        }
    }
}
