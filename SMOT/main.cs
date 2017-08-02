using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SMOT_IO;

namespace Exec
{
    class testSpace
    {
        public static void Main(string[] args)
        {
            //SMOT_IO.CSVFile rainfallTimeSeriesCSV = new SMOT_IO.CSVFile("C:\\Users\\RDCERSL9\\Documents\\Visual Studio 2015\\Projects\\ConsoleApplication1\\SMOT\\SampleInputs\\RainfallData.csv", true);
            SMOT_IO.CSVFile rainfallTimeSeriesCSV = new SMOT_IO.CSVFile("..\\..\\SampleInputs\\RainfallData.csv", true);
            TestModels.SampleInputParams testIn = new TestModels.SampleInputParams();
            SMOT_IO.InputParams input = testIn.input;
            // How are we going to load up the CSV on NZP? 
            //  -> Use a ridiculously long CLI call and have a CLARGS parser read it.
            ModelExecutor modelManager = new ModelExecutor(input, rainfallTimeSeriesCSV);
            modelManager.RunModel();
            modelManager.PresentOutput();
        }
    }

    class ModelExecutor
    {
        SMOT_IO.InputParams input;
        SMOT_IO.CSVFile rainfallTimeseries;
        SMOT_IO.AnalysisParams medianInfo;

        Simulation.AnalysisTrace outputInfo;

        public ModelExecutor(SMOT_IO.InputParams input, SMOT_IO.CSVFile rainfallTimeseries)
        {
            this.input = input;
            this.rainfallTimeseries = rainfallTimeseries;
        }

        public void PresentOutput()
        {
            Console.WriteLine("REC: {0}\nREASON: {1}", outputInfo.analysisRec, outputInfo.reason);
        }

        public void RunModel()
        {
            // Calculate the last bit of soil info needed
            this.medianInfo = _initAnalysisParams();
            this.medianInfo.rainfallCSV = rainfallTimeseries;
            _applyPreprocessing();

            if (_checkValidation()) // If all inputs are good 
            {         
                // Begin analysis process
                this.outputInfo = this._analysis();
            }
            else
            {
                throw new ArgumentException("Invalid inputs!");
            }
        }

        private void _applyPreprocessing()
        {
            Preprocessing.HSGPreprocessing.averageHSGInfo(ref this.input);
            this.input.averageDryDays = Preprocessing.Rainfall.calcAverageDryDays
                                            (this.medianInfo.rainfallCSV, this.input);
            this.input.percentile95Rainfall = Preprocessing.Rainfall.calcAverageDryDays
                                            (this.medianInfo.rainfallCSV, this.input);
        }

        private Simulation.AnalysisTrace _analysis()
        {
           return Simulation.Analysis.RunAnalysis(this.input, this.medianInfo);
        }

        private SMOT_IO.AnalysisParams _initAnalysisParams()
        {
            double totalSoilArea = _totalHsgArea();
            return new SMOT_IO.AnalysisParams(totalSoilArea, input.hsgAreaA, 
                                              input.hsgAreaB, input.hsgAreaC, input.hsgAreaD);
        }

        private bool _checkValidation()
        {
            return Validation.InputValidator.ValidateInputs(this.input);
        }

        private double _totalHsgArea()
        {
            return (input.hsgAreaA + input.hsgAreaB + input.hsgAreaC + input.hsgAreaD);
        }
    }

    

}