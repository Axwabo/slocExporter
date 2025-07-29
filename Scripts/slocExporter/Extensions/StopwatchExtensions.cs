using System;
using System.Diagnostics;

namespace slocExporter.Extensions
{

    public static class StopwatchExtensions
    {

        private const double Frequency = 1000000000;

        private const double TickFrequency = TimeSpan.TicksPerSecond / Frequency;

        public static long Timestamp => Stopwatch.GetTimestamp();

        public static TimeSpan GetElapsedTime(long start, long end) => new((long) ((end - start) * TickFrequency));

        public static TimeSpan GetElapsedTime(long start) => GetElapsedTime(start, Timestamp);

    }

}
