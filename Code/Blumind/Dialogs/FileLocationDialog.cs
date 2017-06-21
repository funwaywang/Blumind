using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Blumind.Controls;

namespace Blumind.Dialogs
{
    partial class FileLocationDialog : BaseDialog
    {
        string _FileName;
        string _Message;
        Icon _FileIcon;

        public FileLocationDialog()
        {
            InitializeComponent();

            MinimumSize = Size;
        }

        public FileLocationDialog(string filename, string message)
            : this()
        {
            FileName = filename;
            Message = message;
        }

        [DefaultValue(null)]
        public string FileName
        {
            get { return _FileName; }
            set
            {
                if (_FileName != value)
                {
                    _FileName = value;
                    OnFileNameChanged();
                }
            }
        }

        [DefaultValue(null)]
        public string Message
        {
            get { return _Message; }
            set 
            {
                if (_Message != value)
                {
                    _Message = value;
                    OnMessageChanged();
                }
            }
        }

        [DefaultValue(null)]
        public Icon FileIcon
        {
            get { return _FileIcon; }
            set 
            {
                if (_FileIcon != value)
                {
                    _FileIcon = value;
                    OnFileIconChanged();
                }
            }
        }

        protected override bool ShowButtonArea
        {
            get
            {
                return true;
            }
        }

        public FileLocationDialog SetFileIcon(Icon icon)
        {
            FileIcon = icon;
            return this;
        }

        void OnFileIconChanged()
        {
            pictureBox1.Icon = FileIcon;
        }

        void OnMessageChanged()
        {
            textBox1.Text = Message;
        }

        void OnFileNameChanged()
        {
            if (!string.IsNullOrEmpty(FileName) && File.Exists(FileName))
            {
                BtnOpen.Enabled = true;
                BtnOpenFolder.Enabled = true;

                try
                {
                    FileIcon = Icon.ExtractAssociatedIcon(FileName);
                }
                catch
                {
                    FileIcon = null;
                }
            }
            else
            {
                BtnOpen.Enabled = false;
                BtnOpenFolder.Enabled = false;
                FileIcon = null;
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!DesignMode)
            {
                BtnOpen.Font = SystemFonts.MessageBoxFont;
                BtnOpenFolder.Font = SystemFonts.MessageBoxFont;
                BtnClose.Font = SystemFonts.MessageBoxFont;
                textBox1.Font = SystemFonts.MessageBoxFont;
            }
        }

        public static void Show(string message, string caption, string filename)
        {
            FileLocationDialog dialog = new FileLocationDialog();
            dialog.Message = message;
            dialog.Text = caption;
            dialog.FileName = filename;
            dialog.ShowDialog();
        }

        void BtnClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        void BtnOpen_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(FileName) && File.Exists(FileName))
            {
                Helper.OpenUrl(FileName);
                Close();
            }
        }

        void BtnOpenFolder_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(FileName) && File.Exists(FileName))
            {
                Helper.OpenContainingFolder(FileName);
                Close();
            }
        }

        protected override void OnLayout(LayoutEventArgs e)
        {
            base.OnLayout(e);

            if (BtnOpen != null)
            {
                var btns = new Button[] { BtnOpen, BtnClose, BtnOpenFolder };
                foreach (var btn in btns)
                {
                    btn.Height = ButtonHeight;
                    btn.Top = BaseLinePosition + 8;
                }
            }
        }
    }
}
