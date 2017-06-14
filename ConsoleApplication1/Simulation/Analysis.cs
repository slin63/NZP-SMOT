using System;
using System.Collections.Generic;

namespace Simulation
{
    static class Analysis
    {
        private static String _blankField = "N/A";
        public static void RunAnalysis(SMOT_IO.InputParams input, SMOT_IO.AnalysisParams medianInfo)
        {
            // TODO :: 
            // Currently void, but should return a detailed trace with analysis results.
            bool heterogeneity = _heteroAnalysis(medianInfo);

            // Checking if has no pond
            if (!input.majorOnlinePond)
            {
                // Checking for no WQ component conditions
                if (_noApplTMDL(input))
                {
                    if (_noMS4Components(input))
                        _designForNoWQComponents(input, medianInfo);
                    else
                        _designForMS4Permit(input, medianInfo);
                }

                else
                {
                    if (_noMS4Components(input))
                        _designForTMDLPermit(input, medianInfo);
                    else
                        _designForTMDLMS4Permit(input, medianInfo);
                }
            }

            // If we have a major online pond
            else
            {
                if (_noApplTMDL(input))
                {
                    if (_noMS4Components(input))
                        _pondDesignForNoWQComponents(input, medianInfo);
                    else
                        _pondDesignForMS4Permit(input, medianInfo);
                }

                else
                {
                    if (_noMS4Components(input))
                        _pondDesignForTMDLPermit(input, medianInfo);
                    else
                        _pondDesignForTMDLMS4Permit(input, medianInfo);
                }

                // If pond: Run the simulation no matter what
                _pondSimulationCall(input, medianInfo);
            }

        }
        
        public static SimTrace _callSimulation(SMOT_IO.InputParams input, SMOT_IO.AnalysisParams medianInfo)
        {
            return ContinuousSim.AMain(input, medianInfo);
        }

        private static void _designForTMDLMS4Permit(SMOT_IO.InputParams input, SMOT_IO.AnalysisParams medianInfo)
        {
            medianInfo.designStormBMP = _designStormBMP(input.totalImpArea, input.percentile95Rainfall);
            SimTrace simTrace = _callSimulation(input, medianInfo);
        }

        private static void _designForTMDLPermit(SMOT_IO.InputParams input, SMOT_IO.AnalysisParams medianInfo)
        {
            throw new NotImplementedException();
        }

        private static void _designForMS4Permit(SMOT_IO.InputParams input, SMOT_IO.AnalysisParams medianInfo)
        {
            throw new NotImplementedException();
        }

        static private void _designForNoWQComponents(SMOT_IO.InputParams input, SMOT_IO.AnalysisParams medianInfo)
        {
            ;
        }

        private static void _pondDesignForTMDLPermit(SMOT_IO.InputParams input, SMOT_IO.AnalysisParams medianInfo)
        {
            throw new NotImplementedException();
        }

        private static void _pondDesignForTMDLMS4Permit(SMOT_IO.InputParams input, SMOT_IO.AnalysisParams medianInfo)
        {
            throw new NotImplementedException();
        }

        private static void _pondSimulationCall(SMOT_IO.InputParams input, SMOT_IO.AnalysisParams medianInfo)
        {
            throw new NotImplementedException();
        }

        private static void _pondDesignForMS4Permit(SMOT_IO.InputParams input, SMOT_IO.AnalysisParams medianInfo)
        {
            throw new NotImplementedException();
        }

        private static void _pondDesignForNoWQComponents(SMOT_IO.InputParams input, SMOT_IO.AnalysisParams medianInfo)
        {
            throw new NotImplementedException();
        }

        static private bool _noWQComponents(SMOT_IO.InputParams input)
        {
            bool noWQComponents = false;
            if ((!input.applTMDL && input.applTMDL_WQ == _blankField) ||
                    (input.applTMDL && input.applTMDL_WQ == _blankField))
            {
                if ((!input.MS4 && input.MS4_WQ == _blankField) ||
                    (input.MS4 && input.MS4_WQ == _blankField))
                {
                    noWQComponents = true;
                }
            }
            return noWQComponents;
        }

        static private bool _noApplTMDL(SMOT_IO.InputParams input)
        {
            if ((!input.applTMDL && input.applTMDL_WQ == _blankField) ||
                   (input.applTMDL && input.applTMDL_WQ == _blankField))
                return true;
            else
                return false;
        }

        static private bool _noMS4Components(SMOT_IO.InputParams input)
        {
            if ((!input.MS4 && input.MS4_WQ == _blankField) ||
                        (input.MS4 && input.MS4_WQ == _blankField))
                return true;
            return false;
        }

        static private bool _heteroAnalysis(SMOT_IO.AnalysisParams median)
        {
            // Tells whether or not a soils PCT Result is heterogeneous      
            // This was painful for me. I dont even know if the program uses this function
            // VBA is so vague
            bool heterogeneity = false;
            if ((median.hsgAPCTResult >= 0.2 &&
                median.hsgAPCTResult <= 0.3) &&
                (median.hsgBPCTResult >= 0.2 &&
                median.hsgBPCTResult <= 0.3) &&
                (median.hsgCPCTResult >= 0.2 &&
                median.hsgCPCTResult <= 0.3) &&
                (median.hsgDPCTResult >= 0.2 &&
                median.hsgDPCTResult <= 0.3))
            {
                heterogeneity = true;
            }
            else if (median.hsgAPCTResult >= 0.7)
            {
                if (median.hsgDPCTResult >= 0.2)
                    heterogeneity = true;
                else
                    heterogeneity = false;
            }

            else if (median.hsgBPCTResult >= 0.7)
                heterogeneity = false;

            else if (median.hsgCPCTResult >= 0.7)
                heterogeneity = false;

            else if (median.hsgDPCTResult >= 0.7)
            {
                if (median.hsgAPCTResult >= 0.2)
                    heterogeneity = true;
                else
                    heterogeneity = false;
            }

            else if (median.hsgAPCTResult >= 0.45)
            {
                if (median.hsgBPCTResult >= 0.45)
                    heterogeneity = false;
                else if ((median.hsgBPCTResult >= 0.2 && median.hsgDPCTResult >= 0.2) &&
                         (median.hsgBPCTResult + median.hsgCPCTResult >= 0.45))
                    heterogeneity = false;
                else
                    heterogeneity = true;
            }

            else if (median.hsgBPCTResult >= 0.45)
            {
                if ((median.hsgAPCTResult >= 0.2 && median.hsgDPCTResult >= 0.2) &&
                    (median.hsgAPCTResult + median.hsgDPCTResult >= 0.45))
                    heterogeneity = false;
                else
                    heterogeneity = true;
            }

            else if (median.hsgCPCTResult >= 0.45)
            {
                if ((median.hsgAPCTResult >= 0.2 && median.hsgDPCTResult >= 0.2) &&
                    (median.hsgAPCTResult + median.hsgDPCTResult >= 0.45))
                    heterogeneity = false;
                else
                    heterogeneity = true;
            }

            else if (median.hsgDPCTResult >= 0.45)
            {
                if ((median.hsgBPCTResult >= 0.2 && median.hsgCPCTResult >= 0.2) &&
                    (median.hsgBPCTResult + median.hsgCPCTResult >= 0.45))
                    heterogeneity = false;
                else
                    heterogeneity = true;
            }

            return heterogeneity;
        }

        static private double _designStormBMP(double developArea, double rainfall)
        {
            // Assumes effective BMP depth = 1.75 ft
            // Development area :: acre
            // Rainfall :: inches
            // Area :: ft^2
            return (developArea * 43560 * rainfall / 12 / 1.75);
        }
    }
}

