namespace Blackbox
{
    internal static  class Extensions
    {
        public static string GetFormattedByteSize(this long bytesCount)
        {
            string[] sizes = ["Bytes", "KB", "MB", "GB", "TB"];
            int order = 0;

            // AI (ChatGPT-4o) generated code here (Kurwa mać!)
            while (bytesCount >= 1024 && order < sizes.Length - 1)
            {
                order++;
                bytesCount /= 1024;
            }

            return $"{bytesCount:0.##} {sizes[order]}";
        }

        public static string GetFormattedByteSize(this IEnumerable<byte> bytes)
            => ((long)bytes.Count()).GetFormattedByteSize();

        public static string GetFormattedByteSize(this MemoryStream ms)
            => ms.Length.GetFormattedByteSize();

    }
}
