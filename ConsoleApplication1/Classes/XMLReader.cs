using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMOT_IO
{
    class XMLReader
    {
        System.Data.DataSet xmlD;
        System.IO.StringReader reader;
        InputParams inputParams;

        public XMLReader(string xmlName)
        {
            var xmlString = System.IO.File.ReadAllText(xmlName);
            reader = new System.IO.StringReader(xmlString);
            xmlD = new System.Data.DataSet();
        }

        public InputParams ExtractInput()
        {
            Console.WriteLine(xmlD.ReadXml(reader));
            return inputParams;
        }
    }
}
