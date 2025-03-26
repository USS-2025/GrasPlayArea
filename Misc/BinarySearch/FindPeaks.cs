using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BinarySearch
{
    internal static class FindPeaks
    {
        public static int CountPeaks(List<double> values)
        {

            // Es ist xxx Uhr


            int peakCount = 0;

            // if <3 values, no left and right neighbor possible
            if (values.Count < 3)
                return 0;

            // first value hasn't got any left neighbor
            // and - vive versa - last value doesn't have any right neighbor
            for (int i = 1; i < values.Count - 1; i++)
            {
                // use 'var' if item type of given list should be changed
                var left = values[i - 1];
                var current = values[i];
                var right = values[i + 1];

                // check for top-peak (higher value than left and right neighbors)
                if (current >= left + 5 && current >= right + 5)
                {
                    Console.WriteLine($"Found top peak at index {i}: {current}");
                    peakCount++;
                }

                // check for bottom-peak (lower value than left and right neighbors)
                if (current <= left - 5 && current <= right - 5)
                {
                    Console.WriteLine($"Found bottom peak at index {i}: {current}");
                    peakCount++;
                }
            }

            Console.WriteLine($"Found {peakCount} peaks.");
            return peakCount;

        }
    }
}
