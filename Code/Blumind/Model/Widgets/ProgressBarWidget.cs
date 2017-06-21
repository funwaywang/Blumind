using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;
using System.Xml;
using Blumind.Canvas;
using Blumind.Controls;
using Blumind.Controls.MapViews;
using Blumind.Core;
using Blumind.Globalization;
using Blumind.Model.MindMaps;

namespace Blumind.Model.Widgets
{
    [DefaultProperty("Value")]
    class ProgressBarWidget : Widget, IExtendActionProvider
    {
        public const string TypeID = "PROGRESSBAR";
        int _Maximum = 100;
        int _Minimum = 0;
        float _Value = 0;
        Color _Color = Color.Green;
        Color _BackColor = Color.White;
        Color _ForeColor = Color.Black;
        bool _ShowText = false;
        bool _AutoCalculation = false;

        public ProgressBarWidget()
        {
        }

        [DefaultValue(100), Browsable(false)]
        public int Maximum
        {
            get { return _Maximum; }
            set
            {
                if (_Maximum != value)
                {
                    int old = _Maximum;
                    _Maximum = value;
                    OnPropertyChanged("Maximum", old, _Maximum, ChangeTypes.Data | ChangeTypes.Visual);
                }
            }
        }

        [DefaultValue(0), Browsable(false)]
        public int Minimum
        {
            get { return _Minimum; }
            set
            {
                if (_Minimum != value)
                {
                    int old = _Minimum;
                    _Minimum = value;
                    OnPropertyChanged("Minimum", old, _Minimum, ChangeTypes.Data | ChangeTypes.Visual);
                }
            }
        }

        [DefaultValue(0f), LocalDisplayName("Progress(%)"), LocalCategory("Data")]
        public float Value
        {
            get { return _Value; }
            set 
            {
                if (_Value != value)
                {
                    float old = _Value;
                    _Value = value;
                    OnValueChanged(old);
                }
            }
        }

        [DefaultValue(false), LocalDisplayName("Auto Calculation"), LocalCategory("Data")]
        [TypeConverter(typeof(Blumind.Design.BoolConverter))]
        public bool AutoCalculation
        {
            get { return _AutoCalculation; }
            set
            {
                if (_AutoCalculation != value)
                {
                    _AutoCalculation = value;
                    OnAutoCalculationChanged();
                }
            }
        }

        [DefaultValue(typeof(Color), "Green"), LocalDisplayName("Color"), LocalCategory("Style")]
        public Color Color
        {
            get { return _Color; }
            set 
            {
                if (_Color != value)
                {
                    Color old = _Color;
                    _Color = value;
                    OnPropertyChanged("Color", old, _Color, ChangeTypes.Data | ChangeTypes.Visual);
                }
            }
        }

        [DefaultValue(typeof(Color), "White"), LocalDisplayName("Back Color"), LocalCategory("Style")]
        public Color BackColor
        {
            get { return _BackColor; }
            set 
            {
                if (_BackColor != value)
                {
                    Color old = _BackColor;
                    _BackColor = value;
                    OnPropertyChanged("BackColor", old, _BackColor, ChangeTypes.Data | ChangeTypes.Visual);
                }
            }
        }

        [DefaultValue(typeof(Color), "Black"), LocalDisplayName("Fore Color"), LocalCategory("Style")]
        public Color ForeColor
        {
            get { return _ForeColor; }
            set
            {
                if (_ForeColor != value)
                {
                    Color old = _ForeColor;
                    _ForeColor = value;
                    OnPropertyChanged("ForeColor", old, _ForeColor, ChangeTypes.Data | ChangeTypes.Visual);
                }
            }
        }

        [DefaultValue(false), LocalDisplayName("Show Text"), LocalCategory("Style")]
        [TypeConverter(typeof(Blumind.Design.BoolConverter))]
        public bool ShowText
        {
            get { return _ShowText; }
            set
            {
                if (_ShowText != value)
                {
                    bool old = _ShowText;
                    _ShowText = value;
                    OnPropertyChanged("ShowText", old, _ShowText, ChangeTypes.Data | ChangeTypes.Visual);
                }
            }
        }

        [Browsable(false)]
        public float Percent
        {
            get
            {
                return (Math.Min(Maximum, Value) - Minimum) / (Maximum - Minimum) * 100f;
            }
        }

        protected virtual void OnAutoCalculationChanged()
        {
            OnPropertyChanged("AutoCalculation", !AutoCalculation, AutoCalculation, ChangeTypes.Data | ChangeTypes.Visual);

            if (AutoCalculation)
            {
                AutoCalculateValue();
            }
        }
        
        public void AutoCalculateValue()
        {
            var topic = this.WidgetContainer as Topic;
            if (topic != null)
            {
                var subbars = (from subtopic in topic.Children
                               from widget in subtopic.Widgets
                               where widget is ProgressBarWidget
                               select (ProgressBarWidget)widget).ToArray();
                if (subbars.Length == 0)
                {
                    Value = Minimum;
                }
                else
                {
                    Value = subbars.Average(b => b.Percent) / 100.0f * (Maximum - Minimum);
                }                
            }
        }

        protected virtual void OnValueChanged(float old)
        {
            OnPropertyChanged("Value", old, _Value, ChangeTypes.Data | ChangeTypes.Visual);

            NotifyParentAutoCalculation(null);
        }

        void NotifyParentAutoCalculation(IWidgetContainer container)
        {
            if (container == null)
                container = this.WidgetContainer;

            var topic = container as Topic;
            if (topic != null && topic.ParentTopic != null)
            {
                var bars = (from widget in topic.ParentTopic.Widgets
                            where widget is ProgressBarWidget
                            select (ProgressBarWidget)widget).ToArray();
                foreach (var bar in bars)
                {
                    if (bar.AutoCalculation)
                        bar.AutoCalculateValue();
                }
            }
        }

        protected override void OnWidgetContainerChanged(IWidgetContainer old)
        {
            base.OnWidgetContainerChanged(old);

            if (old != null)
            {
                NotifyParentAutoCalculation(old);
            }

            NotifyParentAutoCalculation(null);
        }

        public override bool ResponseMouse
        {
            get
            {
                return true;
            }
        }

        public override bool FitContainer
        {
            get
            {
                return true;
            }
        }

        protected override string GetTooltip()
        {
            return (Percent / 100).ToString("P2");
        }

        public override string GetTypeID()
        {
            return TypeID;
        }

        public override Size CalculateSize(MindMapLayoutArgs e)
        {
            Size size = Size.Empty;
            if (e.Graphics == null)
                size = TextRenderer.MeasureText("100%", e.Font, size);
            else
                size = Size.Ceiling(e.Graphics.MeasureString("100%", e.Font, new SizeF(size.Width, size.Height)));

            if (Alignment == WidgetAlignment.Left || Alignment == WidgetAlignment.Right)
            {
                size = new Size(size.Height, size.Width);
            }

            return size;
        }

        public override void Paint(RenderArgs e)
        {
            Rectangle rect = DisplayRectangle;
            if (rect.Width <= 0 || rect.Height <= 0)
                return;
            //Color color = topic.Style.ProgressColor.IsEmpty ? e.MindMap.Style.ProgressColor : topic.Style.ProgressColor;
            //if (color.IsEmpty)
            //    return;

            var sm = e.Graphics.SmoothingMode;
            e.Graphics.SmoothingMode = SmoothingMode.Default;

            rect.Inflate(0, -2);
            /*var path = PaintHelper.GetRoundRectangle(rect, 0); //2
            if (Selected || Hover)
            {
                e.Graphics.FillPath(new LinearGradientBrush(rect, BackColor, PaintHelper.GetLightColor(BackColor), 90.0f), path);
            }
            else
            {
                e.Graphics.FillPath(new SolidBrush(BackColor), path);
            }*/
            e.Graphics.FillRectangle(e.Graphics.SolidBrush(BackColor), rect);

            if (Value > 0 && Maximum > Minimum)
            {
                rect.Inflate(-1, -1);

                if (Alignment == WidgetAlignment.Left || Alignment == WidgetAlignment.Right)
                {
                    int rb = rect.Bottom;
                    rect.Height = (int)Math.Ceiling(rect.Height * (Math.Min(Maximum, Value) - Minimum) / (Maximum - Minimum));
                    rect.Y = rb - rect.Height;
                }
                else
                {
                    rect.Width = (int)Math.Ceiling(rect.Width * (Math.Min(Maximum, Value) - Minimum) / (Maximum - Minimum));
                }

                if (rect.Width > 0 && rect.Height > 0)
                {
                    /*path = PaintHelper.GetRoundRectangle(rect, 0);//2
                    if (Selected || Hover)
                    {
                        e.Graphics.FillPath(new LinearGradientBrush(rect, PaintHelper.GetLightColor(Color), Color, 90.0f), path);
                    }
                    else
                    {
                        e.Graphics.FillPath(new SolidBrush(Color), path);
                    }*/
                    e.Graphics.FillRectangle(e.Graphics.SolidBrush(Color), rect);
                }
            }

            e.Graphics.SmoothingMode = sm;

            // text
            if (ShowText)
            {
                // 左右两端的, 则旋转90度
                var save = e.Graphics.Save();
                try
                {
                    var rectText = Bounds;
                    if (Alignment == WidgetAlignment.Left || Alignment == WidgetAlignment.Right)
                    {
                        e.Graphics.TranslateTransform(rectText.X, rectText.Y);
                        e.Graphics.RotateTransform(-90.0f);

                        var w = rectText.Width;
                        rectText.X = -rectText.Height;
                        rectText.Y = 0;
                        rectText.Width = rectText.Height;
                        rectText.Height = w;
                    }

                    string text = (Percent / 100).ToString("P0");
                    e.Graphics.DrawString(text, e.Font, e.Graphics.SolidBrush(ForeColor), rectText, PaintHelper.SFCenter);
                }
                finally
                {
                    //
                    e.Graphics.Restore(save);
                }
            }
        }

        /*public override void GeneralSvg(XmlElement g, Point translate, Font font)
        {
            //base.GeneralSvg(g);
            Shape shaper = Shape.GetShaper(TopicShape.Rectangle, 2);
            if (shaper != null)
            {
                Rectangle rect = Bounds;
                rect.Offset(translate);
                rect.Inflate(0, -2);
                XmlElement node = shaper.GenerateSvg(rect, g, Color.Empty, BackColor);
                g.AppendChild(node);

                if (Value > 0 && Maximum > Minimum)
                {
                    rect.Inflate(-1, -1);
                    rect.Width = (int)Math.Ceiling(rect.Width * (Math.Min(Maximum, Value) - Minimum) / (Maximum - Minimum));
                    if (rect.Width > 0 && rect.Height > 0)
                    {
                        node = shaper.GenerateSvg(rect, g, Color.Empty, Color);
                        g.AppendChild(node);
                    }
                }
            }

            // text
            if (ShowText)
            {
                string text = string.Format("{0}%", Value);
                Rectangle rect = Bounds;
                rect.Offset(translate);
                Blumind.Core.Exports.SvgEngine.ExportText(text, ForeColor, font, g, rect);
            }
        }*/

        public override void Serialize(XmlDocument dom, XmlElement node)
        {
            base.Serialize(dom, node);

            node.SetAttribute("max", Maximum.ToString());
            node.SetAttribute("min", Minimum.ToString());
            node.SetAttribute("value", Value.ToString());
            node.SetAttribute("show_text", ShowText.ToString());
            node.SetAttribute("auto_calculation", AutoCalculation.ToString());
            //node.SetAttribute("visible", Visible.ToString());
            if (!Color.IsEmpty)
                node.SetAttribute("color", ST.ToString(Color));
            if (!BackColor.IsEmpty)
                node.SetAttribute("back_color", ST.ToString(BackColor));
            if (!ForeColor.IsEmpty)
                node.SetAttribute("fore_color", ST.ToString(ForeColor));
        }

        public override void Deserialize(Version documentVersion, XmlElement node)
        {
            base.Deserialize(documentVersion, node);

            Maximum = ST.GetInt(node.GetAttribute("max"), Maximum);
            Minimum = ST.GetInt(node.GetAttribute("min"), Minimum);
            Value = ST.GetFloat(node.GetAttribute("value"), Value);
            AutoCalculation = ST.GetBool(node.GetAttribute("auto_calculation"), AutoCalculation);
            //Visible = ST.GetBoolean(node.GetAttribute("visible"), Visible);
            ShowText = ST.GetBool(node.GetAttribute("show_text"), ShowText);
            Color = ST.GetColor(node.GetAttribute("color"), Color);
            BackColor = ST.GetColor(node.GetAttribute("back_color"), BackColor);
            ForeColor = ST.GetColor(node.GetAttribute("fore_color"), ForeColor);
        }

        public override void CopyTo(Widget widget)
        {
            base.CopyTo(widget);
            if (widget is ProgressBarWidget)
            {
                ProgressBarWidget bar = (ProgressBarWidget)widget;
                bar.Maximum = Maximum;
                bar.Minimum = Minimum;
                bar.Value = Value;
                bar.Color = Color;
                bar.BackColor = BackColor;
                bar.ForeColor = ForeColor;
                //bar.Visible = Visible;
                bar.ShowText = ShowText;
            }
        }

        public IEnumerable<ExtendActionInfo> GetExtendActions(bool readOnly)
        {
            if (readOnly)
                return null;

            return new ExtendActionInfo[] 
            { 
                new ExtendActionInfo("Auto Calculate", Properties.Resources.calculator, (sender, e) => AutoCalculateValue()) 
            };
        }
    }
}
