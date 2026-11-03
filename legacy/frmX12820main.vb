Imports System.IO
Imports System.Text.RegularExpressions
Imports System.Text

Public Class frmX12820main
    Inherits System.Windows.Forms.Form

#Region " Windows Form Designer generated code "

    Public Sub New()
        MyBase.New()

        'This call is required by the Windows Form Designer.
        InitializeComponent()

        'Add any initialization after the InitializeComponent() call

    End Sub

    'Form overrides dispose to clean up the component list.
    Protected Overloads Overrides Sub Dispose(ByVal disposing As Boolean)
        If disposing Then
            If Not (components Is Nothing) Then
                components.Dispose()
            End If
        End If
        MyBase.Dispose(disposing)
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    Friend WithEvents txtEntLines As System.Windows.Forms.TextBox
    Friend WithEvents btnOpenHipaaFile As System.Windows.Forms.Button
    Friend WithEvents txtFilename As System.Windows.Forms.TextBox
    Friend WithEvents dlg As System.Windows.Forms.OpenFileDialog
    Friend WithEvents txtSTSegment As System.Windows.Forms.TextBox
    Friend WithEvents txtSEsegment As System.Windows.Forms.TextBox
    Friend WithEvents TabControl1 As System.Windows.Forms.TabControl
    Friend WithEvents TabPage1 As System.Windows.Forms.TabPage
    Friend WithEvents TabPage2 As System.Windows.Forms.TabPage
    Friend WithEvents btnOpen834File As System.Windows.Forms.Button
    Friend WithEvents txt834SEsegment As System.Windows.Forms.TextBox
    Friend WithEvents txt834DTPsegment As System.Windows.Forms.TextBox
    Friend WithEvents txt834STsegment As System.Windows.Forms.TextBox
    Friend WithEvents txt834Filename As System.Windows.Forms.TextBox
    Friend WithEvents btnSmallerFonts As System.Windows.Forms.Button
    Friend WithEvents btnBiggerFonts As System.Windows.Forms.Button
    Friend WithEvents TabPage3 As System.Windows.Forms.TabPage
    Friend WithEvents btnOpen835 As System.Windows.Forms.Button
    Friend WithEvents txt835SEsegment As System.Windows.Forms.TextBox
    Friend WithEvents txt835CLPsegment As System.Windows.Forms.TextBox
    Friend WithEvents txt835STsegment As System.Windows.Forms.TextBox
    Friend WithEvents txt835Filename As System.Windows.Forms.TextBox
    Friend WithEvents TabPage4 As System.Windows.Forms.TabPage
    Friend WithEvents btnOpen271 As System.Windows.Forms.Button
    Friend WithEvents txt271SEsegment As System.Windows.Forms.TextBox
    Friend WithEvents txt271HLsegemnts As System.Windows.Forms.TextBox
    Friend WithEvents txt271STsegment As System.Windows.Forms.TextBox
    Friend WithEvents txt271Filename As System.Windows.Forms.TextBox
    Friend WithEvents TabPage5 As System.Windows.Forms.TabPage
    Friend WithEvents btnOpen837File As System.Windows.Forms.Button
    Friend WithEvents txt837SESegment As System.Windows.Forms.TextBox
    Friend WithEvents txt837EntLines As System.Windows.Forms.TextBox
    Friend WithEvents txt837stsegment As System.Windows.Forms.TextBox
    Friend WithEvents txt837Filename As System.Windows.Forms.TextBox
    Friend WithEvents TabPage6 As System.Windows.Forms.TabPage
    Friend WithEvents btnOpen277File As System.Windows.Forms.Button
    Friend WithEvents txt277SESegment As System.Windows.Forms.TextBox
    Friend WithEvents txt277EntsSegment As System.Windows.Forms.TextBox
    Friend WithEvents txt277StSegment As System.Windows.Forms.TextBox
    Friend WithEvents txt277filename As System.Windows.Forms.TextBox
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
        Me.txtEntLines = New System.Windows.Forms.TextBox()
        Me.btnOpenHipaaFile = New System.Windows.Forms.Button()
        Me.txtFilename = New System.Windows.Forms.TextBox()
        Me.dlg = New System.Windows.Forms.OpenFileDialog()
        Me.txtSTSegment = New System.Windows.Forms.TextBox()
        Me.txtSEsegment = New System.Windows.Forms.TextBox()
        Me.TabControl1 = New System.Windows.Forms.TabControl()
        Me.TabPage1 = New System.Windows.Forms.TabPage()
        Me.TabPage2 = New System.Windows.Forms.TabPage()
        Me.btnOpen834File = New System.Windows.Forms.Button()
        Me.txt834SEsegment = New System.Windows.Forms.TextBox()
        Me.txt834DTPsegment = New System.Windows.Forms.TextBox()
        Me.txt834STsegment = New System.Windows.Forms.TextBox()
        Me.txt834Filename = New System.Windows.Forms.TextBox()
        Me.TabPage3 = New System.Windows.Forms.TabPage()
        Me.btnOpen835 = New System.Windows.Forms.Button()
        Me.txt835SEsegment = New System.Windows.Forms.TextBox()
        Me.txt835CLPsegment = New System.Windows.Forms.TextBox()
        Me.txt835STsegment = New System.Windows.Forms.TextBox()
        Me.txt835Filename = New System.Windows.Forms.TextBox()
        Me.TabPage4 = New System.Windows.Forms.TabPage()
        Me.btnOpen271 = New System.Windows.Forms.Button()
        Me.txt271SEsegment = New System.Windows.Forms.TextBox()
        Me.txt271HLsegemnts = New System.Windows.Forms.TextBox()
        Me.txt271STsegment = New System.Windows.Forms.TextBox()
        Me.txt271Filename = New System.Windows.Forms.TextBox()
        Me.TabPage5 = New System.Windows.Forms.TabPage()
        Me.btnOpen837File = New System.Windows.Forms.Button()
        Me.txt837SESegment = New System.Windows.Forms.TextBox()
        Me.txt837EntLines = New System.Windows.Forms.TextBox()
        Me.txt837stsegment = New System.Windows.Forms.TextBox()
        Me.txt837Filename = New System.Windows.Forms.TextBox()
        Me.TabPage6 = New System.Windows.Forms.TabPage()
        Me.btnOpen277File = New System.Windows.Forms.Button()
        Me.txt277SESegment = New System.Windows.Forms.TextBox()
        Me.txt277EntsSegment = New System.Windows.Forms.TextBox()
        Me.txt277StSegment = New System.Windows.Forms.TextBox()
        Me.txt277filename = New System.Windows.Forms.TextBox()
        Me.btnSmallerFonts = New System.Windows.Forms.Button()
        Me.btnBiggerFonts = New System.Windows.Forms.Button()
        Me.TabControl1.SuspendLayout()
        Me.TabPage1.SuspendLayout()
        Me.TabPage2.SuspendLayout()
        Me.TabPage3.SuspendLayout()
        Me.TabPage4.SuspendLayout()
        Me.TabPage5.SuspendLayout()
        Me.TabPage6.SuspendLayout()
        Me.SuspendLayout()
        '
        'txtEntLines
        '
        Me.txtEntLines.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txtEntLines.BackColor = System.Drawing.Color.MistyRose
        Me.txtEntLines.Font = New System.Drawing.Font("Courier New", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtEntLines.Location = New System.Drawing.Point(8, 114)
        Me.txtEntLines.Multiline = True
        Me.txtEntLines.Name = "txtEntLines"
        Me.txtEntLines.ScrollBars = System.Windows.Forms.ScrollBars.Both
        Me.txtEntLines.Size = New System.Drawing.Size(923, 456)
        Me.txtEntLines.TabIndex = 0
        Me.txtEntLines.WordWrap = False
        '
        'btnOpenHipaaFile
        '
        Me.btnOpenHipaaFile.Location = New System.Drawing.Point(4, 4)
        Me.btnOpenHipaaFile.Name = "btnOpenHipaaFile"
        Me.btnOpenHipaaFile.Size = New System.Drawing.Size(58, 34)
        Me.btnOpenHipaaFile.TabIndex = 1
        Me.btnOpenHipaaFile.Text = "Open"
        '
        'txtFilename
        '
        Me.txtFilename.Location = New System.Drawing.Point(68, 10)
        Me.txtFilename.Name = "txtFilename"
        Me.txtFilename.Size = New System.Drawing.Size(448, 22)
        Me.txtFilename.TabIndex = 2
        '
        'txtSTSegment
        '
        Me.txtSTSegment.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txtSTSegment.BackColor = System.Drawing.Color.RosyBrown
        Me.txtSTSegment.Font = New System.Drawing.Font("Courier New", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtSTSegment.Location = New System.Drawing.Point(8, 44)
        Me.txtSTSegment.Multiline = True
        Me.txtSTSegment.Name = "txtSTSegment"
        Me.txtSTSegment.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
        Me.txtSTSegment.Size = New System.Drawing.Size(923, 64)
        Me.txtSTSegment.TabIndex = 3
        '
        'txtSEsegment
        '
        Me.txtSEsegment.Anchor = CType(((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txtSEsegment.BackColor = System.Drawing.Color.RosyBrown
        Me.txtSEsegment.Font = New System.Drawing.Font("Courier New", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtSEsegment.Location = New System.Drawing.Point(8, 574)
        Me.txtSEsegment.Multiline = True
        Me.txtSEsegment.Name = "txtSEsegment"
        Me.txtSEsegment.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
        Me.txtSEsegment.Size = New System.Drawing.Size(923, 32)
        Me.txtSEsegment.TabIndex = 4
        '
        'TabControl1
        '
        Me.TabControl1.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.TabControl1.Controls.Add(Me.TabPage1)
        Me.TabControl1.Controls.Add(Me.TabPage2)
        Me.TabControl1.Controls.Add(Me.TabPage3)
        Me.TabControl1.Controls.Add(Me.TabPage4)
        Me.TabControl1.Controls.Add(Me.TabPage5)
        Me.TabControl1.Controls.Add(Me.TabPage6)
        Me.TabControl1.Location = New System.Drawing.Point(4, 12)
        Me.TabControl1.Name = "TabControl1"
        Me.TabControl1.SelectedIndex = 0
        Me.TabControl1.Size = New System.Drawing.Size(943, 629)
        Me.TabControl1.TabIndex = 5
        '
        'TabPage1
        '
        Me.TabPage1.BackColor = System.Drawing.SystemColors.ControlDark
        Me.TabPage1.Controls.Add(Me.btnOpenHipaaFile)
        Me.TabPage1.Controls.Add(Me.txtSEsegment)
        Me.TabPage1.Controls.Add(Me.txtEntLines)
        Me.TabPage1.Controls.Add(Me.txtSTSegment)
        Me.TabPage1.Controls.Add(Me.txtFilename)
        Me.TabPage1.Location = New System.Drawing.Point(4, 30)
        Me.TabPage1.Name = "TabPage1"
        Me.TabPage1.Size = New System.Drawing.Size(935, 595)
        Me.TabPage1.TabIndex = 0
        Me.TabPage1.Text = "Capitation (820)"
        '
        'TabPage2
        '
        Me.TabPage2.BackColor = System.Drawing.SystemColors.ControlDark
        Me.TabPage2.Controls.Add(Me.btnOpen834File)
        Me.TabPage2.Controls.Add(Me.txt834SEsegment)
        Me.TabPage2.Controls.Add(Me.txt834DTPsegment)
        Me.TabPage2.Controls.Add(Me.txt834STsegment)
        Me.TabPage2.Controls.Add(Me.txt834Filename)
        Me.TabPage2.Location = New System.Drawing.Point(4, 30)
        Me.TabPage2.Name = "TabPage2"
        Me.TabPage2.Size = New System.Drawing.Size(935, 595)
        Me.TabPage2.TabIndex = 1
        Me.TabPage2.Text = "Membership (834)"
        '
        'btnOpen834File
        '
        Me.btnOpen834File.Location = New System.Drawing.Point(4, 4)
        Me.btnOpen834File.Name = "btnOpen834File"
        Me.btnOpen834File.Size = New System.Drawing.Size(58, 31)
        Me.btnOpen834File.TabIndex = 6
        Me.btnOpen834File.Text = "Open"
        '
        'txt834SEsegment
        '
        Me.txt834SEsegment.Anchor = CType(((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txt834SEsegment.BackColor = System.Drawing.Color.RosyBrown
        Me.txt834SEsegment.Font = New System.Drawing.Font("Courier New", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txt834SEsegment.Location = New System.Drawing.Point(8, 569)
        Me.txt834SEsegment.Multiline = True
        Me.txt834SEsegment.Name = "txt834SEsegment"
        Me.txt834SEsegment.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
        Me.txt834SEsegment.Size = New System.Drawing.Size(923, 32)
        Me.txt834SEsegment.TabIndex = 9
        '
        'txt834DTPsegment
        '
        Me.txt834DTPsegment.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txt834DTPsegment.BackColor = System.Drawing.Color.MistyRose
        Me.txt834DTPsegment.Font = New System.Drawing.Font("Courier New", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txt834DTPsegment.Location = New System.Drawing.Point(8, 111)
        Me.txt834DTPsegment.Multiline = True
        Me.txt834DTPsegment.Name = "txt834DTPsegment"
        Me.txt834DTPsegment.ScrollBars = System.Windows.Forms.ScrollBars.Both
        Me.txt834DTPsegment.Size = New System.Drawing.Size(923, 454)
        Me.txt834DTPsegment.TabIndex = 5
        Me.txt834DTPsegment.WordWrap = False
        '
        'txt834STsegment
        '
        Me.txt834STsegment.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txt834STsegment.BackColor = System.Drawing.Color.RosyBrown
        Me.txt834STsegment.Font = New System.Drawing.Font("Courier New", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txt834STsegment.Location = New System.Drawing.Point(9, 41)
        Me.txt834STsegment.Multiline = True
        Me.txt834STsegment.Name = "txt834STsegment"
        Me.txt834STsegment.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
        Me.txt834STsegment.Size = New System.Drawing.Size(923, 64)
        Me.txt834STsegment.TabIndex = 8
        '
        'txt834Filename
        '
        Me.txt834Filename.Location = New System.Drawing.Point(68, 8)
        Me.txt834Filename.Name = "txt834Filename"
        Me.txt834Filename.Size = New System.Drawing.Size(448, 22)
        Me.txt834Filename.TabIndex = 7
        '
        'TabPage3
        '
        Me.TabPage3.BackColor = System.Drawing.SystemColors.ControlDark
        Me.TabPage3.Controls.Add(Me.btnOpen835)
        Me.TabPage3.Controls.Add(Me.txt835SEsegment)
        Me.TabPage3.Controls.Add(Me.txt835CLPsegment)
        Me.TabPage3.Controls.Add(Me.txt835STsegment)
        Me.TabPage3.Controls.Add(Me.txt835Filename)
        Me.TabPage3.Location = New System.Drawing.Point(4, 30)
        Me.TabPage3.Name = "TabPage3"
        Me.TabPage3.Size = New System.Drawing.Size(935, 595)
        Me.TabPage3.TabIndex = 2
        Me.TabPage3.Text = "Remittance (835)"
        '
        'btnOpen835
        '
        Me.btnOpen835.Location = New System.Drawing.Point(4, 3)
        Me.btnOpen835.Name = "btnOpen835"
        Me.btnOpen835.Size = New System.Drawing.Size(58, 33)
        Me.btnOpen835.TabIndex = 11
        Me.btnOpen835.Text = "Open"
        '
        'txt835SEsegment
        '
        Me.txt835SEsegment.Anchor = CType(((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txt835SEsegment.BackColor = System.Drawing.Color.RosyBrown
        Me.txt835SEsegment.Font = New System.Drawing.Font("Courier New", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txt835SEsegment.Location = New System.Drawing.Point(8, 568)
        Me.txt835SEsegment.Multiline = True
        Me.txt835SEsegment.Name = "txt835SEsegment"
        Me.txt835SEsegment.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
        Me.txt835SEsegment.Size = New System.Drawing.Size(923, 32)
        Me.txt835SEsegment.TabIndex = 14
        '
        'txt835CLPsegment
        '
        Me.txt835CLPsegment.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txt835CLPsegment.BackColor = System.Drawing.Color.MistyRose
        Me.txt835CLPsegment.Font = New System.Drawing.Font("Courier New", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txt835CLPsegment.Location = New System.Drawing.Point(8, 112)
        Me.txt835CLPsegment.Multiline = True
        Me.txt835CLPsegment.Name = "txt835CLPsegment"
        Me.txt835CLPsegment.ScrollBars = System.Windows.Forms.ScrollBars.Both
        Me.txt835CLPsegment.Size = New System.Drawing.Size(923, 452)
        Me.txt835CLPsegment.TabIndex = 10
        Me.txt835CLPsegment.WordWrap = False
        '
        'txt835STsegment
        '
        Me.txt835STsegment.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txt835STsegment.BackColor = System.Drawing.Color.RosyBrown
        Me.txt835STsegment.Font = New System.Drawing.Font("Courier New", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txt835STsegment.Location = New System.Drawing.Point(8, 42)
        Me.txt835STsegment.Multiline = True
        Me.txt835STsegment.Name = "txt835STsegment"
        Me.txt835STsegment.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
        Me.txt835STsegment.Size = New System.Drawing.Size(923, 64)
        Me.txt835STsegment.TabIndex = 13
        '
        'txt835Filename
        '
        Me.txt835Filename.Location = New System.Drawing.Point(68, 8)
        Me.txt835Filename.Name = "txt835Filename"
        Me.txt835Filename.Size = New System.Drawing.Size(448, 22)
        Me.txt835Filename.TabIndex = 12
        '
        'TabPage4
        '
        Me.TabPage4.BackColor = System.Drawing.SystemColors.ControlDark
        Me.TabPage4.Controls.Add(Me.btnOpen271)
        Me.TabPage4.Controls.Add(Me.txt271SEsegment)
        Me.TabPage4.Controls.Add(Me.txt271HLsegemnts)
        Me.TabPage4.Controls.Add(Me.txt271STsegment)
        Me.TabPage4.Controls.Add(Me.txt271Filename)
        Me.TabPage4.Location = New System.Drawing.Point(4, 30)
        Me.TabPage4.Name = "TabPage4"
        Me.TabPage4.Size = New System.Drawing.Size(935, 595)
        Me.TabPage4.TabIndex = 3
        Me.TabPage4.Text = "Benefit Info (271)"
        '
        'btnOpen271
        '
        Me.btnOpen271.Location = New System.Drawing.Point(8, 5)
        Me.btnOpen271.Name = "btnOpen271"
        Me.btnOpen271.Size = New System.Drawing.Size(54, 30)
        Me.btnOpen271.TabIndex = 16
        Me.btnOpen271.Text = "Open"
        '
        'txt271SEsegment
        '
        Me.txt271SEsegment.Anchor = CType(((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txt271SEsegment.BackColor = System.Drawing.Color.RosyBrown
        Me.txt271SEsegment.Font = New System.Drawing.Font("Courier New", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txt271SEsegment.Location = New System.Drawing.Point(8, 568)
        Me.txt271SEsegment.Multiline = True
        Me.txt271SEsegment.Name = "txt271SEsegment"
        Me.txt271SEsegment.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
        Me.txt271SEsegment.Size = New System.Drawing.Size(923, 32)
        Me.txt271SEsegment.TabIndex = 19
        '
        'txt271HLsegemnts
        '
        Me.txt271HLsegemnts.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txt271HLsegemnts.BackColor = System.Drawing.Color.MistyRose
        Me.txt271HLsegemnts.Font = New System.Drawing.Font("Courier New", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txt271HLsegemnts.Location = New System.Drawing.Point(8, 111)
        Me.txt271HLsegemnts.Multiline = True
        Me.txt271HLsegemnts.Name = "txt271HLsegemnts"
        Me.txt271HLsegemnts.ScrollBars = System.Windows.Forms.ScrollBars.Both
        Me.txt271HLsegemnts.Size = New System.Drawing.Size(923, 453)
        Me.txt271HLsegemnts.TabIndex = 15
        Me.txt271HLsegemnts.WordWrap = False
        '
        'txt271STsegment
        '
        Me.txt271STsegment.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txt271STsegment.BackColor = System.Drawing.Color.RosyBrown
        Me.txt271STsegment.Font = New System.Drawing.Font("Courier New", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txt271STsegment.Location = New System.Drawing.Point(9, 41)
        Me.txt271STsegment.Multiline = True
        Me.txt271STsegment.Name = "txt271STsegment"
        Me.txt271STsegment.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
        Me.txt271STsegment.Size = New System.Drawing.Size(923, 64)
        Me.txt271STsegment.TabIndex = 18
        '
        'txt271Filename
        '
        Me.txt271Filename.Location = New System.Drawing.Point(68, 9)
        Me.txt271Filename.Name = "txt271Filename"
        Me.txt271Filename.Size = New System.Drawing.Size(448, 22)
        Me.txt271Filename.TabIndex = 17
        '
        'TabPage5
        '
        Me.TabPage5.BackColor = System.Drawing.SystemColors.ControlDark
        Me.TabPage5.Controls.Add(Me.btnOpen837File)
        Me.TabPage5.Controls.Add(Me.txt837SESegment)
        Me.TabPage5.Controls.Add(Me.txt837EntLines)
        Me.TabPage5.Controls.Add(Me.txt837stsegment)
        Me.TabPage5.Controls.Add(Me.txt837Filename)
        Me.TabPage5.Location = New System.Drawing.Point(4, 30)
        Me.TabPage5.Name = "TabPage5"
        Me.TabPage5.Size = New System.Drawing.Size(935, 595)
        Me.TabPage5.TabIndex = 4
        Me.TabPage5.Text = "Claim (837)"
        '
        'btnOpen837File
        '
        Me.btnOpen837File.Location = New System.Drawing.Point(4, 3)
        Me.btnOpen837File.Name = "btnOpen837File"
        Me.btnOpen837File.Size = New System.Drawing.Size(58, 38)
        Me.btnOpen837File.TabIndex = 11
        Me.btnOpen837File.Text = "Open"
        '
        'txt837SESegment
        '
        Me.txt837SESegment.Anchor = CType(((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txt837SESegment.BackColor = System.Drawing.Color.RosyBrown
        Me.txt837SESegment.Font = New System.Drawing.Font("Courier New", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txt837SESegment.Location = New System.Drawing.Point(8, 570)
        Me.txt837SESegment.Multiline = True
        Me.txt837SESegment.Name = "txt837SESegment"
        Me.txt837SESegment.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
        Me.txt837SESegment.Size = New System.Drawing.Size(923, 32)
        Me.txt837SESegment.TabIndex = 14
        '
        'txt837EntLines
        '
        Me.txt837EntLines.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txt837EntLines.BackColor = System.Drawing.Color.MistyRose
        Me.txt837EntLines.Font = New System.Drawing.Font("Courier New", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txt837EntLines.Location = New System.Drawing.Point(8, 117)
        Me.txt837EntLines.Multiline = True
        Me.txt837EntLines.Name = "txt837EntLines"
        Me.txt837EntLines.ScrollBars = System.Windows.Forms.ScrollBars.Both
        Me.txt837EntLines.Size = New System.Drawing.Size(923, 449)
        Me.txt837EntLines.TabIndex = 10
        Me.txt837EntLines.WordWrap = False
        '
        'txt837stsegment
        '
        Me.txt837stsegment.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txt837stsegment.BackColor = System.Drawing.Color.RosyBrown
        Me.txt837stsegment.Font = New System.Drawing.Font("Courier New", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txt837stsegment.Location = New System.Drawing.Point(8, 47)
        Me.txt837stsegment.Multiline = True
        Me.txt837stsegment.Name = "txt837stsegment"
        Me.txt837stsegment.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
        Me.txt837stsegment.Size = New System.Drawing.Size(923, 64)
        Me.txt837stsegment.TabIndex = 13
        '
        'txt837Filename
        '
        Me.txt837Filename.Location = New System.Drawing.Point(68, 11)
        Me.txt837Filename.Name = "txt837Filename"
        Me.txt837Filename.Size = New System.Drawing.Size(448, 22)
        Me.txt837Filename.TabIndex = 12
        '
        'TabPage6
        '
        Me.TabPage6.BackColor = System.Drawing.SystemColors.ControlDark
        Me.TabPage6.Controls.Add(Me.btnOpen277File)
        Me.TabPage6.Controls.Add(Me.txt277SESegment)
        Me.TabPage6.Controls.Add(Me.txt277EntsSegment)
        Me.TabPage6.Controls.Add(Me.txt277StSegment)
        Me.TabPage6.Controls.Add(Me.txt277filename)
        Me.TabPage6.Location = New System.Drawing.Point(4, 30)
        Me.TabPage6.Name = "TabPage6"
        Me.TabPage6.Size = New System.Drawing.Size(935, 595)
        Me.TabPage6.TabIndex = 5
        Me.TabPage6.Text = "Claim Status Response (277)"
        '
        'btnOpen277File
        '
        Me.btnOpen277File.Location = New System.Drawing.Point(4, 3)
        Me.btnOpen277File.Name = "btnOpen277File"
        Me.btnOpen277File.Size = New System.Drawing.Size(58, 38)
        Me.btnOpen277File.TabIndex = 16
        Me.btnOpen277File.Text = "Open"
        '
        'txt277SESegment
        '
        Me.txt277SESegment.Anchor = CType(((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txt277SESegment.BackColor = System.Drawing.Color.RosyBrown
        Me.txt277SESegment.Font = New System.Drawing.Font("Courier New", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txt277SESegment.Location = New System.Drawing.Point(8, 571)
        Me.txt277SESegment.Multiline = True
        Me.txt277SESegment.Name = "txt277SESegment"
        Me.txt277SESegment.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
        Me.txt277SESegment.Size = New System.Drawing.Size(923, 32)
        Me.txt277SESegment.TabIndex = 19
        '
        'txt277EntsSegment
        '
        Me.txt277EntsSegment.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txt277EntsSegment.BackColor = System.Drawing.Color.MistyRose
        Me.txt277EntsSegment.Font = New System.Drawing.Font("Courier New", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txt277EntsSegment.Location = New System.Drawing.Point(8, 117)
        Me.txt277EntsSegment.Multiline = True
        Me.txt277EntsSegment.Name = "txt277EntsSegment"
        Me.txt277EntsSegment.ScrollBars = System.Windows.Forms.ScrollBars.Both
        Me.txt277EntsSegment.Size = New System.Drawing.Size(923, 450)
        Me.txt277EntsSegment.TabIndex = 15
        Me.txt277EntsSegment.WordWrap = False
        '
        'txt277StSegment
        '
        Me.txt277StSegment.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txt277StSegment.BackColor = System.Drawing.Color.RosyBrown
        Me.txt277StSegment.Font = New System.Drawing.Font("Courier New", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txt277StSegment.Location = New System.Drawing.Point(8, 47)
        Me.txt277StSegment.Multiline = True
        Me.txt277StSegment.Name = "txt277StSegment"
        Me.txt277StSegment.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
        Me.txt277StSegment.Size = New System.Drawing.Size(923, 64)
        Me.txt277StSegment.TabIndex = 18
        '
        'txt277filename
        '
        Me.txt277filename.Location = New System.Drawing.Point(68, 11)
        Me.txt277filename.Name = "txt277filename"
        Me.txt277filename.Size = New System.Drawing.Size(448, 22)
        Me.txt277filename.TabIndex = 17
        '
        'btnSmallerFonts
        '
        Me.btnSmallerFonts.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnSmallerFonts.Location = New System.Drawing.Point(726, 0)
        Me.btnSmallerFonts.Name = "btnSmallerFonts"
        Me.btnSmallerFonts.Size = New System.Drawing.Size(115, 36)
        Me.btnSmallerFonts.TabIndex = 6
        Me.btnSmallerFonts.Text = "Fonts Smaller"
        '
        'btnBiggerFonts
        '
        Me.btnBiggerFonts.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnBiggerFonts.Location = New System.Drawing.Point(847, 0)
        Me.btnBiggerFonts.Name = "btnBiggerFonts"
        Me.btnBiggerFonts.Size = New System.Drawing.Size(100, 36)
        Me.btnBiggerFonts.TabIndex = 7
        Me.btnBiggerFonts.Text = "Fonts Bigger"
        '
        'frmX12820main
        '
        Me.AutoScaleBaseSize = New System.Drawing.Size(6, 15)
        Me.ClientSize = New System.Drawing.Size(951, 646)
        Me.Controls.Add(Me.btnBiggerFonts)
        Me.Controls.Add(Me.btnSmallerFonts)
        Me.Controls.Add(Me.TabControl1)
        Me.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Name = "frmX12820main"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "X12 820/834/835/837/271/277 Explorer"
        Me.TabControl1.ResumeLayout(False)
        Me.TabPage1.ResumeLayout(False)
        Me.TabPage1.PerformLayout()
        Me.TabPage2.ResumeLayout(False)
        Me.TabPage2.PerformLayout()
        Me.TabPage3.ResumeLayout(False)
        Me.TabPage3.PerformLayout()
        Me.TabPage4.ResumeLayout(False)
        Me.TabPage4.PerformLayout()
        Me.TabPage5.ResumeLayout(False)
        Me.TabPage5.PerformLayout()
        Me.TabPage6.ResumeLayout(False)
        Me.TabPage6.PerformLayout()
        Me.ResumeLayout(False)

    End Sub

#End Region

    Private Sub DecodeFile(ByVal fname As String)

        Dim ents As String
        Dim ent() As String
        Dim t As Long
        ''Dim reg As Regex

        Dim sb As New StringBuilder

        Dim tr As TextReader = File.OpenText(fname)

        Me.txtEntLines.Text = ""
        ents = tr.ReadToEnd

        ents = ents.Replace(ControlChars.CrLf, "")

        tr.Close()

        ' now take the read in file string and lets begin to break it apart
        ' we'll start by looking for the ~ST header section
        ent = Regex.Split(ents, "~ST")
        ' we should now have a two position array ent(0) withh have DPW spluge
        ' ent(1) will have the rest of the file - the ~ST so well add it back in and discard the 
        ' DPW spluge

        ' add the ~ST back
        ents = "ST" & ent(1)

        ' now lets get the ~SE trailer
        ent = Regex.Split(ents, "~SE")
        ' we should now have a two position array ent(0) has most of the file - the trailing ~
        ' and ent(1) has the SE segment - the SE

        ' lets put it away for future reference
        txtSEsegment.Text = "SE" & ent(1)

        ' lets put the ~ back on the end of the ents string
        ents = ent(0) & "~"

        sb.Capacity = ents.Length

        ent = Regex.Split(ents, "~ENT")

        For t = 1 To ent.GetUpperBound(0)

            If t < ent.GetUpperBound(0) Then
                sb.Append("ENT" & ent(t) & "~" & vbCrLf)
            Else
                sb.Append("ENT" & ent(t))
            End If

            'sb.Append("ENT" & ent(t) & "~" & vbCrLf)

        Next

        txtEntLines.Text = sb.ToString

        txtSTSegment.Text = ent(0) & "~"


    End Sub

    Private Sub Decode277File(ByVal fname As String)

        Dim ents As String
        Dim ent() As String
        Dim enta() As String
        Dim t, tt, ttt As Long
        ''Dim reg As Regex

        Dim sb As New StringBuilder

        Dim tr As TextReader = File.OpenText(fname)

        Me.txtEntLines.Text = ""
        ents = tr.ReadToEnd

        ents = ents.Replace(ControlChars.CrLf, "")

        tr.Close()

        ' now take the read in file string and lets begin to break it apart
        ' we'll start by looking for the ~ST header section
        enta = Regex.Split(ents, "~ST\*")
        ' we should now have a two position array ent(0) withh have DPW spluge
        ' ent(1) will have the rest of the file - the ~ST so well add it back in and discard the 
        ' DPW spluge


        For ttt = 1 To enta.GetUpperBound(0)
            ' add the ~ST back
            ents = "ST*" & enta(ttt)

            ' now lets get the ~SE trailer
            ent = Regex.Split(ents, "~SE\*")
            ' we should now have a two position array ent(0) has most of the file - the trailing ~
            ' and ent(1) has the SE segment - the SE

            ' lets put it away for future reference
            txt277SESegment.Text = "SE*" & enta(1)

            ' lets put the ~ back on the end of the ents string
            ents = ent(0) & "~"

            'sb.Capacity = ents.Length

            ent = Regex.Split(ents, "~HL\*")

            For t = 1 To ent.GetUpperBound(0)

                Dim ss As String = ent(t)

                Dim sss() As String = Regex.Split(ss, "~TRN\*")

                If sss.GetUpperBound(0) >= 1 Then

                    sb.Append("HL*" & sss(0) & "~" & vbCrLf)

                    For tt = 1 To sss.GetUpperBound(0)
                        sb.Append("   TRN*" & sss(tt) & "~" & vbCrLf)
                    Next
                Else
                    sb.Append("HL*" & sss(0) & "~" & vbCrLf)
                End If


                'Dim sss() As String = Regex.Split(ss, "~STC\*")

                'If sss.GetUpperBound(0) > 1 Then

                '    sb.Append("HL*" & sss(0) & "~" & vbCrLf)

                '    For tt = 1 To sss.GetUpperBound(0)
                '        sb.Append("   STC*" & sss(tt) & "~" & vbCrLf)
                '    Next
                'Else
                '    sb.Append("HL*" & sss(0) & "~" & vbCrLf)
                'End If

                'If t < ent.GetUpperBound(0) Then
                '    sb.Append("HL*" & ent(t) & "~" & vbCrLf)
                'Else
                '    sb.Append("HL*" & ent(t))
                'End If

                'sb.Append("ENT" & ent(t) & "~" & vbCrLf)

            Next
        Next

        '' add the ~ST back
        'ents = "ST*" & enta(1)

        '' now lets get the ~SE trailer
        'ent = Regex.Split(ents, "~SE\*")
        '' we should now have a two position array ent(0) has most of the file - the trailing ~
        '' and ent(1) has the SE segment - the SE

        '' lets put it away for future reference
        'txt277SESegment.Text = "SE*" & ent(1)

        '' lets put the ~ back on the end of the ents string
        'ents = ent(0) & "~"

        'sb.Capacity = ents.Length

        'ent = Regex.Split(ents, "~HL\*")

        'For t = 1 To ent.GetUpperBound(0)

        '    Dim ss As String = ent(t)

        '    Dim sss() As String = Regex.Split(ss, "~STC\*")

        '    If sss.GetUpperBound(0) > 1 Then

        '        sb.Append("HL*" & sss(0) & "~" & vbCrLf)

        '        For tt = 1 To sss.GetUpperBound(0)
        '            sb.Append("   STC*" & sss(tt) & "~" & vbCrLf)
        '        Next
        '    Else
        '        sb.Append("HL*" & sss(0) & "~" & vbCrLf)
        '    End If

        '    'If t < ent.GetUpperBound(0) Then
        '    '    sb.Append("HL*" & ent(t) & "~" & vbCrLf)
        '    'Else
        '    '    sb.Append("HL*" & ent(t))
        '    'End If

        '    'sb.Append("ENT" & ent(t) & "~" & vbCrLf)

        'Next

        txt277EntsSegment.Text = sb.ToString

        txt277StSegment.Text = ent(0) & "~"


    End Sub

    Private Function DecodeENT(ByVal theent As String) As String

        Dim ents As String
        Dim ent() As String
        Dim ent1array() As String
        Dim t As Long
        ''Dim tt As Long
        ''Dim reg As Regex

        Dim sb As New StringBuilder


        ' add the ~ST back
        ents = "ST" & theent

        ' now lets get the ~SE trailer
        ent = Regex.Split(ents, "~SE")
        ' we should now have a two position array ent(0) has most of the file - the trailing ~
        ' and ent(1) has the SE segment - the SE

        '' lets put it away for future reference
        'txt837SESegment.Text = "SE" & ent(1)

        ' lets put the ~ back on the end of the ents string
        ents = ent(0) & "~"

        sb.Capacity = ents.Length

        ent = Regex.Split(ents, "~HL")

        For t = 1 To ent.GetUpperBound(0)

            Dim ent1 As String = ent(t)

            ent1array = Regex.Split(ent1, "~")

            Dim flag As Boolean = True

            For Each ss As String In ent1array
                If flag Then
                    sb.Append("HL" & ss & "~" & vbCrLf)
                    flag = False
                Else
                    sb.Append("    " & ss & vbCrLf)
                End If
            Next



            'If t < ent.GetUpperBound(0) Then
            '    sb.Append("HL" & ent(t) & "~" & vbCrLf)
            'Else
            '    sb.Append("HL" & ent(t))
            'End If

            'sb.Append("ENT" & ent(t) & "~" & vbCrLf)

        Next

        Return sb.ToString

    End Function



    Private Sub Decode837File(ByVal fname As String)

        Dim ents As String
        Dim ent() As String
        Dim ent1array() As String
        Dim t As Long
        ''Dim tt As Long
        ''Dim reg As Regex

        Dim sb As New StringBuilder

        Dim tr As TextReader = File.OpenText(fname)

        Me.txtEntLines.Text = ""
        ents = tr.ReadToEnd

        ents = ents.Replace(ControlChars.CrLf, "")

        tr.Close()

        ' here we might try to turn the LFs to ~ characters to account to the CCBH files the deviate from the standard a bit
        ents = ents.Replace(vbLf, "~")

        ' now take the read in file string and lets begin to break it apart
        ' we'll start by looking for the ~ST header section
        ent = Regex.Split(ents, "~ST")
        ' we should now have a two position array ent(0) withh have DPW spluge
        ' ent(1) will have the rest of the file - the ~ST so well add it back in and discard the 
        ' DPW spluge

        For t1 As Integer = 1 To ent.GetUpperBound(0)
            sb.Append(DecodeENT("ST" & ent(t1)))
        Next

        ' add the ~ST back
        ents = "ST" & ent(1)

        ' now lets get the ~SE trailer
        ent = Regex.Split(ents, "~SE")
        ' we should now have a two position array ent(0) has most of the file - the trailing ~
        ' and ent(1) has the SE segment - the SE

        ' lets put it away for future reference
        txt837SESegment.Text = "SE" & ent(1)

        ' lets put the ~ back on the end of the ents string
        ents = ent(0) & "~"

        'sb.Capacity = ents.Length

        ent = Regex.Split(ents, "~HL")

        'For t = 1 To ent.GetUpperBound(0)

        '    Dim ent1 As String = ent(t)

        '    ent1array = Regex.Split(ent1, "~")

        '    Dim flag As Boolean = True

        '    For Each ss As String In ent1array
        '        If flag Then
        '            sb.Append("HL" & ss & "~" & vbCrLf)
        '            flag = False
        '        Else
        '            sb.Append("    " & ss & vbCrLf)
        '        End If
        '    Next



        '    'If t < ent.GetUpperBound(0) Then
        '    '    sb.Append("HL" & ent(t) & "~" & vbCrLf)
        '    'Else
        '    '    sb.Append("HL" & ent(t))
        '    'End If

        '    'sb.Append("ENT" & ent(t) & "~" & vbCrLf)

        'Next

        txt837EntLines.Text = sb.ToString

        txt837stsegment.Text = ent(0) & "~"


    End Sub

    Private Sub DecodeFile834(ByVal fname As String)

        Dim ents As String
        Dim ent() As String
        Dim t As Long
        'Dim reg As Regex

        Dim sb As New StringBuilder

        Dim tr As TextReader = File.OpenText(fname)

        Me.txtEntLines.Text = ""
        ents = tr.ReadToEnd

        ents = ents.Replace(ControlChars.CrLf, "")

        tr.Close()

        ' now take the read in file string and lets begin to break it apart
        ' we'll start by looking for the ~ST header section
        ent = Regex.Split(ents, "~ST")
        ' we should now have a two position array ent(0) withh have DPW spluge
        ' ent(1) will have the rest of the file - the ~ST so well add it back in and discard the 
        ' DPW spluge

        ents = ""

        For t = 1 To ent.GetUpperBound(0)
            ents = ents & "ST" & ent(t)
        Next

        ' add the ~ST back
        'ents = "ST" & ent(1)

        ' now lets get the ~SE trailer
        ent = Regex.Split(ents, "~SE")
        ' we should now have a two position array ent(0) has most of the file - the trailing ~
        ' and ent(1) has the SE segment - the SE

        ' lets put it away for future reference
        txt834SEsegment.Text = "SE" & ent(ent.GetUpperBound(0))

        ' lets put the ~ back on the end of the ents string
        ents = ents & "~"

        sb.Capacity = ents.Length

        ent = Regex.Split(ents, "~INS")

        For t = 1 To ent.GetUpperBound(0)

            If t < ent.GetUpperBound(0) Then
                sb.Append("INS" & ent(t) & "~" & vbCrLf)
            Else
                sb.Append("INS" & ent(t))
            End If

        Next

        txt834DTPsegment.Text = sb.ToString

        txt834STsegment.Text = ent(0) & "~"


    End Sub

    Private Sub DecodeFile835(ByVal fname As String)

        Dim ents As String
        Dim ent() As String
        Dim t As Long
        'Dim reg As Regex

        Dim sb As New StringBuilder

        Dim tr As TextReader = File.OpenText(fname)

        Me.txtEntLines.Text = ""
        ents = tr.ReadToEnd

        ents = ents.Replace(ControlChars.CrLf, "")

        tr.Close()

        ' now take the read in file string and lets begin to break it apart
        ' we'll start by looking for the ~ST header section
        ent = Regex.Split(ents, "~ST")
        ' we should now have a two position array ent(0) withh have DPW spluge
        ' ent(1) will have the rest of the file - the ~ST so well add it back in and discard the 
        ' DPW spluge

        ents = ""

        For t = 1 To ent.GetUpperBound(0)
            ents = ents & "ST" & ent(t)
        Next

        ' add the ~ST back
        'ents = "ST" & ent(1)

        ' now lets get the ~SE trailer
        ent = Regex.Split(ents, "~SE")
        ' we should now have a two position array ent(0) has most of the file - the trailing ~
        ' and ent(1) has the SE segment - the SE

        ' lets put it away for future reference
        txt835SEsegment.Text = "SE" & ent(ent.GetUpperBound(0))

        ' lets put the ~ back on the end of the ents string
        ents = ents & "~"

        sb.Capacity = ents.Length

        ent = Regex.Split(ents, "~CLP")

        For t = 1 To ent.GetUpperBound(0)

            If t < ent.GetUpperBound(0) Then
                sb.Append("CLP" & ent(t) & "~" & vbCrLf)
            Else
                sb.Append("CLP" & ent(t))
            End If

        Next

        txt835CLPsegment.Text = sb.ToString

        txt835STsegment.Text = ent(0) & "~"


    End Sub

    Private Sub DecodeFile271(ByVal fname As String)

        Dim ents As String
        Dim ent() As String
        Dim t As Long
        'Dim reg As Regex

        Dim sb As New StringBuilder

        Dim tr As TextReader = File.OpenText(fname)

        Me.txtEntLines.Text = ""
        ents = tr.ReadToEnd

        ents = ents.Replace(ControlChars.CrLf, "")

        tr.Close()

        ' now take the read in file string and lets begin to break it apart
        ' we'll start by looking for the ~ST header section
        ent = Regex.Split(ents, "~ST")
        ' we should now have a two position array ent(0) withh have DPW spluge
        ' ent(1) will have the rest of the file - the ~ST so well add it back in and discard the 
        ' DPW spluge

        ents = ""

        For t = 1 To ent.GetUpperBound(0)
            ents = ents & "ST" & ent(t)
        Next

        ' add the ~ST back
        'ents = "ST" & ent(1)

        ' now lets get the ~SE trailer
        ent = Regex.Split(ents, "~SE")
        ' we should now have a two position array ent(0) has most of the file - the trailing ~
        ' and ent(1) has the SE segment - the SE

        ' lets put it away for future reference
        txt271SEsegment.Text = "SE" & ent(ent.GetUpperBound(0))

        ' lets put the ~ back on the end of the ents string
        ents = ents & "~"

        sb.Capacity = ents.Length

        ent = Regex.Split(ents, "~HL")

        For t = 1 To ent.GetUpperBound(0)

            If t < ent.GetUpperBound(0) Then
                sb.Append("HL" & ent(t) & "~" & vbCrLf)
            Else
                sb.Append("HL" & ent(t))
            End If

        Next

        txt271HLsegemnts.Text = sb.ToString

        txt271STsegment.Text = ent(0) & "~"


    End Sub

    Private Sub btnOpenHipaaFile_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnOpenHipaaFile.Click
        dlg.InitialDirectory = System.Environment.CurrentDirectory
        dlg.Multiselect = False
        If dlg.ShowDialog = DialogResult.OK Then
            ' we are looking at a file
            txtFilename.Text = dlg.FileName
            DecodeFile(dlg.FileName)

        End If
    End Sub

    Private Sub btnOpen834File_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnOpen834File.Click
        dlg.InitialDirectory = System.Environment.CurrentDirectory
        dlg.Multiselect = False
        If dlg.ShowDialog = DialogResult.OK Then
            ' we are looking at a file
            txt834Filename.Text = dlg.FileName
            DecodeFile834(dlg.FileName)

        End If
    End Sub

    Private Sub btnSmallerFonts_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSmallerFonts.Click
        Dim fnt As System.Drawing.Font

        Dim siz As Single
        fnt = Me.txtSTSegment.Font
        siz = fnt.Size

        If siz < 7 Then
            Exit Sub
        End If

        siz = siz - 1

        fnt = New System.Drawing.Font("Courier New", siz, FontStyle.Bold, GraphicsUnit.Point)

        Me.txtEntLines.Font = fnt
        Me.txtSEsegment.Font = fnt
        Me.txtSTSegment.Font = fnt
        Me.txt834DTPsegment.Font = fnt
        Me.txt834SEsegment.Font = fnt
        Me.txt834STsegment.Font = fnt
        Me.txt835CLPsegment.Font = fnt
        Me.txt835SEsegment.Font = fnt
        Me.txt835STsegment.Font = fnt
        Me.txt271HLsegemnts.Font = fnt
        Me.txt271SEsegment.Font = fnt
        Me.txt271STsegment.Font = fnt
        Me.txt837EntLines.Font = fnt
        Me.txt837SESegment.Font = fnt
        Me.txt837stsegment.Font = fnt
        Me.txt277EntsSegment.Font = fnt
        Me.txt277SESegment.Font = fnt
        Me.txt277StSegment.Font = fnt



    End Sub

    Private Sub btnBiggerFonts_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnBiggerFonts.Click
        Dim fnt As System.Drawing.Font

        Dim siz As Single
        fnt = Me.txtSTSegment.Font
        siz = fnt.Size

        If siz > 30 Then
            Exit Sub
        End If

        siz = siz + 1

        fnt = New System.Drawing.Font("Courier New", siz, FontStyle.Bold, GraphicsUnit.Point)

        Me.txtEntLines.Font = fnt
        Me.txtSEsegment.Font = fnt
        Me.txtSTSegment.Font = fnt
        Me.txt834DTPsegment.Font = fnt
        Me.txt834SEsegment.Font = fnt
        Me.txt834STsegment.Font = fnt
        Me.txt835CLPsegment.Font = fnt
        Me.txt835SEsegment.Font = fnt
        Me.txt835STsegment.Font = fnt
        Me.txt271HLsegemnts.Font = fnt
        Me.txt271SEsegment.Font = fnt
        Me.txt271STsegment.Font = fnt
        Me.txt837EntLines.Font = fnt
        Me.txt837SESegment.Font = fnt
        Me.txt837stsegment.Font = fnt
        Me.txt277EntsSegment.Font = fnt
        Me.txt277SESegment.Font = fnt
        Me.txt277StSegment.Font = fnt


    End Sub

    Private Sub TabPage1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TabPage1.Click
        ' I edited this and placed this comment here for Tabpage1_click
    End Sub

    Private Sub btnOpen835_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnOpen835.Click
        dlg.InitialDirectory = System.Environment.CurrentDirectory
        dlg.Multiselect = False
        If dlg.ShowDialog = DialogResult.OK Then
            ' we are looking at a file
            txt835Filename.Text = dlg.FileName
            DecodeFile835(dlg.FileName)

        End If
    End Sub

    Private Sub btnOpen271_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnOpen271.Click
        dlg.InitialDirectory = System.Environment.CurrentDirectory
        dlg.Multiselect = False
        If dlg.ShowDialog = DialogResult.OK Then
            ' we are looking at a file
            txt271Filename.Text = dlg.FileName
            DecodeFile271(dlg.FileName)
        End If
    End Sub

    Private Sub btnOpen837File_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnOpen837File.Click
        dlg.InitialDirectory = System.Environment.CurrentDirectory
        dlg.Multiselect = False
        If dlg.ShowDialog = DialogResult.OK Then
            ' we are looking at a file
            txt837Filename.Text = dlg.FileName
            Decode837File(dlg.FileName)

        End If
    End Sub

    Private Sub btnOpen277File_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnOpen277File.Click
        dlg.InitialDirectory = System.Environment.CurrentDirectory
        dlg.Multiselect = False
        If dlg.ShowDialog = DialogResult.OK Then
            ' we are looking at a file
            txt277filename.Text = dlg.FileName
            Decode277File(dlg.FileName)

        End If
    End Sub

End Class
