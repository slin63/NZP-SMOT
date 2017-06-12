﻿using System;
using System.Collections.Generic;

namespace SMOT_IO
{
    using System.Linq;
    using RangeInt = Tuple<int, int>;

    class InputParameters
    {
        // TODO: Constructor to load data in via XML sheet
        // Base data
        public String baseLocation;
        public String baseName;

        // Land cover characterization data
        public double landCoverPervious; // Acres
        public double landCoverImpervious;

        public double storageDepthImpervious; // Inches (recommended 0.05")
        public double storageDepthPervious;   // Inches (recommended 0.15") 

        public double totalImpArea; // Total impervious area within AOI proposed for new/redevelopment 
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

        public HSGInfo avgHorton;

        public Dictionary<string, RangeInt> hortonRegionDrying = new Dictionary<string, RangeInt>()
        {
            { "arid", new RangeInt (1, 2) },
            { "semi-arid", new RangeInt (2, 4) },
            { "mid_east_southeast_US", new RangeInt (3, 8) },
            { "humid_or_prolonged_precip", new RangeInt (7, 14) }
        };
        
        public void setAVGHSGInfo(double infilMax, double infilMin, double dryDays, double decayRate)
        {
            this.avgHorton.infilMax = infilMax;
            this.avgHorton.infilMin = infilMin;
            this.avgHorton.dryDays = dryDays;
            this.avgHorton.decayRate = decayRate;
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

    struct HSGInfo // Struct to hold data for HSG soils.
    {
        public double infilMax;  // in/hr
        public double infilMin;  // in/hr
        public double dryDays;   // 1 - 14
        public double decayRate; // (per hour (weird unit??))
    }

    class XMLHolder
    {
    }
}