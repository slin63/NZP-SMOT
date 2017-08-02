using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using Excel = Microsoft.Office.Interop.Excel;

namespace ExcelInterop
{
    public static class LoveExcel
    {
        // TODO: Clear old solver code
        public static void SaveAsTSV(string fileDir)
        {
            //~~> Define your Excel Objects
            Excel.Application xlApp = new Excel.Application();
            Excel.Workbook xlWorkBook;
            Excel.Worksheet worksheet;

            //~~> Start Excel and open the workbook.
            xlWorkBook = xlApp.Workbooks.Open(fileDir);

            worksheet = (Microsoft.Office.Interop.Excel.Worksheet)xlWorkBook.Worksheets[1];
            xlWorkBook.SaveAs(_changeExtension(fileDir, "dat"), Microsoft.Office.Interop.Excel.XlFileFormat.xlTextWindows, Missing.Value, Missing.Value, Missing.Value, false, Microsoft.Office.Interop.Excel.XlSaveAsAccessMode.xlNoChange, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value);

            xlWorkBook.Close(false, Missing.Value, Missing.Value);
            xlApp.Quit();
        }

        //public static void runMacro(string fileDir, string macro)
        //{
        //    //~~> Define your Excel Objects
        //    Excel.Application xlApp = new Excel.Application();
        //    Excel.Workbook xlWorkBook;

        //    //~~> Start Excel and open the workbook.
        //    xlWorkBook = xlApp.Workbooks.Open(fileDir);

        //    //~~> Run the macros by supplying the necessary arguments
        //    xlApp.Run(macro);

        //    //~~> Clean-up: Close the workbook
        //    xlWorkBook.Close(false);

        //    //~~> Quit the Excel Application
        //    xlApp.Quit();

        //    //~~> Clean Up
        //    _releaseObject(xlApp);
        //    _releaseObject(xlWorkBook);
        //}

        private static string _changeExtension(string fileDir, string extension)
        {
            int extensionIndex = fileDir.LastIndexOf('.');
            return fileDir.Substring(0, extensionIndex) + String.Format(".{0}", extension);
        }

        //~~> Release the objects
        //private static void _releaseObject(object obj)
        //{
        //    try
        //    {
        //        System.Runtime.InteropServices.Marshal.ReleaseComObject(obj);
        //        obj = null;
        //    }
        //    catch (Exception ex)
        //    {
        //        obj = null;
        //    }
        //    finally
        //    {
        //        GC.Collect();
        //    }
        //}
    }
}