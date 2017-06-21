using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;

namespace Blumind.Controls
{
    public class HtmlEditBox : Control
    {
        const string EDITOR_ID = "theTextbox";
        string _BLANK_PAGE_HTML_;
        WebBrowser theBrowser;
        bool _ReadOnly;
        bool TextSuspend;

        public HtmlEditBox()
        {
            _BLANK_PAGE_HTML_ = Properties.Resources.html_content;
            InitializeComponents();
        }

        protected bool BrowserReady { get; private set; }

        protected override Size DefaultSize
        {
            get
            {
                return new Size(100, 100);
            }
        }

        /// <summary>
        /// Returns the text the user edited in Html format
        /// </summary>
        [Category("Appearance")]
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [Bindable(BindableSupport.Yes)]
        public override string Text
        {
            get
            {
                return this.InnerHtml;
            }
            set
            {
                this.OriginalText = value;
                this.InnerHtml = value;
            }
        }

        [Category("Appearance")]
        public string PlainText
        {
            get
            {
                if (WaitUntilBrowserReady())
                {
                    var txb = TextBoxElement;
                    if (txb != null)
                        return GetSafeHtml(txb.InnerText);
                }

                return null;
            }
        }

        protected string InnerHtml
        {
            get
            {
                if (WaitUntilBrowserReady())
                {
                    var txb = TextBoxElement;
                    if (txb != null)
                        return GetSafeHtml(txb.InnerHtml);
                }

                return null;
            }

            private set
            {
                if (!this.BrowserReady)
                {
                    TextSuspend = true;
                    return;
                }

                if (TextBoxElement == null)
                {
                    InitializateBlankPage();

                    while (!this.BrowserReady)
                        Thread.Sleep(100);
                }

                var txb = TextBoxElement;
                if (txb != null)
                {
                    txb.InnerHtml = value;
                    TextSuspend = false;
                }
            }
        }

        protected string OriginalText { get; private set; }

        [DefaultValue(false)]
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

        HtmlElement TextBoxElement
        {
            get
            {
                if (BrowserReady)
                    return theBrowser.Document.GetElementById(EDITOR_ID);
                else
                    return null;
            }
        }

        [DefaultValue(typeof(Color), "Window")]
        public override Color BackColor
        {
            get
            {
                return base.BackColor;
            }
            set
            {
                base.BackColor = value;
            }
        }

        void InitializeComponents()
        {
            theBrowser = new WebBrowser();
            //theBrowser.AllowNavigation = false;
            theBrowser.Dock = DockStyle.Fill;
            theBrowser.Visible = false;
            theBrowser.ScriptErrorsSuppressed = true;
            theBrowser.Navigating += theBrowser_Navigating;
            theBrowser.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(this.theBrowser_DocumentCompleted);

            Controls.Add(theBrowser);
            BackColor = SystemColors.Window;
            ReadOnly = false;
        }

        void theBrowser_Navigating(object sender, WebBrowserNavigatingEventArgs e)
        {
            if (e.Url != null)
            {
                if (StringComparer.OrdinalIgnoreCase.Equals(e.Url.AbsoluteUri, "about:blank"))
                    return;
                Helper.OpenUri(e.Url);
            }

            e.Cancel = true;
        }

        void InitializateBlankPage()
        {
            //theBrowser.Navigate("about:blank");
            //theBrowser.Document.OpenNew(true);
            //theBrowser.Document.Write(string.Empty);
            //theBrowser.Document.Write(_BLANK_PAGE_HTML_);
            theBrowser.DocumentText = _BLANK_PAGE_HTML_;
            //theBrowser.Refresh();
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                if (theBrowser != null && !theBrowser.IsDisposed && !theBrowser.Disposing)
                {
                    theBrowser.Dispose();
                    theBrowser = null;
                }
            }
        }

        protected override void OnCreateControl()
        {
            base.OnCreateControl();

            if (!this.IsDesignMode())
            {
                theBrowser.Visible = true;
                InitializateBlankPage();
                OnReadOnlyChanged();
            }
        }

        void theBrowser_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            BrowserReady = true;

            if (TextSuspend)
            {
                InnerHtml = OriginalText;
            }
        }

        bool WaitUntilBrowserReady()
        {
            if (this.BrowserReady)
            {
                return true;
            }

            for (int i = 0; i < 60 && !this.BrowserReady; i++)
            {
                System.Threading.Thread.Sleep(100);
                Application.DoEvents();
            }

            return BrowserReady;
        }

        protected virtual void OnReadOnlyChanged()
        {
            if (!this.Created || theBrowser == null || !theBrowser.Created)
                return;

            if (WaitUntilBrowserReady())
            {
                var txb = TextBoxElement;
                if (txb != null)
                {
                    txb.SetAttribute("contenteditable", ReadOnly ? "false" : "true");
                    txb.LostFocus += txb_LostFocus;
                }
            }
        }

        void txb_LostFocus(object sender, HtmlElementEventArgs e)
        {
            EndEdit();
        }

        public bool EndEdit()
        {
            if (!ReadOnly)
            {
                if (OriginalText != Text)
                {
                    OnTextChanged(EventArgs.Empty);
                    OriginalText = Text;
                    return true;
                }
            }

            return false;
        }
        
        protected override void OnGotFocus(EventArgs e)
        {
            base.OnGotFocus(e);

            OnFocusChanged();
        }

        protected override void OnLostFocus(EventArgs e)
        {
            base.OnLostFocus(e);

            OnFocusChanged();
        }

        void OnFocusChanged()
        {
            if (this.ContainsFocus)
                return;

            Refresh();
            if (theBrowser.Focused)
            {
                WaitUntilBrowserReady();
                theBrowser.Document.InvokeScript("InitFocus");
            }
        }

        protected override void OnFontChanged(EventArgs e)
        {
            base.OnFontChanged(e);

            SetBrowserFont(this.Font);
        }

        void SetBrowserFont(Font font)
        {
            if (BrowserReady)
            {
                if (font != null)
                {
                    theBrowser.Document.InvokeScript("SetFont",
                        new object[] {
                            font.Name,
                            font.Size.ToString(System.Globalization.CultureInfo.InvariantCulture) + "pt"});
                }
                else
                {
                    theBrowser.Document.InvokeScript("SetFont", new object[] { null, null });
                }
            }
        }
        
        #region Security HTML
        static string[] _illegalPatternsDefault = new string[] {
                @"<script.*?>",                            // all <script >
                @"<\w+\s+.*?(j|java|vb|ecma)script:.*?>",  // any tag containing *script:
                @"<\w+(\s+|\s+.*?\s+)on\w+\s*=.+?>",       // any tag containing an attribute starting with "on"
                @"</?input.*?>"                            // <input> and </input>
            };

        string[] _illegalPatterns = _illegalPatternsDefault;

        /// <summary>
        /// Contains a list of regular expression that are cleared from the html.
        /// Like script, of event handlers
        /// </summary>
        [Category("Behavior")]
        [Description(@"A list of regular expressions that are removed from the html. To reset, set to single line with *.")]
        public string[] IllegalPatterns
        {
            get
            {
                if (this._illegalPatterns == null)
                {
                    return new string[0];
                }
                return this._illegalPatterns;
            }
            set
            {
                // When zero length then store a null
                if (value == null || value.Length == 0)
                {
                    this._illegalPatterns = null;
                    return;
                }
                if (value.Length == 1 && value[0] == "*")
                {
                    this._illegalPatterns = _illegalPatternsDefault;
                    return;
                }
                // Remove empty & duplicate strings
                List<string> buf = new List<string>();
                foreach (var item in value)
                {
                    if (!string.IsNullOrEmpty(item) && !buf.Contains(item))
                    {
                        buf.Add(item);
                    }
                }
                this._illegalPatterns = buf.Count == 0 ? null : buf.ToArray();
            }
        }

        public string GetSafeHtml(string original)
        {
            if (string.IsNullOrEmpty(original) || this.IllegalPatterns.Length == 0)
            {
                return original;
            }

            string buf = original;
            foreach (var pattern in this.IllegalPatterns)
            {
                Regex reg = new Regex(pattern,
                    RegexOptions.IgnoreCase |
                    RegexOptions.Multiline |
                    RegexOptions.Singleline);
                buf = reg.Replace(buf, string.Empty);
            }
            System.Diagnostics.Debug.WriteLineIf(buf != original, "Filtered: " + buf);
            return buf;
        }

        #endregion

        #region Operations
        public bool ExecCommand(string command, bool showUI, object value)
        {
            if (DesignMode)
                return false;

            if (WaitUntilBrowserReady())
            {
                theBrowser.Document.ExecCommand(command, showUI, value);
                return true;
            }

            return false;
        }

        public bool ExecCommand(string command, object value)
        {
            return ExecCommand(command, false, value);
        }

        public bool ExecCommand(string command)
        {
            return ExecCommand(command, false, null);
        }

        public void Copy()
        {
            ExecCommand(HtmlCommandIdentifiers.Copy);
        }

        public void Cut()
        {
            ExecCommand(HtmlCommandIdentifiers.Cut);
        }

        public void Paste()
        {
            ExecCommand(HtmlCommandIdentifiers.Paste);
        }

        public void Delete()
        {
            ExecCommand(HtmlCommandIdentifiers.Delete);
        }

        public void Undo()
        {
            ExecCommand(HtmlCommandIdentifiers.Undo);
        }

        public void Redo()
        {
            ExecCommand(HtmlCommandIdentifiers.Redo);
        }

        public void SetBold()
        {
            ExecCommand(HtmlCommandIdentifiers.Bold);
        }

        public void SetItalic()
        {
            ExecCommand(HtmlCommandIdentifiers.Italic);
        }

        public void SetUnderline()
        {
            ExecCommand(HtmlCommandIdentifiers.Underline);
        }

        public void SetStrikeThrough()
        {
            ExecCommand(HtmlCommandIdentifiers.StrikeThrough);
        }

        public void InsertOrderedList()
        {
            ExecCommand(HtmlCommandIdentifiers.InsertOrderedList);
        }

        public void InsertUnOrderedList()
        {
            ExecCommand(HtmlCommandIdentifiers.InsertUnorderedList);
        }

        public void Outdent()
        {
            ExecCommand(HtmlCommandIdentifiers.Outdent);
        }

        public void Indent()
        {
            ExecCommand(HtmlCommandIdentifiers.Indent);
        }

        public void SetFont(Font font)
        {
            if (font == null)
                return;

            SetFontName(font.FontFamily.Name);

            float[] htmlSize = new float[] { 8, 10, 12, 14, 18, 24, 36 };
            int hs = htmlSize.Length - 1;
            for (int i = 0; i < htmlSize.Length; i++)
            {
                if (font.SizeInPoints <= hs)
                {
                    hs = i;
                    break;
                }
            }
            SetFontSize((hs + 1).ToString());

            if ((font.Style & FontStyle.Bold) == FontStyle.Bold)
                SetBold();

            if ((font.Style & FontStyle.Italic) == FontStyle.Italic)
                SetItalic();

            if ((font.Style & FontStyle.Underline) == FontStyle.Underline)
                SetUnderline();

            if ((font.Style & FontStyle.Strikeout) == FontStyle.Strikeout)
                SetStrikeThrough();
        }

        public void SetFontName(string fontName)
        {
            ExecCommand(HtmlCommandIdentifiers.FontName, fontName);
        }

        public void SetFontSize(string fontSize)
        {
            ExecCommand(HtmlCommandIdentifiers.FontSize, fontSize);
        }

        public void IncreaseFontSize()
        {
            ExecCommand(HtmlCommandIdentifiers.IncreaseFontSize, "1");
        }

        public void DecreaseFontSize()
        {
            ExecCommand(HtmlCommandIdentifiers.DecreaseFontSize, "1");
        }

        public void SetForeColor(string color)
        {
            ExecCommand(HtmlCommandIdentifiers.ForeColor, color);
        }

        public void SetForeColor(Color color)
        {
            ExecCommand(HtmlCommandIdentifiers.ForeColor, color.ToWebColor());
        }

        public void SetBackColor(string color)
        {
            ExecCommand(HtmlCommandIdentifiers.BackColor, color);
        }

        public void SetBackColor(Color color)
        {
            ExecCommand(HtmlCommandIdentifiers.BackColor, color.ToWebColor());
        }

        public void AddHyperLink()
        {
            ExecCommand(HtmlCommandIdentifiers.CreateLink);
        }

        public void AddImage()
        {
            ExecCommand(HtmlCommandIdentifiers.InsertImage, true, null);
        }

        public void AlignmentLeft()
        {
            ExecCommand(HtmlCommandIdentifiers.JustifyLeft);
        }

        public void AlignmentCenter()
        {
            ExecCommand(HtmlCommandIdentifiers.JustifyCenter);
        }

        public void AlignmentRight()
        {
            ExecCommand(HtmlCommandIdentifiers.JustifyRight);
        }

        public void AlignmentJustify()
        {
            ExecCommand(HtmlCommandIdentifiers.JustifyFull);
        }
        #endregion
    }
}
