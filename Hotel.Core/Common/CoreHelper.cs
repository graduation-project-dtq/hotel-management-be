

namespace Hotel.Core.Common
{
    public class CoreHelper
    {
        public static DateTimeOffset SystemTimeNow => DateTimeParsing.ConvertToUtcPlus7(DateTimeOffset.Now);

        public static DateTime SystemTime=>DateTimeParsing.ConvertToUtcPlus7(DateTime.Now);

     
        public static DateOnly SystemDateOnly => DateOnly.FromDateTime(DateTimeParsing.ConvertToUtcPlus7(DateTimeOffset.Now).DateTime);


    }
}
