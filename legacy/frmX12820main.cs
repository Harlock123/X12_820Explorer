using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Microsoft.VisualBasic;

namespace X12_820Explorer
{

    public class frmX12820main : Form
    {

        #region  Windows Form Designer generated code 

        public frmX12820main() : base()
        {
            btnOpenHipaaFile.Click += btnOpenHipaaFile_Click;
            btnOpen834File.Click += btnOpen834File_Click;
            btnSmallerFonts.Click += btnSmallerFonts_Click;
            btnBiggerFonts.Click += btnBiggerFonts_Click;
            TabPage1.Click += TabPage1_Click;
            btnOpen835.Click += btnOpen835_Click;
            btnOpen271.Click += btnOpen271_Click;
            btnOpen837File.Click += btnOpen837File_Click;
            btnOpen277File.Click += btnOpen277File_Click;

            // This call is required by the Windows Form Designer.
            InitializeComponent();

            // Add any initialization after the InitializeComponent() call

        }

        // Form overrides dispose to clean up the component list.
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components is not null)
                {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }

        // Required by the Windows Form Designer
        private System.ComponentModel.IContainer components;

        // NOTE: The following procedure is required by the Windows Form Designer
        // It can be modified using the Windows Form Designer.  
        // Do not modify it using the code editor.
        private TextBox _txtEntLines;

        internal virtual TextBox txtEntLines
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get
            {
                return _txtEntLines;
            }

            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                _txtEntLines = value;
            }
        }
        private Button _btnOpenHipaaFile;

        internal virtual Button btnOpenHipaaFile
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get
            {
                return _btnOpenHipaaFile;
            }

            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                if (_btnOpenHipaaFile != null)
                {
                    _btnOpenHipaaFile.Click -= btnOpenHipaaFile_Click;
                }

                _btnOpenHipaaFile = value;
                if (_btnOpenHipaaFile != null)
                {
                    _btnOpenHipaaFile.Click += btnOpenHipaaFile_Click;
                }
            }
        }
        private TextBox _txtFilename;

        internal virtual TextBox txtFilename
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get
            {
                return _txtFilename;
            }

            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                _txtFilename = value;
            }
        }
        private OpenFileDialog _dlg;

        internal virtual OpenFileDialog dlg
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get
            {
                return _dlg;
            }

            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                _dlg = value;
            }
        }
        private TextBox _txtSTSegment;

        internal virtual TextBox txtSTSegment
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get
            {
                return _txtSTSegment;
            }

            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                _txtSTSegment = value;
            }
        }
        private TextBox _txtSEsegment;

        internal virtual TextBox txtSEsegment
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get
            {
                return _txtSEsegment;
            }

            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                _txtSEsegment = value;
            }
        }
        private TabControl _TabControl1;

        internal virtual TabControl TabControl1
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get
            {
                return _TabControl1;
            }

            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                _TabControl1 = value;
            }
        }
        private TabPage _TabPage1;

        internal virtual TabPage TabPage1
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get
            {
                return _TabPage1;
            }

            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                if (_TabPage1 != null)
                {
                    _TabPage1.Click -= TabPage1_Click;
                }

                _TabPage1 = value;
                if (_TabPage1 != null)
                {
                    _TabPage1.Click += TabPage1_Click;
                }
            }
        }
        private TabPage _TabPage2;

        internal virtual TabPage TabPage2
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get
            {
                return _TabPage2;
            }

            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                _TabPage2 = value;
            }
        }
        private Button _btnOpen834File;

        internal virtual Button btnOpen834File
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get
            {
                return _btnOpen834File;
            }

            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                if (_btnOpen834File != null)
                {
                    _btnOpen834File.Click -= btnOpen834File_Click;
                }

                _btnOpen834File = value;
                if (_btnOpen834File != null)
                {
                    _btnOpen834File.Click += btnOpen834File_Click;
                }
            }
        }
        private TextBox _txt834SEsegment;

        internal virtual TextBox txt834SEsegment
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get
            {
                return _txt834SEsegment;
            }

            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                _txt834SEsegment = value;
            }
        }
        private TextBox _txt834DTPsegment;

        internal virtual TextBox txt834DTPsegment
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get
            {
                return _txt834DTPsegment;
            }

            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                _txt834DTPsegment = value;
            }
        }
        private TextBox _txt834STsegment;

        internal virtual TextBox txt834STsegment
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get
            {
                return _txt834STsegment;
            }

            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                _txt834STsegment = value;
            }
        }
        private TextBox _txt834Filename;

        internal virtual TextBox txt834Filename
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get
            {
                return _txt834Filename;
            }

            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                _txt834Filename = value;
            }
        }
        private Button _btnSmallerFonts;

        internal virtual Button btnSmallerFonts
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get
            {
                return _btnSmallerFonts;
            }

            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                if (_btnSmallerFonts != null)
                {
                    _btnSmallerFonts.Click -= btnSmallerFonts_Click;
                }

                _btnSmallerFonts = value;
                if (_btnSmallerFonts != null)
                {
                    _btnSmallerFonts.Click += btnSmallerFonts_Click;
                }
            }
        }
        private Button _btnBiggerFonts;

        internal virtual Button btnBiggerFonts
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get
            {
                return _btnBiggerFonts;
            }

            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                if (_btnBiggerFonts != null)
                {
                    _btnBiggerFonts.Click -= btnBiggerFonts_Click;
                }

                _btnBiggerFonts = value;
                if (_btnBiggerFonts != null)
                {
                    _btnBiggerFonts.Click += btnBiggerFonts_Click;
                }
            }
        }
        private TabPage _TabPage3;

        internal virtual TabPage TabPage3
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get
            {
                return _TabPage3;
            }

            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                _TabPage3 = value;
            }
        }
        private Button _btnOpen835;

        internal virtual Button btnOpen835
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get
            {
                return _btnOpen835;
            }

            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                if (_btnOpen835 != null)
                {
                    _btnOpen835.Click -= btnOpen835_Click;
                }

                _btnOpen835 = value;
                if (_btnOpen835 != null)
                {
                    _btnOpen835.Click += btnOpen835_Click;
                }
            }
        }
        private TextBox _txt835SEsegment;

        internal virtual TextBox txt835SEsegment
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get
            {
                return _txt835SEsegment;
            }

            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                _txt835SEsegment = value;
            }
        }
        private TextBox _txt835CLPsegment;

        internal virtual TextBox txt835CLPsegment
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get
            {
                return _txt835CLPsegment;
            }

            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                _txt835CLPsegment = value;
            }
        }
        private TextBox _txt835STsegment;

        internal virtual TextBox txt835STsegment
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get
            {
                return _txt835STsegment;
            }

            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                _txt835STsegment = value;
            }
        }
        private TextBox _txt835Filename;

        internal virtual TextBox txt835Filename
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get
            {
                return _txt835Filename;
            }

            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                _txt835Filename = value;
            }
        }
        private TabPage _TabPage4;

        internal virtual TabPage TabPage4
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get
            {
                return _TabPage4;
            }

            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                _TabPage4 = value;
            }
        }
        private Button _btnOpen271;

        internal virtual Button btnOpen271
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get
            {
                return _btnOpen271;
            }

            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                if (_btnOpen271 != null)
                {
                    _btnOpen271.Click -= btnOpen271_Click;
                }

                _btnOpen271 = value;
                if (_btnOpen271 != null)
                {
                    _btnOpen271.Click += btnOpen271_Click;
                }
            }
        }
        private TextBox _txt271SEsegment;

        internal virtual TextBox txt271SEsegment
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get
            {
                return _txt271SEsegment;
            }

            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                _txt271SEsegment = value;
            }
        }
        private TextBox _txt271HLsegemnts;

        internal virtual TextBox txt271HLsegemnts
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get
            {
                return _txt271HLsegemnts;
            }

            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                _txt271HLsegemnts = value;
            }
        }
        private TextBox _txt271STsegment;

        internal virtual TextBox txt271STsegment
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get
            {
                return _txt271STsegment;
            }

            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                _txt271STsegment = value;
            }
        }
        private TextBox _txt271Filename;

        internal virtual TextBox txt271Filename
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get
            {
                return _txt271Filename;
            }

            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                _txt271Filename = value;
            }
        }
        private TabPage _TabPage5;

        internal virtual TabPage TabPage5
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get
            {
                return _TabPage5;
            }

            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                _TabPage5 = value;
            }
        }
        private Button _btnOpen837File;

        internal virtual Button btnOpen837File
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get
            {
                return _btnOpen837File;
            }

            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                if (_btnOpen837File != null)
                {
                    _btnOpen837File.Click -= btnOpen837File_Click;
                }

                _btnOpen837File = value;
                if (_btnOpen837File != null)
                {
                    _btnOpen837File.Click += btnOpen837File_Click;
                }
            }
        }
        private TextBox _txt837SESegment;

        internal virtual TextBox txt837SESegment
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get
            {
                return _txt837SESegment;
            }

            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                _txt837SESegment = value;
            }
        }
        private TextBox _txt837EntLines;

        internal virtual TextBox txt837EntLines
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get
            {
                return _txt837EntLines;
            }

            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                _txt837EntLines = value;
            }
        }
        private TextBox _txt837stsegment;

        internal virtual TextBox txt837stsegment
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get
            {
                return _txt837stsegment;
            }

            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                _txt837stsegment = value;
            }
        }
        private TextBox _txt837Filename;

        internal virtual TextBox txt837Filename
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get
            {
                return _txt837Filename;
            }

            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                _txt837Filename = value;
            }
        }
        private TabPage _TabPage6;

        internal virtual TabPage TabPage6
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get
            {
                return _TabPage6;
            }

            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                _TabPage6 = value;
            }
        }
        private Button _btnOpen277File;

        internal virtual Button btnOpen277File
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get
            {
                return _btnOpen277File;
            }

            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                if (_btnOpen277File != null)
                {
                    _btnOpen277File.Click -= btnOpen277File_Click;
                }

                _btnOpen277File = value;
                if (_btnOpen277File != null)
                {
                    _btnOpen277File.Click += btnOpen277File_Click;
                }
            }
        }
        private TextBox _txt277SESegment;

        internal virtual TextBox txt277SESegment
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get
            {
                return _txt277SESegment;
            }

            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                _txt277SESegment = value;
            }
        }
        private TextBox _txt277EntsSegment;

        internal virtual TextBox txt277EntsSegment
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get
            {
                return _txt277EntsSegment;
            }

            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                _txt277EntsSegment = value;
            }
        }
        private TextBox _txt277StSegment;

        internal virtual TextBox txt277StSegment
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get
            {
                return _txt277StSegment;
            }

            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                _txt277StSegment = value;
            }
        }
        private TextBox _txt277filename;

        internal virtual TextBox txt277filename
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get
            {
                return _txt277filename;
            }

            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                _txt277filename = value;
            }
        }
        [DebuggerStepThrough()]
        private void InitializeComponent()
        {
            _txtEntLines = new TextBox();
            _btnOpenHipaaFile = new Button();
            _btnOpenHipaaFile.Click += new EventHandler(btnOpenHipaaFile_Click);
            _txtFilename = new TextBox();
            _dlg = new OpenFileDialog();
            _txtSTSegment = new TextBox();
            _txtSEsegment = new TextBox();
            _TabControl1 = new TabControl();
            _TabPage1 = new TabPage();
            _TabPage1.Click += new EventHandler(TabPage1_Click);
            _TabPage2 = new TabPage();
            _btnOpen834File = new Button();
            _btnOpen834File.Click += new EventHandler(btnOpen834File_Click);
            _txt834SEsegment = new TextBox();
            _txt834DTPsegment = new TextBox();
            _txt834STsegment = new TextBox();
            _txt834Filename = new TextBox();
            _TabPage3 = new TabPage();
            _btnOpen835 = new Button();
            _btnOpen835.Click += new EventHandler(btnOpen835_Click);
            _txt835SEsegment = new TextBox();
            _txt835CLPsegment = new TextBox();
            _txt835STsegment = new TextBox();
            _txt835Filename = new TextBox();
            _TabPage4 = new TabPage();
            _btnOpen271 = new Button();
            _btnOpen271.Click += new EventHandler(btnOpen271_Click);
            _txt271SEsegment = new TextBox();
            _txt271HLsegemnts = new TextBox();
            _txt271STsegment = new TextBox();
            _txt271Filename = new TextBox();
            _TabPage5 = new TabPage();
            _btnOpen837File = new Button();
            _btnOpen837File.Click += new EventHandler(btnOpen837File_Click);
            _txt837SESegment = new TextBox();
            _txt837EntLines = new TextBox();
            _txt837stsegment = new TextBox();
            _txt837Filename = new TextBox();
            _TabPage6 = new TabPage();
            _btnOpen277File = new Button();
            _btnOpen277File.Click += new EventHandler(btnOpen277File_Click);
            _txt277SESegment = new TextBox();
            _txt277EntsSegment = new TextBox();
            _txt277StSegment = new TextBox();
            _txt277filename = new TextBox();
            _btnSmallerFonts = new Button();
            _btnSmallerFonts.Click += new EventHandler(btnSmallerFonts_Click);
            _btnBiggerFonts = new Button();
            _btnBiggerFonts.Click += new EventHandler(btnBiggerFonts_Click);
            _TabControl1.SuspendLayout();
            _TabPage1.SuspendLayout();
            _TabPage2.SuspendLayout();
            _TabPage3.SuspendLayout();
            _TabPage4.SuspendLayout();
            _TabPage5.SuspendLayout();
            _TabPage6.SuspendLayout();
            SuspendLayout();
            // 
            // txtEntLines
            // 
            _txtEntLines.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;

            _txtEntLines.BackColor = Color.MistyRose;
            _txtEntLines.Font = new Font("Courier New", 8.25f, FontStyle.Bold, GraphicsUnit.Point, 0);
            _txtEntLines.Location = new Point(8, 92);
            _txtEntLines.Multiline = true;
            _txtEntLines.Name = "_txtEntLines";
            _txtEntLines.ScrollBars = ScrollBars.Both;
            _txtEntLines.Size = new Size(764, 405);
            _txtEntLines.TabIndex = 0;
            _txtEntLines.WordWrap = false;
            // 
            // btnOpenHipaaFile
            // 
            _btnOpenHipaaFile.Location = new Point(4, 4);
            _btnOpenHipaaFile.Name = "_btnOpenHipaaFile";
            _btnOpenHipaaFile.Size = new Size(48, 20);
            _btnOpenHipaaFile.TabIndex = 1;
            _btnOpenHipaaFile.Text = "Open";
            // 
            // txtFilename
            // 
            _txtFilename.Location = new Point(68, 4);
            _txtFilename.Name = "_txtFilename";
            _txtFilename.Size = new Size(448, 18);
            _txtFilename.TabIndex = 2;
            // 
            // txtSTSegment
            // 
            _txtSTSegment.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            _txtSTSegment.BackColor = Color.RosyBrown;
            _txtSTSegment.Font = new Font("Courier New", 8.25f, FontStyle.Bold, GraphicsUnit.Point, 0);
            _txtSTSegment.Location = new Point(8, 28);
            _txtSTSegment.Multiline = true;
            _txtSTSegment.Name = "_txtSTSegment";
            _txtSTSegment.ScrollBars = ScrollBars.Vertical;
            _txtSTSegment.Size = new Size(764, 64);
            _txtSTSegment.TabIndex = 3;
            // 
            // txtSEsegment
            // 
            _txtSEsegment.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            _txtSEsegment.BackColor = Color.RosyBrown;
            _txtSEsegment.Font = new Font("Courier New", 8.25f, FontStyle.Bold, GraphicsUnit.Point, 0);
            _txtSEsegment.Location = new Point(8, 501);
            _txtSEsegment.Multiline = true;
            _txtSEsegment.Name = "_txtSEsegment";
            _txtSEsegment.ScrollBars = ScrollBars.Vertical;
            _txtSEsegment.Size = new Size(764, 32);
            _txtSEsegment.TabIndex = 4;
            // 
            // TabControl1
            // 
            _TabControl1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;

            _TabControl1.Controls.Add(_TabPage1);
            _TabControl1.Controls.Add(_TabPage2);
            _TabControl1.Controls.Add(_TabPage3);
            _TabControl1.Controls.Add(_TabPage4);
            _TabControl1.Controls.Add(_TabPage5);
            _TabControl1.Controls.Add(_TabPage6);
            _TabControl1.Location = new Point(4, 12);
            _TabControl1.Name = "_TabControl1";
            _TabControl1.SelectedIndex = 0;
            _TabControl1.Size = new Size(784, 556);
            _TabControl1.TabIndex = 5;
            // 
            // TabPage1
            // 
            _TabPage1.BackColor = SystemColors.ControlDark;
            _TabPage1.Controls.Add(_btnOpenHipaaFile);
            _TabPage1.Controls.Add(_txtSEsegment);
            _TabPage1.Controls.Add(_txtEntLines);
            _TabPage1.Controls.Add(_txtSTSegment);
            _TabPage1.Controls.Add(_txtFilename);
            _TabPage1.Location = new Point(4, 21);
            _TabPage1.Name = "_TabPage1";
            _TabPage1.Size = new Size(776, 531);
            _TabPage1.TabIndex = 0;
            _TabPage1.Text = "Capitation (820)";
            // 
            // TabPage2
            // 
            _TabPage2.BackColor = SystemColors.ControlDark;
            _TabPage2.Controls.Add(_btnOpen834File);
            _TabPage2.Controls.Add(_txt834SEsegment);
            _TabPage2.Controls.Add(_txt834DTPsegment);
            _TabPage2.Controls.Add(_txt834STsegment);
            _TabPage2.Controls.Add(_txt834Filename);
            _TabPage2.Location = new Point(4, 22);
            _TabPage2.Name = "_TabPage2";
            _TabPage2.Size = new Size(776, 530);
            _TabPage2.TabIndex = 1;
            _TabPage2.Text = "Membership (834)";
            // 
            // btnOpen834File
            // 
            _btnOpen834File.Location = new Point(4, 4);
            _btnOpen834File.Name = "_btnOpen834File";
            _btnOpen834File.Size = new Size(48, 20);
            _btnOpen834File.TabIndex = 6;
            _btnOpen834File.Text = "Open";
            // 
            // txt834SEsegment
            // 
            _txt834SEsegment.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            _txt834SEsegment.BackColor = Color.RosyBrown;
            _txt834SEsegment.Font = new Font("Courier New", 8.25f, FontStyle.Bold, GraphicsUnit.Point, 0);
            _txt834SEsegment.Location = new Point(8, 496);
            _txt834SEsegment.Multiline = true;
            _txt834SEsegment.Name = "_txt834SEsegment";
            _txt834SEsegment.ScrollBars = ScrollBars.Vertical;
            _txt834SEsegment.Size = new Size(764, 32);
            _txt834SEsegment.TabIndex = 9;
            // 
            // txt834DTPsegment
            // 
            _txt834DTPsegment.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;

            _txt834DTPsegment.BackColor = Color.MistyRose;
            _txt834DTPsegment.Font = new Font("Courier New", 8.25f, FontStyle.Bold, GraphicsUnit.Point, 0);
            _txt834DTPsegment.Location = new Point(8, 92);
            _txt834DTPsegment.Multiline = true;
            _txt834DTPsegment.Name = "_txt834DTPsegment";
            _txt834DTPsegment.ScrollBars = ScrollBars.Both;
            _txt834DTPsegment.Size = new Size(764, 400);
            _txt834DTPsegment.TabIndex = 5;
            _txt834DTPsegment.WordWrap = false;
            // 
            // txt834STsegment
            // 
            _txt834STsegment.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            _txt834STsegment.BackColor = Color.RosyBrown;
            _txt834STsegment.Font = new Font("Courier New", 8.25f, FontStyle.Bold, GraphicsUnit.Point, 0);
            _txt834STsegment.Location = new Point(8, 28);
            _txt834STsegment.Multiline = true;
            _txt834STsegment.Name = "_txt834STsegment";
            _txt834STsegment.ScrollBars = ScrollBars.Vertical;
            _txt834STsegment.Size = new Size(764, 64);
            _txt834STsegment.TabIndex = 8;
            // 
            // txt834Filename
            // 
            _txt834Filename.Location = new Point(68, 4);
            _txt834Filename.Name = "_txt834Filename";
            _txt834Filename.Size = new Size(448, 18);
            _txt834Filename.TabIndex = 7;
            // 
            // TabPage3
            // 
            _TabPage3.BackColor = SystemColors.ControlDark;
            _TabPage3.Controls.Add(_btnOpen835);
            _TabPage3.Controls.Add(_txt835SEsegment);
            _TabPage3.Controls.Add(_txt835CLPsegment);
            _TabPage3.Controls.Add(_txt835STsegment);
            _TabPage3.Controls.Add(_txt835Filename);
            _TabPage3.Location = new Point(4, 22);
            _TabPage3.Name = "_TabPage3";
            _TabPage3.Size = new Size(776, 530);
            _TabPage3.TabIndex = 2;
            _TabPage3.Text = "Remittance (835)";
            // 
            // btnOpen835
            // 
            _btnOpen835.Location = new Point(4, 3);
            _btnOpen835.Name = "_btnOpen835";
            _btnOpen835.Size = new Size(48, 20);
            _btnOpen835.TabIndex = 11;
            _btnOpen835.Text = "Open";
            // 
            // txt835SEsegment
            // 
            _txt835SEsegment.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            _txt835SEsegment.BackColor = Color.RosyBrown;
            _txt835SEsegment.Font = new Font("Courier New", 8.25f, FontStyle.Bold, GraphicsUnit.Point, 0);
            _txt835SEsegment.Location = new Point(8, 495);
            _txt835SEsegment.Multiline = true;
            _txt835SEsegment.Name = "_txt835SEsegment";
            _txt835SEsegment.ScrollBars = ScrollBars.Vertical;
            _txt835SEsegment.Size = new Size(764, 32);
            _txt835SEsegment.TabIndex = 14;
            // 
            // txt835CLPsegment
            // 
            _txt835CLPsegment.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;

            _txt835CLPsegment.BackColor = Color.MistyRose;
            _txt835CLPsegment.Font = new Font("Courier New", 8.25f, FontStyle.Bold, GraphicsUnit.Point, 0);
            _txt835CLPsegment.Location = new Point(8, 91);
            _txt835CLPsegment.Multiline = true;
            _txt835CLPsegment.Name = "_txt835CLPsegment";
            _txt835CLPsegment.ScrollBars = ScrollBars.Both;
            _txt835CLPsegment.Size = new Size(764, 400);
            _txt835CLPsegment.TabIndex = 10;
            _txt835CLPsegment.WordWrap = false;
            // 
            // txt835STsegment
            // 
            _txt835STsegment.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            _txt835STsegment.BackColor = Color.RosyBrown;
            _txt835STsegment.Font = new Font("Courier New", 8.25f, FontStyle.Bold, GraphicsUnit.Point, 0);
            _txt835STsegment.Location = new Point(8, 27);
            _txt835STsegment.Multiline = true;
            _txt835STsegment.Name = "_txt835STsegment";
            _txt835STsegment.ScrollBars = ScrollBars.Vertical;
            _txt835STsegment.Size = new Size(764, 64);
            _txt835STsegment.TabIndex = 13;
            // 
            // txt835Filename
            // 
            _txt835Filename.Location = new Point(68, 3);
            _txt835Filename.Name = "_txt835Filename";
            _txt835Filename.Size = new Size(448, 18);
            _txt835Filename.TabIndex = 12;
            // 
            // TabPage4
            // 
            _TabPage4.BackColor = SystemColors.ControlDark;
            _TabPage4.Controls.Add(_btnOpen271);
            _TabPage4.Controls.Add(_txt271SEsegment);
            _TabPage4.Controls.Add(_txt271HLsegemnts);
            _TabPage4.Controls.Add(_txt271STsegment);
            _TabPage4.Controls.Add(_txt271Filename);
            _TabPage4.Location = new Point(4, 22);
            _TabPage4.Name = "_TabPage4";
            _TabPage4.Size = new Size(776, 530);
            _TabPage4.TabIndex = 3;
            _TabPage4.Text = "Benefit Info (271)";
            // 
            // btnOpen271
            // 
            _btnOpen271.Location = new Point(4, 3);
            _btnOpen271.Name = "_btnOpen271";
            _btnOpen271.Size = new Size(48, 20);
            _btnOpen271.TabIndex = 16;
            _btnOpen271.Text = "Open";
            // 
            // txt271SEsegment
            // 
            _txt271SEsegment.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            _txt271SEsegment.BackColor = Color.RosyBrown;
            _txt271SEsegment.Font = new Font("Courier New", 8.25f, FontStyle.Bold, GraphicsUnit.Point, 0);
            _txt271SEsegment.Location = new Point(8, 495);
            _txt271SEsegment.Multiline = true;
            _txt271SEsegment.Name = "_txt271SEsegment";
            _txt271SEsegment.ScrollBars = ScrollBars.Vertical;
            _txt271SEsegment.Size = new Size(764, 32);
            _txt271SEsegment.TabIndex = 19;
            // 
            // txt271HLsegemnts
            // 
            _txt271HLsegemnts.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;

            _txt271HLsegemnts.BackColor = Color.MistyRose;
            _txt271HLsegemnts.Font = new Font("Courier New", 8.25f, FontStyle.Bold, GraphicsUnit.Point, 0);
            _txt271HLsegemnts.Location = new Point(8, 91);
            _txt271HLsegemnts.Multiline = true;
            _txt271HLsegemnts.Name = "_txt271HLsegemnts";
            _txt271HLsegemnts.ScrollBars = ScrollBars.Both;
            _txt271HLsegemnts.Size = new Size(764, 400);
            _txt271HLsegemnts.TabIndex = 15;
            _txt271HLsegemnts.WordWrap = false;
            // 
            // txt271STsegment
            // 
            _txt271STsegment.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            _txt271STsegment.BackColor = Color.RosyBrown;
            _txt271STsegment.Font = new Font("Courier New", 8.25f, FontStyle.Bold, GraphicsUnit.Point, 0);
            _txt271STsegment.Location = new Point(8, 27);
            _txt271STsegment.Multiline = true;
            _txt271STsegment.Name = "_txt271STsegment";
            _txt271STsegment.ScrollBars = ScrollBars.Vertical;
            _txt271STsegment.Size = new Size(764, 64);
            _txt271STsegment.TabIndex = 18;
            // 
            // txt271Filename
            // 
            _txt271Filename.Location = new Point(68, 3);
            _txt271Filename.Name = "_txt271Filename";
            _txt271Filename.Size = new Size(448, 18);
            _txt271Filename.TabIndex = 17;
            // 
            // TabPage5
            // 
            _TabPage5.BackColor = SystemColors.ControlDark;
            _TabPage5.Controls.Add(_btnOpen837File);
            _TabPage5.Controls.Add(_txt837SESegment);
            _TabPage5.Controls.Add(_txt837EntLines);
            _TabPage5.Controls.Add(_txt837stsegment);
            _TabPage5.Controls.Add(_txt837Filename);
            _TabPage5.Location = new Point(4, 22);
            _TabPage5.Name = "_TabPage5";
            _TabPage5.Size = new Size(776, 530);
            _TabPage5.TabIndex = 4;
            _TabPage5.Text = "Claim (837)";
            // 
            // btnOpen837File
            // 
            _btnOpen837File.Location = new Point(4, 3);
            _btnOpen837File.Name = "_btnOpen837File";
            _btnOpen837File.Size = new Size(48, 20);
            _btnOpen837File.TabIndex = 11;
            _btnOpen837File.Text = "Open";
            // 
            // txt837SESegment
            // 
            _txt837SESegment.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            _txt837SESegment.BackColor = Color.RosyBrown;
            _txt837SESegment.Font = new Font("Courier New", 8.25f, FontStyle.Bold, GraphicsUnit.Point, 0);
            _txt837SESegment.Location = new Point(8, 497);
            _txt837SESegment.Multiline = true;
            _txt837SESegment.Name = "_txt837SESegment";
            _txt837SESegment.ScrollBars = ScrollBars.Vertical;
            _txt837SESegment.Size = new Size(764, 32);
            _txt837SESegment.TabIndex = 14;
            // 
            // txt837EntLines
            // 
            _txt837EntLines.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;

            _txt837EntLines.BackColor = Color.MistyRose;
            _txt837EntLines.Font = new Font("Courier New", 8.25f, FontStyle.Bold, GraphicsUnit.Point, 0);
            _txt837EntLines.Location = new Point(8, 91);
            _txt837EntLines.Multiline = true;
            _txt837EntLines.Name = "_txt837EntLines";
            _txt837EntLines.ScrollBars = ScrollBars.Both;
            _txt837EntLines.Size = new Size(764, 402);
            _txt837EntLines.TabIndex = 10;
            _txt837EntLines.WordWrap = false;
            // 
            // txt837stsegment
            // 
            _txt837stsegment.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            _txt837stsegment.BackColor = Color.RosyBrown;
            _txt837stsegment.Font = new Font("Courier New", 8.25f, FontStyle.Bold, GraphicsUnit.Point, 0);
            _txt837stsegment.Location = new Point(8, 27);
            _txt837stsegment.Multiline = true;
            _txt837stsegment.Name = "_txt837stsegment";
            _txt837stsegment.ScrollBars = ScrollBars.Vertical;
            _txt837stsegment.Size = new Size(764, 64);
            _txt837stsegment.TabIndex = 13;
            // 
            // txt837Filename
            // 
            _txt837Filename.Location = new Point(68, 3);
            _txt837Filename.Name = "_txt837Filename";
            _txt837Filename.Size = new Size(448, 18);
            _txt837Filename.TabIndex = 12;
            // 
            // TabPage6
            // 
            _TabPage6.BackColor = SystemColors.ControlDark;
            _TabPage6.Controls.Add(_btnOpen277File);
            _TabPage6.Controls.Add(_txt277SESegment);
            _TabPage6.Controls.Add(_txt277EntsSegment);
            _TabPage6.Controls.Add(_txt277StSegment);
            _TabPage6.Controls.Add(_txt277filename);
            _TabPage6.Location = new Point(4, 21);
            _TabPage6.Name = "_TabPage6";
            _TabPage6.Size = new Size(776, 531);
            _TabPage6.TabIndex = 5;
            _TabPage6.Text = "Claim Status Response (277)";
            // 
            // btnOpen277File
            // 
            _btnOpen277File.Location = new Point(4, 3);
            _btnOpen277File.Name = "_btnOpen277File";
            _btnOpen277File.Size = new Size(48, 20);
            _btnOpen277File.TabIndex = 16;
            _btnOpen277File.Text = "Open";
            // 
            // txt277SESegment
            // 
            _txt277SESegment.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            _txt277SESegment.BackColor = Color.RosyBrown;
            _txt277SESegment.Font = new Font("Courier New", 8.25f, FontStyle.Bold, GraphicsUnit.Point, 0);
            _txt277SESegment.Location = new Point(8, 498);
            _txt277SESegment.Multiline = true;
            _txt277SESegment.Name = "_txt277SESegment";
            _txt277SESegment.ScrollBars = ScrollBars.Vertical;
            _txt277SESegment.Size = new Size(764, 32);
            _txt277SESegment.TabIndex = 19;
            // 
            // txt277EntsSegment
            // 
            _txt277EntsSegment.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;

            _txt277EntsSegment.BackColor = Color.MistyRose;
            _txt277EntsSegment.Font = new Font("Courier New", 8.25f, FontStyle.Bold, GraphicsUnit.Point, 0);
            _txt277EntsSegment.Location = new Point(8, 91);
            _txt277EntsSegment.Multiline = true;
            _txt277EntsSegment.Name = "_txt277EntsSegment";
            _txt277EntsSegment.ScrollBars = ScrollBars.Both;
            _txt277EntsSegment.Size = new Size(764, 403);
            _txt277EntsSegment.TabIndex = 15;
            _txt277EntsSegment.WordWrap = false;
            // 
            // txt277StSegment
            // 
            _txt277StSegment.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            _txt277StSegment.BackColor = Color.RosyBrown;
            _txt277StSegment.Font = new Font("Courier New", 8.25f, FontStyle.Bold, GraphicsUnit.Point, 0);
            _txt277StSegment.Location = new Point(8, 27);
            _txt277StSegment.Multiline = true;
            _txt277StSegment.Name = "_txt277StSegment";
            _txt277StSegment.ScrollBars = ScrollBars.Vertical;
            _txt277StSegment.Size = new Size(764, 64);
            _txt277StSegment.TabIndex = 18;
            // 
            // txt277filename
            // 
            _txt277filename.Location = new Point(68, 3);
            _txt277filename.Name = "_txt277filename";
            _txt277filename.Size = new Size(448, 18);
            _txt277filename.TabIndex = 17;
            // 
            // btnSmallerFonts
            // 
            _btnSmallerFonts.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            _btnSmallerFonts.Location = new Point(640, 0);
            _btnSmallerFonts.Name = "_btnSmallerFonts";
            _btnSmallerFonts.Size = new Size(72, 20);
            _btnSmallerFonts.TabIndex = 6;
            _btnSmallerFonts.Text = "Fonts Smaller";
            // 
            // btnBiggerFonts
            // 
            _btnBiggerFonts.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            _btnBiggerFonts.Location = new Point(716, 0);
            _btnBiggerFonts.Name = "_btnBiggerFonts";
            _btnBiggerFonts.Size = new Size(72, 20);
            _btnBiggerFonts.TabIndex = 7;
            _btnBiggerFonts.Text = "Fonts Bigger";
            // 
            // frmX12820main
            // 
            AutoScaleBaseSize = new Size(4, 11);
            ClientSize = new Size(792, 573);
            Controls.Add(_btnBiggerFonts);
            Controls.Add(_btnSmallerFonts);
            Controls.Add(_TabControl1);
            Font = new Font("Microsoft Sans Serif", 6.75f, FontStyle.Regular, GraphicsUnit.Point, 0);
            Name = "frmX12820main";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "X12 820/834/835/837/271/277 Explorer";
            _TabControl1.ResumeLayout(false);
            _TabPage1.ResumeLayout(false);
            _TabPage1.PerformLayout();
            _TabPage2.ResumeLayout(false);
            _TabPage2.PerformLayout();
            _TabPage3.ResumeLayout(false);
            _TabPage3.PerformLayout();
            _TabPage4.ResumeLayout(false);
            _TabPage4.PerformLayout();
            _TabPage5.ResumeLayout(false);
            _TabPage5.PerformLayout();
            _TabPage6.ResumeLayout(false);
            _TabPage6.PerformLayout();
            ResumeLayout(false);

        }

        #endregion

        private void DecodeFile(string fname)
        {

            string ents;
            string[] ent;
            long t;
            // 'Dim reg As Regex

            var sb = new StringBuilder();

            TextReader tr = File.OpenText(fname);

            txtEntLines.Text = "";
            ents = tr.ReadToEnd();

            ents = ents.Replace(ControlChars.CrLf, "");

            tr.Close();

            // now take the read in file string and lets begin to break it apart
            // we'll start by looking for the ~ST header section
            ent = Regex.Split(ents, "~ST");
            // we should now have a two position array ent(0) withh have DPW spluge
            // ent(1) will have the rest of the file - the ~ST so well add it back in and discard the 
            // DPW spluge

            // add the ~ST back
            ents = "ST" + ent[1];

            // now lets get the ~SE trailer
            ent = Regex.Split(ents, "~SE");
            // we should now have a two position array ent(0) has most of the file - the trailing ~
            // and ent(1) has the SE segment - the SE

            // lets put it away for future reference
            txtSEsegment.Text = "SE" + ent[1];

            // lets put the ~ back on the end of the ents string
            ents = ent[0] + "~";

            sb.Capacity = ents.Length;

            ent = Regex.Split(ents, "~ENT");

            var loopTo = (long)ent.GetUpperBound(0);
            for (t = 1L; t <= loopTo; t++)
            {

                if (t < ent.GetUpperBound(0))
                {
                    sb.Append("ENT" + ent[(int)t] + "~" + Constants.vbCrLf);
                }
                else
                {
                    sb.Append("ENT" + ent[(int)t]);
                }

                // sb.Append("ENT" & ent(t) & "~" & vbCrLf)

            }

            txtEntLines.Text = sb.ToString();

            txtSTSegment.Text = ent[0] + "~";


        }

        private void Decode277File(string fname)
        {

            string ents;
            var ent = default(string[]);
            string[] enta;
            long t, tt, ttt;
            // 'Dim reg As Regex

            var sb = new StringBuilder();

            TextReader tr = File.OpenText(fname);

            txtEntLines.Text = "";
            ents = tr.ReadToEnd();

            ents = ents.Replace(ControlChars.CrLf, "");

            tr.Close();

            // now take the read in file string and lets begin to break it apart
            // we'll start by looking for the ~ST header section
            enta = Regex.Split(ents, @"~ST\*");
            // we should now have a two position array ent(0) withh have DPW spluge
            // ent(1) will have the rest of the file - the ~ST so well add it back in and discard the 
            // DPW spluge


            var loopTo = (long)enta.GetUpperBound(0);
            for (ttt = 1L; ttt <= loopTo; ttt++)
            {
                // add the ~ST back
                ents = "ST*" + enta[(int)ttt];

                // now lets get the ~SE trailer
                ent = Regex.Split(ents, @"~SE\*");
                // we should now have a two position array ent(0) has most of the file - the trailing ~
                // and ent(1) has the SE segment - the SE

                // lets put it away for future reference
                txt277SESegment.Text = "SE*" + enta[1];

                // lets put the ~ back on the end of the ents string
                ents = ent[0] + "~";

                // sb.Capacity = ents.Length

                ent = Regex.Split(ents, @"~HL\*");

                var loopTo1 = (long)ent.GetUpperBound(0);
                for (t = 1L; t <= loopTo1; t++)
                {

                    string ss = ent[(int)t];

                    string[] sss = Regex.Split(ss, @"~TRN\*");

                    if (sss.GetUpperBound(0) >= 1)
                    {

                        sb.Append("HL*" + sss[0] + "~" + Constants.vbCrLf);

                        var loopTo2 = (long)sss.GetUpperBound(0);
                        for (tt = 1L; tt <= loopTo2; tt++)
                            sb.Append("   TRN*" + sss[(int)tt] + "~" + Constants.vbCrLf);
                    }
                    else
                    {
                        sb.Append("HL*" + sss[0] + "~" + Constants.vbCrLf);
                    }


                    // Dim sss() As String = Regex.Split(ss, "~STC\*")

                    // If sss.GetUpperBound(0) > 1 Then

                    // sb.Append("HL*" & sss(0) & "~" & vbCrLf)

                    // For tt = 1 To sss.GetUpperBound(0)
                    // sb.Append("   STC*" & sss(tt) & "~" & vbCrLf)
                    // Next
                    // Else
                    // sb.Append("HL*" & sss(0) & "~" & vbCrLf)
                    // End If

                    // If t < ent.GetUpperBound(0) Then
                    // sb.Append("HL*" & ent(t) & "~" & vbCrLf)
                    // Else
                    // sb.Append("HL*" & ent(t))
                    // End If

                    // sb.Append("ENT" & ent(t) & "~" & vbCrLf)

                }
            }

            // ' add the ~ST back
            // ents = "ST*" & enta(1)

            // ' now lets get the ~SE trailer
            // ent = Regex.Split(ents, "~SE\*")
            // ' we should now have a two position array ent(0) has most of the file - the trailing ~
            // ' and ent(1) has the SE segment - the SE

            // ' lets put it away for future reference
            // txt277SESegment.Text = "SE*" & ent(1)

            // ' lets put the ~ back on the end of the ents string
            // ents = ent(0) & "~"

            // sb.Capacity = ents.Length

            // ent = Regex.Split(ents, "~HL\*")

            // For t = 1 To ent.GetUpperBound(0)

            // Dim ss As String = ent(t)

            // Dim sss() As String = Regex.Split(ss, "~STC\*")

            // If sss.GetUpperBound(0) > 1 Then

            // sb.Append("HL*" & sss(0) & "~" & vbCrLf)

            // For tt = 1 To sss.GetUpperBound(0)
            // sb.Append("   STC*" & sss(tt) & "~" & vbCrLf)
            // Next
            // Else
            // sb.Append("HL*" & sss(0) & "~" & vbCrLf)
            // End If

            // 'If t < ent.GetUpperBound(0) Then
            // '    sb.Append("HL*" & ent(t) & "~" & vbCrLf)
            // 'Else
            // '    sb.Append("HL*" & ent(t))
            // 'End If

            // 'sb.Append("ENT" & ent(t) & "~" & vbCrLf)

            // Next

            txt277EntsSegment.Text = sb.ToString();

            txt277StSegment.Text = ent[0] + "~";


        }

        private string DecodeENT(string theent)
        {

            string ents;
            string[] ent;
            string[] ent1array;
            long t;
            // 'Dim tt As Long
            // 'Dim reg As Regex

            var sb = new StringBuilder();


            // add the ~ST back
            ents = "ST" + theent;

            // now lets get the ~SE trailer
            ent = Regex.Split(ents, "~SE");
            // we should now have a two position array ent(0) has most of the file - the trailing ~
            // and ent(1) has the SE segment - the SE

            // ' lets put it away for future reference
            // txt837SESegment.Text = "SE" & ent(1)

            // lets put the ~ back on the end of the ents string
            ents = ent[0] + "~";

            sb.Capacity = ents.Length;

            ent = Regex.Split(ents, "~HL");

            var loopTo = (long)ent.GetUpperBound(0);
            for (t = 1L; t <= loopTo; t++)
            {

                string ent1 = ent[(int)t];

                ent1array = Regex.Split(ent1, "~");

                bool flag = true;

                foreach (string ss in ent1array)
                {
                    if (flag)
                    {
                        sb.Append("HL" + ss + "~" + Constants.vbCrLf);
                        flag = false;
                    }
                    else
                    {
                        sb.Append("    " + ss + Constants.vbCrLf);
                    }
                }



                // If t < ent.GetUpperBound(0) Then
                // sb.Append("HL" & ent(t) & "~" & vbCrLf)
                // Else
                // sb.Append("HL" & ent(t))
                // End If

                // sb.Append("ENT" & ent(t) & "~" & vbCrLf)

            }

            return sb.ToString();

        }



        private void Decode837File(string fname)
        {

            string ents;
            string[] ent;
            string[] ent1array;
            long t;
            // 'Dim tt As Long
            // 'Dim reg As Regex

            var sb = new StringBuilder();

            TextReader tr = File.OpenText(fname);

            txtEntLines.Text = "";
            ents = tr.ReadToEnd();

            ents = ents.Replace(ControlChars.CrLf, "");

            tr.Close();

            // here we might try to turn the LFs to ~ characters to account to the CCBH files the deviate from the standard a bit
            ents = ents.Replace(Constants.vbLf, "~");

            // now take the read in file string and lets begin to break it apart
            // we'll start by looking for the ~ST header section
            ent = Regex.Split(ents, "~ST");
            // we should now have a two position array ent(0) withh have DPW spluge
            // ent(1) will have the rest of the file - the ~ST so well add it back in and discard the 
            // DPW spluge

            for (int t1 = 1, loopTo = ent.GetUpperBound(0); t1 <= loopTo; t1++)
                sb.Append(DecodeENT("ST" + ent[t1]));

            // add the ~ST back
            ents = "ST" + ent[1];

            // now lets get the ~SE trailer
            ent = Regex.Split(ents, "~SE");
            // we should now have a two position array ent(0) has most of the file - the trailing ~
            // and ent(1) has the SE segment - the SE

            // lets put it away for future reference
            txt837SESegment.Text = "SE" + ent[1];

            // lets put the ~ back on the end of the ents string
            ents = ent[0] + "~";

            // sb.Capacity = ents.Length

            ent = Regex.Split(ents, "~HL");

            // For t = 1 To ent.GetUpperBound(0)

            // Dim ent1 As String = ent(t)

            // ent1array = Regex.Split(ent1, "~")

            // Dim flag As Boolean = True

            // For Each ss As String In ent1array
            // If flag Then
            // sb.Append("HL" & ss & "~" & vbCrLf)
            // flag = False
            // Else
            // sb.Append("    " & ss & vbCrLf)
            // End If
            // Next



            // 'If t < ent.GetUpperBound(0) Then
            // '    sb.Append("HL" & ent(t) & "~" & vbCrLf)
            // 'Else
            // '    sb.Append("HL" & ent(t))
            // 'End If

            // 'sb.Append("ENT" & ent(t) & "~" & vbCrLf)

            // Next

            txt837EntLines.Text = sb.ToString();

            txt837stsegment.Text = ent[0] + "~";


        }

        private void DecodeFile834(string fname)
        {

            string ents;
            string[] ent;
            long t;
            // Dim reg As Regex

            var sb = new StringBuilder();

            TextReader tr = File.OpenText(fname);

            txtEntLines.Text = "";
            ents = tr.ReadToEnd();

            ents = ents.Replace(ControlChars.CrLf, "");

            tr.Close();

            // now take the read in file string and lets begin to break it apart
            // we'll start by looking for the ~ST header section
            ent = Regex.Split(ents, "~ST");
            // we should now have a two position array ent(0) withh have DPW spluge
            // ent(1) will have the rest of the file - the ~ST so well add it back in and discard the 
            // DPW spluge

            ents = "";

            var loopTo = (long)ent.GetUpperBound(0);
            for (t = 1L; t <= loopTo; t++)
                ents = ents + "ST" + ent[(int)t];

            // add the ~ST back
            // ents = "ST" & ent(1)

            // now lets get the ~SE trailer
            ent = Regex.Split(ents, "~SE");
            // we should now have a two position array ent(0) has most of the file - the trailing ~
            // and ent(1) has the SE segment - the SE

            // lets put it away for future reference
            txt834SEsegment.Text = "SE" + ent[ent.GetUpperBound(0)];

            // lets put the ~ back on the end of the ents string
            ents = ents + "~";

            sb.Capacity = ents.Length;

            ent = Regex.Split(ents, "~INS");

            var loopTo1 = (long)ent.GetUpperBound(0);
            for (t = 1L; t <= loopTo1; t++)
            {

                if (t < ent.GetUpperBound(0))
                {
                    sb.Append("INS" + ent[(int)t] + "~" + Constants.vbCrLf);
                }
                else
                {
                    sb.Append("INS" + ent[(int)t]);
                }

            }

            txt834DTPsegment.Text = sb.ToString();

            txt834STsegment.Text = ent[0] + "~";


        }

        private void DecodeFile835(string fname)
        {

            string ents;
            string[] ent;
            long t;
            // Dim reg As Regex

            var sb = new StringBuilder();

            TextReader tr = File.OpenText(fname);

            txtEntLines.Text = "";
            ents = tr.ReadToEnd();

            ents = ents.Replace(ControlChars.CrLf, "");

            tr.Close();

            // now take the read in file string and lets begin to break it apart
            // we'll start by looking for the ~ST header section
            ent = Regex.Split(ents, "~ST");
            // we should now have a two position array ent(0) withh have DPW spluge
            // ent(1) will have the rest of the file - the ~ST so well add it back in and discard the 
            // DPW spluge

            ents = "";

            var loopTo = (long)ent.GetUpperBound(0);
            for (t = 1L; t <= loopTo; t++)
                ents = ents + "ST" + ent[(int)t];

            // add the ~ST back
            // ents = "ST" & ent(1)

            // now lets get the ~SE trailer
            ent = Regex.Split(ents, "~SE");
            // we should now have a two position array ent(0) has most of the file - the trailing ~
            // and ent(1) has the SE segment - the SE

            // lets put it away for future reference
            txt835SEsegment.Text = "SE" + ent[ent.GetUpperBound(0)];

            // lets put the ~ back on the end of the ents string
            ents = ents + "~";

            sb.Capacity = ents.Length;

            ent = Regex.Split(ents, "~CLP");

            var loopTo1 = (long)ent.GetUpperBound(0);
            for (t = 1L; t <= loopTo1; t++)
            {

                if (t < ent.GetUpperBound(0))
                {
                    sb.Append("CLP" + ent[(int)t] + "~" + Constants.vbCrLf);
                }
                else
                {
                    sb.Append("CLP" + ent[(int)t]);
                }

            }

            txt835CLPsegment.Text = sb.ToString();

            txt835STsegment.Text = ent[0] + "~";


        }

        private void DecodeFile271(string fname)
        {

            string ents;
            string[] ent;
            long t;
            // Dim reg As Regex

            var sb = new StringBuilder();

            TextReader tr = File.OpenText(fname);

            txtEntLines.Text = "";
            ents = tr.ReadToEnd();

            ents = ents.Replace(ControlChars.CrLf, "");

            tr.Close();

            // now take the read in file string and lets begin to break it apart
            // we'll start by looking for the ~ST header section
            ent = Regex.Split(ents, "~ST");
            // we should now have a two position array ent(0) withh have DPW spluge
            // ent(1) will have the rest of the file - the ~ST so well add it back in and discard the 
            // DPW spluge

            ents = "";

            var loopTo = (long)ent.GetUpperBound(0);
            for (t = 1L; t <= loopTo; t++)
                ents = ents + "ST" + ent[(int)t];

            // add the ~ST back
            // ents = "ST" & ent(1)

            // now lets get the ~SE trailer
            ent = Regex.Split(ents, "~SE");
            // we should now have a two position array ent(0) has most of the file - the trailing ~
            // and ent(1) has the SE segment - the SE

            // lets put it away for future reference
            txt271SEsegment.Text = "SE" + ent[ent.GetUpperBound(0)];

            // lets put the ~ back on the end of the ents string
            ents = ents + "~";

            sb.Capacity = ents.Length;

            ent = Regex.Split(ents, "~HL");

            var loopTo1 = (long)ent.GetUpperBound(0);
            for (t = 1L; t <= loopTo1; t++)
            {

                if (t < ent.GetUpperBound(0))
                {
                    sb.Append("HL" + ent[(int)t] + "~" + Constants.vbCrLf);
                }
                else
                {
                    sb.Append("HL" + ent[(int)t]);
                }

            }

            txt271HLsegemnts.Text = sb.ToString();

            txt271STsegment.Text = ent[0] + "~";


        }

        private void btnOpenHipaaFile_Click(object sender, EventArgs e)
        {
            dlg.InitialDirectory = Environment.CurrentDirectory;
            dlg.Multiselect = false;
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                // we are looking at a file
                txtFilename.Text = dlg.FileName;
                DecodeFile(dlg.FileName);

            }
        }

        private void btnOpen834File_Click(object sender, EventArgs e)
        {
            dlg.InitialDirectory = Environment.CurrentDirectory;
            dlg.Multiselect = false;
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                // we are looking at a file
                txt834Filename.Text = dlg.FileName;
                DecodeFile834(dlg.FileName);

            }
        }

        private void btnSmallerFonts_Click(object sender, EventArgs e)
        {
            Font fnt;

            float siz;
            fnt = txtSTSegment.Font;
            siz = fnt.Size;

            if (siz < 7f)
            {
                return;
            }

            siz = siz - 1f;

            fnt = new Font("Courier New", siz, FontStyle.Bold, GraphicsUnit.Point);

            txtEntLines.Font = fnt;
            txtSEsegment.Font = fnt;
            txtSTSegment.Font = fnt;
            txt834DTPsegment.Font = fnt;
            txt834SEsegment.Font = fnt;
            txt834STsegment.Font = fnt;
            txt835CLPsegment.Font = fnt;
            txt835SEsegment.Font = fnt;
            txt835STsegment.Font = fnt;
            txt271HLsegemnts.Font = fnt;
            txt271SEsegment.Font = fnt;
            txt271STsegment.Font = fnt;
            txt837EntLines.Font = fnt;
            txt837SESegment.Font = fnt;
            txt837stsegment.Font = fnt;
            txt277EntsSegment.Font = fnt;
            txt277SESegment.Font = fnt;
            txt277StSegment.Font = fnt;



        }

        private void btnBiggerFonts_Click(object sender, EventArgs e)
        {
            Font fnt;

            float siz;
            fnt = txtSTSegment.Font;
            siz = fnt.Size;

            if (siz > 30f)
            {
                return;
            }

            siz = siz + 1f;

            fnt = new Font("Courier New", siz, FontStyle.Bold, GraphicsUnit.Point);

            txtEntLines.Font = fnt;
            txtSEsegment.Font = fnt;
            txtSTSegment.Font = fnt;
            txt834DTPsegment.Font = fnt;
            txt834SEsegment.Font = fnt;
            txt834STsegment.Font = fnt;
            txt835CLPsegment.Font = fnt;
            txt835SEsegment.Font = fnt;
            txt835STsegment.Font = fnt;
            txt271HLsegemnts.Font = fnt;
            txt271SEsegment.Font = fnt;
            txt271STsegment.Font = fnt;
            txt837EntLines.Font = fnt;
            txt837SESegment.Font = fnt;
            txt837stsegment.Font = fnt;
            txt277EntsSegment.Font = fnt;
            txt277SESegment.Font = fnt;
            txt277StSegment.Font = fnt;


        }

        private void TabPage1_Click(object sender, EventArgs e)
        {
            // I edited this and placed this comment here for Tabpage1_click
        }

        private void btnOpen835_Click(object sender, EventArgs e)
        {
            dlg.InitialDirectory = Environment.CurrentDirectory;
            dlg.Multiselect = false;
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                // we are looking at a file
                txt835Filename.Text = dlg.FileName;
                DecodeFile835(dlg.FileName);

            }
        }

        private void btnOpen271_Click(object sender, EventArgs e)
        {
            dlg.InitialDirectory = Environment.CurrentDirectory;
            dlg.Multiselect = false;
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                // we are looking at a file
                txt271Filename.Text = dlg.FileName;
                DecodeFile271(dlg.FileName);
            }
        }

        private void btnOpen837File_Click(object sender, EventArgs e)
        {
            dlg.InitialDirectory = Environment.CurrentDirectory;
            dlg.Multiselect = false;
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                // we are looking at a file
                txt837Filename.Text = dlg.FileName;
                Decode837File(dlg.FileName);

            }
        }

        private void btnOpen277File_Click(object sender, EventArgs e)
        {
            dlg.InitialDirectory = Environment.CurrentDirectory;
            dlg.Multiselect = false;
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                // we are looking at a file
                txt277filename.Text = dlg.FileName;
                Decode277File(dlg.FileName);

            }
        }

    }
}