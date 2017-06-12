using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Preprocessing
{
    static class HSGParser
    {
        static public void averageHSGInfo(ref SMOT_IO.InputParams input)
        {
            double avgInfilMax = _calcAvgInfilMax(input);
            double avgInfilMin = _calcAvgInfilMin(input);
            double avgDryDays = _calcAvgDryDays(input);
            double avgHortonDecay = _calcAvgHortonDecay(input);

            // By reference so we can update the input info's HSG info
            input.setAVGHSGInfo(avgInfilMax, avgInfilMin, avgDryDays, avgHortonDecay);
        }

        static private double _calcAvgInfilMax(SMOT_IO.InputParams input)
        {
            double lineA = (input.hsgA.infilMax * input.hsgAreaA);
            double lineB = (input.hsgB.infilMax * input.hsgAreaB);
            double lineC = (input.hsgC.infilMax * input.hsgAreaC);
            double lineD = (input.hsgD.infilMax * input.hsgAreaD);

            return (lineA + lineB + lineC + lineD) / input.totalImpArea;
        }

        static private double _calcAvgInfilMin(SMOT_IO.InputParams input)
        {
            double lineA = (input.hsgA.infilMin * input.hsgAreaA);
            double lineB = (input.hsgB.infilMin * input.hsgAreaB);
            double lineC = (input.hsgC.infilMin * input.hsgAreaC);
            double lineD = (input.hsgD.infilMin * input.hsgAreaD);

            return (lineA + lineB + lineC + lineD) / input.totalImpArea;
        }

        static private double _calcAvgDryDays(SMOT_IO.InputParams input)
        {
            double lineA = (input.hsgA.dryDays * input.hsgAreaA);
            double lineB = (input.hsgB.dryDays * input.hsgAreaB);
            double lineC = (input.hsgC.dryDays * input.hsgAreaC);
            double lineD = (input.hsgD.dryDays * input.hsgAreaD);

            return (lineA + lineB + lineC + lineD) / input.totalImpArea;
        }

        static private double _calcAvgHortonDecay(SMOT_IO.InputParams input)
        {

            double lineA = (input.hsgA.decayRate * input.hsgAreaA);
            double lineB = (input.hsgB.decayRate * input.hsgAreaB);
            double lineC = (input.hsgC.decayRate * input.hsgAreaC);
            double lineD = (input.hsgD.decayRate * input.hsgAreaD);

            return (lineA + lineB + lineC + lineD) / input.totalImpArea;
        }
    }
}


