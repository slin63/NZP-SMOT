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
        SMOT_IO.InputParameters input;

        public ModelManager(SMOT_IO.InputParameters input)
        {
            this.input = input;
        }

        public void RunModel()
        {
            if (_checkValidation())
            {
                ;
            }

        }

        private bool _checkValidation()
        {
            return Validation.InputValidator.ValidateInputs(this.input);
        }

    }
}