// See https://aka.ms/new-console-template for more information

// Test: flscher deutscher Text
// Test: wrng English text

using System.Diagnostics;
using BinarySearch;

int size = (int)Math.Pow(2, 31)-1;
Console.WriteLine($"Size of array: {size}");

Stopwatch sw = new();
sw.Restart();
int[] ints = [.. Enumerable.Range(0, size).Take(size)];
Console.WriteLine($"Time to create Array: {sw.Elapsed:g}");

//sw.Restart();
//Array.Sort(ints);
//Console.WriteLine($"Time to sort Array: {sw.Elapsed:g}");

sw.Restart();
int index = Array.BinarySearch(ints, -1);
Console.WriteLine($"Time to search by Array.BinarySearch(ints, 0): {sw.Elapsed:g}");
Console.WriteLine($"Found index: {index}");

sw.Restart();
Console.WriteLine($"Found index: {Answer.Exists(ints, 0)}");
Console.WriteLine($"Time to search by Answer.Exists(ints, 0): {sw.Elapsed:g}");

sw.Restart();
Console.WriteLine(ints.Contains(0));
Console.WriteLine($"Time to search by .Contains(): {sw.Elapsed:g}");

return;

namespace BinarySearch
{
    static class Answer
    {
        public static bool Exists(int[] ints, int k)
        {
            // Binäre Suche verwenden, da das Array sortiert ist (O(log n) Laufzeit)

            int leftPos = 0;
            int rightPos = ints.Length - 1;

            while (leftPos <= rightPos)
            {
                int midPos = leftPos + (rightPos - leftPos) / 2;

                //Debug.WriteLine($"{leftPos}-{midPos}-{rightPos}");

                if (ints[midPos] == k)
                {
                    return true;
                }
                else if (ints[midPos] < k)
                {
                    leftPos = midPos + 1;
                }
                else
                {
                    rightPos = midPos - 1;
                }
            }

            return false;
        }
    }

}



