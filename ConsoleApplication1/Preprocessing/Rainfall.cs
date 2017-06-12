using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Preprocessing
{
    static class DryDays
    {
        // avg dry days = dryDays / dryPeriodcount
        const double dryDayThreshold = 0.0;
        // Every instance where the precip is less than the dryDayThreshold, is a dryDay
        // Every instance where the day before the current row's day is greater than threshold
        //              it is considered a dry period.
        static public double calcAverageDryDays(SMOT_IO.CSVFile dryDaySheet, ref SMOT_IO.InputParameters input)
        // The calculation in the original VBA code doesn't make much sense.
        // This calculation is a placeholder for now. 
        // DO NOT USE IN PRODUCTION!
        {
            double dryDayCount = 0.0;
            double dryPeriodCount = 0.0;

            for (int i = 0; i < dryDaySheet.length(); i++)
            {
                // currentRow :: list<String> formatted ['Element1', 'Element2', 'etc']
                List<String> currentRow = dryDaySheet.rows[i].row;
                double currentRainfall = Convert.ToDouble(currentRow[1]);
                // [1] = Rainfall depth, [0] = Date/Time
                if (currentRainfall <= dryDayThreshold)
                {
                    dryDayCount += 1;
                    if (i > 0)
                    {
                        double previousRainfall = Convert.ToDouble(dryDaySheet.rows[i - 1].row[1]);
                        if (previousRainfall > dryDayThreshold)
                            dryPeriodCount += 1;
                    }
                }
            }

            double avgDryDays = dryDayCount / dryPeriodCount;
            return -1;
        }
    }

    static class Percentile95Rain
    {
        const double threshhold = 0.1;

        static public double calc95thPercentile(SMOT_IO.CSVFile rainfallSheet, ref SMOT_IO.InputParameters input)
        {
            // The calculation in the original VBA code doesn't make much sense.
            // This calculation is a placeholder for now. 
            // DO NOT USE IN PRODUCTION!
            return -1;
        }

        // https://stackoverflow.com/questions/8137391/percentile-calculation
        private static double _percentile(double[] sequence, double excelPercentile)
        {
            Array.Sort(sequence);
            int N = sequence.Length;
            double n = (N - 1) * excelPercentile + 1;
            // Another method: double n = (N + 1) * excelPercentile;
            if (n == 1d) return sequence[0];
            else if (n == N) return sequence[N - 1];
            else
            {
                int k = (int)n;
                double d = n - k;
                return sequence[k - 1] + d * (sequence[k] - sequence[k - 1]);
            }
        }
    }

}
