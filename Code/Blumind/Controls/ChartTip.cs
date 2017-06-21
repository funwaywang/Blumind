using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using Blumind.Configuration;
using Blumind.Model;

namespace Blumind.Controls
{
    class ChartTip : Tip
    {
        const string SIZE_CONFIG = "ChartTipSizeConfig";
        ChartTipControl tipControl;
        public static ChartTip Global { get; private set; }

        static ChartTip()
        {
            Global = new ChartTip();
        }

        ChartTip()
        {
            tipControl = new ChartTipControl();
            tipControl.Size = new System.Drawing.Size(400, 250);
            this.CustomControl = tipControl;

            Size = new System.Drawing.Size(400, 250);
            if (Options.Current.Contains(SIZE_CONFIG))
                Size = Options.Current.GetValue(SIZE_CONFIG, Size);

            AutoSize = false;
            ShowPinButton = true;
            ShowCloseButton = true;
            Resizable = true;
            ShowOnUserFocused = true;

            ApplyTheme(UITheme.Default);
        }

        public void ApplyTheme(UITheme theme)
        {
            BackColor = theme.Colors.MediumLight;
            ForeColor = theme.Colors.Dark;
            tipControl.BackColor = BackColor;
        }

        public void Show(Chart chartControl, Model.ChartObject chartObject)
        {
            if (chartObject == null ||
                (string.IsNullOrEmpty(chartObject.Remark) &&
                    (!(chartObject is IHyperlink) || string.IsNullOrEmpty(((IHyperlink)chartObject).Hyperlink))))
            {
                Hide();
                return;
            }

            var bounds = chartObject.Bounds;
            bounds.Location = chartControl.ChartPointToScreen(chartObject.PointToChart);
            Control = chartControl;
            Show(null, bounds);

            tipControl.Html = chartObject.Remark;
            if (chartObject is IHyperlink)
                tipControl.LinkUrl = ((IHyperlink)chartObject).Hyperlink;
            else
                tipControl.LinkUrl = null;
        }

        protected override void ResizeTipWin(System.Drawing.Size size)
        {
            base.ResizeTipWin(size);

            if (Resizable)
            {
                Options.Current.SetValue(SIZE_CONFIG, size);
            }
        }

        class ChartTipControl : Control
        {
            HtmlEditBox webBrowser;
            LinkLabel labLink;
            string _LinkUrl;
            string _Html;

            public ChartTipControl()
            {
                webBrowser = new HtmlEditBox();
                labLink = new LinkLabel();

                this.SuspendLayout();

                webBrowser.ReadOnly = true;
                webBrowser.VisibleChanged += webBrowser_VisibleChanged;

                labLink.VisibleChanged += labLink_VisibleChanged;
                labLink.AutoSize = true;

                Controls.Add(webBrowser);
                Controls.Add(labLink);
                this.ResumeLayout();
            }

            public string Html
            {
                get
                {
                    return _Html;
                }

                set
                {
                    if (_Html != value)
                    {
                        _Html = value;
                        OnHtmlChanged();
                    }
                }
            }

            public string LinkUrl
            {
                get
                {
                    return _LinkUrl;
                }

                set
                {
                    if (_LinkUrl != value)
                    {
                        _LinkUrl = value;
                        OnLinkUrlChanged();
                    }
                }
            }

            void OnLinkUrlChanged()
            {
                labLink.Visible = !string.IsNullOrEmpty(LinkUrl);
                labLink.Text = LinkUrl;
            }

            void OnHtmlChanged()
            {
                webBrowser.Visible = !string.IsNullOrEmpty(Html);
                webBrowser.Text = Html;
            }

            void webBrowser_VisibleChanged(object sender, EventArgs e)
            {
                if (Created)
                    PerformLayout();
            }

            void labLink_VisibleChanged(object sender, EventArgs e)
            {
                if(Created)
                    PerformLayout();
            }

            protected override void OnLayout(LayoutEventArgs e)
            {
                base.OnLayout(e);

                if (webBrowser != null)
                {
                    var rect = ClientRectangle;
                    if (labLink.Visible && webBrowser.Visible)
                    {
                        labLink.Location = new System.Drawing.Point(rect.Left, rect.Bottom - labLink.Height);
                        rect.Height -= labLink.Height;
                    }

                    if (webBrowser.Visible)
                        webBrowser.Bounds = rect;
                    else if (labLink.Visible)
                        labLink.Location = new System.Drawing.Point(rect.Left, rect.Y + (rect.Height - labLink.Height) / 2);
                }
            }

            protected override void OnBackColorChanged(EventArgs e)
            {
                base.OnBackColorChanged(e);

                if (webBrowser != null)
                    webBrowser.BackColor = this.BackColor;
            }
        }
    }
}
