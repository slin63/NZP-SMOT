using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Validation
{
    static class InputValidator
    {
        private static String _blankField = "N/A";

        static public bool ValidateInputs(SMOT_IO.InputParams input)
        {
            // Calls validation functions
            ValidationTrace regulationPass, watershedPass, rainfallPass, soilsPass;

            regulationPass = ValidateRegulation(input);
            watershedPass = ValidateWatershed(input);
            rainfallPass = ValidateRainfall(input);
            soilsPass = ValidateSoils(input);

            List<ValidationTrace> failed = new List<ValidationTrace>();
            List<ValidationTrace> passed = new List<ValidationTrace>();
            List<ValidationTrace> traces = new List<ValidationTrace> { regulationPass, watershedPass, rainfallPass, soilsPass };

            // Sort ValidationTraces into pass / fail buckets.
            _sortVals(traces, passed, failed);

            // Merge error messages from failed traces into one single trace.
            ValidationTrace failLog =_mergeTraces(failed);

            // Check if all four of our traces passed.
            bool validated = _checkAllPassed(passed);

            if (validated)
                return true;
            else
            {
                String errorText = String.Format("User Input Invalid: {0}", failLog.message);
                throw new System.ArgumentException(errorText);
            }
        }

        static private bool _checkAllPassed(List<ValidationTrace> traces)
        {
            bool pass = false;
            const byte NUMBER_TO_PASS = 4;

            if (traces.Count == NUMBER_TO_PASS)
            {
                pass = true;
            }

            return pass;
        }

        static private void _sortVals(List<ValidationTrace> traces, List<ValidationTrace> passed, List<ValidationTrace> failed)
        {
            foreach (ValidationTrace trace in traces)
            {
                if (trace.passed == true)
                    passed.Add(trace);
                else
                    failed.Add(trace);
            }
        }

        static private ValidationTrace _mergeTraces(List<ValidationTrace> traces)
        {
            ValidationTrace outTrace = new ValidationTrace(ValidationTrace.InputType.Summary);
            String combinedString = "";
            foreach (ValidationTrace trace in traces)
            {
                combinedString += trace.message;
            }
            outTrace.message = combinedString;

            return outTrace;
        }

        static ValidationTrace ValidateRegulation(SMOT_IO.InputParams input)
        {
            // Initialize trace and define it as a "watershed" type trace
            ValidationTrace trace = new ValidationTrace(ValidationTrace.InputType.Regulation);

            const String invalidWQ = "Please enter a valid {0} water quality component";
            const String resetString = "Water quality components for the {0} regulation are reset";

            if (input.MS4 == true && input.MS4_WQ == _blankField)
            {
                trace.SetTraceInfo(false, String.Format(invalidWQ, "MS4"));
                return trace;
            }

            else if (input.applTMDL == true && input.applTMDL_WQ == _blankField)
            {
                trace.SetTraceInfo(false, String.Format(invalidWQ, "TMDL"));
                return trace;
            }

            else if ((input.applTMDL == true && input.applTMDL_WQ != _blankField) &&
                      (input.MS4 == true && input.MS4_WQ != _blankField))
            {
                String outString = @"and water quality related regulations. 
                The water quality related regulations include:
                - {0} (TMDL) - {1} (MS4)";
                trace.SetTraceInfo(true, String.Format(outString, input.applTMDL_WQ, input.MS4_WQ));
                return trace;
            }

            else if ((input.applTMDL == true && input.applTMDL_WQ != _blankField) &&
                      (input.MS4 == false && input.MS4_WQ == _blankField))
            {
                String outString = @"and water quality related regulations. 
                The water quality related regulations include:
                - {0} (TMDL)";
                trace.SetTraceInfo(true, String.Format(outString, input.applTMDL_WQ));
                return trace;
            }

            else if ((input.applTMDL == false && input.applTMDL_WQ == _blankField) &&
                      (input.MS4 == true && input.MS4_WQ != _blankField))
            {
                String outString = @"and water quality related regulations. 
                The water quality related regulations include:
                - {0} (MS4)";
                trace.SetTraceInfo(true, String.Format(outString, input.MS4_WQ));
                return trace;
            }

            else if ((input.MS4 == false && input.MS4_WQ != _blankField))
            {
                input.MS4_WQ = _blankField;
                trace.SetTraceInfo(true, String.Format(resetString, "MS4"));
                return trace;
            }

            else if ((input.applTMDL == false && input.applTMDL_WQ != _blankField))
            {
                input.applTMDL_WQ = _blankField;
                trace.SetTraceInfo(true, String.Format(resetString, "TMDL"));
                return trace;
            }

            else
            {
                trace.SetTraceInfo(true, "Regulation values are valid.");
            }

            return trace;
        }

        static ValidationTrace ValidateWatershed(SMOT_IO.InputParams input)
        {
            ValidationTrace trace = new ValidationTrace(ValidationTrace.InputType.Watershed);

            if (_checkBaseInfo(input) == false)
            {
                trace.SetTraceInfo(false, "Please enter base location or name");
                return trace;
            }

            if (_checkWatershedAreaInfo(input) == false)
            {
                trace.SetTraceInfo(false, "Please enter a valid impervious/pervious storage depth and total impervious area.");
                return trace;
            }

            if (_checkWatershedIsNumbers(input) == false)
            {
                trace.SetTraceInfo(false, "Please enter valid impervious/pervious land cover values.");
                return trace;
            }

            else
            {
                trace.SetTraceInfo(true, "Watershed values are valid.");
                return trace;
            }


        }

        static ValidationTrace ValidateRainfall(SMOT_IO.InputParams input)
        {
            // Initialize trace and define it as a "watershed" type trace
            ValidationTrace trace = new ValidationTrace(ValidationTrace.InputType.Rainfall);

            if ((input.percentile95Rainfall > 0) && (input.averageDryDays > 0))
            {
                trace.SetTraceInfo(true, "Rainfall values are valid.");
            }
            else
            {
                trace.SetTraceInfo(false, "Invalid average dry period or 95th percentile rainfall depth. Check and rerun rainfall data analysis.");
            }

            return trace;
        }

        static ValidationTrace ValidateSoils(SMOT_IO.InputParams input)
        {
            // Initialize trace and define it as a "watershed" type trace
            ValidationTrace trace = new ValidationTrace(ValidationTrace.InputType.Soils);

            if (_checkSoilPercent(input) == false)
            {
                trace.SetTraceInfo(false, "Invalid HSG soil areas. Please make sure they are in decimal format and greater than zero.");
            }
            else
            {
                trace.SetTraceInfo(true, "Soil values are valid.");
            }

            return trace;
        }

        static private bool _checkSoilPercent(SMOT_IO.InputParams input)
        {
            if ((input.hsgAreaA >= 0) && (input.hsgAreaB >= 0) && (input.hsgAreaC >= 0) && (input.hsgAreaD >= 0))
            {
                if (input.hsgAreaA + input.hsgAreaB + input.hsgAreaC + input.hsgAreaD == 0)
                {
                    return false;
                }
                else
                    return true;
            }
            else
                return false;
        }

        static private bool _checkBaseInfo(SMOT_IO.InputParams input)
        {
            if (input.baseLocation != _blankField && input.baseName != _blankField)
                return true;
            else
                return false;
        }

        static private bool _checkWatershedAreaInfo(SMOT_IO.InputParams input)
        {
            if (input.storageDepthImpervious <= 0 || input.storageDepthPervious <= 0 || input.totalImpArea <= 0)
                return false;
            else
                return true;
        }

        static private bool _checkWatershedIsNumbers(SMOT_IO.InputParams input)
        {
            if (input.landCoverImpervious >= 0 && input.landCoverPervious >= 0)
            {
                if (input.landCoverPervious + input.landCoverImpervious == 0)
                {
                    return false;
                }
                else
                    return true;
            }
            else
                return false;
        }


    }

    class ValidationTrace
    {
        public enum InputType { Watershed, Soils, Rainfall, Regulation, Summary };

        public bool passed;
        public String message;
        InputType inputType;

        public ValidationTrace(InputType inputType)
        {
            this.passed = false;
            this.message = "";
            this.inputType = inputType;
        }

        public bool isWatershed()
        {
            if (this.inputType == InputType.Watershed)
                return true;
            else
                return false;
        }

        public bool isSoils()
        {
            if (this.inputType == InputType.Soils)
                return true;
            else
                return false;
        }

        public bool isRainfall()
        {
            if (this.inputType == InputType.Rainfall)
                return true;
            else
                return false;
        }

        public bool isRegulation()
        {
            if (this.inputType == InputType.Regulation)
                return true;
            else
                return false;
        }

        public void SetTraceInfo(bool passed, String message)
        {
            this.passed = passed;
            this.message = message;
        }

        public bool isSummary()
        {
            if (this.inputType == InputType.Summary)
                return true;
            else
                return false;
        }
    }
}
