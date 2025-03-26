// See https://aka.ms/new-console-template for more information

// Test: flscher deutscher Text
// Test: wrng English text

using BinarySearch;
using System.Diagnostics;
using static BinarySearch.FindPeaks;

int[] ints;

//int size = (int)Math.Pow(2, 31)-1;
int size = (int)Math.Pow(2, 26)-1;
Console.WriteLine($"Size of array: {size}");

Stopwatch sw = new();
sw.Restart();

ints = [.. Enumerable.Range(0, size).Take(size)];
Console.WriteLine($"Time to create Array: {sw.Elapsed:g}");

//sw.Restart();
//Array.Sort(ints);
//sw.Stop(); 
//Console.WriteLine($"Time to sort Array: {sw.Elapsed:g}");

int k = -1;

//Console.WriteLine(Answer.Exists(ints, k));


sw.Restart();
bool contains = ints.Contains(0);
sw.Stop(); 
Console.WriteLine($"Time to search by .Contains(): {sw.Elapsed:g}");

ints = [-9, 14, 37, 102];
Console.WriteLine(Answer.Exists(ints, 102)); // true
Console.WriteLine(Answer.Exists(ints, 36)); // false





int peaksCount = CountPeaks(new([1.1, -5.5, 3.3, 14.4, 8.8, 16.6, 10.10]));

return;





