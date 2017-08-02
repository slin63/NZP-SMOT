using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SMOT_IO;
using System.IO;

namespace Simulation
{
    // http://epplus.codeplex.com/wikipage?title=WebapplicationExample
    static class AreaSolver
    {
        static public double SolveBMPArea(List<Timeseries> timeseries, Modules.BMP bmp, double yearsSimulated)
        {
            double netOverflow = 0;
            double netPRunoff = _netPRunoff(timeseries);
            double storage = 0;
            double currentArea = bmp.bmpArea;

            double areaDependentDepth;

            do
            {
                {
                    netOverflow = 0;

                    for (int i = 0; i < timeseries.Count; i++)
                    {
                        Timeseries currentRow = timeseries[i];
                        areaDependentDepth = (currentRow.runoffImp * 43560.0 / currentArea);

                        if (i == 0)
                        {
                            //    0 OR Minimum(BMPDepth, BMPDepth + ET + Infiltration - BMPDepth_Calculated)
                            storage = Math.Max(0, Math.Min(bmp.bmpDepth, bmp.bmpDepth + currentRow.ET + bmp.bmpInfilt - areaDependentDepth));
                            //    0 OR BMPDepth_Calculated - BMPDepth + Infiltration + ET
                            netOverflow += Math.Max(0, areaDependentDepth - bmp.bmpDepth + bmp.bmpInfilt + currentRow.ET);
                        }

                        else
                        {
                            double previousStorage = storage;
                            //    0 OR Minimum(BMPDepth, Previous BMPStorage + ET + Infiltration - BMPDepth_Calculated
                            storage = Math.Max(0, Math.Min(bmp.bmpDepth, previousStorage + currentRow.ET + bmp.bmpInfilt - areaDependentDepth));
                            //    MAX(0, BMPDepth_Calculated - (Previous BMPStorage + Infiltration + ET)) * CurrentArea / 43560 
                            netOverflow += Math.Max(0, areaDependentDepth - (previousStorage + bmp.bmpInfilt + currentRow.ET)) * currentArea / 43560;
                        }
                    }

                    Console.WriteLine(String.Format("DIFF: {0}\n\tAREA: {1}", netPRunoff - netOverflow, currentArea));

                    currentArea += 1;

                    //if ((netPRunoff - netOverflow) / yearsSimulated > 0)
                    //{
                    //    Console.WriteLine("netPRunoff: {0}\nnetOverFlow: {1}\nYears: {2}", netPRunoff, netOverflow, yearsSimulated);
                    //}
                }
            }
            while (((netPRunoff - netOverflow) / yearsSimulated) < 0);

            return currentArea;
        }

        private static double _netPRunoff(List<Timeseries> timeseries)
        {
            double netPRunoff = 0;
            foreach (Timeseries row in timeseries)
            {
                netPRunoff += row.runoffPerv;
            }
            return netPRunoff;
        }
    }   
}


