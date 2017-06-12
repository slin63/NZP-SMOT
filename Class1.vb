


Dim okClear As String
Dim bAllValidated, bSoilsValidated, bRainfallValidated, bWatershedValidated, bRegulationValidated, bHortonValidated, bBMPValidated As Integer
Dim HortonMaxRate, HortonMinRate, HDryDays, HortonDecay As Double


Sub present()
    Worksheets("Introduction").Protect DrawingObjects:=True, Contents:=True, Scenarios:=True
    Worksheets("Introduction").EnableSelection = xlNoSelection
    Worksheets("DecisionFlowChart").Protect DrawingObjects:=True, Contents:=True, Scenarios:=True
    Worksheets("DecisionFlowChart").EnableSelection = xlNoSelection

    Worksheets("ModelInputsGeneral").Protect DrawingObjects:=True, Contents:=True, Scenarios:=True
    'Worksheets("ModelInputsGeneral").EnableSelection = xlUnlockedCells

    Worksheets("ModelInputUserSuppliedRainfall").Protect DrawingObjects:=True, Contents:=True, Scenarios:=True
    Worksheets("ModelInputUserSuppliedRainfall").EnableSelection = xlUnlockedCells

    Worksheets("ModelResults").Protect DrawingObjects:=True, Contents:=True, Scenarios:=True
    Worksheets("ModelResults").EnableSelection = xlNoSelection

    Worksheets("ReferencesAndAssumptions").Protect DrawingObjects:=True, Contents:=True, Scenarios:=True
    Worksheets("ReferencesAndAssumptions").EnableSelection = xlNoSelection

End Sub
Sub Init()
    Application.ScreenUpdating = False
    Worksheets("ModelInputsGeneral").Unprotect
    Worksheets("ModelInputUserSuppliedRainfall").Unprotect
    Worksheets("ModelResults").Unprotect
End Sub
Sub auto_open()
    Worksheets("Introduction").Activate
    Call Init()
    Call present()
    Call Names()
End Sub

'To validate the model inputs, model won't run until all inputs are validated
Sub ValidateInputs()

    Call Init()
    bAllValidated = 0
    bSoilsValidated = 0
    bRainfallValidated = 0
    bWatershedValidated = 0
    bRegulationValidated = 0

    bHortonValidated = 0
    bBMPValidated = 0

    Range("WQ_Out_Part1").Value = "only."
    Range("WQ_Out_Part2").Value = ""
    Range("WQ_Out_Part3").Value = ""
    Range("WQ_Out_Part4").Value = ""


    'Yi Xu 11/14/2013

    Call ValidateWatershed()
    Call ValidateSoils()
    Call ValidateRainfall()
    Call ValidateRegulation()

    'Yi Xu 11/14/2013
    'Call ValidateHorton



    'Call init
    If bSoilsValidated = 1 And bRainfallValidated = 1 And bWatershedValidated = 1 And bRegulationValidated = 1 Then
        bAllValidated = 1
        okClear = MsgBox("All inputs are valid, please run the decision model.", vbInformation, "Inputs Validated")
    End If





    'Ajust Total area of development
    'If Range("TotaliArea").Value <> (Range("HSG_A_Area").Value + Range("HSG_B_Area").Value + Range("HSG_C_Area").Value + Range("HSG_D_Area").Value) Then
    'Range("TotaliArea").Value = Range("HSG_A_Area").Value + Range("HSG_B_Area").Value + Range("HSG_C_Area").Value + Range("HSG_D_Area").Value
    'End If


    'Give Horton parameters based on the ratio between the area of soil HSG and total area of development


    HortonMaxRate = Range("aHortonMaxRate").Value * Range("HSG_A_Area").Value / Range("TotaliArea").Value +
                                   Range("bHortonMaxRate").Value * Range("HSG_B_Area").Value / Range("TotaliArea").Value +
                                   Range("cHortonMaxRate").Value * Range("HSG_C_Area").Value / Range("TotaliArea").Value +
                                   Range("dHortonMaxRate").Value * Range("HSG_D_Area").Value / Range("TotaliArea").Value

    HortonMinRate = Range("aHortonMinRate").Value * Range("HSG_A_Area").Value / Range("TotaliArea").Value +
                                   Range("bHortonMinRate").Value * Range("HSG_B_Area").Value / Range("TotaliArea").Value +
                                   Range("cHortonMinRate").Value * Range("HSG_C_Area").Value / Range("TotaliArea").Value +
                                   Range("dHortonMinRate").Value * Range("HSG_D_Area").Value / Range("TotaliArea").Value

    HDryDays = Range("aDryDays").Value * Range("HSG_A_Area").Value / Range("TotaliArea").Value +
                             Range("bDryDays").Value * Range("HSG_B_Area").Value / Range("TotaliArea").Value +
                             Range("cDryDays").Value * Range("HSG_C_Area").Value / Range("TotaliArea").Value +
                             Range("dDryDays").Value * Range("HSG_D_Area").Value / Range("TotaliArea").Value


    HortonDecay = Range("aHortonDecay").Value * Range("HSG_A_Area").Value / Range("TotaliArea").Value +
                                 Range("bHortonDecay").Value * Range("HSG_B_Area").Value / Range("TotaliArea").Value +
                                 Range("cHortonDecay").Value * Range("HSG_C_Area").Value / Range("TotaliArea").Value +
                                 Range("dHortonDecay").Value * Range("HSG_D_Area").Value / Range("TotaliArea").Value


    'Yi Xu 11/14/2013
    Call WatershedGeneral()



    Call present()


End Sub



Sub ValidateRegulation()
    Call Init()

    If Range("MS4Reg").Value = "Yes" And Range("MS4_WQ").Value = "N/A" Then
        okClear = MsgBox("Please enter a valid MS4 water quality component.", vbExclamation, "Invalid Input")
        bRegulationValidated = 0
    ElseIf Range("TMDLReg").Value = "Yes" And Range("TMDL_WQ").Value = "N/A" Then
        okClear = MsgBox("Please enter a valid TMDL water quality component.", vbInformation, "Invalid Input")
        bRegulationValidated = 0
    ElseIf (Range("TMDLReg").Value = "Yes" And Range("TMDL_WQ").Value <> "N/A") And (Range("MS4Reg").Value = "Yes" And Range("MS4_WQ").Value <> "N/A") Then
        bRegulationValidated = 1
        Range("WQ_Out_Part1").Value = "and water quality related regulations."
        Range("WQ_Out_Part2").Value = "The water quality related regulations include:"
        Range("WQ_Out_Part3").Value = "-" & Range("TMDL_WQ").Value & " (TMDL)"
        Range("WQ_Out_Part4").Value = "-" & Range("MS4_WQ").Value & " (MS4)"
    ElseIf (Range("TMDLReg").Value = "Yes" And Range("TMDL_WQ").Value <> "N/A") And (Range("MS4Reg").Value = "No" And Range("MS4_WQ").Value = "N/A") Then
        bRegulationValidated = 1
        Range("WQ_Out_Part1").Value = "and water quality related regulations."
        Range("WQ_Out_Part2").Value = "The water quality related regulations include:"
        Range("WQ_Out_Part3").Value = "-" & Range("TMDL_WQ").Value & " (TMDL)"
    ElseIf (Range("TMDLReg").Value = "No" And Range("TMDL_WQ").Value = "N/A") And (Range("MS4Reg").Value = "Yes" And Range("MS4_WQ").Value <> "N/A") Then
        bRegulationValidated = 1
        Range("WQ_Out_Part1").Value = "and water quality related regulations."
        Range("WQ_Out_Part2").Value = "The water quality related regulations include:"
        Range("WQ_Out_Part3").Value = "-" & Range("MS4_WQ").Value & " (MS4)"
    ElseIf Range("MS4Reg").Value = "No" And Range("MS4_WQ").Value <> "N/A" Then
        Range("MS4_WQ").Value = "N/A"
        okClear = MsgBox("Water quality components for the MS4 regulation are reset.", vbInformation, "Inputs Updated")
        bRegulationValidated = 1
        Range("WQ_Out_Part1").Value = "only."
    ElseIf Range("TMDLReg").Value = "No" And Range("TMDL_WQ").Value <> "N/A" Then
        Range("TMDL_WQ").Value = "N/A"
        okClear = MsgBox("Water quality components for the TMDL regulation are reset.", vbInformation, "Inputs Updated")
        bRegulationValidated = 1
        Range("WQ_Out_Part1").Value = "only."
    Else
        bRegulationValidated = 1
        Range("WQ_Out_Part1").Value = "only."
    End If
    Call present()
End Sub

'Modified by Yi Xu 11/13/2013
Sub ValidateWatershed()
    Call Init()
    Worksheets("ModelInputsGeneral").Activate

    If Range("BaseName").Value <> "N/A" Then
        If Range("BaseLocation").Value <> "N/A" Then
            'If Range("WatershedNum").Value <> "N/A" Then
            If Range("ImperviousDepth").Value <> "N/A" Then
                If Range("PerviousDepth").Value <> "N/A" Then
                    If Range("TotaliArea").Value <> "N/A" Then
                        If Worksheets.Application.IsNumber(Range("ImpvArea").Value) = True And Range("ImpvArea").Value >= 0 Then
                            If Worksheets.Application.IsNumber(Range("PervArea").Value) = True And Range("PervArea").Value >= 0 Then
                                If Range("ImpvArea").Value + Range("PervArea").Value = 0 Then
                                    okClear = MsgBox("Please check the total watershed area.", vbExclamation, "Invalid Input")
                                Else
                                    bWatershedValidated = 1
                                    Exit Sub
                                End If
                            Else
                                okClear = MsgBox("Please enter a valid watershed impervious area.", vbExclamation, "Invalid Input")
                            End If
                        Else
                            okClear = MsgBox("Please enter a valid watershed pervious area.", vbExclamation, "Invalid Input")
                        End If
                    Else
                        okClear = MsgBox("Please enter a valid area of development.", vbExclamation, "Invalid Input")
                    End If
                Else
                    okClear = MsgBox("Please enter a valid pervious depression storage depth.", vbExclamation, "Invalid Input")
                End If
            Else
                okClear = MsgBox("Please enter a valid impervious depression storage depth.", vbExclamation, "Invalid Input")
            End If
            'Else
            'okClear = MsgBox("Please enter the number of subwatersheds.", vbExclamation, "Invalid Input")
            'End If
        Else
            okClear = MsgBox("Please enter the base location.", vbExclamation, "Invalid Input")
        End If
    Else
        okClear = MsgBox("Please enter the base name.", vbExclamation, "Invalid Input")
    End If
    Call present()
End Sub
'validate rainfall analysis results
Sub ValidateRainfall()
    Call Init()
    Worksheets("ModelInputsGeneral").Activate

    If Worksheets.Application.IsNumber(Range("Rainfall95thPercentile_Output").Value) = True And Range("Rainfall95thPercentile_Output").Value > 0 Then
        If Worksheets.Application.IsNumber(Range("AvgDryDays_Output").Value) = True And Range("AvgDryDays_Output").Value > 0 Then
            bRainfallValidated = 1
            Exit Sub
        Else
            okClear = MsgBox("Invalid average dry period, please run the rainfall data analysis.", vbExclamation, "Invalid Input")
        End If
    Else
        okClear = MsgBox("Invalid 95th percentile rainfall depth, please run the rainfall data analysis.", vbExclamation, "Invalid Input")
    End If
    Call present()
End Sub
'validate soils input
Sub ValidateSoils()
    Call Init()
    Worksheets("ModelInputsGeneral").Activate

    'check to make sure all the acreages for the HSG soil types are numerical, zero values are allowed
    If (Worksheets.Application.IsNumber(Range("HSG_A_Area").Value) = True And Range("HSG_A_Area").Value >= 0) Then
        If (Worksheets.Application.IsNumber(Range("HSG_B_Area").Value) = True And Range("HSG_B_Area").Value >= 0) Then
            If Worksheets.Application.IsNumber(Range("HSG_C_Area").Value) = True And Range("HSG_C_Area").Value >= 0 Then
                If Worksheets.Application.IsNumber(Range("HSG_D_Area").Value) = True And Range("HSG_D_Area").Value >= 0 Then
                    If Range("HSG_A_Area").Value + Range("HSG_B_Area").Value + Range("HSG_C_Area").Value + Range("HSG_D_Area").Value = 0 Then
                        okClear = MsgBox("Please check the total area of soils group.", vbExclamation, "Invalid Input")
                    Else
                        bSoilsValidated = 1


                        Exit Sub
                    End If
                Else
                    okClear = MsgBox("Please check the input for HSG D soils group.", vbExclamation, "Invalid Input")
                End If
            Else
                okClear = MsgBox("Please check the input for HSG C soils group.", vbExclamation, "Invalid Input")
            End If
        Else
            okClear = MsgBox("Please check the input for HSG B soils group.", vbExclamation, "Invalid Input")
        End If
    Else
        okClear = MsgBox("Please check the input for HSG A soils group.", vbExclamation, "Invalid Input")
    End If
    Call present()
End Sub

Sub RunModel()
    'check to see if the validation is complete, then run the model, and present the results
    Call Init()
    Dim TotalSoilArea As Double
    Dim HSG_A_Percent, HSG_B_Percent, HSG_C_Percent, HSG_D_Percent As Double

    Range("BaseName_Results").Value = ""
    Range("BaseLocation_Results").Value = ""
    Range("OnlinePond_Result").Value = ""
    Range("HSG_A_Pct_Result").Value = 0
    Range("HSG_B_Pct_Result").Value = 0
    Range("HSG_C_Pct_Result").Value = 0
    Range("HSG_D_Pct_Result").Value = 0
    Range("AvgDryDays_Result").Value = 0
    Range("Rainfall95thPercentile_Result").Value = 0
    Range("InitAbs_Result").Value = 0
    Range("BaseName_Recommend").Value = ""
    Range("Approach_Recommend").Value = ""
    Range("Reason1").Value = ""
    Range("Reason2").Value = ""
    Range("Reason3").Value = ""
    Range("Reason4").Value = ""
    Range("Reason5").Value = ""

    If bAllValidated = 1 Then

        Range("BaseName_Results").Value = Range("BaseName").Value
        Range("BaseLocation_Results").Value = Range("BaseLocation").Value
        Range("OnlinePond_Result").Value = Range("OnlinePond").Value

        TotalSoilArea = Range("HSG_A_Area").Value + Range("HSG_B_Area").Value + Range("HSG_C_Area").Value + Range("HSG_D_Area").Value

        Range("HSG_A_Pct_Result").Value = Range("HSG_A_Area").Value / TotalSoilArea
        Range("HSG_B_Pct_Result").Value = Range("HSG_B_Area").Value / TotalSoilArea
        Range("HSG_C_Pct_Result").Value = Range("HSG_C_Area").Value / TotalSoilArea
        Range("HSG_D_Pct_Result").Value = Range("HSG_D_Area").Value / TotalSoilArea

        Range("AvgDryDays_Result").Value = Range("AvgDryDays_Output").Value
        Range("Rainfall95thPercentile_Result").Value = Range("Rainfall95thPercentile_Output").Value

        Range("InitAbs_Result").Value = (Range("HSG_A_Area").Value * 2.08 + Range("HSG_B_Area").Value * 0.9 + Range("HSG_C_Area").Value * 0.53 + Range("HSG_D_Area").Value * 0.38) / TotalSoilArea

        With Range("Rainfall95thPercentile_Result")
            .NumberFormat = "0.00"
            .Font.Bold = True
            .Value = Range("Rainfall95thPercentile_Result").Value
            '.Interior.ColorIndex = 44
        End With


        With Range("AvgDryDays_Result")
            .NumberFormat = "0.00"
            .Font.Bold = True
            .Value = Range("AvgDryDays_Result").Value
            '.Interior.ColorIndex = 44
        End With


        With Range("InitAbs_Result")
            .NumberFormat = "0.00"
            .Font.Bold = True
            .Value = Range("InitAbs_Result").Value
            '.Interior.ColorIndex = 44
        End With

        Range("BaseName_Recommend").Value = Range("BaseName").Value

        With Range("BaseName_Recommend")
            .HorizontalAlignment = xlCenter
            .VerticalAlignment = xlCenter
            .WrapText = False
            .Orientation = 0
            .AddIndent = False
            .IndentLevel = 0
            .ShrinkToFit = False
            .ReadingOrder = xlContext
            .MergeCells = True
        End With

        With Range("BaseName_Recommend").Font
            .Color = -4165632
            .TintAndShade = 0
        End With

        Range("Reason1").Value = ""
        Range("Reason2").Value = ""
        Range("Reason3").Value = ""
        Range("Reason4").Value = ""
        Range("Reason5").Value = ""



        Call Analysis()

        bAllValidated = 0

        okClear = MsgBox("Analysis completed, please check the results page.", vbInformation, "Analysis Completed")
        Worksheets("ModelResults").Activate
    Else
        okClear = MsgBox("Before running the model, please validate inputs first.", vbExclamation, "Validation needed")





        Call present()

        Exit Sub
    End If
    Call present()


    Worksheets("WatershedSummary").Visible = False



End Sub

'Modified by Yi Xu 11/18/2014
Sub Analysis()

    'check for soil heterogenity
    Call HeteroAnalysis()
    Call Init()
    Dim i As Integer
    Range("DesignStorm_BMP").Value = ""
    Range("CS_BMP").Value = ""
    Range("CSO_BMP").Value = ""

    'carry out the modeling approach evaluation
    If Range("OnlinePond").Value = "No" Then
        If (Range("TMDLReg").Value = "No" And Range("TMDL_WQ").Value = "N/A") Or (Range("TMDLReg").Value = "Yes" And Range("TMDL_WQ").Value = "N/A") Then

            If (Range("MS4Reg").Value = "No" And Range("MS4_WQ").Value = "N/A") Or (Range("MS4Reg").Value = "Yes" And Range("MS4_WQ").Value = "N/A") Then
                ' the base does not have any water quality components

                'Design storm for BMP
                Range("DesignStorm_BMP").Value = DesignStormBMP(Range("TotaliArea").Value, Range("Rainfall95thPercentile_Output").Value)
                '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
                Call RunSimulation()  'Continuous Simulation
                Range("CS_BMP").Value = Range("CS_BMP_spreadsheet").Value

                Range("CSO_BMP").Value = Range("CS_BMP_spreadsheet").Value + AddHeterogeneity(
                                                     Range("HSG_A_Area").Value,
                                                     Range("HSG_B_Area").Value,
                                                     Range("HSG_C_Area").Value,
                                                     Range("HSG_D_Area").Value) *
                                                     Range("CS_BMP_spreadsheet").Value

                If Range("CS_BMP").Value > Range("DesignStorm_BMP").Value Then

                    Range("Approach_Recommend").Value = "Design Storm"

                    Range("Reason1").Value = "- BMP size in Continuous Simulation > BMP size in Design Storm."
                    Range("Reason2").Value = ""

                    Worksheets("WatershedSummary").Visible = False
                    Charts("Chart1").Visible = False
                    Worksheets("FDC").Visible = False

                Else
                    '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
                    'call Continuous Simulation Coupled with Optimization

                    Range("CS_BMP").Value = Range("CS_BMP_spreadsheet").Value
                    Range("CSO_BMP").Value = Range("CS_BMP_spreadsheet").Value + AddHeterogeneity(
                                             Range("HSG_A_Area").Value,
                                             Range("HSG_B_Area").Value,
                                             Range("HSG_C_Area").Value,
                                             Range("HSG_D_Area").Value) *
                                             Range("CS_BMP_spreadsheet").Value


                    If AddHeterogeneity(Range("HSG_A_Area").Value,
                                             Range("HSG_B_Area").Value,
                                             Range("HSG_C_Area").Value,
                                             Range("HSG_D_Area").Value) < -0.15 Then

                        Range("Approach_Recommend").Value = "Continuous Simulation Coupled with Optimization"
                        Range("Reason1").Value = "- BMP size in Continuous Simulation < BMP size in Design Storm."
                        Range("Reason2").Value = "- The soils are heterogeneous and the optimization result is significant."

                    Else
                        Range("Approach_Recommend").Value = "Continuous Simulation Only"
                        Range("Reason1").Value = "- BMP size in Continuous Simulation < BMP size in Design Storm."
                        Range("Reason2").Value = "- The soils are not heterogeneous enough and the optimization result is not significant."


                    End If


                    Charts("Chart1").Visible = False
                    Worksheets("FDC").Visible = False
                    '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

                End If

            Else
                'the base does have MS4 permit that has water quality components

                'Design storm for BMP
                Range("DesignStorm_BMP").Value = DesignStormBMP(Range("TotaliArea").Value, Range("Rainfall95thPercentile_Output").Value)

                '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
                'call Continuous Simulation Coupled with Optimization
                Call RunSimulation()
                Range("CS_BMP").Value = Range("CS_BMP_spreadsheet").Value
                Range("CSO_BMP").Value = Range("CS_BMP_spreadsheet").Value + AddHeterogeneity(
                Range("HSG_A_Area").Value,
                Range("HSG_B_Area").Value,
                Range("HSG_C_Area").Value,
                Range("HSG_D_Area").Value) *
                Range("CS_BMP_spreadsheet").Value

                If AddHeterogeneity(Range("HSG_A_Area").Value,
                    Range("HSG_B_Area").Value,
                    Range("HSG_C_Area").Value,
                    Range("HSG_D_Area").Value) < -0.15 Then

                    Range("Approach_Recommend").Value = "Continuous Simulation Coupled with Optimization"
                    Range("Reason1").Value = "- The MS4 regulation has water quality components."
                    Range("Reason2").Value = "- The soils are heterogeneous and the optimization results is significant."

                Else
                    Range("Approach_Recommend").Value = "Continuous Simulation Only"
                    Range("Reason1").Value = "- The MS4 regulation has water quality components."
                    Range("Reason2").Value = "- The soils are not heterogeneous enough and the optimization results is not significant."

                End If

                Charts("Chart1").Visible = False
                Worksheets("FDC").Visible = False
                '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

            End If
        Else
            ' the base does have TMDL permit that has water quality components
            If (Range("MS4Reg").Value = "No" And Range("MS4_WQ").Value = "N/A") Or (Range("MS4Reg").Value = "Yes" And Range("MS4_WQ").Value = "N/A") Then
                'the base only has TMDL permit water quality components

                'Design storm for BMP
                Range("DesignStorm_BMP").Value = DesignStormBMP(Range("TotaliArea").Value, Range("Rainfall95thPercentile_Output").Value)
                '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
                'call Continuous Simulation Coupled with Optimization
                Call RunSimulation()
                Range("CS_BMP").Value = Range("CS_BMP_spreadsheet").Value
                Range("CSO_BMP").Value = Range("CS_BMP_spreadsheet").Value + AddHeterogeneity(
                                                     Range("HSG_A_Area").Value,
                                                     Range("HSG_B_Area").Value,
                                                     Range("HSG_C_Area").Value,
                                                     Range("HSG_D_Area").Value) *
                                                     Range("CS_BMP_spreadsheet").Value

                If AddHeterogeneity(Range("HSG_A_Area").Value,
                                                     Range("HSG_B_Area").Value,
                                                     Range("HSG_C_Area").Value,
                                                     Range("HSG_D_Area").Value) < -0.15 Then

                    Range("Approach_Recommend").Value = "Continuous Simulation Coupled with Optimization"
                    Range("Reason1").Value = "- The TMDL regulation has water quality components."
                    Range("Reason2").Value = "- The soils are heterogeneous and the optimization result is significant."
                Else
                    Range("Approach_Recommend").Value = "Continuous Simulation Only"
                    Range("Reason1").Value = "- The TMDL regulation has water quality components."
                    Range("Reason2").Value = "- The soils are not heterogeneous enough and the optimization result is not significant."

                End If

                Charts("Chart1").Visible = False
                Worksheets("FDC").Visible = False

            Else
                ' the base has both TMDL and MS4 water quality componets
                'Design storm for BMP
                Range("DesignStorm_BMP").Value = DesignStormBMP(Range("TotaliArea").Value, Range("Rainfall95thPercentile_Output").Value)

                'call Continuous Simulation Coupled with Optimization
                Call RunSimulation()
                Range("CS_BMP").Value = Range("CS_BMP_spreadsheet").Value
                Range("CSO_BMP").Value = Range("CS_BMP_spreadsheet").Value + AddHeterogeneity(
                                                     Range("HSG_A_Area").Value,
                                                     Range("HSG_B_Area").Value,
                                                     Range("HSG_C_Area").Value,
                                                     Range("HSG_D_Area").Value) *
                                                     Range("CS_BMP_spreadsheet").Value

                If AddHeterogeneity(Range("HSG_A_Area").Value,
                                                     Range("HSG_B_Area").Value,
                                                     Range("HSG_C_Area").Value,
                                                     Range("HSG_D_Area").Value) < -0.15 Then

                    Range("Approach_Recommend").Value = "Continuous Simulation Coupled with Optimization"
                    Range("Reason1").Value = "- The MS4 regulation has water quality components."
                    Range("Reason2").Value = "- The TMDL regulation has water quality components."
                    Range("Reason3").Value = "- The soils are heterogeneous and the optimization result is significant."
                Else
                    Range("Approach_Recommend").Value = "Continuous Simulation Only"
                    Range("Reason1").Value = "- The MS4 regulation has water quality components."
                    Range("Reason2").Value = "- The TMDL regulation has water quality components."
                    Range("Reason3").Value = "- The soils are not heterogeneous enough and the optimization result is not significant."
                End If

                Charts("Chart1").Visible = False
                Worksheets("FDC").Visible = False
                '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            End If
        End If
    Else
        'the base does have online pond

        If (Range("TMDLReg").Value = "No" And Range("TMDL_WQ").Value = "N/A") Or (Range("TMDLReg").Value = "Yes" And Range("TMDL_WQ").Value = "N/A") Then
            If (Range("MS4Reg").Value = "No" And Range("MS4_WQ").Value = "N/A") Or (Range("MS4Reg").Value = "Yes" And Range("MS4_WQ").Value = "N/A") Then
                If AddHeterogeneity(Range("HSG_A_Area").Value,
                                                     Range("HSG_B_Area").Value,
                                                     Range("HSG_C_Area").Value,
                                                     Range("HSG_D_Area").Value) < -0.15 Then

                    Range("Approach_Recommend").Value = "Continuous Simulation Coupled with Optimization"
                    Range("Reason1").Value = "- The watershed has major online pond."
                    Range("Reason2").Value = "- The soils are heterogeneous and the optimization result is significant."
                Else
                    Range("Approach_Recommend").Value = "Continuous Simulation Only"
                    Range("Reason1").Value = "- The watershed has major online pond."
                    Range("Reason2").Value = "- The soils are not heterogeneous enough and the optimization result is not significant."
                End If
            Else
                If AddHeterogeneity(Range("HSG_A_Area").Value,
                                                     Range("HSG_B_Area").Value,
                                                     Range("HSG_C_Area").Value,
                                                     Range("HSG_D_Area").Value) < -0.15 Then

                    Range("Approach_Recommend").Value = "Continuous Simulation Coupled with Optimization."
                    Range("Reason1").Value = "- The watershed has major online pond."
                    Range("Reason2").Value = "- The MS4 regulation has water quality components."
                    Range("Reason3").Value = "- The soils are heterogeneous and the optimization result is significant."
                Else
                    Range("Approach_Recommend").Value = "Continuous Simulation Only."
                    Range("Reason1").Value = "- The watershed has major online pond."
                    Range("Reason2").Value = "- The MS4 regulation has water quality components."
                    Range("Reason3").Value = "- The soils are not heterogeneous enough and the optimization result is not significant."
                End If
            End If
        Else

            If (Range("MS4Reg").Value = "No" And Range("MS4_WQ").Value = "N/A") Or (Range("MS4Reg").Value = "Yes" And Range("MS4_WQ").Value = "N/A") Then
                If AddHeterogeneity(Range("HSG_A_Area").Value,
                                                     Range("HSG_B_Area").Value,
                                                     Range("HSG_C_Area").Value,
                                                     Range("HSG_D_Area").Value) < -0.15 Then
                    Range("Approach_Recommend").Value = "Continuous Simulation Coupled with Optimization"
                    Range("Reason1").Value = "- The watershed has major online pond."
                    Range("Reason2").Value = "- The TMDL regulation has water quality components."
                    Range("Reason3").Value = "- The soils are heterogeneous and the optimization result is significant"

                Else
                    Range("Approach_Recommend").Value = "Continuous Simulation Only"
                    Range("Reason1").Value = "- The watershed has major online pond."
                    Range("Reason2").Value = "- The TMDL regulation has water quality components."
                    Range("Reason3").Value = "- The soils are not heterogeneous enough and the optimization result is not significant"

                End If

            Else
                If AddHeterogeneity(Range("HSG_A_Area").Value,
                                                     Range("HSG_B_Area").Value,
                                                     Range("HSG_C_Area").Value,
                                                     Range("HSG_D_Area").Value) < -0.15 Then
                    Range("Approach_Recommend").Value = "Continuous Simulation Coupled with Optimization"
                    Range("Reason1").Value = "- The watershed has major online pond."
                    Range("Reason2").Value = "- The MS4 regulation has water quality components."
                    Range("Reason3").Value = "- The TMDL regulation has water quality components."
                    Range("Reason4").Value = "- The soils are heterogeneous and the optimization result is significant"
                Else
                    Range("Approach_Recommend").Value = "Continuous Simulation Only"
                    Range("Reason1").Value = "- The watershed has major online pond."
                    Range("Reason2").Value = "- The MS4 regulation has water quality components."
                    Range("Reason3").Value = "- The TMDL regulation has water quality components."
                    Range("Reason4").Value = "- The soils are not heterogeneous enough and the optimization result is not significant"
                End If



            End If
        End If


        'Design storm for BMP
        Range("DesignStorm_BMP").Value = DesignStormBMP(Range("TotaliArea").Value, Range("Rainfall95thPercentile_Output").Value)

        '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        'call Continuous Simulation Coupled with Optimization
        Call RunSimulation()
        Range("CS_BMP").Value = Range("CS_BMP_spreadsheet").Value
        Range("CSO_BMP").Value = Range("CS_BMP_spreadsheet").Value + AddHeterogeneity(
                                 Range("HSG_A_Area").Value,
                                 Range("HSG_B_Area").Value,
                                 Range("HSG_C_Area").Value,
                                 Range("HSG_D_Area").Value) *
                                 Range("CS_BMP_spreadsheet").Value
        Charts("Chart1").Visible = False
        Worksheets("FDC").Visible = False
        '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''



    End If

    With Range("Approach_Recommend")
        .HorizontalAlignment = xlCenter
        .VerticalAlignment = xlCenter
        .WrapText = False
        .Orientation = 0
        .AddIndent = False
        .IndentLevel = 0
        .ShrinkToFit = False
        .ReadingOrder = xlContext
        .MergeCells = True
    End With

    With Range("Approach_Recommend").Font
        .Color = -4165632
        .TintAndShade = 0
        .Bold = True
    End With

    'Call present
End Sub



Sub reset()
    'reset all the inputs
    Call Init()
    Range("BaseName").Value = "N/A"
    Range("BaseLocation").Value = "N/A"
    Range("ImpvArea").Value = 0
    Range("PervArea").Value = 0
    Range("OnlinePond").Value = "No"

    Range("EISA438Reg").Value = "Yes"
    Range("TMDLReg").Value = "No"
    Range("MS4Reg").Value = "No"

    Range("EISA_WQ").Value = "N/A"
    Range("TMDL_WQ").Value = "N/A"
    Range("MS4_WQ").Value = "N/A"

    'Modified by Yi Xu 11/13/2013
    'Range("WatershedNum").Value = 0
    Range("ImperviousDepth").Value = 0
    Range("PerviousDepth").Value = 0


    bAllValidated = 0

    With Range("Rainfall95thPercentile_Output")
        .Value = 0
        .Font.Bold = False
        .Interior.Color = RGB(217, 217, 217)
        .NumberFormat = "0"
    End With


    With Range("AvgDryDays_Output")
        .Value = 0
        .Font.Bold = False
        .Interior.Color = RGB(217, 217, 217)
        .NumberFormat = "0"
    End With


    Range("HSG_A_Area").Value = 0
    Range("HSG_B_Area").Value = 0
    Range("HSG_C_Area").Value = 0
    Range("HSG_D_Area").Value = 0

    Range("percentile").Value = 0
    Range("avg_dry_days").Value = 0

    Range("percentile").Interior.Color = RGB(217, 217, 217)
    Range("avg_dry_days").Interior.Color = RGB(217, 217, 217)

    Range("BaseName").Select
    okClear = MsgBox("Reset completed, please enter base information.", vbInformation, "Reset completed")
    Call present()
End Sub

Sub CalculateBothMetrics()

    Call Init()
    Application.ScreenUpdating = False
    'first check to see if the rainfall time series are loaded
    If Range("dt_start").Offset(1, 0).Value = vbNullString Then
        okClear = MsgBox(
            "There are no data available, please paste hourly rainfall data at the designated starting point.",
            vbExclamation,
            "Missing Data")

        Range(Range("dt_start", Range("dt_start").Offset(1, 0).End(xlDown)), Range("dt_start", Range("dt_start").Offset(1, 1).End(xlDown))).Select
        Selection.Locked = False
        Worksheets("ModelInputUserSuppliedRainfall").Protect DrawingObjects:=True, Contents:=True, Scenarios:=True
            Worksheets("ModelInputUserSuppliedRainfall").EnableSelection = xlUnlockedCells
        Worksheets("ModelInputUserSuppliedRainfall").Range("D10").Select

        Exit Sub
    End If

    'carry out the calculations
    Call AggregateTimeseriesToDaily()
    Call Calculate95thPercentile()
    Call CalculateAverageDryDays()
    Call Init()
    Application.DisplayAlerts = False
    ThisWorkbook.Sheets("scratchpad").Delete
    Application.DisplayAlerts = True

    Worksheets("ModelInputUserSuppliedRainfall").Activate

    Range("f9", Range("h9").End(xlDown)).Select
    Selection.Clear
    Selection.Interior.Color = RGB(217, 217, 217)
    Selection.Interior.Pattern = xlSolid

    Range("dt_start").Offset(1, 0).Select

    Range(Range("dt_start", Range("dt_start").Offset(1, 0).End(xlDown)), Range("dt_start", Range("dt_start").Offset(1, 1).End(xlDown))).Select
    Selection.Locked = False

    Worksheets("ModelInputUserSuppliedRainfall").Protect DrawingObjects:=True, Contents:=True, Scenarios:=True
    Worksheets("ModelInputUserSuppliedRainfall").EnableSelection = xlUnlockedCells
    Worksheets("ModelInputUserSuppliedRainfall").Range("D10").Select

    Call present()
End Sub
Sub Calculate95thPercentile()
    'calculate the 95th percentile of rainfall
    Call Init()
    Dim minDate, maxDate As Date
    Dim threshold As Double

    threshold = 0.1

    minDate = Application.Min(Range(
        Range("dt_start").Offset(1),
        Range("dt_start").Offset(1).End(xlDown)))
    minDate = DateSerial(Year(minDate), Month(minDate), Day(minDate))
    maxDate = Application.Max(Range(
        Range("dt_start").Offset(1),
        Range("dt_start").Offset(1).End(xlDown)))
    maxDate = DateSerial(Year(maxDate), Month(maxDate), Day(maxDate))

    Range("percentile").FormulaArray = "=PERCENTILE(" &
        "if('scratchpad'!R1C2:R" &
        (Int(maxDate - minDate) + 1) & "C2>=" & threshold &
        ", 'scratchpad'!R1C2:R" &
        (Int(maxDate - minDate) + 1) &
        "C2), 0.95)"
    Range("percentile").Value = Range("percentile").Value
    Range("percentile").Interior.ColorIndex = 44

    With Range("percentile")
        .NumberFormat = "0.00"
        .Font.Bold = True
        .Interior.ColorIndex = 44
    End With

    With Range("Rainfall95thPercentile_Output")
        .NumberFormat = "0.00"
        .Font.Bold = True
        .Value = Range("percentile").Value
        .Interior.ColorIndex = 44
    End With
    Call present()
End Sub
Sub CalculateAverageDryDays()
    'calculate the average dry periods
    Call Init()
    Dim dryDayCount, dryPeriodCount As Integer
    Dim minDate, maxDate As Date
    Dim dryDayThreshold As Double
    Dim i

    dryDayThreshold = 0

    minDate = Application.Min(Range(
        Range("dt_start").Offset(1),
        Range("dt_start").Offset(1).End(xlDown)))
    minDate = DateSerial(Year(minDate), Month(minDate), Day(minDate))
    maxDate = Application.Max(Range(
        Range("dt_start").Offset(1),
        Range("dt_start").Offset(1).End(xlDown)))
    maxDate = DateSerial(Year(maxDate), Month(maxDate), Day(maxDate))

    ThisWorkbook.Sheets("scratchpad").Activate
    dryPeriodCount = 1
    For i = 0 To (Int(maxDate - minDate) + 1)
        If Range("B1").Offset(i, 0).Value <= dryDayThreshold Then
            dryDayCount = dryDayCount + 1
            If i > 0 Then
                If Range("B1").Offset(i - 1, 0).Value > dryDayTheshold Then
                    dryPeriodCount = dryPeriodCount + 1
                End If
            End If
        End If
    Next i

    Range("avg_dry_days").Value = dryDayCount / dryPeriodCount

    With Range("avg_dry_days")
        .NumberFormat = "0.00"
        .Font.Bold = True
        .Interior.ColorIndex = 44
    End With

    With Range("AvgDryDays_Output")
        .NumberFormat = "0.00"
        .Font.Bold = True
        .Value = Range("avg_dry_days").Value
        .Interior.ColorIndex = 44
    End With
    Call present()
End Sub
Sub AggregateTimeseriesToDaily()
    Call Init()
    Dim minDate, maxDate As Date
    Dim dataCount As Long
    Dim i As Long
    Dim dateRange As Range
    Dim okClear As String

    Worksheets("ModelInputUserSuppliedRainfall").Activate

    ' Check that there are data that we can use
    dataCount = Application.WorksheetFunction.Count(
    Range(Range("dt_start").Offset(1),
        Range("dt_start").Offset(1).End(xlDown)))

    ' Calculate YEAR, MONTH, DAY
    Range("dt_start").Offset(0, 2).Value = "Year"
    Range("dt_start").Offset(0, 3).Value = "Month"
    Range("dt_start").Offset(0, 4).Value = "Day"
    Range("dt_start").Offset(1, 2).Formula = "=YEAR(d10)"
    Range("dt_start").Offset(1, 3).Formula = "=MONTH(d10)"
    Range("dt_start").Offset(1, 4).Formula = "=DAY(d10)"
    Range("f10:H" & (dataCount + 8)).FillDown


    ' Calculate the min/max dates
    minDate = Application.Min(Range(
        Range("dt_start").Offset(1),
        Range("dt_start").Offset(1).End(xlDown)))
    minDate = DateSerial(Year(minDate), Month(minDate), Day(minDate))
    maxDate = Application.Max(Range(
        Range("dt_start").Offset(1),
        Range("dt_start").Offset(1).End(xlDown)))
    maxDate = DateSerial(Year(maxDate), Month(maxDate), Day(maxDate))

    ' Create scratchpad
    If SheetExists("scratchpad") Then
        Application.DisplayAlerts = False
        ThisWorkbook.Sheets("scratchpad").Delete
        Application.DisplayAlerts = True
    End If
    ThisWorkbook.Worksheets.Add().Name = "scratchpad"
    ThisWorkbook.Sheets("scratchpad").Activate

    ' Build a new date range on scratch pad
    For i = 0 To Int(maxDate - minDate)
        Range("A1").Offset(i, 0).Value =
            DateSerial(Year(minDate), Month(minDate), Day(minDate) + i)
    Next i

    ' Add formulas to calculate daily timeseries
    Range("B1").Formula =
        "=SUMIFS('ModelInputUserSuppliedRainfall'!$e$10:$e$" & (dataCount + 7) _
        & ", 'ModelInputUserSuppliedRainfall'!$f$10:$f$" & (dataCount + 7) _
        & ", YEAR('scratchpad'!A1)" _
        & ", 'ModelInputUserSuppliedRainfall'!$g$10:$g$" & (dataCount + 7) _
        & ", MONTH('scratchpad'!A1)" _
        & ", 'ModelInputUserSuppliedRainfall'!$h$10:$h$" & (dataCount + 7) _
        & ", DAY('scratchpad'!A1))"

    Range("B1:B" & (Int(maxDate - minDate) + 1)).FillDown
    Call present()
End Sub

Sub ClearData()
    'clear the time series input
    Call Init()
    Dim doClear As String


    Worksheets("ModelInputUserSuppliedRainfall").Activate

    If Range("dt_start").Offset(1, 0).Value <> vbNullString Then
        doClear = MsgBox("Clear existing hourly rainfall timeseries?",
                          vbYesNo,
                          "Confirm Clearing of rainfall data")
        If doClear <> vbNo Then
            Range("dt_start", Range("dt_start").Offset(1, 0).End(xlDown)).ClearContents
            Range("dt_start", Range("dt_start").Offset(1, 1).End(xlDown)).ClearContents
            Range("dt_start").Value = "Date/Time"
            Range("dt_start").Offset(0, 1).Value = "Hourly rainfall depth (in)"

            Range(Range("dt_start", Range("dt_start").Offset(1, 0).End(xlDown)), Range("dt_start", Range("dt_start").Offset(1, 1).End(xlDown))).Select
            With Selection.Interior
                .Pattern = xlSolid
                .PatternColorIndex = xlAutomatic
                .Color = 6750207
                .TintAndShade = 0
                .PatternTintAndShade = 0
            End With

            Selection.Borders(xlDiagonalDown).LineStyle = xlNone
            Selection.Borders(xlDiagonalUp).LineStyle = xlNone
            With Selection.Borders(xlEdgeLeft)
                .LineStyle = xlContinuous
                .ThemeColor = 1
                .TintAndShade = -0.499984740745262
                .Weight = xlThin
            End With
            With Selection.Borders(xlEdgeTop)
                .LineStyle = xlContinuous
                .ThemeColor = 1
                .TintAndShade = -0.499984740745262
                .Weight = xlThin
            End With
            With Selection.Borders(xlEdgeBottom)
                .LineStyle = xlContinuous
                .ThemeColor = 1
                .TintAndShade = -0.499984740745262
                .Weight = xlThin
            End With
            With Selection.Borders(xlEdgeRight)
                .LineStyle = xlContinuous
                .ThemeColor = 1
                .TintAndShade = -0.499984740745262
                .Weight = xlThin
            End With
            With Selection.Borders(xlInsideVertical)
                .LineStyle = xlContinuous
                .ThemeColor = 1
                .TintAndShade = -0.499984740745262
                .Weight = xlThin
            End With
            With Selection.Borders(xlInsideHorizontal)
                .LineStyle = xlContinuous
                .ThemeColor = 1
                .TintAndShade = -0.499984740745262
                .Weight = xlThin
            End With


            Range("dt_start").Interior.ColorIndex = 43
            Range("dt_start").Offset(0, 1).Interior.ColorIndex = 43


            Range("f9", Range("h9").End(xlDown)).Select
            Selection.Clear
            Selection.Interior.Color = RGB(217, 217, 217)
            Selection.Interior.Pattern = xlSolid


            With Range("avg_dry_days")
                .Value = 0
                .Font.Bold = False
                .Interior.Color = RGB(217, 217, 217)
                .NumberFormat = "0"
            End With

            With Range("percentile")
                .Value = 0
                .Font.Bold = False
                .Interior.Color = RGB(217, 217, 217)
                .NumberFormat = "0"
            End With


            okClear = MsgBox("The time series are cleared, please paste hourly rainfall.", vbInformation, "Time series cleared")

            Range("dt_start").Offset(1, 0).Select

        End If
    Else
        okClear = MsgBox("No time series found, please make sure that hourly rainfall are pasted at the designated starting point.", vbExclamation, "Time series not found")
        Range("dt_start").Offset(1, 0).Select

    End If
    Range("dt_start").Offset(1, 0).Select

    Call present()

End Sub

Sub Names()
    Dim nName As Name

    For Each nName In ThisWorkbook.Names
        nName.Visible = True
    Next nName

End Sub

Function SheetExists(sheetName As String) As Boolean

    SheetExists = False
    For Each ws In Worksheets
        If sheetName = ws.Name Then
            SheetExists = True
        End If
    Next ws
End Function

Sub Swtich2UserRainfallInput()
    Worksheets("ModelInputUserSuppliedRainfall").Activate
    Call Init()
    Range("dt_start").Offset(1, 0).Select
    Range(Range("dt_start", Range("dt_start").Offset(1, 0).End(xlDown)), Range("dt_start", Range("dt_start").Offset(1, 1).End(xlDown))).Select
    Selection.Locked = False
    Worksheets("ModelInputUserSuppliedRainfall").Protect DrawingObjects:=True, Contents:=True, Scenarios:=True
    Worksheets("ModelInputUserSuppliedRainfall").EnableSelection = xlUnlockedCells
    Worksheets("ModelInputUserSuppliedRainfall").Range("D10").Select


End Sub

Sub Back2ModelInputs()
    Worksheets("ModelInputsGeneral").Activate
    Range("BaseName").Select
End Sub

Sub HeteroAnalysis()

    Range("Heterogeneity").Value = "False"
    Call Init()

    If (Range("HSG_A_Pct_Result").Value >= 0.2 And Range("HSG_A_Pct_Result").Value <= 0.3) And (Range("HSG_B_Pct_Result").Value >= 0.2 And Range("HSG_B_Pct_Result").Value <= 0.3) And (Range("HSG_C_Pct_Result").Value >= 0.2 And Range("HSG_C_Pct_Result").Value <= 0.3) And (Range("HSG_D_Pct_Result").Value >= 0.2 And Range("HSG_D_Pct_Result").Value <= 0.3) Then
        Range("Heterogeneity").Value = "True"
    ElseIf Range("HSG_A_Pct_Result").Value >= 0.7 Then
        If Range("HSG_D_Pct_Result").Value >= 0.2 Then
            Range("Heterogeneity").Value = "True"
        Else
            Range("Heterogeneity").Value = "False"
        End If
    ElseIf Range("HSG_B_Pct_Result").Value >= 0.7 Then
        Range("Heterogeneity").Value = "False"
    ElseIf Range("HSG_C_Pct_Result").Value >= 0.7 Then
        Range("Heterogeneity").Value = "False"
    ElseIf Range("HSG_D_Pct_Result").Value >= 0.7 Then
        If Range("HSG_A_Pct_Result").Value >= 0.2 Then
            Range("Heterogeneity").Value = "True"
        Else
            Range("Heterogeneity").Value = "False"
        End If
    ElseIf Range("HSG_A_Pct_Result").Value >= 0.45 Then
        If Range("HSG_B_Pct_Result").Value >= 0.45 Then
            Range("Heterogeneity").Value = "False"
        ElseIf (Range("HSG_B_Pct_Result").Value >= 0.2 And Range("HSG_D_Pct_Result").Value >= 0.2) And Range("HSG_B_Pct_Result").Value + Range("HSG_C_Pct_Result").Value >= 0.45 Then
            Range("Heterogeneity").Value = "False"
        Else
            Range("Heterogeneity").Value = "True"
        End If
    ElseIf Range("HSG_B_Pct_Result").Value >= 0.45 Then
        If (Range("HSG_A_Pct_Result").Value >= 0.2 And Range("HSG_D_Pct_Result").Value >= 0.2) And Range("HSG_A_Pct_Result").Value + Range("HSG_D_Pct_Result").Value >= 0.45 Then
            Range("Heterogeneity").Value = "True"
        Else
            Range("Heterogeneity").Value = "False"
        End If
    ElseIf Range("HSG_C_Pct_Result").Value >= 0.45 Then
        If (Range("HSG_A_Pct_Result").Value >= 0.2 And Range("HSG_D_Pct_Result").Value >= 0.2) And Range("HSG_A_Pct_Result").Value + Range("HSG_D_Pct_Result").Value >= 0.45 Then
            Range("Heterogeneity").Value = "True"
        Else
            Range("Heterogeneity").Value = "False"
        End If
    ElseIf Range("HSG_D_Pct_Result").Value >= 0.45 Then
        If (Range("HSG_B_Pct_Result").Value >= 0.2 And Range("HSG_C_Pct_Result").Value >= 0.2) And Range("HSG_B_Pct_Result").Value + Range("HSG_C_Pct_Result").Value >= 0.45 Then
            Range("Heterogeneity").Value = "False"
        Else
            Range("Heterogeneity").Value = "True"
        End If
    End If
    Call present()
End Sub






Sub checkRefAssump()

    Worksheets("ReferencesAndAssumptions").Activate

End Sub

Sub Swtich2Flowchart()
    Worksheets("DecisionFlowChart").Activate
End Sub

Sub Switch2Introduction()
    Worksheets("Introduction").Activate
End Sub

Sub WatershedGeneral()


    Dim x As Integer
    Dim indxRange As Range
    Dim NumWatersheds As String
    
    ' Initilze starting range.
    Set indxRange = Worksheets("ModelInputModify").Range("D11")
    
    ' Validate number of subwatersheds.
    'NumWatersheds = Range("WatershedNum").Value
    NumWatersheds = "1"
    If Not ValidateFormInput(NumWatersheds) Then
        MsgBox("Please check entry for number of watersheds!")
        Exit Sub
    End If

    ' Turn screen updating off.
    'Application.ScreenUpdating = False

    ' Initialize project parameters.
    Call ClearWatersheds(indxRange)
    For x = 1 To 1

        ' Write default watershed name.
        indxRange.Value = "Watershed " & x



        ' Write initial surface properties.
        'indxRange.Offset(0, 1).Value = Range("TotaliArea").Value * 43560 / Range("WatershedNum").Value
        indxRange.Offset(0, 1).Value = Range("TotaliArea").Value * 43560 / 1

        indxRange.Offset(0, 2).Value = Range("ImperviousDepth").Value
        indxRange.Offset(0, 3).Value = Range("PerviousDepth").Value


        ' Write initial infiltration properties.
        indxRange.Offset(0, 4).Value = HortonMaxRate
        indxRange.Offset(0, 5).Value = HortonMinRate
        indxRange.Offset(0, 6).Value = HortonDecay

        ' Write initial BMP properties.
        indxRange.Offset(0, 7).Value = Range("BMPDepth").Value
        indxRange.Offset(0, 8).Value = (1.02 * Range("HSG_A_Area").Value + 0.52 * Range("HSG_B_Area").Value + 0.27 * Range("HSG_C_Area").Value +
                                       0.05 * Range("HSG_D_Area").Value) / (Range("HSG_A_Area").Value + Range("HSG_B_Area").Value +
                                       Range("HSG_C_Area").Value + Range("HSG_D_Area").Value)


        indxRange.Offset(0, 9).Value = 12.4
        indxRange.Offset(0, 10).Value = 0.76


        ' Format current wtaershed.
        Call FormatOneLine(indxRange)
        
        ' Advance to next cell.
        Set indxRange = indxRange.Offset(1, 0)
            
    Next x

    Worksheets("ModelInputUserSuppliedRainfall").Unprotect

    ' Save the user-specified number of dry days for infiltration recovery..
    Worksheets("ModelInputUserSuppliedRainfall").Range("T7").Value = HDryDays

    ' Turn screen updating on.
    'Application.ScreenUpdating = True

    ' Close user form.
    ' Unload Me

End Sub



Function ValidateFormInput(theVal As String) As Boolean

    '
    ' Validate that value is numeric, positive and non-zero.
    '

    ValidateFormInput = True
    If IsNumeric(theVal) Then
        If theVal <= 0 Then
            ValidateFormInput = False
        End If
    Else
        ValidateFormInput = False
    End If

End Function

Sub FormatOneLine(indxRange As Range)

    Dim x As Integer
    Dim headerRange As Range
    
    ' Format header row.
    Set headerRange = Range(Worksheets("ModelInputModify").Range("D8"), Worksheets("ModelInputModify").Range("P10"))
    headerRange.Borders(xlEdgeBottom).LineStyle = xlContinuous
    headerRange.Borders.Weight = xlThin

    ' Format watershed name.
    indxRange.HorizontalAlignment = xlLeft
    indxRange.Borders.LineStyle = XlLineStyle.xlDot
    indxRange.Borders.Weight = xlHairline

    ' Format other parameters.
    For x = 1 To 12

        ' Set number format.
        Select Case x
            Case 1, 11
                indxRange.Offset(0, x).NumberFormat = "##,###,###"

            Case 12
                indxRange.Offset(0, x).NumberFormat = "$ ##,000.00"

            Case Else
                indxRange.Offset(0, x).NumberFormat = "0.0##"

        End Select

        ' Set borders and cell alignment.
        indxRange.Offset(0, x).HorizontalAlignment = xlCenter
        indxRange.Offset(0, x).Borders.LineStyle = XlLineStyle.xlDot
        indxRange.Offset(0, x).Borders.Weight = xlHairline

    Next x

End Sub

Sub ClearWatersheds(indxRange As Range)

    '
    ' Clear all existing project conents.
    '

    Dim tmpRange As Range
    Set tmpRange = indxRange
    
    ' Select all existing watersheds and parameters.
    Set tmpRange = Range(tmpRange, tmpRange.Offset(0, 12))
    Set tmpRange = Range(tmpRange, tmpRange.End(xlDown))
    
    ' Clear all cell contents and change border style to nothing.
    tmpRange.ClearContents
    tmpRange.Borders.LineStyle = XlLineStyle.xlLineStyleNone

End Sub

'call Continuous Simulation

Sub RunSimulation()

    ' Check for duplicate watershed names.
    If Not ValidateWatershedNames() Then
        MsgBox "All watershed names my be unique! Please check for duplicate names.",
            vbExclamation, "WATERSHED NAME ERROR"
        Exit Sub
    End If

    ' Switch to note the simulation method.
    ' TRUE = Optimize using Solver.
    ' FALSE = Run w/ current values.
    Dim simMethod As Boolean
    'If ActiveSheet.OptionButtons("optAutomatic").Value Then
    simMethod = True
    '   With AddIns("Solver Add-In").Installed = True
    '  Call enableSolverAddIn
    ' Else
    '  If ValidateSurfaceArea Then
    '     simMethod = False
    ' Else
    '     MsgBox "Invalid surface area! Please enter valid surface areas for all BMPs.", _
    '         vbExclamation, "MANUAL MODE ERROR"
    '     Exit Sub
    ' End If
    ' End If

    ' Start runoff simulation.
    Call Simulation.AMain(simMethod)
    ' Call ThisWorkbook.ClearWatershedList


    Timeseries.ClearAllOutput





End Sub

Function AddHeterogeneity(ByVal ASoilArea As Double,
                          ByVal BSoilArea As Double,
                          ByVal CSoilArea As Double,
                          ByVal DSoilArea As Double)
    Dim i, j As Integer
    Dim Avg_Infiltration_Rate As Double
    Dim Weighted_Variance As Double
    Dim Total_Area As Double
    Dim BMP_Difference As Double   'BMP Sizing Difference between the Optimized and non-Optimized Sizing Strategies (%)
    'Infiltration for calculating BMP size
    'Soil A: 1.02 in/hr
    'Soil B: 0.52 in/hr
    'Soil C: 0.27 in/hr
    'Soil D: 0.05 in/hr

    Total_Area = ASoilArea + BSoilArea + CSoilArea + DSoilArea
    Avg_Infiltration_Rate = 1.02 * ASoilArea +
                            0.52 * BSoilArea +
                            0.27 * CSoilArea +
                            0.05 * DSoilArea

    Weighted_Variance = (1.02 - Avg_Infiltration_Rate) ^ 2 * ASoilArea / Total_Area +
                        (0.52 - Avg_Infiltration_Rate) ^ 2 * BSoilArea / Total_Area +
                        (0.27 - Avg_Infiltration_Rate) ^ 2 * CSoilArea / Total_Area +
                        (0.05 - Avg_Infiltration_Rate) ^ 2 * DSoilArea / Total_Area


    BMP_Difference = -1.2886 * Weighted_Variance + 0.0055 'Heterogeneity Curves

    AddHeterogeneity = BMP_Difference



End Function



Function ValidateWatershedNames() As Boolean
    Dim tmpWatersheds As New Collection
    Dim i As Integer

    ' Logic to check for duplicate watershed names.
    i = 0
    On Error Resume Next
    Do While Worksheets("ModelInputModify").Range("D11").Offset(i, 0).Value <> vbNullString
        tmpWatersheds.Add Worksheets("ModelInputModify").Range("D11").Offset(i, 0).Value, CStr(Worksheets("ModelInputModify").Range("D11").Offset(i, 0).Value)
        i = i + 1
    Loop
    On Error GoTo 0

    If tmpWatersheds.Count <> i Then
        ValidateWatershedNames = False
    Else
        ValidateWatershedNames = True
    End If

End Function


' Copy data

Sub Copydata()
    Dim SWS As String

    ' Check if a valid watershed is selected.

    SWS = Trim("Watershed 1")
    Call Timeseries.CopyTimeseries(ThisWorkbook.GetTimeseriesPath, SWS)
    Call Reporting.ClearSummary
    Call Reporting.CreateSummary(SWS)
    'ActiveSheet.OptionButtons("optVolume").Value = 1









End Sub



' Plot for Continuous Simulation

Sub CreateFlowDurationCurve()


    Dim myObj As Chart
    Dim iArea As Double
    Dim indxRange As Range

    ' Turn off screen updating.
    Application.DisplayAlerts = False
    Application.ScreenUpdating = False

    ' Get the impervous area.


    iArea = Worksheets("ModelInputsGeneral").Range("TotaliArea") * 43560

    Call Copydata() ' Copy data Yi Xu
    ' Create flow duration curve.
    Call InitializeFDC()
    Call CopyRunoffTimeseries(Worksheets("WatershedSummary").Range("I11"), Worksheets("FDC").Range("C2"))
    Call CopyRunoffTimeseries(Worksheets("WatershedSummary").Range("J11"), Worksheets("FDC").Range("D2"))
    Call CopyRunoffTimeseries(Worksheets("WatershedSummary").Range("M11"), Worksheets("FDC").Range("E2"))
    Call RankRunoffTimeseries(Worksheets("FDC").Range("C2"), iArea)

    ' Select chart object.
    Charts("Chart1").Unprotect
    Charts("Chart1").Activate
    Set myObj = Charts("Chart1")
    
    ' Post-Development.
    Set indxRange = Worksheets("FDC").Range("B2")
    With myObj.SeriesCollection(1)
        .Values = Range(indxRange, indxRange.End(xlDown)).Offset(0, 1)
        .XValues = Range(indxRange, indxRange.End(xlDown))
    End With

    ' Pre-Development.
    With myObj.SeriesCollection(2)
        .Values = Range(indxRange, indxRange.End(xlDown)).Offset(0, 2)
        .XValues = Range(indxRange, indxRange.End(xlDown))
    End With

    ' Managed with BMP.
    With myObj.SeriesCollection(3)
        .Values = Range(indxRange, indxRange.End(xlDown)).Offset(0, 3)
        .XValues = Range(indxRange, indxRange.End(xlDown))
    End With

    ' Turn on screen updating.
    Charts("Chart1").Protect
    Application.ScreenUpdating = True
    Application.DisplayAlerts = True

End Sub

Sub InitializeFDC()

    Dim indxRange As Range
    
    ' Clear old dataset.
    Set indxRange = Worksheets("FDC").Range("A1")
    Set indxRange = Range(indxRange, indxRange.End(xlDown))
    Set indxRange = Range(indxRange, indxRange.End(xlToRight))
    indxRange.Clear
    
    ' Write sheet header.
    Set indxRange = Worksheets("FDC").Range("A1")
    indxRange.Offset(0, 0).Value = "RANK"
    indxRange.Offset(0, 1).Value = "PCT"
    indxRange.Offset(0, 2).Value = "POST"
    indxRange.Offset(0, 3).Value = "PRE"
    indxRange.Offset(0, 4).Value = "BMP"

End Sub

Sub CopyRunoffTimeseries(ByVal indxRange As Range, ByVal wRange As Range)

    Dim i As Integer
    Dim tmpRange As Range
    
    ' Copy and paste the impervious runoff timeseries.
    Set tmpRange = Range(indxRange, indxRange.End(xlDown))
    tmpRange.Copy Destination:=wRange
    Set tmpRange = Range(wRange, wRange.End(xlDown))
    
    ' Set worksheet object sorting parameters.
    wRange.Worksheet.Sort.SortFields.Clear
    wRange.Worksheet.Sort.SortFields.Add Key:=tmpRange, SortOn:=xlSortOnValues,
        Order:=xlDescending, DataOption:=xlSortNormal

    ' Sort the dataset.
    With ActiveWorkbook.Worksheets("FDC").Sort
        .SetRange tmpRange
        .Header = xlNo
        .MatchCase = False
        .Orientation = xlTopToBottom
        .SortMethod = xlPinYin
        .Apply
    End With

End Sub

Sub RankRunoffTimeseries(ByVal indxRange As Range, iArea As Double)

    Dim i As Integer
    Dim tmpRange As Range

    ' Rank non-zero impervious flows.
    i = 0
    While indxRange.Offset(i, 0).Value > 0
        indxRange.Offset(i, 0).Value = indxRange.Offset(i, 0).Value * iArea / 12
        indxRange.Offset(i, 1).Value = indxRange.Offset(i, 1).Value * iArea / 12
        indxRange.Offset(i, 2).Value = indxRange.Offset(i, 2).Value * iArea / 12
        indxRange.Offset(i, -2).Value = i + 1
        i = i + 1
    Wend
    
    ' Remove all flows with value of zero
    ' from the post-development timeseries.
    Set tmpRange = indxRange.Offset(i, 0)
    Set tmpRange = Range(tmpRange, tmpRange.End(xlDown))
    Set tmpRange = Range(tmpRange, tmpRange.End(xlToRight))
    tmpRange.Clear

    ' Calculate percentiles.
    While indxRange.Value <> ""
        indxRange.Offset(0, -1).Value = indxRange.Offset(0, -2).Value / i
        Set indxRange = indxRange.Offset(1, 0)
    Wend

End Sub

'Yi Xu 11/19/2013
Function DesignStormBMP(ByVal DevelopArea As Double, Rainfall As Double)
    'Assume effective depth of BMP is 1.75 ft
    'DevelopArea: acre
    'Rainfall: inch
    'DesignStormBMP size: ft2


    DesignStormBMP = DevelopArea * 43560 * Rainfall / 12 / 1.75


End Function
