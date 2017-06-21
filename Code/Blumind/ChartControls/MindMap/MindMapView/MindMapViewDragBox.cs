using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Blumind.Core;
using Blumind.Globalization;
using Blumind.Model;
using Blumind.Model.MindMaps;

namespace Blumind.Controls.MapViews
{
    enum DragTopicsMethod
    {
        None,
        Move,
        Copy,
    }

    class MindMapViewDragBox : IDisposable
    {
        const int Padding = 10;
        MindMapView OwnerView;
        //Topic _Topic;
        Topic[] _Topics;
        Bitmap ObjectImage;
        bool _CanDragDrop = false;
        public bool Visible;
        public Rectangle Bounds;

        public MindMapViewDragBox(MindMapView view)
        {
            OwnerView = view;
        }

        //public Topic Topic
        //{
        //    get { return _Topic; }
        //    set
        //    {
        //        if (_Topic != value)
        //        {
        //            _Topic = value;
        //            OnTopicChanged();
        //        }
        //    }
        //}

        public Topic[] Topics
        {
            get { return _Topics; }
            private set
            {
                if (_Topics != value)
                {
                    _Topics = value;
                    OnTopicsChanged();
                }
            }
        }

        Point StartMousePosition { get; set; }

        Rectangle ObjectsOriginalBounds { get; set; }

        Point LogicLocation { get; set; }

        Size SubTitleSize { get; set; }

        string SubTitle { get; set; }

        Image SubTitleIcon { get; set; }

        public bool CanDragDrop
        {
            get { return _CanDragDrop; }
            set { _CanDragDrop = value; }
        }

        DragTopicsMethod DraggingMethod { get; set; }

        //void OnTopicChanged()
        //{
        //    if (ObjectImage != null)
        //    {
        //        ObjectImage.Dispose();
        //        ObjectImage = null;
        //    }
        //}

        void OnTopicsChanged()
        {
            if (ObjectImage != null)
            {
                ObjectImage.Dispose();
                ObjectImage = null;
            }
        }

        public void Draw(PaintEventArgs e)
        {
            //System.Diagnostics.Debug.WriteLine("Draw");
            if (Bounds.IsEmpty || Topics.IsNullOrEmpty() || OwnerView == null)
                return;

            //e.Graphics.DrawRectangle(Pens.Blue, Bounds.X, Bounds.Y, Bounds.Width - 1, Bounds.Height - 1);

            if (ObjectImage == null)
            {
                ObjectImage = new Bitmap(Bounds.Width, Bounds.Height);
                using (Graphics grfImg = Graphics.FromImage(ObjectImage))
                {
                    if (!Topics.IsNullOrEmpty())
                    {
                        PaintHelper.SetHighQualityRender(grfImg);

                        //grfImg.DrawRectangle(Pens.Red, 0, 0, Bounds.Width - 1, Bounds.Height - 1);
                        //grfImg.TranslateTransform( Bounds.X - StartBounds.X, Bounds.Y - StartBounds.Y);
                        grfImg.TranslateTransform(-LogicLocation.X + Padding, -LogicLocation.Y + Padding);

                        var args = new RenderArgs(RenderMode.UserInface, grfImg, OwnerView, ChartBox.DefaultChartFont);
                        OwnerView.Render.PaintTopics(Topics, args);
                    }
                }
                PaintHelper.TransparentImage(ObjectImage, 0.75f);
            }

            e.Graphics.DrawImage(ObjectImage, Bounds, 0, 0, ObjectImage.Width, ObjectImage.Height, GraphicsUnit.Pixel);

            if (SubTitle != null || SubTitleIcon != null)
            {
                var rectSubTitle = new Rectangle(Bounds.X + Padding, Bounds.Bottom - SubTitleSize.Height, Bounds.Width - Padding * 2, SubTitleSize.Height);
                if (SubTitleIcon != null)
                {
                    var rectIcon = new Rectangle(rectSubTitle.X, rectSubTitle.Y + (rectSubTitle.Height - 16) / 2, 16, 16);
                    if (!CanDragDrop)
                        e.Graphics.DrawImageUnscaled(Properties.Resources.stop, rectIcon);
                    else
                        e.Graphics.DrawImageUnscaled(SubTitleIcon, rectIcon);
                }

                if (!string.IsNullOrEmpty(SubTitle))
                {
                    var rectText = new Rectangle(rectSubTitle.X + 18, rectSubTitle.Y, SubTitleSize.Width, rectSubTitle.Height);
                    rectText.Inflate(0, -1);
                    e.Graphics.FillRectangle(new SolidBrush(UITheme.Default.Colors.MediumLight), rectText);
                    rectText.Inflate(-2, 0);
                    //using (var p = PaintHelper.GetRoundRectangle(rectText, 2))
                    //{
                    //    e.Graphics.FillPath(new SolidBrush(UITheme.Default.Colors.MediumLight), p);
                    //    rectText.Inflate(-2, 0);
                    //}

                    var sf = PaintHelper.SFLeft;
                    sf.FormatFlags |= StringFormatFlags.NoWrap;
                    sf.Trimming |= StringTrimming.None;

                    e.Graphics.DrawString(SubTitle,
                        SystemFonts.MenuFont,
                        new SolidBrush(UITheme.Default.Colors.ControlText),
                        rectText,
                        sf);
                }
            }
            else
            {
                if (!CanDragDrop)
                {
                    Image icon = Properties.Resources.stop;
                    e.Graphics.DrawImage(icon,
                        new Rectangle(Bounds.Left + (Bounds.Width - icon.Width) / 2, Bounds.Top + (Bounds.Height - icon.Height) / 2, icon.Width, icon.Height),
                        0, 0, icon.Width, icon.Height, GraphicsUnit.Pixel);
                }
            }

            //e.Graphics.DrawRectangle(Pens.Red, Bounds.Left, Bounds.Top, Bounds.Width - 1, Bounds.Height - 1);
        }

        #region IDisposable Members

        public void Dispose()
        {
            if (ObjectImage != null)
            {
                ObjectImage.Dispose();
                ObjectImage = null;
            }
        }

        #endregion

        public void Start(Point position, IEnumerable<Topic> topics)
        {
            StartMousePosition = position;

            if (topics.IsNullOrEmpty())
            {
                ClearTopics();
            }
            else
            {
                Topics = topics.ToArray();
                var rect = Topics[0].Bounds;
                for (int i = 1; i < Topics.Length; i++)
                {
                    rect = Rectangle.Union(rect, Topics[i].Bounds);
                }
                LogicLocation = rect.Location;
                rect.Location = this.OwnerView.PointToReal(rect.Location);
                rect.Inflate(Padding, Padding);

                ObjectsOriginalBounds = rect;
                Bounds = new Rectangle(0, 0, rect.Width, rect.Height);
            }
        }

        public void Refresh(int x, int y, bool canDragDrop, DragTopicsMethod dragMethod, Size clientSize)
        {
            if (Topics.IsNullOrEmpty())
            {
                ClearTopics();
            }
            else
            {
                var location = new Point(ObjectsOriginalBounds.X + (x - StartMousePosition.X), ObjectsOriginalBounds.Y + (y - StartMousePosition.Y));
                location.X = Math.Max(0, Math.Min(clientSize.Width - Bounds.Width, location.X));
                location.Y = Math.Max(0, Math.Min(clientSize.Height - Bounds.Height, location.Y));

                Bounds = new Rectangle(location, ObjectsOriginalBounds.Size);
                CanDragDrop = canDragDrop;

                DraggingMethod = dragMethod;
                switch (dragMethod)
                {
                    case DragTopicsMethod.Move:
                        SubTitle = Lang.GetTextWithEllipsis("Moving");
                        SubTitleIcon = Properties.Resources.cut;
                        break;
                    case DragTopicsMethod.Copy:
                        SubTitle = Lang.GetTextWithEllipsis("Copying");
                        SubTitleIcon = Properties.Resources.copy;
                        break;
                    default:
                        SubTitle = null;
                        SubTitleIcon = null;
                        break;
                }
                if (!string.IsNullOrEmpty(SubTitle))
                {
                    SubTitleSize = TextRenderer.MeasureText(SubTitle, SystemFonts.MenuFont);
                    SubTitleSize = new Size(SubTitleSize.Width + 2, SubTitleSize.Height + 6);
                    Bounds.Size = new Size(Math.Max(Bounds.Width, SubTitleSize.Width + 18), Bounds.Height + SubTitleSize.Height);
                }
                else
                {
                    SubTitleSize = Size.Empty;
                }

                //this.Invalidate();
            }
        }

        public void ClearTopics()
        {
            Topics = new Topic[0];
            DraggingMethod = DragTopicsMethod.None;
            Bounds = Rectangle.Empty;
        }

        public void Invalidate()
        {
            if (this.OwnerView != null)
            {
                var rect = Bounds;
                rect.X += LogicLocation.X - ObjectsOriginalBounds.X;
                rect.Y += LogicLocation.Y - ObjectsOriginalBounds.Y;
                //System.Diagnostics.Debug.Write(rect.ToString());
                rect = this.OwnerView.RectangleToReal(rect);
                rect.Inflate(20, 20);

                //System.Diagnostics.Debug.Write(" -> ");
                //System.Diagnostics.Debug.WriteLine(rect.ToString());
                this.OwnerView.InvalidateChart(rect, true);
            }
        }
    }
}
