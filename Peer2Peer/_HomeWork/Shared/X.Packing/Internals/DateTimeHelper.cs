using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace X.Packing.Internals
{
    static class DateTimeHelper
    {
        internal static readonly DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        public static DateTimeOffset OffsetFromEpoch(long unixTime)
        {
            Console.WriteLine(unixTime);
            return (DateTimeOffset)epoch.AddSeconds(unixTime);
        }

        public static long SecondsSinceEpoch(DateTimeOffset date)
        {
            long tot = Convert.ToInt64((date.ToUniversalTime() - epoch).TotalSeconds);
            Console.WriteLine("W:" + tot);
            return tot;
        }
    }
}
