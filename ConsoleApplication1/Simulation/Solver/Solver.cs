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
        // TODO: Remove this and make it read directories as NZP requires
        const string outDir = @"C:\Users\RDCERSL9\Documents\Visual Studio 2015\Projects\ConsoleApplication1\ConsoleApplication1\bin\Debug\";

        // BMP Depth, Storage, and overflow will be calculated during this . Not beforehand!!
        //static public void SolveBMPArea(List<SMOT_IO.Timeseries> timeseries, Modules.BMP bmp, int timeSpan, double yearsSimulated)
        //{
        //    _clearFiles(outDir, "xls");
        //    _clearFiles(outDir, "dat");

        //    var solveFile = new System.IO.FileInfo(outDir + "solveFile.xls");

        //    double bmpArea = _SolveBMPArea(timeseries, bmp, yearsSimulated);

        //    //using (OfficeOpenXml.ExcelPackage xlPackage = new OfficeOpenXml.ExcelPackage(solveFile))
        //    //{
        //    //    OfficeOpenXml.ExcelWorksheet bmpWS = xlPackage.Workbook.Worksheets.Add("SolveBMPArea");
        //    //    _populateTimeSeries(timeseries, ref bmpWS, timeSpan, yearsSimulated, bmp);

        //    //    OfficeOpenXml.ExcelWorkbook bmpWB = xlPackage.Workbook;
        //    //    //_writeSolveMacro(timeseries, ref bmpWB);

        //    //    xlPackage.Save();

        //    //    ExcelInterop.LoveExcel.SaveAsTSV(outDir + "solveFile.xls");
        //    //}
        //}

        static public double SolveBMPArea(List<Timeseries> timeseries, Modules.BMP bmp, double yearsSimulated)
        {
            double netOverflow = 0;
            double netPRunoff  = _netPRunoff(timeseries);
            double storage     = 0;
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

        private static void _clearFiles(string outDir, string extension)
        {
            foreach (string file in Directory.GetFiles(outDir, String.Format("*.{0}", extension)).Where(item => item.EndsWith(String.Format(".{0}", extension)))) 
            {
                File.Delete(file);
            }
        }

        //static private void _writeSolveMacro(List<Timeseries> timeseries, ref OfficeOpenXml.ExcelWorkbook wb)
        //{
        //    // https://www.codeproject.com/Questions/757102/How-to-add-macro-in-excel-worksheet-using-Epplus-l
        //    wb.CreateVBAProject();
        //    wb.CodeModule.Name = "solver";
        //    wb.CodeModule.Code = SolverCode.solverCode;
        //}

        static private void _populateTimeSeries(List<SMOT_IO.Timeseries> timeseries, ref OfficeOpenXml.ExcelWorksheet ws,
            int timeSpan, double yearsSimulated, Modules.BMP bmp)
        {
            // Writing headers
            ws.Cells["A1"].Value = "param";
            ws.Cells["B1"].Value = ":";
            ws.Cells["C1"].Value = "DATE";
            ws.Cells["D1"].Value = "RAIN";
            ws.Cells["E1"].Value = "ET";
            ws.Cells["F1"].Value = "IMP_STOR";
            ws.Cells["G1"].Value = "PERV_STOR";
            ws.Cells["H1"].Value = "IMP_RUN";
            ws.Cells["I1"].Value = "PERV_RUN";
            //ws.Cells["J1"].Value = "BMPDEPTH";
            //ws.Cells["K1"].Value = "STORAGE";
            //ws.Cells["L1"].Value = "OVERFLOW";
            //ws.Cells["M1"].Value = ":=";
            ws.Cells["J1"].Value = ":=";

            int _row = 2;

            // Actually outputting the data and the appropriate formulas for BMPDEPTH, STORAGE, OVERFLOW
            foreach (SMOT_IO.Timeseries row in timeseries)
            {
                String storage;
                String overflow;
                ws.Cells["A" + _row.ToString()].Value = _row - 1;
                ws.Cells["C" + _row.ToString()].Value = row.date.ToString();
                ws.Cells["D" + _row.ToString()].Value = row.rainfall;
                ws.Cells["E" + _row.ToString()].Value = row.ET;
                ws.Cells["F" + _row.ToString()].Value = row.storeImp;
                ws.Cells["G" + _row.ToString()].Value = row.storePerv;
                ws.Cells["H" + _row.ToString()].Value = row.runoffImp;
                ws.Cells["I" + _row.ToString()].Value = row.runoffPerv;
                //ws.Cells["J" + _row.ToString()].Formula = String.Format("$G${0}*43560/$A$1", _row);

                //if (_row == 2)
                //{
                ////    0 OR Minimum(BMPDepth, BMPDepth + ET + Infiltration - BMPDepth_Calculated)
                //    storage = String.Format(@"MAX(0,MIN({0}, {0}+$D${1}+{2}-$I${1}))", bmp.bmpDepth, _row, bmp.bmpInfilt);
                ////    0 OR BMPDepth_Calculated - BMPDepth + Infiltration + ET
                //    overflow = String.Format(@"MAX(0,$I${0}-({1}+{2}+$D${0}))", _row, bmp.bmpDepth, bmp.bmpInfilt);
                //}

                //else
                //{
                ////    0 OR Minimum(BMPDepth, Previous BMPStorage + ET + Infiltration - BMPDepth_Calculated
                //    storage = String.Format(@"MAX(0,MIN({0}, $J${3}+$D${1}+{2}-$I${1}))", bmp.bmpDepth, _row, bmp.bmpInfilt, _row - 1);
                ////    0 OR BMPDepth_Calculated - Previous BMPStorage + Infiltration + ET
                //    overflow = String.Format(@"MAX(0,$I${0}-($J${1}+{2}+$D${0})) * $A$1/43560", _row, _row - 1, bmp.bmpInfilt);
                //}

                //ws.Cells["K" + _row.ToString()].Formula = storage;
                //ws.Cells["L" + _row.ToString()].Formula = overflow;

                _row++;
            }

            ws.Cells["A" + _row.ToString()].Value = ";";

            // Summary cells for solver to use later (Overflow, Pervious runoff)
            //ws.Cells["$M$1"].Formula = String.Format("SUM(K2:K{0}) / {1}", _row, yearsSimulated);
            //ws.Cells["$N$1"].Formula = String.Format("SUM(H2:H{0}) / {1}", _row, yearsSimulated);
        }
    }

    
    static class SolverCode
    {
        static public string solverCode = @"Private Sub solverLoop()
        Dim myWS As Worksheet
        Set myWS = Sheets(""SolveBMPArea"")

        Dim Result As Integer
        Application.ScreenUpdating = False
    
        ' Reset Solver.
        Application.Run ""Solver.xlam!SolverReset""

        ' Set up new analysis.
        Application.Run ""Solver.xlam!SolverOk"", myWS.Range(""$M$1""), 3, myWS.Range(""$N$1"").Value, myWS.Range(""$A$1""), 1
    
        ' Add constraints.
        Application.Run ""Solver.xlam!SolverAdd"", myWS.Range(""$A$1""), 3, 0
    
        ' Run the analysis.
        Result = Application.Run(""Solver.xlam!SolverSolve"", True)
    
        ' Finish the analysis.
        Application.Run ""Solver.xlam!SolverFinish""
    
        Do While Result = 0
            ' Reset Solver.
            Application.Run ""Solver.xlam!SolverReset""
            ' Set up new analysis.
            Application.Run ""Solver.xlam!SolverOk"", myWS.Range(""$M$1""), 3, myWS.Range(""$N$1"").Value, myWS.Range(""$A$1""), 1
            ' Add constraints.
            Application.Run ""Solver.xlam!SolverAdd"", myWS.Range(""$A$1""), 3, 0
            Application.Run ""Solver.xlam!SolverAdd"", myWS.Range(""$A$1""), 1, myWS.Range(""$A$1"") - 50
            Result = Application.Run(""Solver.xlam!SolverSolve"", True)
            Application.Run ""Solver.xlam!SolverFinish""
        
            If Result <> 0 Then
                myWS.Range(""$A$1"") = myWS.Range(""$A$1"") + 100
            End If

        Loop

End Sub";
    }
}


