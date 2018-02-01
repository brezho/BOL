using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace X.Editor.Controls.Utils
{
    public class CallsPerSecond
    {
        public int Count { get; private set; }

        private int n1;
        private long timeout;

        private System.Diagnostics.Stopwatch watch;
        
        public CallsPerSecond()
        {
            watch = new System.Diagnostics.Stopwatch();
            Reset();
        }

        public void Reset()
        {
            watch.Reset();
            n1 = 0;
            timeout = watch.ElapsedMilliseconds;
            Count = 0;
            watch.Start();
        }

        public void Increment()
        {
            n1++;
            if (watch.ElapsedMilliseconds - timeout >= 1000)
            {
                Count = n1;
                n1 = 0;
                timeout = watch.ElapsedMilliseconds;
            }
        }
    }
}
