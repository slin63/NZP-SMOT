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
            //SMOT_IO.XMLReader reader = new SMOT_IO.XMLReader("C:\\Users\\RDCERSL9\\Documents\\Visual Studio 2015\\Projects\\ConsoleApplication1\\ConsoleApplication1\\SampleInputs\\SMOTInput.xml");
            //reader.ExtractInput();
            //SMOT_IO.CSVFile csv = new SMOT_IO.CSVFile("C:\\Users\\RDCERSL9\\Documents\\Visual Studio 2015\\Projects\\ConsoleApplication1\\ConsoleApplication1\\SampleInputs\\RainfallData.csv", true);
            ////var test = Preprocessing.Rainfall._rainPerDay(csv);
            //SMOT_IO.InputParams ip = new SMOT_IO.InputParams();
            //Preprocessing.Rainfall.calc95thPercentile(csv, ref ip);
            //Preprocessing.Rainfall.calcAverageDryDays(csv, ref ip);

            //ModelExecutor modelManager = new ModelExecutor(ip);

            SMOT_IO.CSVFile csv = new SMOT_IO.CSVFile("C:\\Users\\RDCERSL9\\Documents\\Visual Studio 2015\\Projects\\ConsoleApplication1\\ConsoleApplication1\\SampleInputs\\RainfallData.csv", true);
            TestModels.SampleInputParams testIn = new TestModels.SampleInputParams();

            ModelExecutor modelManager = new ModelExecutor(testIn.input, csv);
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
            this.medianInfo.rainfallTimeseries = rainfallTimeseries;
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
                                            (this.medianInfo.rainfallTimeseries, this.input);
            this.input.percentile95Rainfall = Preprocessing.Rainfall.calcAverageDryDays
                                            (this.medianInfo.rainfallTimeseries, this.input);

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