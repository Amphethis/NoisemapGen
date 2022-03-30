Module Generation

    'Vector structure
    Private Structure Vector
        Public Xval, Yval As Double
    End Structure

    'Cosine Interpolation
    Private Function Cerp(Val1, Val2, Pos, Endpos)
        Dim Frac As Double = Pos / Endpos
        Dim Frac2 As Double = (1 - Math.Cos(Frac * Math.PI)) / 2
        Return (Val1 * (1 - Frac2) + Val2 * Frac2)
    End Function

    'fade function, smooths value between 0 and 1 to curve
    Private Function Fade(x)
        Return x * x * x * (x * (x * 6 - 15) + 10)
    End Function

    'Main Perlin noise generation

    Sub PerlinOctaves(ZoomMax As Integer)
        'FileRead
        Dim OctaveMin, OctaveMax As Integer
        Dim WaterThr, SandThr, GrassThr, ForestThr, MountainThr, SnowThr As Single
        FileOpen(1, String.Format(".\presets\{0}", CanvasForm.CmbPresets.Text), OpenMode.Input)
        Input(1, OctaveMin)
        Input(1, OctaveMax)
        Input(1, WaterThr)
        Input(1, SandThr)
        Input(1, GrassThr)
        Input(1, ForestThr)
        Input(1, MountainThr)
        Input(1, SnowThr)
        FileClose(1)

        'Graphic variables
        Dim Width As Integer = 1024
        Dim Noisemap As New Bitmap(Width * ZoomMax, Width * ZoomMax)
        Dim GFX As Graphics = Graphics.FromImage(Noisemap)
        Dim B As Brush
        'Octave variables
        Dim MathMap(Width, Width) As Double
        Dim OctaveRoot, OctaveCount, CellCount, CellWidth As Integer
        Dim OctaveFrac As Double
        OctaveRoot = 2
        OctaveCount = Int(OctaveMax)
        CellCount = OctaveRoot ^ OctaveCount
        CellWidth = Width / CellCount
        Dim Vectorlist(CellCount + 1, CellCount + 1) As Vector

        'Interpolation variables
        Dim CurrentPoint As Point
        Dim FunctionValue, DPTL, DPTR, DPBL, DPBR As Double
        Dim TL, TR, BL, BR, DTL, DTR, DBL, DBR As Vector

        Randomize()

        'makes the array of vectors
        For y = 0 To CellCount + 1
            For x = 0 To CellCount + 1
                Vectorlist(x, y).Xval = 2 * CInt(Rnd()) - 1
                Vectorlist(x, y).Yval = 2 * CInt(Rnd()) - 1
            Next
        Next

        For Octave = Int(OctaveMin) To OctaveCount - 1
            CellCount = OctaveRoot ^ Octave
            'the number of cells is the "octaveroot" (the power being used to define each octave, here it's 2) raised to the number of octaves.
            CellWidth = Width / CellCount
            For y = 0 To CellCount - 1
                For x = 0 To CellCount - 1
                    For y2 = 0 To CellWidth - 1
                        For x2 = 0 To CellWidth - 1
                            CurrentPoint.X = (x * CellWidth) + x2
                            CurrentPoint.Y = (y * CellWidth) + y2

                            'Get appropriate vectors from list
                            TL = Vectorlist(x, y)
                            TR = Vectorlist(x + 1, y)
                            BL = Vectorlist(x, y + 1)
                            BR = Vectorlist(x + 1, y + 1)
                            DTL.Xval = (0 - x2) / CellWidth
                            DTL.Yval = (0 - y2) / CellWidth
                            DTR.Xval = (CellWidth - x2) / CellWidth
                            DTR.Yval = (0 - y2) / CellWidth
                            DBL.Xval = (0 - x2) / CellWidth
                            DBL.Yval = (CellWidth - y2) / CellWidth
                            DBR.Xval = (CellWidth - x2) / CellWidth
                            DBR.Yval = (CellWidth - y2) / CellWidth

                            'GetDotProducts
                            'distance to node vectors need to be mapped to -1 to 1. divide by 45. ?????
                            DPTL = TL.Xval * DTL.Xval + TL.Yval * DTL.Yval
                            DPTR = TR.Xval * DTR.Xval + TR.Yval * DTR.Yval
                            DPBL = BL.Xval * DBL.Xval + BL.Yval * DBL.Yval
                            DPBR = BR.Xval * DBR.Xval + BR.Yval * DBR.Yval

                            'the 4 dot products of the vectors and distances interpolated in 2 steps
                            FunctionValue = Fade((Cerp(Cerp(DPTL, DPTR, x2, CellWidth), Cerp(DPBL, DPBR, x2, CellWidth), y2, CellWidth) + 1) / 2)
                            MathMap(CurrentPoint.X, CurrentPoint.Y) += FunctionValue * (1 / 2 ^ Octave)

                            'Fills the grid with that colour
                        Next x2
                    Next y2
                Next x
            Next y
            OctaveFrac += (1 / 2 ^ Octave)
        Next Octave

        'Convert to saving format
        Dim CurrentPixel As Double

        For y = 0 To Width * ZoomMax
            For x = 0 To Width * ZoomMax
                CurrentPixel = MathMap(Int(x / ZoomMax), Int(y / ZoomMax)) / OctaveFrac
                If CurrentPixel < WaterThr Then
                    'Water
                    B = New SolidBrush(Color.FromArgb(CurrentPixel * 58 * (1 / WaterThr) + 10, CurrentPixel * 89 * (1 / WaterThr) + 20, CurrentPixel * 152 * (1 / WaterThr) + 70))
                ElseIf CurrentPixel < SandThr Then
                    'Sand
                    B = New SolidBrush(Color.FromArgb(CurrentPixel * 237 * (1 / SandThr), CurrentPixel * 213 * (1 / SandThr), CurrentPixel * 177 * (1 / SandThr)))
                ElseIf CurrentPixel < GrassThr Then
                    'Grass
                    B = New SolidBrush(Color.FromArgb((Rnd() * 0.08 + 0.96) * 35, (Rnd() * 0.08 + 0.96) * 122, (Rnd() * 0.08 + 0.96) * 36))
                ElseIf CurrentPixel < ForestThr Then
                    'Forest
                    B = New SolidBrush(Color.FromArgb(CurrentPixel * 9 * (1 / ForestThr), CurrentPixel * 99 * (1 / ForestThr), CurrentPixel * 12 * (1 / ForestThr)))
                ElseIf CurrentPixel < MountainThr Then
                    'Mountain
                    B = New SolidBrush(Color.FromArgb(CurrentPixel * 139 * (1 / MountainThr), CurrentPixel * 137 * (1 / MountainThr), CurrentPixel * 137 * (1 / MountainThr)))
                ElseIf CurrentPixel < SnowThr Then
                    'Snow
                    B = New SolidBrush(Color.FromArgb(CurrentPixel * 255 * (1 / SnowThr), CurrentPixel * 250 * (1 / SnowThr), CurrentPixel * 250 * (1 / SnowThr)))
                Else
                    'Never used, error catch
                    B = New SolidBrush(Color.FromArgb(0, 0, 0))
                End If
                GFX.FillRectangle(B, x, y, 1, 1)
                'I don't know why I can't find an alternative to fillrectangle. pen just wants to break. so does setpixel.
            Next
        Next

        Dim SaveMap(Width, Width) As Integer
        For y = 0 To Width
            For x = 0 To Width
                CurrentPixel = MathMap(x, y) / OctaveFrac
                If CurrentPixel < WaterThr Then
                    'Water
                    SaveMap(x, y) = 0
                ElseIf CurrentPixel < SandThr Then
                    'Sand
                    SaveMap(x, y) = 1
                ElseIf CurrentPixel < GrassThr Then
                    'Grass
                    SaveMap(x, y) = 2
                ElseIf CurrentPixel < ForestThr Then
                    'Forest
                    SaveMap(x, y) = 3
                ElseIf CurrentPixel < MountainThr Then
                    'Mountain
                    SaveMap(x, y) = 4
                ElseIf CurrentPixel < SnowThr Then
                    'Snow
                    SaveMap(x, y) = 5
                End If

            Next
        Next

        Dim Name As String
        Do
            Name = InputBox("Enter a filename")
            For i = 1 To FileIO.FileSystem.GetDirectories(".\Worlds\").Count

                If Name = FileIO.FileSystem.GetDirectoryInfo(FileIO.FileSystem.GetDirectories(".\Worlds\").Item(i - 1)).Name Then Name = ""
            Next i
        Loop Until Name IsNot ""

        My.Computer.FileSystem.CreateDirectory(".\Worlds\" & Name)
        Noisemap.Save(String.Format(".\worlds\{0}\{0}-image.png", Name), Imaging.ImageFormat.Png)

        'NOISEMAP IS DONE, ADDING FEATURES//NOISEMAP IS DONE, ADDING FEATURES//NOISEMAP IS DONE, ADDING FEATURES//NOISEMAP IS DONE, ADDING FEATURES//NOISEMAP IS DONE, ADDING FEATURES//NOISEMAP IS DONE, ADDING FEATURES
        'NOISEMAP IS DONE, ADDING FEATURES//NOISEMAP IS DONE, ADDING FEATURES//NOISEMAP IS DONE, ADDING FEATURES//NOISEMAP IS DONE, ADDING FEATURES//NOISEMAP IS DONE, ADDING FEATURES//NOISEMAP IS DONE, ADDING FEATURES

        Dim GeneratedFeatures As New List(Of MapObject)
        Dim fx, fy As Integer
        Dim item As MapObject

        For i = 1 To 30
            fx = Rnd() * 896 + 64
            fy = Rnd() * 896 + 64

            CurrentPixel = MathMap(fx, fy) / OctaveFrac
            item = New MapObject With {.CentrePoint = New Point(fx, fy)}

            If CurrentPixel < WaterThr Then
                'Water
                item.ImageType = "water"
            ElseIf CurrentPixel < SandThr Then
                'Sand
                item.ImageType = "sand"
            ElseIf CurrentPixel < GrassThr Then
                'Grass
                item.ImageType = "grass"
            ElseIf CurrentPixel < ForestThr Then
                'Forest
                item.ImageType = "forest"
            ElseIf CurrentPixel < MountainThr Then
                'Mountain
                item.ImageType = "mountain"
            ElseIf CurrentPixel < SnowThr Then
                'Snow
                item.ImageType = "snow"
            End If
            GeneratedFeatures.Add(item)
        Next

        'Sort GeneratedFeatures

        'MergeSort(GeneratedFeatures)

        'Output the features as a file
        CompileFeaturesToFile(GeneratedFeatures, String.Format(".\worlds\{0}\{0}-features.ftr", Name))
    End Sub

End Module


