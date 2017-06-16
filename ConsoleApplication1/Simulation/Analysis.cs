using System;
using System.Collections.Generic;

namespace Simulation
{
    static class Analysis
    {
        // TODO: We might need to pass input and medianInfo refs.
        //       That depends on if this is the final function that'll be using those datasets
        private static String _blankField = "N/A";
        public enum Approaches { DesignStorm, ContinuousOnly, ContinuousWithOptimization };

        public static AnalysisTrace RunAnalysis(SMOT_IO.InputParams input, SMOT_IO.AnalysisParams medianInfo)
        {
            // TODO :: 
            // Currently void, but should return a detailed trace with analysis results.
            bool heterogeneity = _heteroAnalysis(medianInfo);
            AnalysisTrace aTrace = new AnalysisTrace();

            // Checking if has no pond
            if (!input.majorOnlinePond)
            {
                // Checking for no WQ component conditions
                if (_noApplTMDL(input))
                {
                    if (_noMS4Components(input))
                        aTrace = _designForNoWQComponents(ref aTrace, input, medianInfo);
                    else
                        aTrace = _designForMS4Permit(ref aTrace, input, medianInfo);
                }

                else
                {
                    if (_noMS4Components(input))
                        aTrace = _designForTMDLPermit(ref aTrace, input, medianInfo);
                    else
                        aTrace = _designForTMDLMS4Permit(ref aTrace, input, medianInfo);
                }
            }

            // If we have a major online pond
            else
            {
                if (_noApplTMDL(input))
                {
                    if (_noMS4Components(input))
                        aTrace = _pondDesignForNoWQComponents(ref aTrace, input, medianInfo);
                    else
                        aTrace = _pondDesignForMS4Permit(ref aTrace, input, medianInfo);
                }

                else
                {
                    if (_noMS4Components(input))
                        aTrace = _pondDesignForTMDLPermit(ref aTrace, input, medianInfo);
                    else
                        aTrace = _pondDesignForTMDLMS4Permit(ref aTrace, input, medianInfo);
                }

                //// If pond: Run the simulation no matter what
                //aTrace = _pondSimulationCall(ref aTrace, input, medianInfo);
            }

            return aTrace;
        }
        
        public static SimModels _callSimulation(SMOT_IO.InputParams input, SMOT_IO.AnalysisParams medianInfo)
        {
            return ContinuousSim.AMain(input, medianInfo);
        }
        
        static private AnalysisTrace _designForNoWQComponents(ref AnalysisTrace aTrace, SMOT_IO.InputParams input, SMOT_IO.AnalysisParams medianInfo)
        {
            // DONE
            // Lines 410 - 456
            medianInfo.designStormBMP = _designStormBMP(input.totalImpArea, input.percentile95Rainfall);
            SimModels simTrace = _callSimulation(input, medianInfo);

            medianInfo.CSBMPArea = simTrace.bmp.bmpArea;
            medianInfo.CSOBMPArea = simTrace.bmp.bmpArea +
                __AddHeterogeneity(input.hsgAreaA, input.hsgAreaB, input.hsgAreaC, input.hsgAreaD)
                * simTrace.bmp.bmpArea;

            if (medianInfo.CSBMPArea > medianInfo.designStormBMP)
            {
                aTrace.analysisRec = Approaches.DesignStorm;
                aTrace.reason = "BMP Size in Continuous Simulation > BMP size in Design Storm.";
            }
                
            else
            {
                if (__AddHeterogeneity(input.hsgAreaA, input.hsgAreaB, input.hsgAreaC, input.hsgAreaD) < -0.15)
                {
                    aTrace.analysisRec = Approaches.ContinuousWithOptimization;
                    aTrace.reason = "BMP size in Continuous Simulation < BMP size in Design Storm. The soils are heterogeneous and the optimization result is significant.";
                }
                    
                else
                {
                    aTrace.analysisRec = Approaches.ContinuousOnly;
                    aTrace.reason = "BMP size in Continuous Simulation < BMP size in Design Storm. The soils are not heterogeneous enough and the optimization result is not significant.";
                }       
            }

            return aTrace;
        }

        private static AnalysisTrace _designForMS4Permit(ref AnalysisTrace aTrace, SMOT_IO.InputParams input, SMOT_IO.AnalysisParams medianInfo)
        {
            medianInfo.designStormBMP = _designStormBMP(input.totalImpArea, input.percentile95Rainfall);
            SimModels simTrace = _callSimulation(input, medianInfo);

            medianInfo.CSBMPArea = simTrace.bmp.bmpArea;
            medianInfo.CSOBMPArea = simTrace.bmp.bmpArea +
                __AddHeterogeneity(input.hsgAreaA, input.hsgAreaB, input.hsgAreaC, input.hsgAreaD)
                * simTrace.bmp.bmpArea;

            if (__AddHeterogeneity(input.hsgAreaA, input.hsgAreaB, input.hsgAreaC, input.hsgAreaD) < -0.15)
            {
                aTrace.analysisRec = Approaches.ContinuousWithOptimization;
                aTrace.reason = "The MS4 regulation has water quality components. The soils are heterogeneous and the optimization results is significant.";
            }

            else
            {
                aTrace.analysisRec = Approaches.ContinuousOnly;
                aTrace.reason = "The MS4 regulation has water quality components. The soils are not heterogeneous enough and the optimization results is not significant.";
            }

            return aTrace;
        }

        private static AnalysisTrace _designForTMDLPermit(ref AnalysisTrace aTrace, SMOT_IO.InputParams input, SMOT_IO.AnalysisParams medianInfo)
        {
            medianInfo.designStormBMP = _designStormBMP(input.totalImpArea, input.percentile95Rainfall);
            SimModels simTrace = _callSimulation(input, medianInfo);

            medianInfo.CSBMPArea = simTrace.bmp.bmpArea;
            medianInfo.CSOBMPArea = simTrace.bmp.bmpArea +
                __AddHeterogeneity(input.hsgAreaA, input.hsgAreaB, input.hsgAreaC, input.hsgAreaD)
                * simTrace.bmp.bmpArea;

            if (__AddHeterogeneity(input.hsgAreaA, input.hsgAreaB, input.hsgAreaC, input.hsgAreaD) < -0.15)
            {
                aTrace.analysisRec = Approaches.ContinuousWithOptimization;
                aTrace.reason = "The TMDL regulation has water quality components. The soils are heterogeneous and the optimization results is significant.";
            }

            else
            {
                aTrace.analysisRec = Approaches.ContinuousOnly;
                aTrace.reason = "The TMDL regulation has water quality components. The soils are not heterogeneous enough and the optimization results is not significant.";
            }

            return aTrace;
        }

        private static AnalysisTrace _designForTMDLMS4Permit(ref AnalysisTrace aTrace, SMOT_IO.InputParams input, SMOT_IO.AnalysisParams medianInfo)
        {
            medianInfo.designStormBMP = _designStormBMP(input.totalImpArea, input.percentile95Rainfall);
            SimModels simTrace = _callSimulation(input, medianInfo);

            medianInfo.CSBMPArea = simTrace.bmp.bmpArea;
            medianInfo.CSOBMPArea = simTrace.bmp.bmpArea +
                __AddHeterogeneity(input.hsgAreaA, input.hsgAreaB, input.hsgAreaC, input.hsgAreaD)
                * simTrace.bmp.bmpArea;

            if (__AddHeterogeneity(input.hsgAreaA, input.hsgAreaB, input.hsgAreaC, input.hsgAreaD) < -0.15)
            {
                aTrace.analysisRec = Approaches.ContinuousWithOptimization;
                aTrace.reason = "The MS4 and TMDL regulation has water quality components. The soils are heterogeneous and the optimization results is significant.";
            }

            else
            {
                aTrace.analysisRec = Approaches.ContinuousOnly;
                aTrace.reason = "The MS4 and TMDL regulation has water quality components. The soils are not heterogeneous enough and the optimization results is not significant.";
            }

            return aTrace;
        }

        private static AnalysisTrace _pondDesignForNoWQComponents(ref AnalysisTrace aTrace, SMOT_IO.InputParams input, SMOT_IO.AnalysisParams medianInfo)
        {
            if (__AddHeterogeneity(input.hsgAreaA, input.hsgAreaB, input.hsgAreaC, input.hsgAreaD) < -0.15)
            {
                aTrace.analysisRec = Approaches.ContinuousWithOptimization;
                aTrace.reason = "The watershed has a major online pond. The soils are heterogeneous and the optimization results is significant.";
            }

            else
            {
                aTrace.analysisRec = Approaches.ContinuousOnly;
                aTrace.reason = "The watershed has a major online pond. The soils are not heterogeneous enough and the optimization results is not significant.";
            }

            return aTrace;
        }
        
        private static AnalysisTrace _pondDesignForMS4Permit(ref AnalysisTrace aTrace, SMOT_IO.InputParams input, SMOT_IO.AnalysisParams medianInfo)
        {
            if (__AddHeterogeneity(input.hsgAreaA, input.hsgAreaB, input.hsgAreaC, input.hsgAreaD) < -0.15)
            {
                aTrace.analysisRec = Approaches.ContinuousWithOptimization;
                aTrace.reason = "The watershed has a major online pond. The MS4 regulation has water quality components. The soils are heterogeneous and the optimization results is significant.";
            }

            else
            {
                aTrace.analysisRec = Approaches.ContinuousOnly;
                aTrace.reason = "The watershed has a major online pond. The MS4 regulation has water quality components. The soils are not heterogeneous enough and the optimization results is not significant.";
            }

            return aTrace;
        }

        private static AnalysisTrace _pondDesignForTMDLPermit(ref AnalysisTrace aTrace, SMOT_IO.InputParams input, SMOT_IO.AnalysisParams medianInfo)
        {
            if (__AddHeterogeneity(input.hsgAreaA, input.hsgAreaB, input.hsgAreaC, input.hsgAreaD) < -0.15)
            {
                aTrace.analysisRec = Approaches.ContinuousWithOptimization;
                aTrace.reason = "The watershed has a major online pond. The TMDL regulation has water quality components. The soils are heterogeneous and the optimization results is significant.";
            }                                                               
                                                                            
            else                                                            
            {                                                               
                aTrace.analysisRec = Approaches.ContinuousOnly;             
                aTrace.reason = "The watershed has a major online pond. The TMDL regulation has water quality components. The soils are not heterogeneous enough and the optimization results is not significant.";
            }

            return aTrace;
        }

        private static AnalysisTrace _pondDesignForTMDLMS4Permit(ref AnalysisTrace aTrace, SMOT_IO.InputParams input, SMOT_IO.AnalysisParams medianInfo)
        {
            if (__AddHeterogeneity(input.hsgAreaA, input.hsgAreaB, input.hsgAreaC, input.hsgAreaD) < -0.15)
            {
                aTrace.analysisRec = Approaches.ContinuousWithOptimization;
                aTrace.reason = "The watershed has a major online pond. The TMDL and MS4 regulations have water quality components. The soils are heterogeneous and the optimization results is significant.";
            }

            else
            {
                aTrace.analysisRec = Approaches.ContinuousOnly;
                aTrace.reason = "The watershed has a major online pond. The TMDL and MS4 regulations have water quality components. The soils are not heterogeneous enough and the optimization results is not significant.";
            }

            return aTrace;
        }

        private static AnalysisTrace _pondSimulationCall(ref AnalysisTrace aTrace, SMOT_IO.InputParams input, SMOT_IO.AnalysisParams medianInfo)
        {
            // None of the pond functions actually use this data. 
            // This function may just get scrapped.
            medianInfo.designStormBMP = _designStormBMP(input.totalImpArea, input.percentile95Rainfall);
            SimModels simTrace = _callSimulation(input, medianInfo);

            medianInfo.CSBMPArea = simTrace.bmp.bmpArea;
            medianInfo.CSOBMPArea = simTrace.bmp.bmpArea +
                __AddHeterogeneity(input.hsgAreaA, input.hsgAreaB, input.hsgAreaC, input.hsgAreaD)
                * simTrace.bmp.bmpArea;

            return aTrace;
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

        private static double __AddHeterogeneity(double hsgAreaA, double hsgAreaB, double hsgAreaC, double hsgAreaD)
        {
            // BMP Sizing Difference between the Optimized and non-Optimized Sizing Strategies (%)
            // Infiltration for calculating BMP size
            // Soil A: 1.02 in/hr
            // Soil B: 0.52 in/hr
            // Soil C: 0.27 in/hr
            // Soil D: 0.05 in/hr
            double totalArea = hsgAreaA + hsgAreaB + hsgAreaC + hsgAreaD;
            double avgInfilRate = 1.02 * hsgAreaA +
                                  0.52 * hsgAreaB +
                                  0.27 * hsgAreaC +
                                  0.05 * hsgAreaD;
            double weightedVariance = (Math.Pow((1.02 - avgInfilRate), 2) * hsgAreaA +
                                        Math.Pow((0.52 - avgInfilRate), 2) * hsgAreaB +
                                        Math.Pow((0.27 - avgInfilRate), 2) * hsgAreaC +
                                        Math.Pow((0.05 - avgInfilRate), 2) * hsgAreaD) / totalArea;
            double bmpDifference = -1.2886 * weightedVariance + 0.0055; // 0.0055 : from heterogeneity curves

            return bmpDifference;

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

    class AnalysisTrace
    {
        public Analysis.Approaches analysisRec;
        public String reason;
    }
}

