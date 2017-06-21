using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Blumind.Core;
using Blumind.Globalization;

namespace Blumind.Controls
{
    [DefaultProperty("SelectedFont")]
    [DefaultEvent("SelectedFontChanged")]
    class FontButton : FlatButton
    {
        Font _SelectedFont;

        public event EventHandler SelectedFontChanged;

        public FontButton()
        {
            OnSelectedFontChanged();

            LanguageManage.CurrentChanged += LanguageManage_CurrentChanged;
        }

        [DefaultValue(null)]
        public Font SelectedFont
        {
            get { return _SelectedFont; }
            set 
            {
                if (_SelectedFont != value)
                {
                    _SelectedFont = value;
                    OnSelectedFontChanged();
                }
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override string Text
        {
            get
            {
                return base.Text;
            }
            set
            {
                base.Text = value;
            }
        }

        void OnSelectedFontChanged()
        {
            if (SelectedFont != null)
                Text = string.Format("{0}({1})", SelectedFont.FontFamily.Name, Math.Round(SelectedFont.SizeInPoints));
            else
                Text = string.Format("({0})", Lang._("None"));

            if (SelectedFontChanged != null)
                SelectedFontChanged(this, EventArgs.Empty);
        }

        void LanguageManage_CurrentChanged(object sender, EventArgs e)
        {
            if (SelectedFont == null)
                Text = string.Format("({0})", Lang._("None"));
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            //base.OnPaint(e);
            InvokePaintBackground(this, e);
            PaintBackground(e);

            if (!string.IsNullOrEmpty(Text))
            {
                Font font = Font;
                if (SelectedFont != null)
                {
                    font = new Font(SelectedFont.FontFamily, Font.Size, SelectedFont.Style);
                }

                e.Graphics.DrawString(Text, font, new SolidBrush(ForeColor), ClientRectangle, PaintHelper.SFCenter);
            }
        }
    }
}
