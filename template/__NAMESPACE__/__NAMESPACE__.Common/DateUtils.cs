namespace __NAMESPACE__.Common
{
    public static class DateUtils
    {
        public static DateTimeOffset GetStartDate(this DateTimeOffset value)
        {
            var result = value.RemoveTime();
            return result;
        }

        public static DateTimeOffset? GetStartDate(this DateTimeOffset? value)
        {
            if (!value.HasValue) return value;
            var result = value.Value.RemoveTime();
            return result;
        }

        public static DateTimeOffset GetEndDate(this DateTimeOffset value)
        {
            var result = value.RemoveTime().AddDays(1);
            return result;
        }

        public static DateTimeOffset? GetEndDate(this DateTimeOffset? value)
        {
            if (!value.HasValue) return value;
            var result = value.Value.RemoveTime().AddDays(1);
            return result;
        }

        private static DateTimeOffset RemoveTime(this DateTimeOffset value)
        {
            return value
                .AddHours(-value.Hour)
                .AddMinutes(-value.Minute)
                .AddSeconds(-value.Second)
                .AddMilliseconds(-value.Millisecond);
        }
    }
}
