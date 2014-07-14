using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Linq.Expressions;
using Signum.Utilities.ExpressionTrees;
using System.Text.RegularExpressions;
using System.Reflection;
using System.ComponentModel;

namespace Signum.Utilities
{
    public static class DateTimeExtensions
    {
        /// <summary>
        /// Checks if the date is inside a C interval defined by the two given dates
        /// </summary>
        [MethodExpander(typeof(IsInIntervalExpander))]
        public static bool IsInInterval(this DateTime date, DateTime minDate, DateTime maxDate)
        {
            return minDate <= date && date < maxDate;
        }

        /// <summary>
        /// Checks if the date is inside a C interval defined by the two given dates
        /// </summary>
        [MethodExpander(typeof(IsInIntervalExpanderNull))]
        public static bool IsInInterval(this DateTime date, DateTime minDate, DateTime? maxDate)
        {
            return minDate <= date && (maxDate == null || date < maxDate);
        }

        /// <summary>
        /// Checks if the date is inside a C interval defined by the two given dates
        /// </summary>
        [MethodExpander(typeof(IsInIntervalExpanderNullNull))]
        public static bool IsInInterval(this DateTime date, DateTime? minDate, DateTime? maxDate)
        {
            return (minDate == null || minDate <= date) &&
                   (maxDate == null || date < maxDate);
        }

        static void AssertDateOnly(DateTime? date)
        {
            if (date == null)
                return;
            DateTime d = date.Value;
            if (d.Hour != 0 || d.Minute != 0 || d.Second != 0 || d.Millisecond != 0)
                throw new InvalidOperationException("The date has some hours, minutes, seconds or milliseconds");
        }

        /// <summary>
        /// Checks if the date is inside a date-only interval (compared by entires days) defined by the two given dates
        /// </summary>
        [MethodExpander(typeof(IsInIntervalExpander))]
        public static bool IsInDateInterval(this DateTime date, DateTime minDate, DateTime maxDate)
        {
            AssertDateOnly(date);
            AssertDateOnly(minDate);
            AssertDateOnly(maxDate);
            return minDate <= date && date <= maxDate;
        }

        /// <summary>
        /// Checks if the date is inside a date-only interval (compared by entires days) defined by the two given dates
        /// </summary>
        [MethodExpander(typeof(IsInIntervalExpanderNull))]
        public static bool IsInDateInterval(this DateTime date, DateTime minDate, DateTime? maxDate)
        {
            AssertDateOnly(date);
            AssertDateOnly(minDate);
            AssertDateOnly(maxDate);
            return (minDate == null || minDate <= date) &&
                   (maxDate == null || date < maxDate);
        }

        /// <summary>
        /// Checks if the date is inside a date-only interval (compared by entires days) defined by the two given dates
        /// </summary>
        [MethodExpander(typeof(IsInIntervalExpanderNullNull))]
        public static bool IsInDateInterval(this DateTime date, DateTime? minDate, DateTime? maxDate)
        {
            AssertDateOnly(date);
            AssertDateOnly(minDate);
            AssertDateOnly(maxDate);
            return (minDate == null || minDate <= date) &&
                   (maxDate == null || date < maxDate);
        }

        class IsInIntervalExpander : IMethodExpander
        {
            static readonly Expression<Func<DateTime, DateTime, DateTime, bool>> func = (date, minDate, maxDate) => minDate <= date && date < maxDate;

            public Expression Expand(Expression instance, Expression[] arguments, MethodInfo mi)
            {
                return Expression.Invoke(func, arguments[0], arguments[1], arguments[2]);
            }
        }

        class IsInIntervalExpanderNull : IMethodExpander
        {
            Expression<Func<DateTime, DateTime, DateTime?, bool>> func = (date, minDate, maxDate) => minDate <= date && (maxDate == null || date < maxDate);

            public Expression Expand(Expression instance, Expression[] arguments, MethodInfo mi)
            {
                return Expression.Invoke(func, arguments[0], arguments[1], arguments[2]);
            }
        }

        class IsInIntervalExpanderNullNull : IMethodExpander
        {
            Expression<Func<DateTime, DateTime?, DateTime?, bool>> func = (date, minDate, maxDate) =>
                (minDate == null || minDate <= date) &&
                (maxDate == null || date < maxDate);

            public Expression Expand(Expression instance, Expression[] arguments, MethodInfo mi)
            {
                return Expression.Invoke(func, arguments[0], arguments[1], arguments[2]);
            }
        }

        public static int YearsTo(this DateTime start, DateTime end)
        {
            int result = end.Year - start.Year;
            if (end.Month < start.Month || (end.Month == start.Month & end.Day < start.Day))
                result--;

            return result;
        }

        public static int MonthsTo(this DateTime start, DateTime end)
        {
            int result = end.Month - start.Month + (end.Year - start.Year) * 12;
            if (end.Day < start.Day)
                result--;

            return result;
        }

        public static DateSpan DateSpanTo(this DateTime min, DateTime max)
        {
            return DateSpan.FromToDates(min, max);
        }

        public static DateTime Add(this DateTime date, DateSpan dateSpan)
        {
            return dateSpan.AddTo(date);
        }

        public static DateTime Min(this DateTime a, DateTime b)
        {
            return a < b ? a : b;
        }

        public static DateTime Max(this DateTime a, DateTime b)
        {
            return a > b ? a : b;
        }

        public static DateTime Min(this DateTime a, DateTime? b)
        {
            if (b == null)
                return a;

            return a < b.Value ? a : b.Value;
        }

        public static DateTime Max(this DateTime a, DateTime? b)
        {
            if (b == null)
                return a;

            return a > b.Value ? a : b.Value;
        }

        public static DateTime? Min(this DateTime? a, DateTime? b)
        {
            if (a == null)
                return b;

            if (b == null)
                return a;

            return a.Value < b.Value ? a.Value : b.Value;
        }

        public static DateTime? Max(this DateTime? a, DateTime? b)
        {
            if (a == null)
                return b;

            if (b == null)
                return a;

            return a.Value > b.Value ? a.Value : b.Value;
        }

        /// <param name="precision">Using Milliseconds does nothing, using Days use DateTime.Date</param>
        public static DateTime TrimTo(this DateTime dateTime, DateTimePrecision precision)
        {
            switch (precision)
            {
                case DateTimePrecision.Days: return dateTime.Date;
                case DateTimePrecision.Hours: return TrimToHours(dateTime);
                case DateTimePrecision.Minutes: return TrimToMinutes(dateTime);
                case DateTimePrecision.Seconds: return TrimToSeconds(dateTime);
                case DateTimePrecision.Milliseconds: return dateTime;
            }
            throw new ArgumentException("precission");
        }

        public static DateTime TrimToSeconds(this DateTime dateTime)
        {
            return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute, dateTime.Second, dateTime.Kind);
        }

        public static DateTime TrimToMinutes(this DateTime dateTime)
        {
            return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute, 0, dateTime.Kind);
        }

        public static DateTime TrimToHours(this DateTime dateTime)
        {
            return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, 0, 0, dateTime.Kind);
        }

        public static DateTimePrecision GetPrecision(this DateTime dateTime)
        {
            if (dateTime.Millisecond != 0)
                return DateTimePrecision.Milliseconds;
            if (dateTime.Second != 0)
                return DateTimePrecision.Seconds;
            if (dateTime.Minute != 0)
                return DateTimePrecision.Minutes;
            if (dateTime.Hour != 0)
                return DateTimePrecision.Hours;

            return DateTimePrecision.Days;
        }

        public static TimeSpan TrimTo(this TimeSpan timeSpan, DateTimePrecision precision)
        {
            switch (precision)
            {
                case DateTimePrecision.Days: return timeSpan.TrimToDays();
                case DateTimePrecision.Hours: return TrimToHours(timeSpan);
                case DateTimePrecision.Minutes: return TrimToMinutes(timeSpan);
                case DateTimePrecision.Seconds: return TrimToSeconds(timeSpan);
                case DateTimePrecision.Milliseconds: return timeSpan;
            }
            throw new ArgumentException("precission");
        }

        public static TimeSpan TrimToSeconds(this TimeSpan dateTime)
        {
            return new TimeSpan(dateTime.Days, dateTime.Hours, dateTime.Minutes, dateTime.Seconds);
        }

        public static TimeSpan TrimToMinutes(this TimeSpan dateTime)
        {
            return new TimeSpan(dateTime.Days, dateTime.Hours, dateTime.Minutes, 0);
        }

        public static TimeSpan TrimToHours(this TimeSpan dateTime)
        {
            return new TimeSpan(dateTime.Days, dateTime.Hours, 0, 0);
        }

        public static TimeSpan TrimToDays(this TimeSpan dateTime)
        {
            return new TimeSpan(dateTime.Days, 0, 0, 0);
        }

        public static DateTimePrecision? GetPrecision(this TimeSpan timeSpan)
        {
            if (timeSpan.Milliseconds != 0)
                return DateTimePrecision.Milliseconds;
            if (timeSpan.Seconds != 0)
                return DateTimePrecision.Seconds;
            if (timeSpan.Minutes != 0)
                return DateTimePrecision.Minutes;
            if (timeSpan.Hours != 0)
                return DateTimePrecision.Hours;
            if (timeSpan.Days != 0)
                return DateTimePrecision.Days;
            
            return null;
        }

        public static string SmartDatePattern(this DateTime date)
        {
            DateTime currentdate = DateTime.Today;
            return SmartDatePattern(date, currentdate);
        }

        public static string SmartDatePattern(this DateTime date, DateTime currentdate)
        {
            int datediff = (date.Date - currentdate).Days;

            if (-7 <= datediff && datediff <= -2)
                return DateTimeMessage.DateLast.NiceToString().Formato(CultureInfo.CurrentCulture.DateTimeFormat.GetDayName(date.DayOfWeek).FirstUpper());

            if (datediff == -1)
                return DateTimeMessage.Yesterday.NiceToString();

            if (datediff == 0)
                return DateTimeMessage.Today.NiceToString();

            if (datediff == 1)
                return DateTimeMessage.Tomorrow.NiceToString();

            if (2 <= datediff && datediff <= 7)
                return DateTimeMessage.DateThis.NiceToString().Formato(CultureInfo.CurrentCulture.DateTimeFormat.GetDayName(date.DayOfWeek).FirstUpper());

            if (date.Year == currentdate.Year)
            {
                string pattern = CultureInfo.CurrentCulture.DateTimeFormat.LongDatePattern;
                pattern = Regex.Replace(pattern, "('[^']*')?yyy?y?('[^']*')?", "");
                string dateString = date.ToString(pattern);
                return dateString.Trim().FirstUpper();
            }
            return date.ToLongDateString();
        }

        public static string ToAgoString(this DateTime dateTime)
        {
            DateTime now = dateTime.Kind == DateTimeKind.Utc ? DateTime.UtcNow : DateTime.Now;

            TimeSpan ts = now.Subtract(dateTime);
            string resource = null;
            if (ts.TotalMilliseconds < 0)
                resource = DateTimeMessage.In.NiceToString();
            else
                resource = DateTimeMessage.Ago.NiceToString();

            int months = Math.Abs(ts.Days) / 30;
            if (months > 0)
                return resource.Formato((months == 1 ? DateTimeMessage._0Month.NiceToString() : DateTimeMessage._0Months.NiceToString()).Formato(Math.Abs(months))).ToLower();
            if (Math.Abs(ts.Days) > 0)
                return resource.Formato((ts.Days == 1 ? DateTimeMessage._0Day.NiceToString() : DateTimeMessage._0Days.NiceToString()).Formato(Math.Abs(ts.Days))).ToLower();
            if (Math.Abs(ts.Hours) > 0)
                return resource.Formato((ts.Hours == 1 ? DateTimeMessage._0Hour.NiceToString() : DateTimeMessage._0Hours.NiceToString()).Formato(Math.Abs(ts.Hours))).ToLower();
            if (Math.Abs(ts.Minutes) > 0)
                return resource.Formato((ts.Minutes == 1 ? DateTimeMessage._0Minute.NiceToString() : DateTimeMessage._0Minutes.NiceToString()).Formato(Math.Abs(ts.Minutes))).ToLower();

            return resource.Formato((ts.Seconds == 1 ? DateTimeMessage._0Second.NiceToString() : DateTimeMessage._0Seconds.NiceToString()).Formato(Math.Abs(ts.Seconds))).ToLower();
        }
        
        public static DateTime MonthStart(this DateTime dateTime)
        {
            return new DateTime(dateTime.Year, dateTime.Month, 1, 0, 0, 0, dateTime.Kind); 
        }

        public static string NiceToString(this TimeSpan timeSpan)
        {
            return timeSpan.NiceToString(DateTimePrecision.Milliseconds); 
        }

        public static string NiceToString(this TimeSpan timeSpan, DateTimePrecision precission)
        {
            StringBuilder sb = new StringBuilder();
            bool any = false;
            if (timeSpan.Days != 0/* && precission >= DateTimePrecision.Days*/)
            {
                sb.AppendFormat("{0}d", timeSpan.Days);
                any = true;
            }

            if ((any || timeSpan.Hours != 0) && precission >= DateTimePrecision.Hours)
            {
                if (any)
                    sb.Append(" ");

                sb.AppendFormat("{0,2}h", timeSpan.Hours);
                any = true;
            }

            if ((any || timeSpan.Minutes != 0) && precission >= DateTimePrecision.Minutes)
            {
                if (any)
                    sb.Append(" ");

                sb.AppendFormat("{0,2}m", timeSpan.Minutes);
                any = true;
            }

            if ((any || timeSpan.Seconds != 0) && precission >= DateTimePrecision.Seconds)
            {
                if (any)
                    sb.Append(" ");

                sb.AppendFormat("{0,2}s", timeSpan.Seconds);
                any = true;
            }

            if ((any || timeSpan.Milliseconds != 0) && precission >= DateTimePrecision.Milliseconds)
            {
                if (any)
                    sb.Append(" ");

                sb.AppendFormat("{0,3}ms", timeSpan.Milliseconds);
            }

            return sb.ToString();
        }

        public static long JavascriptMilliseconds(this DateTime dateTime)
        {
            if (dateTime.Kind != DateTimeKind.Utc)
                throw new InvalidOperationException("dateTime should be UTC"); 

            return (long)new TimeSpan(dateTime.Ticks - new DateTime(1970, 1, 1).Ticks).TotalMilliseconds;
        }

        public static string ToMonthName(this DateTime dateTime)
        {
            return CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(dateTime.Month);
        }

        public static string ToShortMonthName(this DateTime dateTime)
        {
            return CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(dateTime.Month);
        }

        public static int WeekNumber(this DateTime dateTime)
        {
            var cc = CultureInfo.CurrentCulture;

            return cc.Calendar.GetWeekOfYear(dateTime, CalendarWeekRule.FirstDay, cc.DateTimeFormat.FirstDayOfWeek); 
        }
    }

    [DescriptionOptions(DescriptionOptions.Members)]
    public enum DateTimePrecision
    {
        Days,
        Hours,
        Minutes,
        Seconds,
        Milliseconds,
    }

    public struct DateSpan
    {
        public static readonly DateSpan Zero = new DateSpan(0, 0, 0);

        public readonly int Years;
        public readonly int Months;
        public readonly int Days;

        public DateSpan(int years, int months, int days)
        {
            int sign = Math.Sign(years).DefaultToNull() ?? Math.Sign(months).DefaultToNull() ?? Math.Sign(days);

            if (0 < sign && (months < 0 || days < 0) ||
                sign < 0 && (0 < months || 0 < days))
                throw new ArgumentException("All arguments should have the same sign");

            this.Years = years;
            this.Months = months;
            this.Days = days;
        }

        public static DateSpan FromToDates(DateTime min, DateTime max)
        {
            if (min > max) return FromToDates(max, min).Invert();

            int years = max.Year - min.Year;
            int months = max.Month - min.Month;


            if (max.Day < min.Day)
                months -= 1;

            if (months < 0)
            {
                years -= 1;
                months += 12;
            }

            int days = max.Subtract(min.AddYears(years).AddMonths(months)).Days;

            return new DateSpan(years, months, days);
        }

        public DateTime AddTo(DateTime date)
        {
            return date.AddDays(Days).AddMonths(Months).AddYears(Years);
        }

        public DateSpan Invert()
        {
            return new DateSpan(-Years, -Months, -Days);
        }

        public override string ToString()
        {
            string result= ", ".Combine(
                         Years == 0 ? null :
                         Years == 1 ? DateTimeMessage._0Year.NiceToString().Formato(Years) :
                                     DateTimeMessage._0Years.NiceToString().Formato(Years),
                         Months == 0 ? null :
                         Months == 1 ? DateTimeMessage._0Month.NiceToString().Formato(Months) :
                                      DateTimeMessage._0Months.NiceToString().Formato(Months),
                         Days == 0 ? null :
                         Days == 1 ? DateTimeMessage._0Day.NiceToString().Formato(Days) :
                                    DateTimeMessage._0Days.NiceToString().Formato(Days));

            if (string.IsNullOrEmpty(result))
                result = DateTimeMessage._0Day.NiceToString().Formato(0);

            return result;

        }
    }

    public enum DateTimeMessage
    {
        [Description("{0} Day")]
        _0Day,
        [Description("{0} Days")]
        _0Days,
        [Description("{0} Hour")]
        _0Hour,
        [Description("{0} Hours")]
        _0Hours,
        [Description("{0} Minute")]
        _0Minute,
        [Description("{0} Minutes")]
        _0Minutes,
        [Description("{0} Month")]
        _0Month,
        [Description("{0} Months")]
        _0Months,
        [Description("{0} Second")]
        _0Second,
        [Description("{0} Seconds")]
        _0Seconds,
        [Description("{0} Year")]
        _0Year,
        [Description("{0} Years")]
        _0Years,
        [Description("{0} ago ")]
        Ago,
        [Description("Last {0}")]
        DateLast,
        [Description("This {0}")]
        DateThis,
        [Description("In {0}")]
        In,
        Today,
        Tomorrow,
        Yesterday
    }
}
