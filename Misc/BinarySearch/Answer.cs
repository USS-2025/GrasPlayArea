
namespace BinarySearch
{
    static class Answer
    {
        public static bool Exists(int[] ints, int k)
        {



            System.Diagnostics.Stopwatch sw = new();

            const int ALGORITHM_TO_USE = 2;

            switch (ALGORITHM_TO_USE)
            {
                case 1:
                    {
                        Console.WriteLine($"Used algorithm: {ALGORITHM_TO_USE}" +
                                            $" (System.Array.BinarySearch implemented in Array.CoreCLR.cs)");
                        sw.Restart();
                        // Algorithm 1 not working for IDE CoderPad Screen: Array.BinarySearch
                        int index = Array.BinarySearch(ints, k);
                        sw.Stop();

                        if (index >= 0)
                        {
                            Console.WriteLine($"{k} was found at index {index}, searching time was: {sw.Elapsed:g}");
                            return true;
                        }
                        else
                        {
                            // if index is negative, k could not be found
                            Console.WriteLine($"{k} could not be found, searching time was: {sw.Elapsed:g}");
                            return false;
                        }
                    }

                case 2:
                    {
                        Console.WriteLine($"Used algorithm: {ALGORITHM_TO_USE}" +
                                            $" (Own, manual implementation by binary searching in a while loop)");

                        // Algorithm 2 working for IDE CoderPad Screen: : Own, manual implementation 
                        int leftPos = 0;
                        int rightPos = ints.Length - 1;

                        sw.Restart();
                        while (leftPos <= rightPos)
                        {
                            int midPos = leftPos + (rightPos - leftPos) / 2;

                            //Debug.WriteLine($"{leftPos}-{midPos}-{rightPos}");

                            if (ints[midPos] == k)
                            {
                                sw.Stop();
                                Console.WriteLine($"{k} was found at index {midPos}, searching time was: {sw.Elapsed:g}");

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
                        sw.Stop();

                        // if no (ints[midPos] == k) matching, k could not be found
                        Console.WriteLine($"{k} could not be found, searching time was: {sw.Elapsed:g}");
                        return false;
                    }
                
                default: throw new NotImplementedException($"Algorithm {ALGORITHM_TO_USE} not implemented.");
            }
        
        
        
        
        }
    }
}
