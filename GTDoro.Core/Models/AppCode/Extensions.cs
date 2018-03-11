using GTDoro.Core.Attributes;
using GTDoro.Core.Models;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.Net.Http;
using System.Collections.Generic;
using System.Linq;
using GTDoro.Core.ViewModels;

namespace GTDoro.Core.Extensions
{
    public static class Extensions
    {
        public static int ToPomoLengthPeriod (this TimeSpan timespan)
        {
            return (int) (timespan.Ticks / new TimeSpan(0, Settings.POMOCYCLE, 0).Ticks);
        }

        public static string ToIntegerPercentageString(this decimal? percentage)
        {
            return percentage.HasValue ? (percentage.Value.ToString("0") + " %") : "0 %";
        }

        public static decimal ToDecimalOrZero(this decimal? d)
        {
            return (d.HasValue) ? d.Value : 0;
        }

        public static long ToDateTicksOrZero(this ILoggable p)
        {
            return (p != null) ? p.StartLocal.ToTicksOrZero() : 0;
        }

        public static long ToTicksOrZero(this DateTime? dt)
        {
            return dt.HasValue ? dt.Value.Ticks : 0;
        }
        
        public static string ToDateStringOrEmpty(this ILoggable p)
        {
            return (p != null) ? p.StartLocal.ToStringOrEmpty() : string.Empty;
        }

        public static string ToDateStringOrDefault(this ILoggable p)
        {
            return (p != null) ? p.StartLocal.ToStringOrEmpty() : Settings.DEFAULT_NULL_STRING_VALUE;
        }

        /// <summary>
        /// "dd/MM/yyyy - HH:mm" format
        /// </summary>
        public static string ToStringOrEmpty (this DateTime? dt)
        {
            return dt.HasValue ? dt.Value.ToString("dd/MM/yyyy - HH:mm") : string.Empty;
        }

        /// <summary>
        /// "dd/MM/yyyy - HH:mm" format
        /// </summary>
        public static string ToStringOrDefault(this DateTime? dt)
        {
            return dt.HasValue ? dt.Value.ToString("dd/MM/yyyy - HH:mm") : Settings.DEFAULT_NULL_STRING_VALUE;
        }

        /// <summary>
        /// "dd/MM/yyyy" format
        /// </summary>
        public static string ToDateStringOrEmpty(this DateTime? dt)
        {
            return dt.HasValue ? dt.Value.ToString("dd/MM/yyyy") : string.Empty;
        }

        /// <summary>
        /// "dd/MM/yyyy" format
        /// </summary>
        public static string ToDateString(this DateTime dt)
        {
            return dt.ToString("dd/MM/yyyy");
        }

        /// <summary>
        /// "dd/MM/yyyy" format
        /// </summary>
        public static string ToDateStringOrDefault(this DateTime? dt)
        {
            return dt.HasValue ? dt.Value.ToString("dd/MM/yyyy") : Settings.DEFAULT_NULL_STRING_VALUE;
        }

        public static string ToDateRelativeString(this ILoggable p)
        {
            return (p != null) ? p.Start.ToRelativeString() : "never";
        }

        public static string ToDateRelativeShortString(this ILoggable p)
        {
            return (p != null) ? p.Start.ToRelativeShortString() : "never";
        }

        public static String ToRelativeShortString(this DateTime dt)
        {
            return ToRelativeShortString((DateTime?)dt);
        }

        public static String ToRelativeShortString(this DateTime? dt)
        {
            if (dt.HasValue == false)
            {
                return "never";
            }
            var ts = new TimeSpan(DateTime.UtcNow.Ticks - dt.Value.Ticks);
            bool isFuture = (ts.TotalSeconds < 0);

            ts = new TimeSpan(Math.Abs(ts.Ticks));
            double delta = ts.TotalSeconds;

            const int SECOND = 1;
            const int MINUTE = 60 * SECOND;
            const int HOUR = 60 * MINUTE;
            const int DAY = 24 * HOUR;
            const int MONTH = 30 * DAY;

            if (delta < 0)
            {
                return "fut";
            }
            if (delta < 1 * MINUTE)
            {
                return ts.Seconds + " s";
            }
            if (delta < 45 * MINUTE)
            {
                return ts.Minutes + " mi";
            }
            if (delta < 24 * HOUR)
            {
                return ts.Hours + " h";
            }
            if (delta < 30 * DAY)
            {
                return ts.Days + " d";
            }
            if (delta < 12 * MONTH)
            {
                int months = Convert.ToInt32(Math.Floor((double)ts.Days / 30));
                return months + " mo";
            }
            else
            {
                int years = Convert.ToInt32(Math.Floor((double)ts.Days / 365));
                return ">" + years + " y";
            }
        }
        
        public static String ToRelativeString(this DateTime dt)
        {
            return ToRelativeString((DateTime?)dt);
        }

        public static String ToRelativeString(this DateTime? dt)
        {
            if(dt.HasValue == false)
            {
                return "never";
            }
            var ts = new TimeSpan(DateTime.UtcNow.Ticks - dt.Value.Ticks);
            bool isFuture = (ts.TotalSeconds < 0);
            string lastWord = isFuture ? "left" : "ago";

            ts = new TimeSpan(Math.Abs(ts.Ticks));
            double delta = ts.TotalSeconds;

            const int SECOND = 1;
            const int MINUTE = 60 * SECOND;
            const int HOUR = 60 * MINUTE;
            const int DAY = 24 * HOUR;
            const int MONTH = 30 * DAY;

            if (delta < 0)
            {
                return "not yet";
            }
            if (delta < 1 * MINUTE)
            {
                return ts.Seconds == 1 ? "one second " + lastWord : ts.Seconds + " seconds " + lastWord;
            }
            if (delta < 2 * MINUTE)
            {
                return "a minute " + lastWord;
            }
            if (delta < 45 * MINUTE)
            {
                return ts.Minutes + " minutes " + lastWord;
            }
            if (delta < 90 * MINUTE)
            {
                return "an hour " + lastWord;
            }
            if (delta < 24 * HOUR)
            {
                return ts.Hours + " hours " + lastWord;
            }
            if (delta < 48 * HOUR)
            {
                return isFuture ? "tomorrow" : "yesterday";
            }
            if (delta < 30 * DAY)
            {
                return ts.Days + " days " + lastWord;
            }
            if (delta < 12 * MONTH)
            {
                int months = Convert.ToInt32(Math.Floor((double)ts.Days / 30));
                return months <= 1 ? "one month " + lastWord : months + " months " + lastWord;
            }
            else
            {
                int years = Convert.ToInt32(Math.Floor((double)ts.Days / 365));
                return years <= 1 ? "one year " + lastWord : years + " years " + lastWord;
            }
        }

        public static DateTime? ToUserLocalTime(this DateTime? utcTime, string TimeZoneId)
        {
            if(!utcTime.HasValue)
            {
                return null;
            }
            return utcTime.RoundDateOrEmpty().Value.ToUserLocalTime(TimeZoneId);
        }

        public static DateTime ToUserLocalTime(this DateTime utcTime, string TimeZoneId)
        {
            //TimeZoneInfo targetTimeZone = TimeZoneInfo.CreateCustomTimeZone(
            //    "Ireland", TimeSpan.FromMinutes(Settings.TIMEZONE_OFFSET_MIN), "Ireland", "Ireland");
            //return TimeZoneInfo.ConvertTimeFromUtc(utcTime, targetTimeZone);
            TimeZoneInfo targetTimeZone = null;
            if (string.IsNullOrWhiteSpace(TimeZoneId))
            {
                targetTimeZone = TimeZoneInfo.CreateCustomTimeZone(
                    "Ireland", TimeSpan.FromMinutes(Settings.TIMEZONE_OFFSET_MIN), "Ireland", "Ireland");
            }
            else 
            { 
                try
                {
                    targetTimeZone = TimeZoneInfo.FindSystemTimeZoneById(TimeZoneId);
                }
                catch
                {
                    targetTimeZone = TimeZoneInfo.CreateCustomTimeZone(
                        "Ireland", TimeSpan.FromMinutes(Settings.TIMEZONE_OFFSET_MIN), "Ireland", "Ireland");
                }
            }
            return TimeZoneInfo.ConvertTimeFromUtc(utcTime, targetTimeZone);
        }

        public static DateTime? RoundDateOrEmpty(this DateTime? dateTime)
        {
            if (!dateTime.HasValue)
            {
                return null;
            }
            return dateTime.Value.Round(new TimeSpan(0, 5, 0));
        }

        private static DateTime Floor(this DateTime dateTime, TimeSpan interval)
        {
            return dateTime.AddTicks(-(dateTime.Ticks % interval.Ticks));
        }

        private static DateTime Ceiling(this DateTime dateTime, TimeSpan interval)
        {
            return dateTime.AddTicks(interval.Ticks - (dateTime.Ticks % interval.Ticks));
        }

        private static DateTime Round(this DateTime dateTime, TimeSpan interval)
        {
            var halfIntervelTicks = ((interval.Ticks + 1) >> 1);

            return dateTime.AddTicks(halfIntervelTicks - ((dateTime.Ticks + halfIntervelTicks) % interval.Ticks));
        }

        public static string GetAttributeIconCssClass(this Enum enumValue)
        {
            var type = enumValue.GetType();
            var memInfo = type.GetMember(enumValue.ToString());
            var attributes = memInfo[0].GetCustomAttributes(typeof(IconCssClassAttribute), false);
            return (attributes.Length > 0) ? ((IconCssClassAttribute)attributes[0]).cssClassName : string.Empty;
        }

        public static string GetAttributeDisplayName(this Enum enumValue)
        {
            var type = enumValue.GetType();
            var memInfo = type.GetMember(enumValue.ToString());
            var attributes = memInfo[0].GetCustomAttributes(typeof(DisplayAttribute), false);
            return (attributes.Length > 0) ? ((DisplayAttribute)attributes[0]).Name : enumValue.ToString();
        }

        public static string GetIconHtmlTag(this Enum enumValue)
        {
            return "<i class=\"fa fa-fw " + enumValue.GetAttributeIconCssClass()
                + " gt-status\" title=\"" + enumValue.GetAttributeDisplayName() + "\"></i>";
        }
        
        public static string RemoveDuplicateLineBreaks(this string originalString)
        {
            while(originalString.Contains("\r\n\r\n"))
            {
                originalString = originalString.Replace("\r\n\r\n", "\r\n");
            }
            return originalString;
        }
        
        public static Color GetRandomColor(this string text)
        {
            int hash = text.GetHashCode();
            int r = (hash & 0xFF0000) >> 16;
            int g = (hash & 0x00FF00) >> 8;
            int b = hash & 0x0000FF;
            return Color.FromArgb(r, g, b);
        }        
    }

    public static class ClaimsIdentityExtensions
    {
        private const string PersistentLoginClaimType = "PersistentLogin";

        public static bool GetIsPersistent(this System.Security.Claims.ClaimsIdentity identity)
        {
            return identity.Claims.FirstOrDefault(c => c.Type == PersistentLoginClaimType) != null;
        }

        public static void SetIsPersistent(this System.Security.Claims.ClaimsIdentity identity, bool isPersistent)
        {
            var claim = identity.Claims.FirstOrDefault(c => c.Type == PersistentLoginClaimType);
            if (isPersistent)
            {
                if (claim == null)
                {
                    identity.AddClaim(new System.Security.Claims.Claim(PersistentLoginClaimType, Boolean.TrueString));
                }
            }
            else if (claim != null)
            {
                identity.RemoveClaim(claim);
            }
        }
    }
}