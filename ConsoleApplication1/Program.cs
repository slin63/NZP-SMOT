using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// Will probably need a seperate module for     

namespace SMOTOutline
{
    using RangeDouble = Tuple<double, double>;
    using RangeInt = Tuple<int, int>;

    public class PseudoMain
    {
        static XMLHolder processParameters(SMOTParameters params)
        {
            XMLHolder output;
            return output;
        }
    }

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

        // Regulatory data
        public bool applTMDL; // Applicable total maximum daily load
        public bool EISA438; // Technical Guidance on Implementing the Stormwater Runoff Requirements for Federal Projects under Section 438 of the Energy Independence and Security Act 
        public bool MS4; // Municipal Separate Storm Sewer System (MS4)

        // Ambiguous fields. . . no units specified
        // Precipitation data
        public double hourlyDepthInches;
        public double gageProximity; // Distance to installation
        public double periodOfRecord; // Length of record? I guess?
        public double dataQuality; // Quality of data (% of data flagged / missing)

        // Evaporation data
        public double avgInchPerDay; // Listed as "record type", monthly estimate for average daily depth

        // Hydrologic soil group data, sums up to 1.0
        public double percentA;
        public double percentB;
        public double percentC;
        public double percentD;

        // Major pond data
        public bool majorOnlinePond; // True if pond consumes > 5% of total AOI



        // Horton Equation Values (in/hr)
        public Dictionary<string, RangeDouble> hortonInfilVals = new Dictionary<string, RangeDouble>()
        {
            { "A_min", new RangeDouble (0.4, 4.7) },
            { "A_max", new RangeDouble (6.0, 10.0) },
            { "B_min", new RangeDouble (0.1, 0.3) } // not going to finish its gonna be tedious look at the doc
        };

        public Dictionary<string, RangeInt> hortonRegionDrying = new Dictionary<string, RangeInt>()
        {
            { "arid", new RangeInt (1, 2) },
            { "semi-arid", new RangeInt (2, 4) },
            { "mid_east_southeast_US", new RangeInt (3, 8) },
            { "humid_or_prolonged_precip", new RangeInt (7, 14) }
        };

        

    }

    class XMLHolder
    {
    }
}