Public Class CanvasForm
    'Generation
    Dim OctaveMin, OctaveMax, WaterThr, SandThr, GrassThr, ForestThr, MountainThr, SnowThr As Single
    'rendering variables
    Dim MasterMult As Integer = 4
    Dim Canvas As New Bitmap(1024 * MasterMult, 1024 * MasterMult)
    Dim VisibleCanvas As New Bitmap(1024, 1024)
    Dim VisibleGFX As Graphics = Graphics.FromImage(VisibleCanvas)
    Dim Lens As Rectangle
    Dim ZoomDegree As Single = 1
    Dim CXOff, CYOff As Integer
    Dim ListMapObjects As New List(Of MapObject)

    '//LOADING AND RENDERING//LOADING AND RENDERING//LOADING AND RENDERING//LOADING AND RENDERING//LOADING AND RENDERING//LOADING AND RENDERING//LOADING AND RENDERING//LOADING AND RENDERING
    '//LOADING AND RENDERING//LOADING AND RENDERING//LOADING AND RENDERING//LOADING AND RENDERING//LOADING AND RENDERING//LOADING AND RENDERING//LOADING AND RENDERING//LOADING AND RENDERING

    Private Sub CanvasForm_Load(sender As Object, e As EventArgs) Handles Me.Load
        ReloadInterface()
    End Sub

    Private Sub btnGenerate_Click(sender As Object, e As EventArgs) Handles btnGenerate.Click
        'Generate a new map, at the specified resolution. Not changeable by the user for various reasons.
        PerlinOctaves(MasterMult)
        'Sets the map to be zoomed out all the way
        ZoomDegree = 1
        'restarting clears out a memory leak issue that happened previously
        Application.Restart()
    End Sub

    'When given a topleft and a length, set visiblecanvas to the square of that length at that point.
    Private Sub Render(X, Y, L)
        Lens = New Rectangle(X, Y, L, L)
        VisibleGFX.DrawImage(Canvas.Clone(Lens, Imaging.PixelFormat.Format24bppRgb), 0, 0, 1024, 1024)

        'here is the space for rendering ALL the game objects
        Dim Topleft As Point
        Dim CurrentImage As Image
        For Each MO As MapObject In ListMapObjects

            If MO.ImageType = "water" Then
                CurrentImage = My.Resources.BlueBlock
            ElseIf MO.ImageType = "sand" Then
                CurrentImage = My.Resources.YellowBlock
            ElseIf MO.ImageType = "grass" Then
                CurrentImage = My.Resources.GreenBlock
            ElseIf MO.ImageType = "forest" Then
                CurrentImage = My.Resources.DarkGreenBlock
            ElseIf MO.ImageType = "mountain" Then
                CurrentImage = My.Resources.CaveIcon
            ElseIf MO.ImageType = "snow" Then
                CurrentImage = My.Resources.SnowIcon
            ElseIf MO.ImageType = "marker" Then
                CurrentImage = My.Resources.Marker
            Else
                CurrentImage = My.Resources.Marker
            End If
            'since 0,0 in the form is the top left corner, working out the co-ordinate the top left corner should be on based on where the centre of the image should be is required.
            Topleft = GetTopLeft(MO.CentrePoint, CurrentImage.Width, CurrentImage.Height)
            'draw the image at the correct location with respect to the zoom level.
            VisibleGFX.DrawImage(CurrentImage, CInt((Topleft.X - Lens.X / MasterMult) * ZoomDegree), CInt((Topleft.Y - Lens.Y / MasterMult) * ZoomDegree), CurrentImage.Width * ZoomDegree / 8, CurrentImage.Height * ZoomDegree / 8)
        Next

        'set the image on the form to the output rendered map
        picCanvas.BackgroundImage = VisibleCanvas

        'Reset to clear canvas. perhaps there's a better way?
        VisibleCanvas = New Bitmap(1024, 1024)
        VisibleGFX = Graphics.FromImage(VisibleCanvas)
    End Sub

    Sub ReloadInterface()
        Dim PathToFile, DirFileName As String

        LstImageFiles.Items.Clear()
        CmbPresets.Items.Clear()

        For i = 1 To FileIO.FileSystem.GetDirectories(".\Worlds\").Count
            'Set FullPath to the string directory of the current world file
            PathToFile = FileIO.FileSystem.GetDirectories(".\Worlds\").Item(i - 1)
            'Set WorldFileName to the name of the file of the current world
            DirFileName = FileIO.FileSystem.GetDirectoryInfo(PathToFile).Name
            LstImageFiles.Items.Add(DirFileName)
        Next i

        LstImageFiles.SelectedItem = 0

        For i = 1 To FileIO.FileSystem.GetFiles(".\Presets\").Count
            'Set FullPath to the string directory of the current world file
            PathToFile = FileIO.FileSystem.GetFiles(".\Presets\").Item(i - 1)
            'Set WorldFileName to the name of the file of the current world
            DirFileName = FileIO.FileSystem.GetFileInfo(PathToFile).Name
            CmbPresets.Items.Add(DirFileName)
        Next i

        CmbPresets.SelectedItem = "Default.txt"

    End Sub

    'KEYINPUT//KEYINPUT//KEYINPUT//KEYINPUT//KEYINPUT//KEYINPUT//KEYINPUT//KEYINPUT//KEYINPUT//KEYINPUT//KEYINPUT//KEYINPUT//KEYINPUT//KEYINPUT//KEYINPUT//KEYINPUT//KEYINPUT//KEYINPUT//KEYINPUT//KEYINPUT
    'KEYINPUT//KEYINPUT//KEYINPUT//KEYINPUT//KEYINPUT//KEYINPUT//KEYINPUT//KEYINPUT//KEYINPUT//KEYINPUT//KEYINPUT//KEYINPUT//KEYINPUT//KEYINPUT//KEYINPUT//KEYINPUT//KEYINPUT//KEYINPUT//KEYINPUT//KEYINPUT

    'These Zooms are reverse engineered from google maps. When zooming in and out, if the resulting image does NOT border the edge of the total image, 
    'the position of the cursor in the section of the image is preserved. This prevents jarring transitions and helps you identify where you are more easily.
    Private Sub Scrolling(ByVal sender As Object, ByVal e As MouseEventArgs) Handles picCanvas.MouseWheel
        If e.Delta > 0 Then
            If ZoomDegree < MasterMult Then
                'for zooming in. Please have a topleft and a length output to render.

                CXOff += (e.X / 2) * MasterMult / ZoomDegree
                CYOff += (e.Y / 2) * MasterMult / ZoomDegree
                ZoomDegree *= 2
                Render(CXOff, CYOff, Canvas.Width / ZoomDegree)

            End If
        Else
            If ZoomDegree > 1 Then
                'for zooming out. Please have a topleft and a length output to render.
                CXOff -= e.X / ZoomDegree * 4
                CYOff -= e.Y / ZoomDegree * 4
                'Apparently it's NOT NOT NOT just this simple! divide zoomdegree by 4.

                ZoomDegree /= 2

                If CXOff > Canvas.Width - Canvas.Width / ZoomDegree Then
                    CXOff = Canvas.Width - Canvas.Width / ZoomDegree
                ElseIf CXOff < 0 Then
                    CXOff = 0
                End If
                If CYOff > Canvas.Width - Canvas.Width / ZoomDegree Then
                    CYOff = Canvas.Width - Canvas.Width / ZoomDegree
                ElseIf CYOff < 0 Then
                    CYOff = 0
                End If

                Render(CInt(CXOff / 2) * 2, CInt(CYOff / 2) * 2, Canvas.Width / ZoomDegree)
                'a strange bug induced additional blur if the passed co-ordinates were not even. zooming out is now a little wonky
            End If
        End If
    End Sub

    'panning. When zoomed in, WASD can be used to shift the view up down left and right.
    Private Sub Canvas_KeyDown(sender As Object, e As KeyEventArgs) Handles MyBase.KeyDown
        Select Case Chr(e.KeyCode)
            Case "W"
                'move up 
                CYOff -= 400 / ZoomDegree
                'stop it going off screen
                If CYOff < 0 Then CYOff = 0
                'render at new position
                Render(CXOff, CYOff, Canvas.Width / ZoomDegree)
            Case "A"
                'move left
                CXOff -= 400 / ZoomDegree
                'stop it going off screen
                If CXOff < 0 Then CXOff = 0
                'render at new position
                Render(CXOff, CYOff, Canvas.Width / ZoomDegree)
            Case "S"
                'move down
                CYOff += 400 / ZoomDegree
                'stop it going off screen (more complex due to it not being at the co-ordinate "0"
                If CYOff > Canvas.Width - Canvas.Width / ZoomDegree Then CYOff = Canvas.Width - Canvas.Width / ZoomDegree
                'render at new position
                Render(CXOff, CYOff, Canvas.Width / ZoomDegree)
            Case "D"
                'move right
                CXOff += 400 / ZoomDegree
                'stop it going off screen (more complex due to it not being at the co-ordinate "0"
                If CXOff > Canvas.Width - Canvas.Width / ZoomDegree Then CXOff = Canvas.Width - Canvas.Width / ZoomDegree
                'render at new position
                Render(CXOff, CYOff, Canvas.Width / ZoomDegree)
        End Select
    End Sub

    'When an item in the list is clicked, get the map data of that file and display it in the program.
    Private Sub LstImageFiles_MouseDoubleClick(sender As Object, e As MouseEventArgs) Handles LstImageFiles.MouseDoubleClick
        ZoomDegree = 1
        Canvas = Image.FromFile(String.Format(".\worlds\{0}\{0}-image.png", LstImageFiles.SelectedItem))
        ListMapObjects = ExtractFeaturesToList(String.Format(".\worlds\{0}\{0}-features.ftr", LstImageFiles.SelectedItem))
        Render(0, 0, Canvas.Width)
    End Sub

    'OTHER//OTHER//OTHER//OTHER//OTHER//OTHER//OTHER//OTHER//OTHER//OTHER//OTHER//OTHER//OTHER//OTHER//OTHER//OTHER//OTHER//OTHER//OTHER//OTHER//OTHER//OTHER//OTHER//OTHER//OTHER//OTHER//OTHER//OTHER
    'OTHER//OTHER//OTHER//OTHER//OTHER//OTHER//OTHER//OTHER//OTHER//OTHER//OTHER//OTHER//OTHER//OTHER//OTHER//OTHER//OTHER//OTHER//OTHER//OTHER//OTHER//OTHER//OTHER//OTHER//OTHER//OTHER//OTHER//OTHER

    'Create a new preset of parameters for map creation
    Private Sub BtnNewPreset_Click(sender As Object, e As EventArgs) Handles BtnNewPreset.Click
        PopulatePresetCreator()
    End Sub

    'Close the form, but ask the user if they're sure and react appropriately
    Private Sub BtnClose_Click(sender As Object, e As EventArgs) Handles BtnClose.Click
        If MsgBox("Close the program?", MsgBoxStyle.OkCancel, "") = 1 Then
            Close()
        End If
    End Sub

    'Custom minimise button
    Private Sub BtnMinimise_Click(sender As Object, e As EventArgs) Handles BtnMinimise.Click
        WindowState = FormWindowState.Minimized
    End Sub

End Class
