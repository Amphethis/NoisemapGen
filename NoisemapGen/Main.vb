Module Main

    'The variables for a feature on the map
    Structure MapObject
        Dim CentrePoint As Point
        Dim ImageType As String
    End Structure


    'For getting the co-ordinate of the top left corner of a specifically sized area
    Function GetTopLeft(Point As Point, width As Integer, height As Integer)
        Dim Output As New Point(Point.X - (width / 16), Point.Y - (height / 16))
        Return Output
    End Function


    'Takes a list of map objects and a file path and saves a file to disk at the path containing the data
    Sub CompileFeaturesToFile(FeatureList As List(Of MapObject), FilePath As String)
        FileOpen(1, FilePath, OpenMode.Output)
        For Each MO As MapObject In FeatureList
            'print the x co-ordinate to file
            Print(1, MO.CentrePoint.X)
            'print a comma to indicate seperation
            Print(1, ",")
            'print the y co-ordinate to file
            Print(1, MO.CentrePoint.Y)
            Print(1, ",")
            'print the user defined image type property
            Print(1, MO.ImageType)
            Print(1, ",")
        Next
        FileClose(1)
    End Sub



    'Takes a file saved by the above subroutine and returns a list containing the data
    Function ExtractFeaturesToList(FilePath As String)
        FileOpen(1, FilePath, OpenMode.Input)
        Dim Output As New List(Of MapObject)
        Dim MO As MapObject
        Do Until EOF(1)
            Input(1, MO.CentrePoint.X)
            Input(1, MO.CentrePoint.Y)
            Input(1, MO.ImageType)
            Output.Add(MO)
        Loop
        FileClose(1)
        Return Output
    End Function



    'Dynamically creates custom input form in code. WithEvents lets events happen, see below sub.
    Public WithEvents PresetCreator As New Form With {
            .MinimizeBox = False,
            .MaximizeBox = False,
            .MinimumSize = New Size(150, 400),
            .MaximumSize = New Size(150, 400),
            .StartPosition = FormStartPosition.CenterScreen
        }



    'Adds controls to the PresetCreator Form
    Sub PopulatePresetCreator()

        'Declare Objects

        'disabling interaction, setting form size and placing

        Dim TxtOctaveMin, TxtOctaveMax, TxtWaterThr, TxtSandThr, TxtGrassThr, TxtForestThr, TxtMountainThr, TxtSnowThr As New TextBox
        Dim TxtList As New List(Of TextBox) From {TxtOctaveMin, TxtOctaveMax, TxtWaterThr, TxtSandThr, TxtGrassThr, TxtForestThr, TxtMountainThr, TxtSnowThr}

        Dim LblOctaveMin, LblOctaveMax, LblWaterThr, LblSandThr, LblGrassThr, LblForestThr, LblMountainThr, LblSnowThr As New Label
        Dim LblList As New List(Of Label) From {LblOctaveMin, LblOctaveMax, LblWaterThr, LblSandThr, LblGrassThr, LblForestThr, LblMountainThr, LblSnowThr}

        Dim Namelist As New List(Of String) From {"Zoom", "Detail", "Water Threshold", "Sand Threshold", "Grass Threshold", "Forest Threshold", "Mountain Threshold", "Snow Threshold"}
        Dim ValueList As New List(Of Single) From {2, 8, 0.51, 0.53, 0.63, 0.7, 0.81, 1}

        Dim Loc As New Point(6, 9)

        Dim LblHelp As New Label With {
            .Size = New Size(125, 200),
        .Text = "The Upper and lower bounds define the detail of the map. The threshold levels are what height the colour should change at, from 0 to 1. They should be in ascending order. Snow should always be 1. Close the window to Save and exit.",
        .Left = 6
        }

        'Wipe
        PresetCreator.Controls.Clear()

        'position labels
        For Each Lbl As Label In LblList
            Lbl.Text = Lbl.Name
            Lbl.Width = 75
            Lbl.Location = Loc
            Loc.Y += 26
            Lbl.Text = Namelist(LblList.IndexOf(Lbl))       'text field is set to the text in the same position in the text array that the lbl sits in in the lbl array


            PresetCreator.Controls.Add(Lbl)
        Next Lbl

        'shift loc
        Loc.X = 90
        Loc.Y = 6

        'position text boxes
        For Each Txt As TextBox In TxtList
            Txt.Width = 30
            Txt.Location = Loc
            Txt.Text = 0
            Loc.Y += 26
            Txt.Text = ValueList(TxtList.IndexOf(Txt))      'above equivalent

            PresetCreator.Controls.Add(Txt)
        Next Txt

        'Place help textbox
        LblHelp.Top = Loc.Y
        PresetCreator.Controls.Add(LblHelp)

        TxtSnowThr.Enabled = False

        PresetCreator.ShowDialog()
        CanvasForm.ReloadInterface()
        'form opens
        'input data
        'form closes, passes all fields to logic
        'if data scans, give option to save with name, else reject and error message
    End Sub


    'Handles closing event for the form, saving the data.
    Private Sub Closing(sender As Object, e As FormClosingEventArgs) Handles PresetCreator.FormClosing
        Dim PresetName As String
        Dim C As Integer = 0 'counter
        Dim Input(7) As Single


        'For each control in the form, if it's a textbox, add its contents to the array and increment the counter.
        For Each Ctrl As Control In PresetCreator.Controls
            If TypeName(Ctrl) = "TextBox" Then
                Input(C) = Ctrl.Text
                C += 1
            End If
        Next Ctrl

        'Data Validation
        'If it is not the case that the numbers ascend in this exact order, exit subroutine and cancel closing.
        If (Input(2) > Input(3) Or Input(3) > Input(4) Or Input(4) > Input(5) Or Input(5) > Input(6) Or Input(6) > Input(7) Or Input(7) > Input(0) Or Input(0) > Input(1) - 1) Then
            MsgBox("Invalid Data input.")
            e.Cancel = True

            Exit Sub
        End If

        'If it is not the case that the lowest threshold value is between 1 and 0, exit subroutine and cancel closing.
        If 0 > Input(2) Or Input(2) > 1 Then
            MsgBox("Invalid Data input.")
            e.Cancel = True
            Exit Sub
        End If

        'If it is not the case that the detail value is between 1 and 15, exit subroutine and cancel closing.
        If 1 > Input(1) Or Input(1) > 12 Then
            MsgBox("Invalid Data input.")
            e.Cancel = True
            Exit Sub
        End If

        'loop until you get a correct output name
        Do
            'get the name input
            PresetName = InputBox("Enter a name for the new preset.", "Name new preset.", DefaultResponse:="New Preset")
            If PresetName = "" Then Exit Sub
            PresetName += ".txt"

            'if the name matches 
            For i = 1 To FileIO.FileSystem.GetFiles(".\Presets\").Count
                If LCase(PresetName) = LCase(FileIO.FileSystem.GetFileInfo(FileIO.FileSystem.GetFiles(".\Presets\").Item(i - 1)).Name) Then
                    'if entered preset name is equal to the name of the currently selected file in presets, exit sub
                    PresetName = ""
                    MsgBox("A file with that name already exists.")
                End If
            Next i
        Loop Until Not PresetName = ""

        FileOpen(1, String.Format("./Presets/{0}", PresetName), OpenMode.Output)

        'Store data
        For i = 0 To 7
            Print(1, Input(i))
            Print(1, ",")
        Next i

        FileClose(1)

    End Sub

    'Function MergeSort(List) As List(Of MapObject)

    '    Dim FirstHalf, LastHalf As New List(Of MapObject)
    '    Dim Midpoint As Integer = Int(MergeSort.Count / 2)
    '    If MergeSort.Count > 1 Then

    '        For i = 1 To Midpoint
    '            FirstHalf.Item(i) = List.item(i)
    '        Next

    '        For i = Midpoint To MergeSort.Count
    '            LastHalf.Item(i) = List.item(i)
    '        Next

    '    End If

    'End Function
End Module