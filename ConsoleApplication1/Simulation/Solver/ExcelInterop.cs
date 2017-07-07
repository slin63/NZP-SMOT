using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using Excel = Microsoft.Office.Interop.Excel;

namespace ExcelInterop
{
    public static class LoveExcel
    {
        public static void runMacro(string fileDir, string macro)
        {
            //~~> Define your Excel Objects
            Excel.Application xlApp = new Excel.Application();
            Excel.Workbook xlWorkBook;

            //~~> Start Excel and open the workbook.
            xlWorkBook = xlApp.Workbooks.Open(fileDir);

            //~~> Run the macros by supplying the necessary arguments
            xlApp.Run(macro);

            //~~> Clean-up: Close the workbook
            xlWorkBook.Close(false);

            //~~> Quit the Excel Application
            xlApp.Quit();

            //~~> Clean Up
            _releaseObject(xlApp);
            _releaseObject(xlWorkBook);
        }

        //~~> Release the objects
        private static void _releaseObject(object obj)
        {
            try
            {
                System.Runtime.InteropServices.Marshal.ReleaseComObject(obj);
                obj = null;
            }
            catch (Exception ex)
            {
                obj = null;
            }
            finally
            {
                GC.Collect();
            }
        }
    }
}