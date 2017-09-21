using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Preprocessing
{
    static class Rainfall
    {
        // avg dry days = dryDays / dryPeriodcount
        const double dryDayThreshold = 0.0;
        const double rainThreshold = 0.1;

        static public double calcAverageDryDays(SMOT_IO.CSVFile rainfallSheet, SMOT_IO.InputParams input)        
        {
            // Every instance where the precip is less than the dryDayThreshold, is a dryDay
            // Every instance where the day before the current row's day is greater than threshold
            // is considered a dry period.
            double dryDayCount = 0.0;
            double dryPeriodCount = 0.0;

            List<double> rainPerDay = _rainPerDay(rainfallSheet, dryDayThreshold);

            for (int i = 0; i < rainPerDay.Count; i++)
            {
                if (rainPerDay[i] <= dryDayThreshold)
                {
                    dryDayCount += 1;
                    if (i > 0)
                    {
                        double previousRainfall = rainPerDay[i - 1];
                        if (previousRainfall > dryDayThreshold)
                            dryPeriodCount += 1;
                    }
                }
            }
            double avgDryDays = dryDayCount / dryPeriodCount;

            return avgDryDays;
        }
        
        static public double calc95thPercentile(SMOT_IO.CSVFile rainfallSheet, SMOT_IO.InputParams input)
        {
            List<double> rainPerDay = _rainPerDay(rainfallSheet, rainThreshold);
            double percentile95 = _percentile(rainPerDay, 0.95);

            return percentile95;
        }

        // https://stackoverflow.com/questions/8137391/percentile-calculation
        private static double _percentile(List<double> list, double excelPercentile)
        {
            double[] sequence = list.ToArray();
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

        private static List<double> _rainPerDay(SMOT_IO.CSVFile rainfallSheet, double threshold)
        {
            int i = 0;
            double currentTotalRain = 0;
            List<double> rainPerDay = new List<double>();
           
            foreach (SMOT_IO.CSVRow row in rainfallSheet.rows)
            {
                try
                {
                    DateTime currentDate = DateTime.Parse(row.row[0]);
                    currentTotalRain += Convert.ToDouble(row.row[1]);
                    i++;

                    if (i % 24 == 0)
                    {
                        i = 0;

                        if (currentTotalRain >= threshold)
                            rainPerDay.Add(currentTotalRain);

                        currentTotalRain = 0;
                    }
                }
                catch (System.FormatException e)
                {
                    Console.WriteLine("ERROR PARSING ROW: " + row.row[0] + " " + row.row[1] + " " + e.Message);
                }
            }

            return rainPerDay;
        }
    }
}
