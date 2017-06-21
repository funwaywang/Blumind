using System.Drawing;
using System.Windows.Forms;
using Blumind.Configuration;

namespace Blumind.Controls
{
    class ChartBox : Control
    {
        private Keys _ExtentInputKey = Keys.None;

        public ChartBox()
        {
            this.SetStyle(ControlStyles.UserPaint
                | ControlStyles.AllPaintingInWmPaint
                | ControlStyles.OptimizedDoubleBuffer
                | ControlStyles.ResizeRedraw, true);
        }

        public Keys ExtentInputKey
        {
            get { return _ExtentInputKey; }
            set { _ExtentInputKey = value; }
        }

        protected override bool IsInputKey(Keys keyData)
        {
            if ((keyData & ExtentInputKey) != Keys.None)
                return true;

            return base.IsInputKey(keyData);
        }

        protected override Padding DefaultMargin
        {
            get
            {
                return Padding.Empty;
            }
        }

        public static Font DefaultChartFont
        {
            get
            {
                return Options.Current.GetValue("Charts.DefaultFont", SystemFonts.MessageBoxFont);
            }
        }

#if DEBUG
        //protected override void WndProc(ref Message m)
        //{
        //    switch (m.Msg)
        //    {
        //        case WinMessages.WM_IME_CHAR:
        //            System.Diagnostics.Debug.WriteLine(string.Format("ChartBox > WM_IME_CHAR: {0}, {1}", m.WParam, m.LParam));
        //            break;
        //        case WinMessages.WM_CHAR:
        //            System.Diagnostics.Debug.WriteLine(string.Format("ChartBox > WM_CHAR: {0}, {1}", m.WParam, m.LParam));
        //            break;
        //    }

        //    base.WndProc(ref m);
        //}

        protected override void OnFontChanged(System.EventArgs e)
        {
            base.OnFontChanged(e);
        }
#endif
    }
}
