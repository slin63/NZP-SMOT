using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SolverFoundation.Common;
using Microsoft.SolverFoundation.Solvers;



namespace Simulation
{
    static class ContinuousSim
    {
        static double DAYS_IN_YEAR = 365;

        public static SimModels AMain(SMOT_IO.InputParams input, SMOT_IO.AnalysisParams medianInfo)
        {
            // Initialize all watershed objects
            Modules.Watershed watershed = new Modules.Watershed
                (input.avgHorton.infilMax, input.avgHorton.infilMin, input.avgHorton.decayRate,
                 input.storageDepthPervious, input.storageDepthImpervious,
                 input.totalDevelImpArea, input.averageDryDays, input.baseName);

            Modules.BMP bmp = new Modules.BMP(watershed, input);

            // There's this AddArea function in the VBA code
            // I can't find a definition for. What does it do?
            // ReDim AddArea(NumWatersheds)

            // Calculate hourly water balance
            List<SMOT_IO.Timeseries> timeseriesSummary = _calculateWaterBalance(input.ETInfo, medianInfo, ref watershed, ref bmp, input);

            // Using the Excel solver add-in, determine required BMP surface areas
            _findSurfaceArea(watershed, ref bmp, medianInfo.rainfallCSV, timeseriesSummary);
            double bmpCost = bmp.evaluateCost();

            SimModels simTrace = new Simulation.SimModels(watershed, bmp);

            return simTrace;
        }

        private static void _findSurfaceArea(Modules.Watershed watershed, ref Modules.BMP bmp, SMOT_IO.CSVFile rainfallTimeseries,
                                             List<SMOT_IO.Timeseries> timeseriesSummary)
        {
            // Calculate the number of years simulated
            DateTime firstDate = DateTime.Parse(rainfallTimeseries.rows[0].row[0]);
            DateTime lastDate = DateTime.Parse(rainfallTimeseries.rows[rainfallTimeseries.rows.Count - 1].row[0]);
            var timeSpan = (lastDate - firstDate).Days;
            double yearsSimulated = timeSpan / DAYS_IN_YEAR;

            //double netBmpOverflow = __netBmpOverflow(timeseriesSummary, yearsSimulated);
            //double netPervRunoff = __netPervRunoff(timeseriesSummary, yearsSimulated);

            double bmpArea = _solveBmpArea(timeseriesSummary, bmp, timeSpan, yearsSimulated);

            bmp.bmpArea = bmpArea;
        }

        private static double _solveBmpArea(List<SMOT_IO.Timeseries> timeseries, Modules.BMP bmp, int timeSpan, double yearsSimulated)
        {
            // TODO!!!!!! FINISH WRITING THIS SOLVEBMPAREA FUNCTION
            Simulation.AreaSolver.SolveBMPArea(timeseries, bmp, timeSpan, yearsSimulated);

            return -9999;
        }

        private static List<SMOT_IO.Timeseries> _calculateWaterBalance(SMOT_IO.ETInfo ETInfo, SMOT_IO.AnalysisParams medianInfo,
                                                   ref Modules.Watershed watershed, ref Modules.BMP bmp, SMOT_IO.InputParams input)
        {
            // Loops through the precipitation timeseries and
            // calculates the watershed and BMP water balance
            // at each timestep.
            List<SMOT_IO.Timeseries> runoff = new List<SMOT_IO.Timeseries>();
            foreach (SMOT_IO.CSVRow currentRow in medianInfo.rainfallCSV.rows)
            {
                // info :: list<String> formatted ['Element1', 'Element2', 'etc']
                // [1] = Rainfall depth, [0] = Date/Time
                List<String> info = currentRow.row;
                double rainfall = Convert.ToDouble(info[1]);
                DateTime date = DateTime.Parse(info[0]);
                double ET = __getMonthlyET(ETInfo, date) / 24;

                double runoffPerv = watershed.CalculatePerviousRunoff(rainfall, ET);
                double runoffImp = watershed.CalculateImperviousRunoff(rainfall, ET);
                //double overflow = bmp.CalculateBMPOverflow(ET);

                SMOT_IO.Timeseries timeseries = new SMOT_IO.Timeseries(date, rainfall, ET,
                    watershed.impDstor, watershed.perDstor, runoffPerv, runoffImp);
                runoff.Add(timeseries);
            }

            return runoff;
        }

        //private static double __netBmpOverflow(List<SMOT_IO.Timeseries> timeseriesSummary, double yearsSimulated)
        //{
        //    double netBmpOverflow = 0.0;

        //    foreach (var row in timeseriesSummary)
        //    {
        //        netBmpOverflow += row.Item3; // row[3] = Overflow for that time step
        //    }
        //    return netBmpOverflow / yearsSimulated;
        //}

        //private static double __netPervRunoff(List<SMOT_IO.Timeseries> timeseriesSummary, double yearsSimulated)
        //{
        //    double netPervRunoff = 0.0;

        //    foreach (var row in timeseriesSummary)
        //    {
        //        netPervRunoff += row.Item2; // row[2] = Pervious runoff for that time step
        //    }
        //    return netPervRunoff / yearsSimulated;
        //}

        private static double __getMonthlyET(SMOT_IO.ETInfo ETInfo, DateTime currentDate)
        {
            int month = currentDate.Month;
            return ETInfo.getMonthlyET(month);
        }
    }

    class SimModels
    {
        public Modules.Watershed watershed;
        public Modules.BMP bmp;
        public SimModels(Modules.Watershed watershed, Modules.BMP bmp)
        {
            this.watershed = watershed;
            this.bmp = bmp;
        }
    }
}
