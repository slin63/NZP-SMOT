using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestModels
{
    class SampleInputParams
    {
        public SMOT_IO.InputParams input;

        public SampleInputParams()
        {
            input = new SMOT_IO.InputParams();

            input.baseLocation = "LOCATION";
            input.baseName = "BASENAME";
            input.landCoverPervious = 10;
            input.landCoverImpervious = 20;
            input.storageDepthImpervious = 0.05;
            input.storageDepthPervious = 0.15;
            input.totalImpArea = 1;
            input.effectiveBMPDepth = 27;
            input.EISA438 = true;
            input.EISA438_WQ = "N/A";
            input.applTMDL = false;
            input.applTMDL_WQ = "N/A";
            input.MS4 = false;
            input.MS4_WQ = "N/A";
            //input.percentile95Rainfall = 2.59;
            //input.averageDryDays = 4.70;

            input.ETInfo.ETJan = .01302;
            input.ETInfo.ETFeb = 0.01939;
            input.ETInfo.ETMarch = 0.03506;
            input.ETInfo.ETApril = 0.05843;
            input.ETInfo.ETMay = 0.09645;
            input.ETInfo.ETJune = 0.013232;
            input.ETInfo.ETJuly = 0.16747;
            input.ETInfo.ETAug = 0.13444;
            input.ETInfo.ETSep = 0.07984;
            input.ETInfo.ETOct = 0.03715;
            input.ETInfo.ETNov = 0.01870;
            input.ETInfo.ETDec = 0.01154;

            input.hsgAreaA = 1.0;
            input.hsgAreaB = 0;
            input.hsgAreaC  = 0;
            input.hsgAreaD = 0;

            input.hsgA = new SMOT_IO.HSGInfo();
            input.hsgB = new SMOT_IO.HSGInfo();
            input.hsgC = new SMOT_IO.HSGInfo();
            input.hsgD = new SMOT_IO.HSGInfo();

            input.hsgA.infilMax = -6.0;
            input.hsgA.infilMin = .5;
            input.hsgA.dryDays = 7.0;
            input.hsgA.decayRate = 2.0;

            input.hsgB.infilMax = 5.0;
            input.hsgB.infilMin = .3;
            input.hsgB.dryDays = 7.0;
            input.hsgB.decayRate = 3.0;

            input.hsgC.infilMax = 3.0;
            input.hsgC.infilMin = .1;
            input.hsgC.dryDays = 7.0;
            input.hsgC.decayRate = 3.0;

            input.hsgD.infilMax = 1.0;
            input.hsgD.infilMin = .0;
            input.hsgD.dryDays = 7.0;
            input.hsgD.decayRate = 3.0;
        }
    }
}
