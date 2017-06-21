using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using Blumind.Globalization;

namespace Blumind.Controls
{
    class HtmlEditToolStrip : ToolStripPro
    {
        HtmlEditor Editor;
        HtmlEditBox htmlBox;
        bool _MiniMode;
        bool _ReadOnly;
        HtmlEditorViewType _ViewType;

        ToolStripButton tsbCut;
        ToolStripButton tsbCopy;
        ToolStripButton tsbPaste;
        ToolStripButton tsbBold;
        ToolStripButton tsbItalic;
        ToolStripSplitButton tsbDecoration;
        ToolStripMenuItem tsbUnderline;
        ToolStripMenuItem tsbStrikeThrough;
        ToolStripDropDownButton tsbAlignment;
        ToolStripMenuItem tsbAlignLeft;
        ToolStripMenuItem tsbAlignCenter;
        ToolStripMenuItem tsbAlignRight;
        ToolStripMenuItem tsbAlignJustify;
        ToolStripDropDownButton tsbList;
        ToolStripMenuItem tsbOrderedList;
        ToolStripMenuItem tsbBulletList;
        ToolStripDropDownButton tsbIndents;
        ToolStripMenuItem tsbIndent;
        ToolStripMenuItem tsbUnIndent;
        ToolStripButton tsbImage;
        ToolStripDropDownButton tsbFontSize;
        //ToolStripMenuItem tsbFontSizeUp;
        //ToolStripMenuItem tsbFontSizeDown;
        ToolStripDropDownButton tsbFont;
        ToolStripButton tsbForeColor;
        ToolStripButton tsbBackColor;
        ToolStripButton tsbUndo;
        ToolStripButton tsbRedo;
        ToolStripButton tsbDelete;
        ToolStripButton tsbHyperLink;
        ToolStripButton tsbShowCode;
        ToolStripSeparator toolStripSeparator1;
        ToolStripSeparator toolStripSeparator2;
        ToolStripSeparator toolStripSeparator3;
        ToolStripSeparator toolStripSeparator4;
        ToolStripSeparator toolStripSeparator6;
        ToolStripSeparator toolStripSeparator7;
        ToolStripSeparator toolStripSeparator8;

        public HtmlEditToolStrip()
        {
            InitializeComponents();

            PriorityItems = new List<ToolStripItem>();
            PriorityItems.AddRange(new ToolStripItem[]{
                    tsbBold,
                    tsbItalic,
                    tsbDecoration,
                    toolStripSeparator3,
                    tsbForeColor,
                    tsbBackColor,
                    toolStripSeparator4,
                    tsbHyperLink,
                    tsbImage,
                    //tsbUnIndent,
                    //tsbIndent,
                    //tsbFontSize,
                });

            EditButtons = new List<ToolStripItem>();
            EditButtons.AddRange(new ToolStripItem[]{
                    tsbBold,
                    tsbItalic,
                    tsbDecoration,
                    toolStripSeparator3,
                    tsbForeColor,
                    tsbBackColor,
                    toolStripSeparator4,
                    tsbHyperLink,
                    tsbImage,
                    tsbIndents,
                    tsbFontSize,
                    tsbUndo,
                    tsbRedo,
                    tsbPaste,
                    tsbCut,
                    tsbFont,
                    tsbFontSize,
                    tsbDelete,
                    tsbList,
                    tsbAlignment,
                    toolStripSeparator1,
                    toolStripSeparator2,
                    toolStripSeparator6,
                    toolStripSeparator7,
                    toolStripSeparator8,
            });

            LanguageManage.CurrentChanged += LanguageManage_CurrentChanged;
        }

        public HtmlEditToolStrip(HtmlEditor editor)
            : this()
        {
            Editor = editor;
            htmlBox = editor.EditBox;
        }

        [Browsable(false)]
        public List<ToolStripItem> PriorityItems { get; private set; }

        [Browsable(false)]
        public List<ToolStripItem> EditButtons { get; private set; }

        [DefaultValue(false)]
        public bool MiniMode
        {
            get { return _MiniMode; }
            set 
            {
                if (_MiniMode != value)
                {
                    _MiniMode = value;
                    OnMiniModeChanged();
                }
            }
        }

        [Browsable(false), DefaultValue(false)]
        public bool ReadOnly
        {
            get { return _ReadOnly; }
            set 
            {
                if (_ReadOnly != value)
                {
                    _ReadOnly = value;
                    OnReadOnlyChanged();
                }
            }
        }

        [Browsable(false), DefaultValue(HtmlEditorViewType.Design)]
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

        void OnReadOnlyChanged()
        {
            RefreshItemsVisible();
        }

        void OnMiniModeChanged()
        {
            RefreshItemsVisible();
        }

        void RefreshItemsVisible()
        {
            foreach (ToolStripItem item in Items)
            {
                var v = true;

                //if (MiniMode && !PriorityItems.Contains(item))
                //    v = false;

                if (ReadOnly && EditButtons.Contains(item))
                    v = false;

                item.Visible = v;
            }
        }

        void InitializeComponents()
        {
            tsbUndo = new ToolStripButton();
            tsbRedo = new ToolStripButton();
            tsbDelete = new ToolStripButton();
            tsbCut = new ToolStripButton();
            tsbCopy = new ToolStripButton();
            tsbPaste = new ToolStripButton();
            tsbBold = new ToolStripButton();
            tsbItalic = new ToolStripButton();
            tsbDecoration = new ToolStripSplitButton();
            tsbUnderline = new ToolStripMenuItem();
            tsbStrikeThrough = new ToolStripMenuItem();
            tsbImage = new ToolStripButton();
            tsbAlignment = new ToolStripDropDownButton();
            tsbAlignLeft = new ToolStripMenuItem();
            tsbAlignCenter = new ToolStripMenuItem();
            tsbAlignRight = new ToolStripMenuItem();
            tsbAlignJustify = new ToolStripMenuItem();
            tsbList = new ToolStripDropDownButton();
            tsbOrderedList = new ToolStripMenuItem();
            tsbBulletList = new ToolStripMenuItem();
            tsbIndents = new ToolStripDropDownButton();
            tsbIndent = new ToolStripMenuItem();
            tsbUnIndent = new ToolStripMenuItem();
            tsbFont = new ToolStripDropDownButton();
            tsbFontSize = new ToolStripDropDownButton();
            //tsbFontSizeUp = new ToolStripMenuItem();
            //tsbFontSizeDown = new ToolStripMenuItem();
            tsbForeColor = new ToolStripButton();
            tsbBackColor = new ToolStripButton();
            tsbHyperLink = new ToolStripButton();
            tsbShowCode = new ToolStripButton();
            toolStripSeparator1 = new ToolStripSeparator();
            toolStripSeparator2 = new ToolStripSeparator();
            toolStripSeparator3 = new ToolStripSeparator();
            toolStripSeparator4 = new ToolStripSeparator();
            toolStripSeparator6 = new ToolStripSeparator();
            toolStripSeparator7 = new ToolStripSeparator();
            toolStripSeparator8 = new ToolStripSeparator();
            SuspendLayout();

            // 
            // toolBar
            // 
            this.Items.AddRange(new ToolStripItem[] {
                tsbUndo,
                tsbRedo,
                toolStripSeparator1,
                tsbCut,
                tsbCopy,
                tsbPaste,
                tsbDelete,
                toolStripSeparator2,
                tsbBold,
                tsbItalic,
                tsbDecoration,
                toolStripSeparator3,
                tsbForeColor,
                tsbBackColor,
                toolStripSeparator4,
                tsbAlignment,
                tsbList,
                tsbIndents,                toolStripSeparator6,
                tsbImage,
                tsbHyperLink,
                toolStripSeparator7,
                tsbFont,
                tsbFontSize,
                toolStripSeparator8,
                tsbShowCode});

            // tsbUndo
            tsbUndo.DisplayStyle = ToolStripItemDisplayStyle.Image;
            tsbUndo.Image = Properties.Resources.undo;
            tsbUndo.Text = "Undo";
            tsbUndo.Click += tsbUndo_Click;

            // tsbRedo
            tsbRedo.DisplayStyle = ToolStripItemDisplayStyle.Image;
            tsbRedo.Image = Properties.Resources.redo;
            tsbRedo.Text = "Redo";
            tsbRedo.Click += tsbRedo_Click;

            // tsbCut
            tsbCut.DisplayStyle = ToolStripItemDisplayStyle.Image;
            tsbCut.Image = Properties.Resources.cut;
            tsbCut.Text = "Cut";
            tsbCut.Click += new System.EventHandler(this.tsbCut_Click);
            // tsbCopy
            tsbCopy.DisplayStyle = ToolStripItemDisplayStyle.Image;
            tsbCopy.Image = Properties.Resources.copy;
            tsbCopy.Text = "Copy";
            tsbCopy.Click += new System.EventHandler(this.tsbCopy_Click);

            // tsbPaste
            tsbPaste.DisplayStyle = ToolStripItemDisplayStyle.Image;
            tsbPaste.Image = Properties.Resources.paste;
            tsbPaste.Text = "Paste";
            tsbPaste.Click += new System.EventHandler(this.tsbPaste_Click);

            // tsbDelete
            tsbDelete.DisplayStyle = ToolStripItemDisplayStyle.Image;
            tsbDelete.Image = Properties.Resources.delete;
            tsbDelete.Text = "Delete";
            tsbDelete.Click += tsbDelete_Click;

            // tsbHyperLink
            tsbHyperLink.DisplayStyle = ToolStripItemDisplayStyle.Image;
            tsbHyperLink.Image = Properties.Resources.hyperlink;
            tsbHyperLink.Text = "Hyper Link";
            tsbHyperLink.Click += tsbHyperLink_Click;

            // tsbImage
            tsbImage.DisplayStyle = ToolStripItemDisplayStyle.Image;
            tsbImage.Image = Properties.Resources.image;
            tsbImage.Text = "Image";
            tsbImage.Click += tsbImage_Click;

            // tsbBold
            tsbBold.DisplayStyle = ToolStripItemDisplayStyle.Image;
            tsbBold.Image = Properties.Resources.bold;
            tsbBold.Text = "Bold";
            tsbBold.Click += new System.EventHandler(this.tsbBold_Click);

            // tsbItalic
            tsbItalic.DisplayStyle = ToolStripItemDisplayStyle.Image;
            tsbItalic.Image = Properties.Resources.italic;
            tsbItalic.Text = "Italic";
            tsbItalic.Click += new System.EventHandler(this.tsbItalic_Click);
            //
            tsbDecoration.Image = Properties.Resources.underline;
            tsbDecoration.DisplayStyle = ToolStripItemDisplayStyle.Image;
            tsbDecoration.Text = "Decoration";
            tsbDecoration.DropDownItems.AddRange(new ToolStripItem[] { tsbUnderline, tsbStrikeThrough });
            tsbDecoration.ButtonClick += tsbDecoration_ButtonClick;
            //
            tsbStrikeThrough.Image = Properties.Resources.strike;
            tsbStrikeThrough.Text = "Strike Through";
            tsbStrikeThrough.Click += tsbStrikeThrough_Click;
            // tsbUnderline
            tsbUnderline.Image = Properties.Resources.underline;
            tsbUnderline.Text = "Underline";
            tsbUnderline.Click += tsbUnderline_Click;

            // tsbForeColor
            tsbForeColor.DisplayStyle = ToolStripItemDisplayStyle.Image;
            tsbForeColor.Image = Properties.Resources.fore_color;
            tsbForeColor.Text = "Fore Color";
            tsbForeColor.Click += tsbForeColor_Click;

            // tsbItalic
            tsbBackColor.DisplayStyle = ToolStripItemDisplayStyle.Image;
            tsbBackColor.Image = Properties.Resources.back_color;
            tsbBackColor.Text = "Back Color";
            tsbBackColor.Click += tsbBackColor_Click;
            //
            tsbAlignment.Image = Properties.Resources.edit_alignment;
            tsbAlignment.DisplayStyle = ToolStripItemDisplayStyle.Image;
            tsbAlignment.DropDownItems.AddRange(new ToolStripItem[] { tsbAlignLeft, tsbAlignCenter, tsbAlignRight, tsbAlignJustify });
            //
            tsbAlignLeft.Image = Properties.Resources.edit_alignment;
            tsbAlignLeft.Text = "Align Left";
            tsbAlignLeft.Click += tsbAlignLeft_Click;
            //
            tsbAlignCenter.Image = Properties.Resources.edit_alignment_center;
            tsbAlignCenter.Text = "Align Center";
            tsbAlignCenter.Click += tsbAlignCenter_Click;
            //
            tsbAlignRight.Image = Properties.Resources.edit_alignment_right;
            tsbAlignRight.Text = "Align Right";
            tsbAlignRight.Click += tsbAlignRight_Click;
            //
            tsbAlignJustify.Image = Properties.Resources.edit_alignment_justify;
            tsbAlignJustify.Text = "Align Justify";
            tsbAlignJustify.Click += tsbAlignJustify_Click;
            //
            tsbList.DisplayStyle = ToolStripItemDisplayStyle.Image;
            tsbList.Image = Properties.Resources.list_bullets;
            tsbList.DropDownItems.AddRange(new ToolStripItem[] { tsbOrderedList, tsbBulletList });
            // tsbOrderedList
            tsbOrderedList.Image = Properties.Resources.list_numbered;
            tsbOrderedList.Text = "Ordered list";
            tsbOrderedList.Click += new System.EventHandler(this.tsbOrderedList_Click);
            // tsbBulletList
            tsbBulletList.Image = Properties.Resources.list_bullets;
            tsbBulletList.Text = "Bullet List";
            tsbBulletList.Click += new System.EventHandler(this.tsbBulletList_Click);
            // tsbIndents
            tsbIndents.DropDownItems.AddRange(new ToolStripItem[] { tsbIndent, tsbUnIndent });
            tsbIndents.Image = Properties.Resources.indent;
            tsbIndents.DisplayStyle = ToolStripItemDisplayStyle.Image;
            // tsbUnIndent
            tsbUnIndent.Image = Properties.Resources.outdent;
            tsbUnIndent.Text = "Unindent";
            tsbUnIndent.Click += new System.EventHandler(this.tsbUnIndent_Click);
            // tsbIndent
            tsbIndent.Image = Properties.Resources.indent;
            tsbIndent.Text = "Indent";
            tsbIndent.Click += new System.EventHandler(this.tsbIndent_Click);
            // tsbFont
            tsbFont.Image = Properties.Resources.font;
            tsbFont.Text = "Font";
            tsbFont.DisplayStyle = ToolStripItemDisplayStyle.Image;
            // 
            // tsbFontSize
            // 
            this.tsbFontSize.DisplayStyle = ToolStripItemDisplayStyle.Image;
            tsbFontSize.Image = Properties.Resources.font_size;
            float[] htmlSize = new float[] { 8, 10, 12, 14, 18, 24, 36 };
            for (int i = 0; i < htmlSize.Length; i++)
            {
                var miFontSize = new ToolStripMenuItem();
                miFontSize.Text = (i + 1).ToString();
                miFontSize.Tag = i;
                miFontSize.Click +=miFontSize_Click;
                tsbFontSize.DropDownItems.Add(miFontSize);
            }
            //tsbFontSize.DropDownItems.AddRange(new ToolStripItem[] { new ToolStripSeparator(), tsbFontSizeUp, tsbFontSizeDown });

            ////
            //tsbFontSizeUp.Text = "Increase";
            //tsbFontSizeUp.Image = Properties.Resources.edit_font_size_up;
            //tsbFontSizeUp.Click +=tsbFontSizeUp_Click;

            ////
            //tsbFontSizeDown.Text = "Decrease";
            //tsbFontSizeDown.Image = Properties.Resources.edit_font_size_down;
            //tsbFontSizeDown.Click += tsbFontSizeDown_Click;

            //
            // tsbShowCode
            //
            tsbShowCode.DisplayStyle = ToolStripItemDisplayStyle.Image;
            tsbShowCode.Image = Properties.Resources.edit_code;
            tsbShowCode.Text = "Source Code";
            tsbShowCode.Click += tsbShowCode_Click;

            //
            LanguageManage_CurrentChanged(this, EventArgs.Empty);

            //
            ResumeLayout(false);
        }

        void tsbCut_Click(object sender, EventArgs e)
        {
            if (htmlBox != null)
            {
                htmlBox.Cut();
            }
        }

        void tsbCopy_Click(object sender, EventArgs e)
        {
            Editor.Copy();
        }

        void tsbPaste_Click(object sender, EventArgs e)
        {
            if (htmlBox != null)
            {
                htmlBox.Paste();
            }
        }

        void tsbBold_Click(object sender, EventArgs e)
        {
            if (htmlBox != null)
            {
                htmlBox.SetBold();
            }
        }

        void tsbItalic_Click(object sender, EventArgs e)
        {
            if (htmlBox != null)
            {
                htmlBox.SetItalic();
            }
        }

        void tsbStrikeThrough_Click(object sender, EventArgs e)
        {
            if (htmlBox != null)
            {
                htmlBox.SetStrikeThrough();
            }
        }

        void tsbDecoration_ButtonClick(object sender, EventArgs e)
        {
            tsbUnderline_Click(sender, e);
        }

        void tsbUnderline_Click(object sender, EventArgs e)
        {
            if (htmlBox != null)
            {
                htmlBox.SetUnderline();
            }
        }

        void tsbOrderedList_Click(object sender, EventArgs e)
        {
            if (htmlBox != null)
            {
                htmlBox.InsertOrderedList();
            }
        }

        void tsbBulletList_Click(object sender, EventArgs e)
        {
            if (htmlBox != null)
            {
                htmlBox.InsertUnOrderedList();
            }
        }

        void tsbUnIndent_Click(object sender, EventArgs e)
        {
            if (htmlBox != null)
            {
                htmlBox.Outdent();
            }
        }

        void tsbIndent_Click(object sender, EventArgs e)
        {
            if (htmlBox != null)
            {
                htmlBox.Indent();
            }
        }

        void tsbBackColor_Click(object sender, EventArgs e)
        {
            if (htmlBox != null)
            {
                ColorDialog cd = new ColorDialog();
                if (cd.ShowDialog(this) == DialogResult.OK)
                {
                    htmlBox.SetBackColor(cd.Color);
                }
            }
        }

        void tsbForeColor_Click(object sender, EventArgs e)
        {
            if (htmlBox != null)
            {
                ColorDialog cd = new ColorDialog();
                if (cd.ShowDialog(this) == DialogResult.OK)
                {
                    htmlBox.SetForeColor(cd.Color);
                }
            }
        }

        void tsbDelete_Click(object sender, EventArgs e)
        {
            if (htmlBox != null)
            {
                htmlBox.Delete();
            }
        }

        void tsbRedo_Click(object sender, EventArgs e)
        {
            if (htmlBox != null)
            {
                htmlBox.Redo();
            }
        }

        void tsbUndo_Click(object sender, EventArgs e)
        {
            if (htmlBox != null)
            {
                htmlBox.Undo();
            }
        }

        void tsbHyperLink_Click(object sender, EventArgs e)
        {
            if (htmlBox != null)
            {
                htmlBox.AddHyperLink();
            }
        }

        void tsbImage_Click(object sender, EventArgs e)
        {
            if (htmlBox != null)
            {
                htmlBox.AddImage();
            }
        }

        void tsbShowCode_Click(object sender, EventArgs e)
        {
            if (tsbShowCode.Checked)
                Editor.ViewType = HtmlEditorViewType.Design;
            else
                Editor.ViewType = HtmlEditorViewType.SourceCode;
        }

        void tsbAlignJustify_Click(object sender, EventArgs e)
        {
            if (htmlBox != null)
            {
                htmlBox.AlignmentJustify();
            }
        }

        void tsbAlignRight_Click(object sender, EventArgs e)
        {
            if (htmlBox != null)
            {
                htmlBox.AlignmentRight();
            }
        }

        void tsbAlignCenter_Click(object sender, EventArgs e)
        {
            if (htmlBox != null)
            {
                htmlBox.AlignmentCenter();
            }
        }

        void tsbAlignLeft_Click(object sender, EventArgs e)
        {
            if (htmlBox != null)
            {
                htmlBox.AlignmentLeft();
            }
        }

        void OnViewTypeChanged()
        {
            switch (ViewType)
            {
                case HtmlEditorViewType.Design:
                    tsbShowCode.Checked = false;
                    break;
                case HtmlEditorViewType.SourceCode:
                    tsbShowCode.Checked = true;
                    break;
            }
        }

        void LanguageManage_CurrentChanged(object sender, EventArgs e)
        {
            tsbUndo.Text = Lang._("Undo");
            tsbRedo.Text = Lang._("Redo");
            tsbCut.Text = Lang._("Cut");
            tsbCopy.Text = Lang._("Copy");
            tsbPaste.Text = Lang._("Paste");
            tsbDelete.Text = Lang._("Delete");
            tsbHyperLink.Text = Lang._("Hyper Link");
            tsbImage.Text = Lang._("Image");
            tsbBold.Text = Lang._("Bold");
            tsbItalic.Text = Lang._("Italic");
            tsbDecoration.Text = Lang._("Decoration");
            tsbUnderline.Text = Lang._("Underline");
            tsbStrikeThrough.Text = Lang._("Strike Through");
            tsbForeColor.Text = Lang._("Fore Color");
            tsbBackColor.Text = Lang._("Back Color");
            tsbAlignment.Text = Lang._("Alignment");
            tsbAlignLeft.Text = Lang._("Align Left");
            tsbAlignCenter.Text = Lang._("Align Center");
            tsbAlignRight.Text = Lang._("Align Right");
            tsbAlignJustify.Text = Lang._("Align Justify");
            tsbList.Text = Lang._("List");
            tsbOrderedList.Text = Lang._("Ordered list");
            tsbBulletList.Text = Lang._("Bullet List");
            tsbIndents.Text = Lang._("Indent");
            tsbIndent.Text = Lang._("Indent");
            tsbUnIndent.Text = Lang._("Unindent");
            tsbFont.Text = Lang._("Font");
            //tsbFontSizeUp.Text = Lang._("Increase");
            //tsbFontSizeDown.Text = Lang._("Decrease");
            tsbShowCode.Text = Lang._("Source Code");

            if (MenuMoreFonts != null)
                MenuMoreFonts.Text = Lang.GetTextWithEllipsis("More");
        }

        #region Fonts
        IEnumerable<string> _Fonts;
        ToolStripMenuItem MenuMoreFonts;

        public IEnumerable<string> Fonts
        {
            get { return _Fonts; }
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
            if (tsbFont != null && Fonts != null)
            {
                tsbFont.DropDownItems.Clear();
                foreach (var f in Fonts)
                {
                    var mi = new ToolStripMenuItem(f);
                    mi.Tag = f;
                    mi.Click += mi_Click;
                    tsbFont.DropDownItems.Add(mi);
                }

                tsbFont.DropDownItems.Add(new ToolStripSeparator());
                MenuMoreFonts = new ToolStripMenuItem();
                MenuMoreFonts.Text = Lang.GetTextWithEllipsis("More");
                MenuMoreFonts.Click += MenuMoreFonts_Click;
                tsbFont.DropDownItems.Add(MenuMoreFonts);
            }
        }

        void MenuMoreFonts_Click(object sender, EventArgs e)
        {
            var dialog = new FontDialog();
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                if (htmlBox != null)
                    htmlBox.SetFont(dialog.Font);
            }
        }

        void mi_Click(object sender, EventArgs e)
        {
            if (sender is ToolStripMenuItem)
            {
                var mi = (ToolStripMenuItem)sender;
                if (mi.Tag is string)
                {
                    if (htmlBox != null)
                    {
                        htmlBox.SetFontName((string)mi.Tag);
                    }
                }
            }
        }

        void tsbFontSizeDown_Click(object sender, EventArgs e)
        {
            if (htmlBox != null)
                htmlBox.DecreaseFontSize();
        }

        void tsbFontSizeUp_Click(object sender, EventArgs e)
        {
            if (htmlBox != null)
                htmlBox.IncreaseFontSize();
        }

        void tsbFont_Leave(object sender, EventArgs e)
        {
            if (htmlBox != null)
            {
                htmlBox.SetFontName(tsbFont.Text);
            }
        }

        void tsbFont_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Return)
            {
                if (htmlBox != null)
                {
                    htmlBox.SetFontName(tsbFont.Text);
                    htmlBox.Focus();
                }
                e.Handled = true;
            }
        }

        void tsbFont_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (htmlBox != null)
            {
                htmlBox.SetFontName(tsbFont.Text);
            }
        }

        void miFontSize_Click(object sender, EventArgs e)
        {
            if (sender is ToolStripItem)
            {
                var tsi = (ToolStripItem)sender;
                if (tsi.Tag is int)
                {
                    var fontSize = (int)tsi.Tag;

                    if (htmlBox != null)
                    {
                        htmlBox.SetFontSize(fontSize.ToString());
                        htmlBox.Focus();
                    }
                }
            }
        }

        #endregion
    }
}
