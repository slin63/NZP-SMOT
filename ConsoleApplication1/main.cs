using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TopMaster
{
    class testSpace
    {
        public static void Main(string[] args)
        {
            string fileString = "C:\\Users\\RDCERSL9\\Documents\\Visual Studio 2015\\Projects\\ConsoleApplication1\\ConsoleApplication1\\SampleInputs\\RainfallData.csv";
            SMOT_IO.CSVFile sampleData = new SMOT_IO.CSVFile(fileString, true);
            //double dryDays = Preprocessing.DryDays.calcAverageDryDays(sampleData);

        }
    }

    class ModelManager
    {
        SMOT_IO.InputParams input;
        SMOT_IO.AnalysisParams medianInfo;

        public ModelManager(SMOT_IO.InputParams input)
        {
            this.input = input;
        }

        public void RunModel()
        {
            if (_checkValidation()) // If all inputs are good
            {
                // Calculate the last bit of soil info needed
                this.medianInfo = _initAnalysisParams();

                // Begin analysis process
                this._Analysis();
            }
        }

        private void _analysis()
        {
            Analysis.runAnalysis(this.input, this.medianInfo);
        }

        private SMOT_IO.AnalysisParams _initAnalysisParams()
        {
            double totalSoilArea = totalHsgArea();
            return new SMOT_IO.AnalysisParams
            (totalSoilArea, input.hsgAreaA, input.hsgAreaB, input.hsgAreaC, input.hsgAreaD);

        }

        private bool _checkValidation()
        {
            return Validation.InputValidator.ValidateInputs(this.input);
        }
    }

    static class Analysis
    {
        private static String _blankField = "N/A";
        static public void runAnalysis(SMOT_IO.InputParams input, SMOT_IO.AnalysisParams medianInfo)
        {
            // TODO :: 
            // Currently void, but should return a detailed trace with analysis results.
            bool heterogeneity = _heteroAnalysis(medianInfo);


            // BREAK THESE LONG CONDITIONALS INTO PRIVATE SUB-FUNCTIONS. Get rid of them
            // i hate them

            // Checking if has no pond
            if (!input.majorOnlinePond)
            {
                // Checking for no WQ component conditions
                if ((!input.applTMDL && input.applTMDL_WQ == _blankField) ||
                    (input.applTMDL && input.applTMDL_WQ == _blankField))
                {
                    if ((!input.MS4 && input.MS4_WQ== _blankField) ||
                        (input.MS4 && input.MS4_WQ == _blankField))
                    {
                        _designForNoWQComponents(input, medianInfo);
                    }
                }

                // Checking for MS4 component conditions
                // TODO

                // Checking for TMDL component conditions
                // TODO

                // Checking for both TMLD/MS4 component conditions
                // TODO
            }

            // Checking if has pond
        }

        static private void _designForNoWQComponents(SMOT_IO.InputParams input, SMOT_IO.AnalysisParams medianInfo)
        {
            ;
        }

        static private bool _heteroAnalysis(SMOT_IO.AnalysisParams median)
        {
            // Tells whether or not a soils PCT Result is heterogeneous      
            // This was painful for me. I dont even know if the program uses this function
            // VBA is so vague
            bool heterogeneity = false;
            if ((median.hsgAPCTResult >= 0.2 &&
                median.hsgAPCTResult <= 0.3) &&
                (median.hsgBPCTResult >= 0.2 &&
                median.hsgBPCTResult <= 0.3) &&
                (median.hsgCPCTResult >= 0.2 &&
                median.hsgCPCTResult <= 0.3) &&
                (median.hsgDPCTResult >= 0.2 &&
                median.hsgDPCTResult <= 0.3))
            {
                heterogeneity = true;
            }
            else if (median.hsgAPCTResult >= 0.7)
            {
                if (median.hsgDPCTResult >= 0.2)
                    heterogeneity = true;
                else
                    heterogeneity = false;
            }

            else if (median.hsgBPCTResult >= 0.7)
                heterogeneity = false;

            else if (median.hsgCPCTResult >= 0.7)
                heterogeneity = false;

            else if (median.hsgDPCTResult >= 0.7)
            {
                if (median.hsgAPCTResult >= 0.2)
                    heterogeneity = true;
                else
                    heterogeneity = false;
            }

            else if (median.hsgAPCTResult >= 0.45)
            {
                if (median.hsgBPCTResult >= 0.45)
                    heterogeneity = false;
                else if ((median.hsgBPCTResult >= 0.2 && median.hsgDPCTResult >= 0.2) &&
                         (median.hsgBPCTResult + median.hsgCPCTResult >= 0.45))
                    heterogeneity = false;
                else
                    heterogeneity = true;
            }

            else if (median.hsgBPCTResult >= 0.45)
            {
                if ((median.hsgAPCTResult >= 0.2 && median.hsgDPCTResult >= 0.2) &&
                    (median.hsgAPCTResult + median.hsgDPCTResult >= 0.45))
                    heterogeneity = false;
                else
                    heterogeneity = true;
            }

            else if (median.hsgCPCTResult >= 0.45)
            {
                if ((median.hsgAPCTResult >= 0.2 && median.hsgDPCTResult >= 0.2) &&
                    (median.hsgAPCTResult + median.hsgDPCTResult >= 0.45))
                    heterogeneity = false;
                else
                    heterogeneity = true;
            }

            else if (median.hsgDPCTResult >= 0.45)
            {
                if ((median.hsgBPCTResult >= 0.2 && median.hsgCPCTResult >= 0.2) &&
                    (median.hsgBPCTResult + median.hsgCPCTResult >= 0.45))
                    heterogeneity = false;
                else
                    heterogeneity = true;
            }
            
            return heterogeneity;
        }
    }

}