<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class CanvasForm
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()>
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Me.picCanvas = New System.Windows.Forms.PictureBox()
        Me.btnGenerate = New System.Windows.Forms.Button()
        Me.LstImageFiles = New System.Windows.Forms.ListBox()
        Me.LblListexp = New System.Windows.Forms.Label()
        Me.BtnClose = New System.Windows.Forms.Button()
        Me.BtnMinimise = New System.Windows.Forms.Button()
        Me.BtnNewPreset = New System.Windows.Forms.Button()
        Me.CmbPresets = New System.Windows.Forms.ComboBox()
        CType(Me.picCanvas, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'picCanvas
        '
        Me.picCanvas.BackColor = System.Drawing.Color.Transparent
        Me.picCanvas.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom
        Me.picCanvas.Location = New System.Drawing.Point(443, 23)
        Me.picCanvas.Name = "picCanvas"
        Me.picCanvas.Size = New System.Drawing.Size(1024, 1024)
        Me.picCanvas.TabIndex = 1
        Me.picCanvas.TabStop = False
        '
        'btnGenerate
        '
        Me.btnGenerate.Location = New System.Drawing.Point(12, 23)
        Me.btnGenerate.Name = "btnGenerate"
        Me.btnGenerate.Size = New System.Drawing.Size(75, 50)
        Me.btnGenerate.TabIndex = 1
        Me.btnGenerate.Text = "Generate Noisemap"
        Me.btnGenerate.UseVisualStyleBackColor = True
        '
        'LstImageFiles
        '
        Me.LstImageFiles.FormattingEnabled = True
        Me.LstImageFiles.Location = New System.Drawing.Point(317, 23)
        Me.LstImageFiles.Name = "LstImageFiles"
        Me.LstImageFiles.Size = New System.Drawing.Size(120, 927)
        Me.LstImageFiles.TabIndex = 4
        '
        'LblListexp
        '
        Me.LblListexp.AutoSize = True
        Me.LblListexp.BackColor = System.Drawing.Color.Transparent
        Me.LblListexp.ForeColor = System.Drawing.Color.White
        Me.LblListexp.Location = New System.Drawing.Point(208, 23)
        Me.LblListexp.Name = "LblListexp"
        Me.LblListexp.Size = New System.Drawing.Size(103, 13)
        Me.LblListexp.TabIndex = 25
        Me.LblListexp.Text = "Double click an item"
        '
        'BtnClose
        '
        Me.BtnClose.Location = New System.Drawing.Point(1833, 12)
        Me.BtnClose.Name = "BtnClose"
        Me.BtnClose.Size = New System.Drawing.Size(75, 23)
        Me.BtnClose.TabIndex = 6
        Me.BtnClose.Text = "Close"
        Me.BtnClose.UseVisualStyleBackColor = True
        '
        'BtnMinimise
        '
        Me.BtnMinimise.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.BtnMinimise.Location = New System.Drawing.Point(1752, 12)
        Me.BtnMinimise.Name = "BtnMinimise"
        Me.BtnMinimise.Size = New System.Drawing.Size(75, 23)
        Me.BtnMinimise.TabIndex = 5
        Me.BtnMinimise.Text = "Minimise"
        Me.BtnMinimise.UseVisualStyleBackColor = True
        '
        'BtnNewPreset
        '
        Me.BtnNewPreset.Location = New System.Drawing.Point(93, 23)
        Me.BtnNewPreset.Name = "BtnNewPreset"
        Me.BtnNewPreset.Size = New System.Drawing.Size(100, 23)
        Me.BtnNewPreset.TabIndex = 2
        Me.BtnNewPreset.Text = "New Preset"
        Me.BtnNewPreset.UseVisualStyleBackColor = True
        '
        'CmbPresets
        '
        Me.CmbPresets.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.CmbPresets.FormattingEnabled = True
        Me.CmbPresets.Location = New System.Drawing.Point(93, 52)
        Me.CmbPresets.Name = "CmbPresets"
        Me.CmbPresets.Size = New System.Drawing.Size(100, 21)
        Me.CmbPresets.TabIndex = 3
        '
        'CanvasForm
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.Color.Black
        Me.BackgroundImage = Global.NoisemapGen.My.Resources.Resources.BG1
        Me.CancelButton = Me.BtnMinimise
        Me.ClientSize = New System.Drawing.Size(1920, 1080)
        Me.Controls.Add(Me.CmbPresets)
        Me.Controls.Add(Me.BtnNewPreset)
        Me.Controls.Add(Me.BtnMinimise)
        Me.Controls.Add(Me.BtnClose)
        Me.Controls.Add(Me.LblListexp)
        Me.Controls.Add(Me.LstImageFiles)
        Me.Controls.Add(Me.btnGenerate)
        Me.Controls.Add(Me.picCanvas)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
        Me.KeyPreview = True
        Me.Name = "CanvasForm"
        CType(Me.picCanvas, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents picCanvas As PictureBox
    Friend WithEvents btnGenerate As Button
    Friend WithEvents LstImageFiles As ListBox
    Friend WithEvents LblListexp As Label
    Friend WithEvents BtnClose As Button
    Friend WithEvents BtnMinimise As Button
    Friend WithEvents BtnNewPreset As Button
    Friend WithEvents CmbPresets As ComboBox
End Class
