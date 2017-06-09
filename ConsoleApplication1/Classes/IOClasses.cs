using System;
using System.Collections.Generic;

namespace SMOTStructs
{
    using RangeInt = Tuple<int, int>;
    
    class SMOTParameters
    {
        // Land cover characterization data
        public double landCoverPervious; // Acres
        public double landCoverImpervious;

        public double storageDepthImpervious; // Inches (recommended 0.05")
        public double storageDepthPervious;   // Inches (recommended 0.15") 

        public double totalAreaDev; // Total impervious area within AOI proposed for new/redevelopment 
        public double effectiveBMPDepth; // Inches; Total effective storage depth of proposed BMP 
                                         // (ponding depth + [media depth * media porosity] + [gravel depth * gravel porosity]) 
                                         // Recommended 21"

        // Major pond data
        public bool majorOnlinePond; // True if pond consumes > 5% of total AOI

        // Regulatory data
        public bool applTMDL; // Applicable total maximum daily load
        public bool EISA438; // Technical Guidance on Implementing the Stormwater Runoff Requirements for Federal Projects under Section 438 of the Energy Independence and Security Act 
        public bool MS4; // Municipal Separate Storm Sewer System (MS4)

        // Ambiguous fields. . . no units specified
        // Precipitation data
        public double percentile95Rainfall; // Inches; from spreadsheet; 95th percentile of rainfall
        public double averageDryPeriods; // Days; from spreadsheet

        // Evaporation data
        public double avgInchPerDay; // Listed as "record type", monthly estimate for average daily depth

        // Hydrologic soil group data, sums up to 1.0
        public double percentA;
        public double percentB;
        public double percentC;
        public double percentD;

        // Horton Soil Properties (see SMOTStructs namespace for info)
        public HSGInfo hsgA;
        public HSGInfo hsgB;
        public HSGInfo hsgC;
        public HSGInfo hsgD;

        public Dictionary<string, RangeInt> hortonRegionDrying = new Dictionary<string, RangeInt>()
        {
            { "arid", new RangeInt (1, 2) },
            { "semi-arid", new RangeInt (2, 4) },
            { "mid_east_southeast_US", new RangeInt (3, 8) },
            { "humid_or_prolonged_precip", new RangeInt (7, 14) }
        };

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