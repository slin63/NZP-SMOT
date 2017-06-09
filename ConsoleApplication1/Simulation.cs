using System;

// More or less C# translations of the VBA code in the spreadsheet.
// The original functions are pretty large. Trying to modulate it.
// Will be directly reusing some of the original function names.
//      They don't make much sense so they'll be subject to change.
namespace SMOTCalc
{
    public class AMain
    {
        public static void Main(string[] args)
        {
            for (int i = 0; i <= 50; i = i + 1)
            {
                Console.WriteLine("value of a: {0}", i);
            }
        }

        public void AMainMaster()
        {
        }
    }
}

