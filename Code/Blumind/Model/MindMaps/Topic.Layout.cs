using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using Blumind.Configuration;
using Blumind.Core;
using Blumind.Globalization;

namespace Blumind.Model.MindMaps
{
    partial class Topic
    {
        int? _CustomWidth;
        int? _CustomHeight;
        //Rectangle _TextBounds;

        [DefaultValue(null), LocalDisplayName("Custom Width"), LocalCategory("Layout")]
        public int? CustomWidth
        {
            get { return _CustomWidth; }
            set
            {
                if (_CustomWidth != value)
                {
                    _CustomWidth = value;
                    OnWidthChanged();
                }
            }
        }

        [DefaultValue(null), LocalDisplayName("Custom Height"), LocalCategory("Layout")]
        public int? CustomHeight
        {
            get { return _CustomHeight; }
            set
            {
                if (_CustomHeight != value)
                {
                    _CustomHeight = value;
                    OnHeightChanged();
                }
            }
        }

        [Browsable(false)]
        public Rectangle TextBounds { get; set; }

        /// <summary>
        /// 已经经过坐标转换
        /// 相对 Topic 左上角
        /// </summary>
        [Browsable(false)]
        public Rectangle RemarkIconBounds
        {
            get;
            set;
            //get
            //{
            //    const int IconSize = 16;
            //    const int Space = 2;

            //    return new Rectangle(0,
            //        Height + Space,
            //        IconSize,
            //        IconSize);
            //}
        }

        [Browsable(false)]
        public Rectangle FullBounds
        {
            get
            {
                var rect = Bounds;
                if (this.HaveRemark && Options.Current.GetBool(Blumind.Configuration.OptionNames.Charts.ShowRemarkIcon))
                {
                    rect = Rectangle.Union(rect, RemarkIconBounds);
                }
                return rect;
            }
        }

        [Browsable(false)]
        public Rectangle FoldingButton { get; set; }

        [Browsable(false)]
        public bool FoldingButtonVisible { get; set; }
    }
}
