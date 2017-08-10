using System;
using System.Collections.Generic;

namespace SMOT_IO
{
    using System.Linq;
    using RangeInt = Tuple<int, int>;

    class InputParams
    {
        // TODO: Constructor to load data in via XML sheet'
        #region Internals
        // Base data
        //       Defaults should read "N/A"
        public String baseLocation;
        public String baseName;

        // Land cover characterization data
        public double landCoverPervious; // Acres
        public double landCoverImpervious;

        public double storageDepthImpervious; // Inches (recommended 0.05")
        public double storageDepthPervious;   // Inches (recommended 0.15") 

        public double totalDevelImpArea; // Total impervious area within AOI proposed for new/redevelopment 
        public double effectiveBMPDepth; // Inches; Total effective storage depth of proposed BMP 
                                         // (ponding depth + [media depth * media porosity] + [gravel depth * gravel porosity]) 
                                         // Recommended 21"

        // Major pond data
        public bool majorOnlinePond; // True if pond consumes > 5% of total AOI

        // Regulatory data
        public bool applTMDL; // Applicable total maximum daily load
        public String applTMDL_WQ;

        public bool EISA438; // Technical Guidance on Implementing the Stormwater Runoff Requirements for Federal Projects under Section 438 of the Energy Independence and Security Act 
        public String EISA438_WQ;

        public bool MS4; // Municipal Separate Storm Sewer System (MS4)
        public String MS4_WQ;


        // Ambiguous fields. . . no units specified
        // Precipitation data
        public double percentile95Rainfall; // Inches; from spreadsheet; 95th percentile of rainfall
        public double averageDryDays; // Days; from spreadsheet

        // Evaporation data
        public double avgInchPerDay; // Listed as "record type", monthly estimate for average daily depth

        public ETInfo ETInfo = new ETInfo();

        // Hydrologic soil group data, sums up to 1.0
        public double hsgAreaA;
        public double hsgAreaB;
        public double hsgAreaC;
        public double hsgAreaD;

        // Horton Soil Properties (see SMOTStructs namespace for info)
        public HSGInfo hsgA;
        public HSGInfo hsgB;
        public HSGInfo hsgC;
        public HSGInfo hsgD;

        public HSGInfo avgHorton = new HSGInfo();

        //public Dictionary<string, RangeInt> hortonRegionDrying = new Dictionary<string, RangeInt>()
        //{
        //    { "arid", new RangeInt (1, 2) },
        //    { "semi-arid", new RangeInt (2, 4) },
        //    { "mid_east_southeast_US", new RangeInt (3, 8) },
        //    { "humid_or_prolonged_precip", new RangeInt (7, 14) }
        //};

        public void setAVGHSGInfo(double infilMax, double infilMin, double dryDays, double decayRate)
        {
            this.avgHorton.infilMax = infilMax;
            this.avgHorton.infilMin = infilMin;
            this.avgHorton.dryDays = dryDays;
            this.avgHorton.decayRate = decayRate;
        }

        public double totalHsgArea()
        {
            return hsgAreaA + hsgAreaB + hsgAreaC + hsgAreaD;
        }
        #endregion

        #region Constructors
        public InputParams() { ; }

        public InputParams(string[] args)
        {
            // Sample call
            // C:\Users\RDCERSL9\Documents\Visual Studio 2015\Projects\ConsoleApplication1\ConsoleApplication1\bin\Debug\ConsoleApplication1.exe 1 1 1 1 1 1 false EW false AT false M4 1 2 3 4 5 6 7 8 9 10 11 12 1.0 0 0 0 1 1 1 1 2 2 2 2 3 3 3 3 4 4 4 4
            baseLocation = "LOCATION";
            baseName = "BASENAME";
            landCoverPervious      = Convert.ToDouble(args[0]);                                
            landCoverImpervious    = Convert.ToDouble(args[1]);
            storageDepthImpervious = Convert.ToDouble(args[2]);
            storageDepthPervious   = Convert.ToDouble(args[3]);
            totalDevelImpArea      = Convert.ToDouble(args[4]);
            effectiveBMPDepth      = Convert.ToDouble(args[5]);
            EISA438                = Convert.ToBoolean(args[6]);
            EISA438_WQ             = (args[7]);
            applTMDL               = Convert.ToBoolean(args[8]);
            applTMDL_WQ            = (args[9]);
            MS4                    = Convert.ToBoolean(args[10]);
            MS4_WQ                 = (args[11]);

            majorOnlinePond        = Convert.ToBoolean(args[12]);
                                     
            ETInfo.ETJan           = Convert.ToDouble(args[13]);
            ETInfo.ETFeb           = Convert.ToDouble(args[14]);
            ETInfo.ETMarch         = Convert.ToDouble(args[15]);
            ETInfo.ETApril         = Convert.ToDouble(args[16]);
            ETInfo.ETMay           = Convert.ToDouble(args[17]);
            ETInfo.ETJune          = Convert.ToDouble(args[18]);
            ETInfo.ETJuly          = Convert.ToDouble(args[19]);
            ETInfo.ETAug           = Convert.ToDouble(args[20]);
            ETInfo.ETSep           = Convert.ToDouble(args[21]);
            ETInfo.ETOct           = Convert.ToDouble(args[22]);
            ETInfo.ETNov           = Convert.ToDouble(args[23]);
            ETInfo.ETDec           = Convert.ToDouble(args[24]);

            hsgAreaA               = Convert.ToDouble(args[25]);
            hsgAreaB               = Convert.ToDouble(args[26]);
            hsgAreaC               = Convert.ToDouble(args[27]);
            hsgAreaD               = Convert.ToDouble(args[28]);

            hsgA = new SMOT_IO.HSGInfo();
            hsgB = new SMOT_IO.HSGInfo();
            hsgC = new SMOT_IO.HSGInfo();
            hsgD = new SMOT_IO.HSGInfo();

            hsgA.infilMax          = Convert.ToDouble(args[29]);
            hsgA.infilMin          = Convert.ToDouble(args[30]);
            hsgA.dryDays           = Convert.ToDouble(args[31]);
            hsgA.decayRate         = Convert.ToDouble(args[32]);
                                                           
            hsgB.infilMax          = Convert.ToDouble(args[33]);
            hsgB.infilMin          = Convert.ToDouble(args[34]);
            hsgB.dryDays           = Convert.ToDouble(args[35]);
            hsgB.decayRate         = Convert.ToDouble(args[36]);
                                                           
            hsgC.infilMax          = Convert.ToDouble(args[37]);
            hsgC.infilMin          = Convert.ToDouble(args[38]);
            hsgC.dryDays           = Convert.ToDouble(args[39]);
            hsgC.decayRate         = Convert.ToDouble(args[40]);
                                                           
            hsgD.infilMax          = Convert.ToDouble(args[41]);
            hsgD.infilMin          = Convert.ToDouble(args[42]);
            hsgD.dryDays           = Convert.ToDouble(args[43]);
            hsgD.decayRate         = Convert.ToDouble(args[44]);
            _debugArgs(args);
        }
        #endregion  

        private void _debugArgs(string[] args)
        {
            foreach (var ele in args)
                Console.WriteLine(ele);
        }

    }

    #region Assistant Data Classes
    class AnalysisParams
    {
        public double totalSoilArea;
        public double hsgAPCTResult;
        public double hsgBPCTResult;
        public double hsgCPCTResult;
        public double hsgDPCTResult;
        
        public double initAbsResult;

        public CSVFile rainfallCSV;

        public double designStormBMP;
        public double CSBMPArea;    // Surface area of BMP found through ContinuousSim._findSurfaceArea()
        public double CSOBMPArea;   // CSBMPArea + AddHeterogeneity(Soils Info, CSBMPArea)

        public AnalysisParams(double totalSoilArea, double hsgAreaA, double hsgAreaB, double hsgAreaC, double hsgAreaD)
        {
            this.totalSoilArea = totalSoilArea;
            this.hsgAPCTResult = hsgAreaA / totalSoilArea;
            this.hsgBPCTResult = hsgAreaB / totalSoilArea;
            this.hsgCPCTResult = hsgAreaC / totalSoilArea;
            this.hsgDPCTResult = hsgAreaD / totalSoilArea;

            this.initAbsResult = ((hsgAreaA * 2.08) + (hsgAreaB * 0.9) + (hsgAreaC * 0.53) + (hsgAreaD * 0.38)) / totalSoilArea;
        }
    }

    // https://stackoverflow.com/questions/5282999/reading-csv-file-and-storing-values-into-an-array
    class CSVFile
    {
        // TODO: 
        //     : Make this point to an uploaded file on the website. 
        //     : This will be fun!
        private String _fileLocation;
        public List<CSVRow> rows = new List<CSVRow>();
        public bool hasHeader;

        public CSVFile(string[] args)
        {
            // Starting at 45 because that's the index where RainfallTimeseries data is passed to us
            for (int i = 45; i < args.Length; i += 2)
            {
                string dateTime = args[i];
                string rainfall = args[i + 1];
                string rowString = String.Format("{0},{1}", dateTime, rainfall);

                CSVRow lineProcessed = new CSVRow(rowString);
                this.rows.Add(lineProcessed);
            }
        }

        public CSVFile(String fileLocation, bool hasHeader)
        {
            this._fileLocation = fileLocation;
            this.hasHeader = hasHeader;
            this._readIntoRows();
        }

        public int length()
        {
            return rows.Count;
        }

        private void _readIntoRows()
        {
            using (var fs = System.IO.File.OpenRead(this._fileLocation))
            using (var reader = new System.IO.StreamReader(fs))
            {
                if (this.hasHeader) // Skip a line if this file has a header
                {
                    reader.ReadLine();
                }
                while (!reader.EndOfStream)
                {  
                    String line = reader.ReadLine();
                    CSVRow lineProcessed = new CSVRow(line);
                    this.rows.Add(lineProcessed);
                }
            }
        }   
    }

    struct CSVRow
    {
        // Helper struct that parses CSV row strings and sorts them into ordered lists.
        public List<String> row;

        public CSVRow(String rowString)
        {
            this.row = rowString.Split(',').ToList<String>();
        }
    }

    class HSGInfo // Struct to hold data for HSG soils.
    {
        public double infilMax;  // in/hr
        public double infilMin;  // in/hr
        public double dryDays;   // 1 - 14
        public double decayRate; // (per hour (weird unit??))

        public bool AnyNegative()
        {
            return (infilMax < 0 || infilMin < 0 || dryDays < 0 || decayRate < 0);
        }
    }

    class ETInfo
    {
        public double ETJan;
        public double ETFeb;
        public double ETMarch;
        public double ETApril;
        public double ETMay;
        public double ETJune;
        public double ETJuly;
        public double ETAug;
        public double ETSep;
        public double ETOct;
        public double ETNov;
        public double ETDec;

        public double getMonthlyET(int monthInt)
        {
            double monthlyET;
            switch (monthInt)
            {
                case 1:
                    monthlyET = ETJan;
                    break;
                case 2:
                    monthlyET = ETFeb;
                    break;
                case 3:
                    monthlyET = ETMarch;
                    break;
                case 4:
                    monthlyET = ETApril;
                    break;
                case 5:
                    monthlyET = ETMay;
                    break;
                case 6:
                    monthlyET = ETJune;
                    break;
                case 7:
                    monthlyET = ETJuly;
                    break;
                case 8:
                    monthlyET = ETAug;
                    break;
                case 9:
                    monthlyET = ETSep;
                    break;
                case 10:
                    monthlyET = ETOct;
                    break;
                case 11:
                    monthlyET = ETNov;
                    break;
                case 12:
                    monthlyET = ETDec;
                    break;
                default:
                    throw new IndexOutOfRangeException("Attempted to access ET values for a month with invalid integer value: " + monthInt);
            }

            if (monthlyET < 0)
                throw new ArgumentException(String.Format("User supplied invalid Monthly ET info: {0}", monthlyET));

            return monthlyET;
        }
        
    }

    class Timeseries
    {
        public DateTime date;
        public double rainfall;
        public double ET;
        public double storeImp;
        public double storePerv;
        public double runoffImp;
        public double runoffPerv;

        public Timeseries(DateTime date, double rainfall, double ET, double storeImp, double storePerv, double runoffPerv,
                          double runoffImp)
        {
            this.date = date;
            this.rainfall = rainfall;
            this.ET = ET;
            this.storeImp = storeImp;
            this.storePerv = storePerv;
            this.runoffImp = runoffImp;
            this.runoffPerv = runoffPerv;
        }
    }
    #endregion
}

