

namespace Hotel.Core.Common
{
    public class CoreHelper
    {
        public static DateTimeOffset SystemTimeNow => DateTimeParsing.ConvertToUtcPlus7(DateTimeOffset.Now);

        public static DateTime SystemTime=>DateTimeParsing.ConvertToUtcPlus7(DateTime.Now);
    }
}
