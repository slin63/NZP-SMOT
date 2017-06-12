using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RainfallProcessing
{
    static class DryDays
    {
        // avg dry days = dryDays / dryPeriodcount
        const double dryDayThreshold = 0.0;
        // Every instance where the precip is less than the dryDayThreshold, is a dryDay
        // Every instance where the day before the current row's day is greater than threshold
        //              it is considered a dry period.
        static public double calcAverageDryDays(SMOT_IO.CSVFile dryDaySheet, ref SMOT_IO.SMOTParameters input)
        {
            //    double dryDayCount = 0.0;
            //    double dryPeriodCount = 0.0;
            //    double avgDryDays = 0.0;

            //    for(int i = 0; i <= dryDaySheet.rows(); i += 1)
            //    {
            //        if (dryDaySheet[index = i] <= dryDayThreshold)
            //        {
            //            dryDayCount++;
            //            if (i > 0 && dryDaySheet[indexer = i - 1] >= dryDayThreshold)
            //            {
            //                dryPeriodCount++;
            //            }
            //        }
            //    }

            //    return avgDryDays = dryDayCount / dryPeriodCount;
            return -1;
        }
}

    static class Percentile95Rain
    {
        const double threshhold = 0.1;

        static public double calc95thPercentile(SMOT_IO.CSVFile rainfallSheet, ref SMOT_IO.SMOTParameters input)
        {
            // The calculation in the original VBA code doesn't make much sense.
            // This calculation is a placeholder for now. 
            // DO NOT USE IN PRODUCTION!
            return -1;
        }
    }

}
