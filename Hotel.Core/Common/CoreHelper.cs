

namespace Hotel.Core.Common
{
    public class CoreHelper
    {
        public static DateTimeOffset SystemTimeNow => DateTimeParsing.ConvertToUtcPlus7(DateTimeOffset.Now);
    }
}
