#define DEBUG

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
            //SMOT_IO.CSVFile rainfallTimeSeriesCSV = new SMOT_IO.CSVFile("..\\..\\SampleInputs\\RainfallData.csv", true);
            //TestModels.SampleInputParams testIn = new TestModels.SampleInputParams();
            //SMOT_IO.InputParams input = testIn.input;
            //// How are we going to load up the CSV on NZP? 
            ////  -> Use a ridiculously long CLI call and have a CLARGS parser read it.
            //ModelExecutor modelManager = new ModelExecutor(input, rainfallTimeSeriesCSV);
            //modelManager.RunModel();
            //modelManager.PresentOutput();

            // Placeholder 
            //SMOT_IO.CSVFile rainfallTimeSeriesCSV = new SMOT_IO.CSVFile("..\\..\\SampleInputs\\RainfallData.csv", true);
            InputParams input = null;
            CSVFile rainfallTimeseries = null;

            if (args.Length == 0)
            {
                var CLICall = testCallStrings.CLICallFromTestInput(new TestModels.SampleInputParams(), ("..\\..\\SampleInputs\\RainfallData.csv"));
                input = new SMOT_IO.InputParams(CLICall);
                rainfallTimeseries = new SMOT_IO.CSVFile(CLICall);
            }
            // CLICall represents string[] Main.args, built up from test input parameters

            else
            {
                // For when we get an actual CLI call
                input = new SMOT_IO.InputParams(args);
                rainfallTimeseries = new SMOT_IO.CSVFile(args);
            }


            ModelExecutor modelManager = new ModelExecutor(input, rainfallTimeseries);
            modelManager.RunModel();
            modelManager.PresentOutput();
        }
    }

    static class testCallStrings
    {   
        public static string[] CLICallFromTestInput(TestModels.SampleInputParams testIn, string rainfallTimeseriesDirectory)
        {
            List<string> args = new List<string>();
            
            args.Add(Convert.ToString(testIn.input.landCoverPervious     ));
            args.Add(Convert.ToString(testIn.input.landCoverImpervious   ));
            args.Add(Convert.ToString(testIn.input.storageDepthImpervious));
            args.Add(Convert.ToString(testIn.input.storageDepthPervious  ));
            args.Add(Convert.ToString(testIn.input.totalDevelImpArea     ));
            args.Add(Convert.ToString(testIn.input.effectiveBMPDepth     ));
            args.Add(Convert.ToString(testIn.input.EISA438               ));
            args.Add(Convert.ToString(testIn.input.EISA438_WQ            ));
            args.Add(Convert.ToString(testIn.input.applTMDL              ));
            args.Add(Convert.ToString(testIn.input.applTMDL_WQ           ));
            args.Add(Convert.ToString(testIn.input.MS4                   ));
            args.Add(Convert.ToString(testIn.input.MS4_WQ                ));     
            args.Add(Convert.ToString(testIn.input.majorOnlinePond       ));
            args.Add(Convert.ToString(testIn.input.ETInfo.ETJan          ));
            args.Add(Convert.ToString(testIn.input.ETInfo.ETFeb          ));
            args.Add(Convert.ToString(testIn.input.ETInfo.ETMarch        ));
            args.Add(Convert.ToString(testIn.input.ETInfo.ETApril        ));
            args.Add(Convert.ToString(testIn.input.ETInfo.ETMay          ));
            args.Add(Convert.ToString(testIn.input.ETInfo.ETJune         ));
            args.Add(Convert.ToString(testIn.input.ETInfo.ETJuly         ));
            args.Add(Convert.ToString(testIn.input.ETInfo.ETAug          ));
            args.Add(Convert.ToString(testIn.input.ETInfo.ETSep          ));
            args.Add(Convert.ToString(testIn.input.ETInfo.ETOct          ));
            args.Add(Convert.ToString(testIn.input.ETInfo.ETNov          ));
            args.Add(Convert.ToString(testIn.input.ETInfo.ETDec          ));
            args.Add(Convert.ToString(testIn.input.hsgAreaA              ));
            args.Add(Convert.ToString(testIn.input.hsgAreaB              ));
            args.Add(Convert.ToString(testIn.input.hsgAreaC              ));
            args.Add(Convert.ToString(testIn.input.hsgAreaD              ));
            args.Add(Convert.ToString(testIn.input.hsgA.infilMax         ));
            args.Add(Convert.ToString(testIn.input.hsgA.infilMin         ));
            args.Add(Convert.ToString(testIn.input.hsgA.dryDays          ));
            args.Add(Convert.ToString(testIn.input.hsgA.decayRate        ));
            args.Add(Convert.ToString(testIn.input.hsgB.infilMax         ));
            args.Add(Convert.ToString(testIn.input.hsgB.infilMin         ));
            args.Add(Convert.ToString(testIn.input.hsgB.dryDays          ));
            args.Add(Convert.ToString(testIn.input.hsgB.decayRate        ));
            args.Add(Convert.ToString(testIn.input.hsgC.infilMax         ));
            args.Add(Convert.ToString(testIn.input.hsgC.infilMin         ));
            args.Add(Convert.ToString(testIn.input.hsgC.dryDays          ));
            args.Add(Convert.ToString(testIn.input.hsgC.decayRate        ));
            args.Add(Convert.ToString(testIn.input.hsgD.infilMax         ));
            args.Add(Convert.ToString(testIn.input.hsgD.infilMin         ));
            args.Add(Convert.ToString(testIn.input.hsgD.dryDays          ));
            args.Add(Convert.ToString(testIn.input.hsgD.decayRate        ));

            List<String> rainfallTimeseries = new List<String>();
            using (var fs = System.IO.File.OpenRead(rainfallTimeseriesDirectory))
            using (var reader = new System.IO.StreamReader(fs))
            {
#if DEBUG
                var watch = System.Diagnostics.Stopwatch.StartNew();
                Console.WriteLine("READING RAINFALL TIMESERIES");
#endif
                while (!reader.EndOfStream)
                {
                    String line = reader.ReadLine();
                    rainfallTimeseries = rainfallTimeseries.Concat(line.Split(',').ToList()).ToList();
                }
#if DEBUG
                watch.Stop();
                var elapsedMs = watch.ElapsedMilliseconds;
                Console.WriteLine(String.Format("FINISHED READING RAINFALL TIMESERIES\n\tTIME: {0}", elapsedMs));
#endif
            }

            args = args.Concat(rainfallTimeseries).ToList();
            return args.ToArray<string>();
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

        public void PresentOutput()
        {
            Console.WriteLine("REC: {0}\nREASON: {1}", outputInfo.analysisRec, outputInfo.reason);
        }

        

        private void _applyPreprocessing()
        {
            Preprocessing.HSGPreprocessing.averageHSGInfo(ref this.input);
            this.input.averageDryDays = Preprocessing.Rainfall.calcAverageDryDays
                                            (this.medianInfo.rainfallCSV, this.input);
            this.input.percentile95Rainfall = Preprocessing.Rainfall.calc95thPercentile
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