namespace Clouded.Shared;

public static class DateTimeExtensions
{
    public static DateTime? ChangeTime(this DateTime? dateTime, TimeSpan time) =>
        dateTime.HasValue
            ? new DateTime(
                dateTime.Value.Year,
                dateTime.Value.Month,
                dateTime.Value.Day,
                time.Hours,
                time.Minutes,
                time.Seconds
            )
            : null;
}
