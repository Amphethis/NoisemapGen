Public Class CanvasForm
    'Generation
    Dim OctaveMin, OctaveMax, WaterThr, SandThr, GrassThr, ForestThr, MountainThr, SnowThr As Single
    'rendering
    Dim MasterMult As Integer = 4
    Dim Canvas As New Bitmap(1024 * MasterMult, 1024 * MasterMult)
    Dim VisibleCanvas As New Bitmap(1024, 1024)
    Dim VisibleGFX As Graphics = Graphics.FromImage(VisibleCanvas)
    Dim Lens As Rectangle
    Dim ZoomDegree As Single = 1
    Dim CXOff, CYOff As Integer
    'Game things
    Dim ListMapObjects As New List(Of MapObject)
    'Dim player As MapObject

    '//LOADING AND RENDERING//LOADING AND RENDERING//LOADING AND RENDERING//LOADING AND RENDERING//LOADING AND RENDERING//LOADING AND RENDERING//LOADING AND RENDERING//LOADING AND RENDERING
    '//LOADING AND RENDERING//LOADING AND RENDERING//LOADING AND RENDERING//LOADING AND RENDERING//LOADING AND RENDERING//LOADING AND RENDERING//LOADING AND RENDERING//LOADING AND RENDERING

    Private Sub CanvasForm_Load(sender As Object, e As EventArgs) Handles Me.Load
        ScanImages()
    End Sub

    Private Sub btnGenerate_Click(sender As Object, e As EventArgs) Handles btnGenerate.Click
        PerlinOctaves(MasterMult)
        ZoomDegree = 1
        ScanImages()
    End Sub

    'When given a topleft and a length, set visiblecanvas to the square of that length at that point.
    Private Sub Render(X, Y, L)
        Lens = New Rectangle(X, Y, L, L)
        VisibleGFX.DrawImage(Canvas.Clone(Lens, Imaging.PixelFormat.Format24bppRgb), 0, 0, 1024, 1024)

        'here is the space for rendering ALL the objects
        Dim Topleft As Point
        Dim CurrentImage As Image
        For Each MO As MapObject In ListMapObjects
            Select Case MO.ImageType
                Case "water"
                    CurrentImage = My.Resources.BlueBlock
                Case "sand"
                    CurrentImage = My.Resources.YellowBlock
                Case "grass"
                    CurrentImage = My.Resources.GreenBlock
                Case "forest"
                    CurrentImage = My.Resources.DarkGreenBlock
                Case "mountain"
                    CurrentImage = My.Resources.CaveIcon
                Case "snow"
                    CurrentImage = My.Resources.SnowIcon
                Case "marker"
                    CurrentImage = My.Resources.Marker
                Case "orb"
                    CurrentImage = My.Resources.Orb
                Case Else
                    CurrentImage = My.Resources.Marker
            End Select
            Topleft = GetTopLeft(MO.CentrePoint, CurrentImage.Width, CurrentImage.Height)
            VisibleGFX.DrawImage(CurrentImage, CInt((Topleft.X - Lens.X / MasterMult) * ZoomDegree), CInt((Topleft.Y - Lens.Y / MasterMult) * ZoomDegree), CurrentImage.Width * ZoomDegree / 8, CurrentImage.Height * ZoomDegree / 8)
        Next
        'adds red marker to map
        'CurrentImage = My.Resources.Marker
        'Topleft = GetTopLeft(player.CentrePoint, CurrentImage.Width, CurrentImage.Height)
        'VisibleGFX.DrawImage(CurrentImage, CInt((Topleft.X - Lens.X / MasterMult) * ZoomDegree), CInt((Topleft.Y - Lens.Y / MasterMult) * ZoomDegree), CurrentImage.Width * ZoomDegree / 8, CurrentImage.Height * ZoomDegree / 8)

        picCanvas.BackgroundImage = VisibleCanvas

        'Reset to clear canvas. perhaps there's a better way?
        VisibleCanvas = New Bitmap(1024, 1024)
        VisibleGFX = Graphics.FromImage(VisibleCanvas)
    End Sub

    Sub ScanImages()
        Dim PathToWorldfile, WorldFileName As String

        LstImageFiles.Items.Clear()
        For i = 1 To FileIO.FileSystem.GetDirectories(".\worlds\").Count
            'Set FullPath to the string directory of the current world file
            PathToWorldfile = FileIO.FileSystem.GetDirectories(".\worlds\").Item(i - 1)
            'Set WorldFileName to the name of the file of the current world
            WorldFileName = FileIO.FileSystem.GetDirectoryInfo(PathToWorldfile).Name
            LstImageFiles.Items.Add(WorldFileName)
        Next i
        LstImageFiles.SelectedItem = LstImageFiles.Items(0)
    End Sub

    'KEYINPUT//KEYINPUT//KEYINPUT//KEYINPUT//KEYINPUT//KEYINPUT//KEYINPUT//KEYINPUT//KEYINPUT//KEYINPUT//KEYINPUT//KEYINPUT//KEYINPUT//KEYINPUT//KEYINPUT//KEYINPUT//KEYINPUT//KEYINPUT//KEYINPUT//KEYINPUT
    'KEYINPUT//KEYINPUT//KEYINPUT//KEYINPUT//KEYINPUT//KEYINPUT//KEYINPUT//KEYINPUT//KEYINPUT//KEYINPUT//KEYINPUT//KEYINPUT//KEYINPUT//KEYINPUT//KEYINPUT//KEYINPUT//KEYINPUT//KEYINPUT//KEYINPUT//KEYINPUT

    'These Zooms are reverse engineered from google maps.
    Private Sub Scrolling(ByVal sender As Object, ByVal e As MouseEventArgs) Handles picCanvas.MouseWheel
        If e.Delta > 0 Then
            If ZoomDegree < MasterMult Then
                'design space for zooming in. Please have a topleft and a length output to render.

                CXOff += (e.X / 2) * MasterMult / ZoomDegree
                CYOff += (e.Y / 2) * MasterMult / ZoomDegree
                ZoomDegree *= 2
                Render(CXOff, CYOff, Canvas.Width / ZoomDegree)

            End If
        Else
            If ZoomDegree > 1 Then
                'design space for zooming out. Please have a topleft and a length output to render.
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

    'panning.
    Private Sub Canvas_KeyDown(sender As Object, e As KeyEventArgs) Handles MyBase.KeyDown
        Select Case Chr(e.KeyCode)
            Case "W"
                CYOff -= 400 / ZoomDegree
                If CYOff < 0 Then CYOff = 0
                Render(CXOff, CYOff, Canvas.Width / ZoomDegree)
            Case "A"
                CXOff -= 400 / ZoomDegree
                If CXOff < 0 Then CXOff = 0
                Render(CXOff, CYOff, Canvas.Width / ZoomDegree)
            Case "S"
                CYOff += 400 / ZoomDegree
                If CYOff > Canvas.Width - Canvas.Width / ZoomDegree Then CYOff = Canvas.Width - Canvas.Width / ZoomDegree
                Render(CXOff, CYOff, Canvas.Width / ZoomDegree)
            Case "D"
                CXOff += 400 / ZoomDegree
                If CXOff > Canvas.Width - Canvas.Width / ZoomDegree Then CXOff = Canvas.Width - Canvas.Width / ZoomDegree
                Render(CXOff, CYOff, Canvas.Width / ZoomDegree)
        End Select
    End Sub

    'world select.
    Private Sub LstImageFiles_MouseDoubleClick(sender As Object, e As MouseEventArgs) Handles LstImageFiles.MouseDoubleClick
        Timer.Stop()
        Timer.Start()
        'player.CentrePoint = New Point(512, 512)
        ZoomDegree = 1
        Canvas = Image.FromFile(String.Format(".\worlds\{0}\{0}-image.png", LstImageFiles.SelectedItem))
        ListMapObjects = ExtractFeaturesToList(String.Format(".\worlds\{0}\{0}-features.ftr", LstImageFiles.SelectedItem))
        Render(0, 0, Canvas.Width)
    End Sub

End Class
'FUNCTIONALITY//FUNCTIONALITY//FUNCTIONALITY//FUNCTIONALITY//FUNCTIONALITY//FUNCTIONALITY//FUNCTIONALITY//FUNCTIONALITY//FUNCTIONALITY//FUNCTIONALITY//FUNCTIONALITY//FUNCTIONALITY//FUNCTIONALITY
'FUNCTIONALITY//FUNCTIONALITY//FUNCTIONALITY//FUNCTIONALITY//FUNCTIONALITY//FUNCTIONALITY//FUNCTIONALITY//FUNCTIONALITY//FUNCTIONALITY//FUNCTIONALITY//FUNCTIONALITY//FUNCTIONALITY//FUNCTIONALITY

'    Dim Target As Point
'    Private Sub Timer_Tick(sender As Object, e As EventArgs) Handles Timer.Tick
'        If Target = Nothing Then Target = ListMapObjects.Item(0).CentrePoint
'        Select Case player.CentrePoint.X
'            Case > Target.X
'                player.CentrePoint.X -= 5
'                If player.CentrePoint.X < Target.X Then player.CentrePoint.X = Target.X
'            Case < Target.X
'                player.CentrePoint.X += 5
'                If player.CentrePoint.X > Target.X Then player.CentrePoint.X = Target.X

'        End Select
'        Select Case player.CentrePoint.Y
'            Case > Target.Y
'                player.CentrePoint.Y -= 5
'                If player.CentrePoint.Y < Target.Y Then player.CentrePoint.Y = Target.Y
'            Case < Target.Y
'                player.CentrePoint.Y += 5
'                If player.CentrePoint.Y > Target.Y Then player.CentrePoint.Y = Target.Y
'        End Select
'        Render(CInt(CXOff / 2) * 2, CInt(CYOff / 2) * 2, Canvas.Width / ZoomDegree)
'        'MsgBox(player.CentrePoint.X & " " & player.CentrePoint.Y)
'    End Sub

'    Private Sub picCanvas_MouseClick(sender As Object, e As MouseEventArgs) Handles picCanvas.MouseClick
'        Target = New Point((e.X - Lens.X) * ZoomDegree, (e.Y - Lens.Y) * ZoomDegree)
'    End Sub



'test with co-ordinates for a box around the feature for collision