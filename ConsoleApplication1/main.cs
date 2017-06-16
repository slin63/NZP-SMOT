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
            SMOT_IO.XMLReader reader = new SMOT_IO.XMLReader("C:\\Users\\RDCERSL9\\Documents\\Visual Studio 2015\\Projects\\ConsoleApplication1\\ConsoleApplication1\\SampleInputs\\SMOTInput.xml");
            reader.ExtractInput();


        }
    }

    class ModelManager
    {
        // TODO:
        //       Get rainfall algorithms working.
        //       Attach rainfall CSV to AnalysisParams object.
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
                this._analysis();
            }
        }

        private void _analysis()
        {
            Simulation.Analysis.RunAnalysis(this.input, this.medianInfo);
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