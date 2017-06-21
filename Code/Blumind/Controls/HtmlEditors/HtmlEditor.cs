using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Text;
using System.Linq;
using System.Windows.Forms;
using Blumind.Core;

namespace Blumind.Controls
{
    class HtmlEditor : ModifyControl
    {
        Panel panel1;
        HtmlEditToolStrip toolStrip1;
        HtmlEditBox editBox;
        HtmlCodeEditor codeBox;
        HtmlEditorViewType _ViewType;

        public HtmlEditor()
        {
            InitializeComponent();
        }

        void InitializeComponent()
        {
            panel1 = new Panel();
            editBox = new HtmlEditBox();
            codeBox = new HtmlCodeEditor();
            toolStrip1 = new HtmlEditToolStrip(this);
            panel1.SuspendLayout();
            SuspendLayout();

            //
            editBox.Dock = DockStyle.Fill;
            editBox.TabIndex = 0;
            editBox.ReadOnly = ReadOnly;
            editBox.TextChanged += editBox_TextChanged;

            //
            codeBox.Dock = DockStyle.Fill;
            codeBox.ReadOnly = ReadOnly;
            codeBox.TextChanged += codeBox_TextChanged;

            //
            toolStrip1.TabIndex = 1;
            //toolStrip1.LayoutStyle = ToolStripLayoutStyle.Flow;
            toolStrip1.Fonts = DefaultFonts;
            toolStrip1.Dock = DockStyle.Top;
            toolStrip1.RenderMode = ToolStripRenderMode.System;

            //
            panel1 = new Panel();
            panel1.Dock = DockStyle.Fill;
            panel1.TabIndex = 0;

            //
            Controls.AddRange(new Control[] { panel1, toolStrip1 });
            OnViewTypeChanged();
            panel1.ResumeLayout();
            ResumeLayout();
        }

        [Browsable(false)]
        public HtmlEditBox EditBox
        {
            get { return editBox; }
        }

        [Browsable(false)]
        public HtmlEditToolStrip ToolStrip
        {
            get { return toolStrip1; }
        }

        /// <summary>
        /// Returns the text the user edited in Html format
        /// </summary>
        [Category("Appearance"), Browsable(true), DefaultValue(null)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [Bindable(BindableSupport.Yes)]
        public override string Text
        {
            get
            {
                switch (ViewType)
                {
                    case HtmlEditorViewType.SourceCode:
                        return codeBox.Text;
                    case HtmlEditorViewType.Design:
                        return editBox.Text;
                    default:
                        return OriginalText;
                }
            }
            set
            {
                OriginalText = value;
                switch (ViewType)
                {
                    case HtmlEditorViewType.SourceCode:
                        codeBox.Text = value;
                        break;
                    case HtmlEditorViewType.Design:
                        editBox.Text = value;
                        break;
                }

                Modified = false;
            }
        }

        [Category("Appearance")]
        public string PlainText
        {
            get { return editBox.PlainText; }
        }

        protected override Size DefaultSize
        {
            get
            {
                return new Size(100, 100);
            }
        }

        protected string OriginalText { get; private set; }
        
        [DefaultValue(false)]
        public HtmlEditorViewType ViewType
        {
            get { return _ViewType; }
            set
            {
                if (_ViewType != value)
                {
                    _ViewType = value;
                    OnViewTypeChanged();
                }
            }
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing && editBox != null)
            {
                editBox.Dispose();
                editBox = null;
            }
        }
                
        void OnViewTypeChanged()
        {
            switch (ViewType)
            {
                case HtmlEditorViewType.SourceCode:
                    if (editBox != null && panel1.Controls.Contains(editBox))
                        panel1.Controls.Remove(editBox);

                    if (!panel1.Controls.Contains(codeBox))
                    {
                        panel1.Controls.Add(codeBox);
                    }

                    codeBox.Text = editBox.Text;
                    break;
                case HtmlEditorViewType.Design:
                default:
                    if (codeBox != null && panel1.Controls.Contains(codeBox))
                        panel1.Controls.Remove(codeBox);

                    if (!panel1.Controls.Contains(editBox))
                    {
                        panel1.Controls.Add(editBox);
                    }

                    editBox.Text = codeBox.Text;
                    break;
            }

            toolStrip1.ViewType = ViewType;
            //PerformLayout();
        }
                
        #region Fonts
        string[] _Fonts = null;

        public static string[] DefaultFonts
        {
            get
            {
                var list = new List<string>();
                list.Add("Sans-Serif");
                list.Add("Serif");

                //
                var commonFonts = new string[] { 
                    "Arial",
                    "Comic Sans MS",
                    "Corbel",
                    "Courier New",
                    "Georgia",
                    "Monospace",
                    "Tahoma",
                    "Times",
                    "Verdana",
                };
                var installedFonts = (from fm in new InstalledFontCollection().Families
                                     select fm.Name).ToArray();
                foreach(var cf in commonFonts)
                {
                    if(installedFonts.Contains(cf, StringComparer.OrdinalIgnoreCase))
                        list.Add(cf);
                }

                //
                var sysFonts = new Font[]{SystemFonts.DefaultFont, 
                    SystemFonts.MenuFont, 
                    SystemFonts.MessageBoxFont,
                    SystemFonts.CaptionFont,
                    SystemFonts.DialogFont};
                foreach (var f in sysFonts)
                {
                    if (!list.Contains(f.FontFamily.Name, StringComparer.OrdinalIgnoreCase))
                        list.Add(f.FontFamily.Name);
                }

                return list.ToArray();
            }
        }

        [Category("Behavior"), DefaultValue(null)]
        public string[] Fonts
        {
            get 
            {
                return _Fonts; 
            }

            set
            {
                if (_Fonts != value)
                {
                    _Fonts = value;
                    OnFontsChanged();
                }
            }
        }

        protected virtual void OnFontsChanged()
        {
            if (toolStrip1 != null)
            {
                if (Fonts == null)
                    toolStrip1.Fonts = DefaultFonts;
                else
                    toolStrip1.Fonts = Fonts;
            }
        }

        #endregion

        protected override void OnReadOnlyChanged()
        {
            base.OnReadOnlyChanged();

            RefreshViewStatus();
        }

        void RefreshViewStatus()
        {
            if (editBox != null)
                editBox.ReadOnly = this.ReadOnly;

            if (codeBox != null)
                codeBox.ReadOnly = this.ReadOnly;

            if (toolStrip1 != null)
            {
                toolStrip1.ReadOnly = this.ReadOnly;
                toolStrip1.Visible = !this.ReadOnly;
            }
        }

        void editBox_TextChanged(object sender, EventArgs e)
        {
            if (!ReadOnly && ViewType == HtmlEditorViewType.Design)
            {
                if (Text != OriginalText)
                {
                    OnTextChanged(EventArgs.Empty);
                    OriginalText = Text;
                }
            }
        }

        void codeBox_TextChanged(object sender, EventArgs e)
        {
            if (!ReadOnly && ViewType == HtmlEditorViewType.SourceCode)
            {
                if (Text != OriginalText)
                {
                    OnTextChanged(EventArgs.Empty);
                    OriginalText = Text;
                }
            }
        }

        public void EndEdit()
        {
            if (!ReadOnly)
            {
                switch (ViewType)
                {
                    case HtmlEditorViewType.Design:
                        editBox.EndEdit();
                        break;
                    case HtmlEditorViewType.SourceCode:
                        if (Text != OriginalText)
                        {
                            OnTextChanged(EventArgs.Empty);
                            OriginalText = Text;
                        }
                        break;
                }
            }
        }

        protected override void OnTextChanged(EventArgs e)
        {
            base.OnTextChanged(e);

            Modified = true;
        }

        public void Copy()
        {
            switch (ViewType)
            {
                case HtmlEditorViewType.Design:
                    if (editBox != null)
                        editBox.Copy();
                    break;
                case HtmlEditorViewType.SourceCode:
                    if (codeBox != null)
                        codeBox.Copy();
                    break;
            }
        }
    }
}
