using System;

namespace L05
{
    class Program
    {
        static void Main(string[] args)
        {
            new MetricsRepository().GetStatistics().GetAwaiter().GetResult();
        }
    }
}
