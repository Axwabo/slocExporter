using System;
using System.Diagnostics;

namespace slocExporter.Extensions
{

    public static class StopwatchExtensions
    {

        public static readonly double TickFrequency = (double) TimeSpan.TicksPerSecond / Stopwatch.Frequency;

        public static long Timestamp => Stopwatch.GetTimestamp();

        public static TimeSpan GetElapsedTime(long start, long end) => new((long) ((end - start) * TickFrequency));

        public static TimeSpan GetElapsedTime(long start) => GetElapsedTime(start, Timestamp);

    }

}
