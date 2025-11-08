namespace ECOMMERCE.SUBMISSION.HELPER;
public class DateTimeHelper
{
    /// <summary>
    /// Get unix time
    /// </summary>
    /// <param name="isMilisecond">Return miliseconds if True</param>
    /// <returns></returns>
    public static long GetUtcTimestamp(bool isMilisecond=false)
    {
        if(isMilisecond)
            return DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        return DateTimeOffset.UtcNow.ToUnixTimeSeconds();
    }

    /// <summary>
    /// Convert DateTime to unix
    /// </summary>
    /// <param name="dateTime"></param>
    /// <param name="isMilisecond">Return miliseconds if True</param>
    /// <returns></returns>
    public static long ToUnixTime(DateTime dateTime, bool isMilisecond = false)
    {
        DateTimeOffset dto = new DateTimeOffset(dateTime.ToUniversalTime());
        return dto.ToUnixTimeSeconds();
    }

    
    /// <summary>
    /// Get current datetime by time zone
    /// </summary>
    /// <param name="timeZoneId"></param>
    /// <returns></returns>
    public static DateTimeOffset GetCurrentDateTimeOffsetByTimeZone(string timeZoneId)
    {
        TimeZoneInfo _tz = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
        return TimeZoneInfo.ConvertTime(DateTimeOffset.UtcNow, _tz);    
    }
}
