using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simulation
{
    using timeseriesTup = Tuple<DateTime, double, double, double>;

    static class ContinuousSim
    {
        static double DAYS_IN_YEAR = 365;

        public static SimTrace AMain(SMOT_IO.InputParams input, SMOT_IO.AnalysisParams medianInfo)
        {
            SimTrace simTrace = new Simulation.SimTrace();

            // Initialize all watershed objects
            Modules.Watershed watershed = new Modules.Watershed
                (input.avgHorton.infilMax, input.avgHorton.infilMin, input.avgHorton.decayRate,
                 input.storageDepthPervious, input.storageDepthImpervious,
                 input.totalImpArea, input.averageDryDays, input.baseName);

            Modules.BMP bmp = new Modules.BMP(watershed, input);

            // There's this AddArea function in the VBA code
            // I can't find a definition for. What does it do?
            // ReDim AddArea(NumWatersheds)

            // Calculate hourly water balance
            List<timeseriesTup> timeseriesSummary = _calculateWaterBalance(input.ETInfo, medianInfo, ref watershed, ref bmp);

            // Using the Excel solver add-in, determine required BMP surface areas
            _findSurfaceArea(watershed, ref bmp, medianInfo.rainfallTimeseries, timeseriesSummary);

            return simTrace;
        }

        private static void _findSurfaceArea(Modules.Watershed watershed, ref Modules.BMP bmp, SMOT_IO.CSVFile rainfallTimeseries,
                                             List<timeseriesTup> timeseriesSummary)
        {
            // Calculate the number of years simulated
            DateTime firstDate = DateTime.Parse(rainfallTimeseries.rows[0].row[0]);
            DateTime lastDate = DateTime.Parse(rainfallTimeseries.rows[rainfallTimeseries.rows.Count - 1].row[0]);
            var timeSpan = (lastDate - firstDate).Days;
            double yearsSimulated = timeSpan / DAYS_IN_YEAR;

            double netBmpOverflow = __netBmpOverflow(timeseriesSummary, yearsSimulated);
            double netPervRunoff = __netPervRunoff(timeseriesSummary, yearsSimulated);

        //If simMethod Then
            //Call Simulation.RunSolver(Sheets(myWS.SWSID))
            //myBMP.bmpArea = Sheets(myWS.SWSID).Range("$A$1").Value
        //End If

        //' Copy and paste values to remove active worksheet formuals.
        //Set tmpRange = Sheets(myWS.SWSID).Range("$I$2")
        //Set tmpRange = Range(tmpRange, tmpRange.End(xlDown))
        //Set tmpRange = Range(tmpRange, tmpRange.Offset(0, 2))
        //tmpRange.Copy
        //tmpRange.PasteSpecial Paste:= xlPasteValues

        }
        private static List<timeseriesTup> _calculateWaterBalance(SMOT_IO.ETInfo ETInfo, SMOT_IO.AnalysisParams medianInfo,
                                                   ref Modules.Watershed watershed, ref Modules.BMP bmp)
        {
            // Loops through the precipitation timeseries and
            // calculates the watershed and BMP water balance
            // at each timestep.
            List<timeseriesTup> runoff = new List<timeseriesTup>();
            foreach (SMOT_IO.CSVRow currentRow in medianInfo.rainfallTimeseries.rows)
            {
                // info :: list<String> formatted ['Element1', 'Element2', 'etc']
                // [1] = Rainfall depth, [0] = Date/Time
                List<String> info = currentRow.row;
                double currentRainfall = Convert.ToDouble(info[1]);
                DateTime currentDate = DateTime.Parse(info[0]);
                double currentET = __getMonthlyET(ETInfo, currentDate);

                double pervR = watershed.CalculatePerviousRunoff(currentRainfall, currentET);
                double impvR = watershed.CalculateImperviousRunoff(currentRainfall, currentET);
                double overflow = bmp.CalculateBMPOverflow(currentET);

                var summary = new timeseriesTup(currentDate, pervR, impvR, overflow);
                runoff.Add(summary);
            }

            return runoff;
        }

        private static double __netBmpOverflow(List<timeseriesTup> timeseriesSummary, double yearsSimulated)
        {
            double netBmpOverflow = 0.0;

            foreach (timeseriesTup row in timeseriesSummary)
            {
                netBmpOverflow += row.Item3; // row[3] = Overflow for that time step
            }
            return netBmpOverflow / yearsSimulated;
        }

        private static double __netPervRunoff(List<timeseriesTup> timeseriesSummary, double yearsSimulated)
        {
            double netPervRunoff = 0.0;

            foreach (timeseriesTup row in timeseriesSummary)
            {
                netPervRunoff += row.Item2; // row[2] = Pervious runoff for that time step
            }
            return netPervRunoff / yearsSimulated;
        }

        private static double __getMonthlyET(SMOT_IO.ETInfo ETInfo, DateTime currentDate)
        {
            int month = currentDate.Month;
            return ETInfo.getMonthlyET(month);
        }
    }

    class SimTrace
    {
        // TODO
        // Placeholder class for whatever we'll need the simulation to return later
        public SimTrace()
        {
            throw new NotImplementedException();
        }
    }
}
