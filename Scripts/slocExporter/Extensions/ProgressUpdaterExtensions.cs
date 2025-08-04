namespace slocExporter.Extensions
{

    public static class ProgressUpdaterExtensions
    {

        public static void Count(this ProgressUpdater updater, int current, int total, string format)
        {
            var progress = current / (float) total;
            updater(string.Format(format, current, total, progress), progress);
        }

    }

}
