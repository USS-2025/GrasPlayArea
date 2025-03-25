// See https://aka.ms/new-console-template for more information

// Test: flscher deutscher Text
// Test: wrng English text

int[] ints = { -9, 14, 37, 102 };

var testVarible = 1;

Console.WriteLine(Answer.Exists(ints, 102)); // true
Console.WriteLine(Answer.Exists(ints, 36)); // false

public class Answer
{
    public static bool Exists(int[] ints, int k)
    {
        // Binäre Suche verwenden, da das Array sortiert ist (O(log n) Laufzeit)
        return Array.BinarySearch(ints, k) >= 0;

        int leftPos = 0;
        int rightPos = ints.Length - 1;

        while(leftPos <= rightPos)
        {
            int midPos = leftPos + (rightPos - leftPos) / 2;
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







