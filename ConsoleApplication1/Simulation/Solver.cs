using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Simulation
{
    // http://epplus.codeplex.com/wikipage?title=WebapplicationExample
    static class AreaSolver
    {
        // BMP Depth, Storage, and overflow will be calculated during this . Not beforehand!!
        static public void SolveBMPArea(List<SMOT_IO.Timeseries> timeseries, Modules.BMP bmp, int timeSpan, double yearsSimulated)
        {
            #region debug
            string outDir = @"C:\Users\RDCERSL9\Documents\Visual Studio 2015\Projects\ConsoleApplication1\ConsoleApplication1\bin\Debug\";
            #endregion
            var solveFile = new System.IO.FileInfo(outDir + "solveFile.xlsx");

            using (OfficeOpenXml.ExcelPackage xlPackage = new OfficeOpenXml.ExcelPackage(solveFile))
            {
                OfficeOpenXml.ExcelWorksheet bmpWS = xlPackage.Workbook.Worksheets.Add("SolveBMPArea");
                _populateTimeSeries(timeseries, ref bmpWS, timeSpan, yearsSimulated, bmp);
                xlPackage.Save();
            }
        }

        static private void _populateTimeSeries(List<SMOT_IO.Timeseries> timeseries, ref OfficeOpenXml.ExcelWorksheet ws,
            int timeSpan, double yearsSimulated, Modules.BMP bmp)
        {
            // Writing headers
            ws.Cells["A1"].Value = bmp.bmpArea;
            ws.Cells["B1"].Value = "DATE";
            ws.Cells["C1"].Value = "RAIN";
            ws.Cells["D1"].Value = "ET";
            ws.Cells["E1"].Value = "IMP_STOR";
            ws.Cells["F1"].Value = "PERV_STOR";
            ws.Cells["G1"].Value = "IMP_RUN";
            ws.Cells["H1"].Value = "PERV_RUN";
            ws.Cells["I1"].Value = "BMPDEPTH";
            ws.Cells["J1"].Value = "STORAGE";
            ws.Cells["K1"].Value = "OVERFLOW";

            int _row = 2;

            // Actually outputting the data and the appropriate formulas for BMPDEPTH, STORAGE, OVERFLOW
            foreach (SMOT_IO.Timeseries row in timeseries)
            {
                String storage;
                String overflow;

                ws.Cells["B" + _row.ToString()].Value = row.date.ToString();
                ws.Cells["C" + _row.ToString()].Value = row.rainfall;
                ws.Cells["D" + _row.ToString()].Value = row.ET;
                ws.Cells["E" + _row.ToString()].Value = row.storeImp;
                ws.Cells["F" + _row.ToString()].Value = row.storePerv;
                ws.Cells["G" + _row.ToString()].Value = row.runoffImp;
                ws.Cells["H" + _row.ToString()].Value = row.runoffPerv;
                ws.Cells["I" + _row.ToString()].Formula = String.Format("$G${0}*43560/$A$1", _row);

                if (_row == 2)
                {
                    storage = String.Format(@"MAX(0,MIN({0}, {0}+$D${1}+{2}-$I${1}))", bmp.bmpDepth, _row, bmp.bmpInfilt);
                    overflow = String.Format(@"MAX(0,$I${0}-({1}+{2}+$D${0}))", _row, bmp.bmpDepth, bmp.bmpInfilt);
                }

                else
                {
                    storage = String.Format(@"MAX(0,MIN({0}, $J${3}+$D${1}+{2}-$I${1}))", bmp.bmpDepth, _row, bmp.bmpInfilt, _row - 1);
                    overflow = String.Format(@"MAX(0,$I${0}-($J${1}+{2}+$D${0})) * $A$1/43560", _row, _row - 1, bmp.bmpInfilt);
                }

                ws.Cells["J" + _row.ToString()].Formula = storage;
                ws.Cells["K" + _row.ToString()].Formula = overflow;

                _row++;
            }

            // Summary cells for solver to use later (Overflow, Pervious runoff)
            ws.Cells["$M$1"].Formula = String.Format("SUM(K2:K{0}) / {1}", _row, yearsSimulated);
            ws.Cells["$N$1"].Formula = String.Format("SUM(H2:H{0}) / {1}", _row, yearsSimulated);
            //Sheets(myWS.SWSID).Range("$M$1").Value = "=SUM(" & Range("K2", Range("K2").End(xlDown)).Address & ")/" & NumYears
            //Sheets(myWS.SWSID).Range("$N$1").Value = "=SUM(" & Range("H2", Range("H2").End(xlDown)).Address & ")/" & NumYears
        }
    }
}

