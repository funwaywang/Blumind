using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace Blumind.Controls.Paint
{
    class BezierHelper
    {
        public static BezierLayoutInfo Layout(Point pts, Point ptd, BezierControlPoint controlPoint1, BezierControlPoint controlPoint2)
        {
            var layout = new BezierLayoutInfo();
            Point cp1, cp2;

            if (pts == ptd)
            {
                cp1 = pts;
                cp2 = ptd;

                layout.Bounds = Rectangle.Empty;
                layout.Region = null;
            }
            else
            {
                cp1 = GetControlPoint(pts, layout.CP1);
                cp2 = GetControlPoint(ptd, layout.CP2);

                //Point[] controlPoints = GetControlPoints(pts, ptd);

                /*Shape sShape = Shape.GetShaper(From);
                pts = sShape.GetBorderPoint(From.Bounds, cp1);

                Shape dShape = Shape.GetShaper(Target);
                ptd = sShape.GetBorderPoint(Target.Bounds, cp2);

                GraphicsPath gp = new GraphicsPath();
                gp.AddBezier(pts, cp1, cp2, ptd);
                Pen penWiden = new Pen(Color.Black, LineWidth + 5);
                gp.Widen(penWiden);

                RectangleF rect = gp.GetBounds();
                rect.Inflate(LineWidth, LineWidth);
                layout.Bounds = rect;
                layout.Region = new Region(gp);*/
            }

            Point ptCenter = BezierHelper.GetPoint(pts, cp1, cp2, ptd, 0.5f);
            Rectangle textBounds = layout.TextBounds;
            textBounds.X = ptCenter.X - layout.TextBounds.Width / 2;
            textBounds.Y = ptCenter.Y - layout.TextBounds.Height / 2;

            // cache layout info
            //layout.StartBounds = rect1;
            //layout.EndBounds = rect2;
            layout.StartPoint = pts;
            layout.EndPoint = ptd;
            layout.ControlPoint1 = cp1;
            layout.ControlPoint2 = cp2;
            layout.TextBounds = textBounds;
            return layout;
        }

        public static Point GetControlPoint(Point orgPoint, BezierControlPoint point)
        {
            return GetControlPoint(orgPoint, point.Angle, point.Length);
        }

        public static Point GetControlPoint(Point orgPoint, int angle, int length)
        {
            double a = angle * Math.PI / 180;
            return new Point(orgPoint.X + (int)Math.Ceiling(Math.Cos(a) * length),
                orgPoint.Y + (int)Math.Ceiling(Math.Sin(a) * length));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="startPoint"></param>
        /// <param name="controlPoint1"></param>
        /// <param name="controlPoint2"></param>
        /// <param name="endPoint"></param>
        /// <param name="width">线条宽度</param>
        /// <param name="samples">采样点数</param>
        /// <returns></returns>
        public static Region GetBezierUpdateRegion(Point startPoint, Point controlPoint1, Point controlPoint2, Point endPoint
            , int width = 1, int samples = 10)
        {
            float step = 1f;
            if (samples > 0)
                step = 1.0f / samples;

            var pts = new PointF[Math.Max(1, samples) + 1];
            int index = 0;
            for (float t = 0; t <= 1.0f; t += step)
            {
                var pt = GetPointF(startPoint, controlPoint1, controlPoint2, endPoint, t);
                pts[index++] = pt;
            }

            pts[pts.Length - 1] = new PointF(endPoint.X, endPoint.Y);
            return GetUpdateRegion(pts, width);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="startPoint"></param>
        /// <param name="endPoint"></param>
        /// <param name="width">线条宽度</param>
        /// <param name="samples">采样点数</param>
        /// <returns></returns>
        public static Region GetBezierUpdateRegion(Point startPoint, Point endPoint, int width = 1, int samples = 10)
        {
            if (startPoint.X == endPoint.X)
            {
                return new Region(
                    new Rectangle(startPoint.X - width,
                        Math.Min(startPoint.Y, endPoint.Y) - width,
                        width * 2, 
                        Math.Max(startPoint.Y, endPoint.Y) + width));
            }
            else if (startPoint.Y == endPoint.Y)
            {
                return new Region(
                    new Rectangle(Math.Min(startPoint.X, endPoint.X) - width,
                        startPoint.Y - width,
                        Math.Max(startPoint.X, endPoint.X) + width,
                        width * 2));
            }
            else
            {
                float step = 1f;
                if (samples > 0)
                    step = 1.0f / samples;

                var pts = new PointF[Math.Max(1, samples) + 1];
                int index = 0;
                for (float t = 0; t <= 1.0f; t += step)
                {
                    var pt = GetPointF(startPoint, endPoint, t);
                    pts[index++] = pt;
                }

                pts[pts.Length - 1] = new PointF(endPoint.X, endPoint.Y);
                return GetUpdateRegion(pts, width);
            }
        }

        static Region GetUpdateRegion(PointF[] pts, int width)
        {
            if (pts == null || pts.Length == 0)
                return null;

            width = Math.Max(1, width);
            Region region = null;
            for (int i = 1; i < pts.Length; i++)
            {
                var rectblock = PaintHelper.GetRectangle(pts[i - 1], pts[i]);
                rectblock.Inflate(width, width);
                if (region == null)
                    region = new Region(rectblock);
                else
                    region.Union(rectblock);
            }

            return region;
        }

        public static Region GetBezierUpdateRegionWidthHandles(Point startPoint, Point controlPoint1, Point controlPoint2, Point endPoint, int width = 1, int sample = 10)
        {
            var region = GetBezierUpdateRegion(startPoint, controlPoint1, controlPoint2, endPoint, width, sample);

            if (region != null)
            {
                region.Union(GetBezierUpdateRegion(startPoint, controlPoint1));
                region.Union(GetBezierUpdateRegion(endPoint, controlPoint2));
            }

            return region;
        }

        public static PointF GetPointF(Point startPoint, Point controlPoint1, Point controlPoint2, Point endPoint, float t)
        {
            var p = new Point[] { startPoint, controlPoint1, controlPoint2, endPoint };

            t = Math.Max(0.0f, Math.Min(1.0f, t));
            var x = Math.Pow(1 - t, 3) * p[0].X + 3 * p[1].X * t * Math.Pow(1 - t, 2) + 3 * p[2].X * Math.Pow(t, 2) * (1 - t) + p[3].X * Math.Pow(t, 3);
            var y = Math.Pow(1 - t, 3) * p[0].Y + 3 * p[1].Y * t * Math.Pow(1 - t, 2) + 3 * p[2].Y * Math.Pow(t, 2) * (1 - t) + p[3].Y * Math.Pow(t, 3);

            return new PointF((float)x, (float)y);
        }

        public static Point GetPoint(Point startPoint, Point controlPoint1, Point controlPoint2, Point endPoint, float t)
        {
            var pt = GetPointF(startPoint, controlPoint1, controlPoint2, endPoint, t);
            return Point.Round(pt);
        }

        public static PointF GetPointF(Point startPoint, Point endPoint, float t)
        {
            var p = new Point[] { startPoint, endPoint };

            t = Math.Max(0.0f, Math.Min(1.0f, t));
            var x = (1 - t) * p[0].X + t * p[1].X;
            var y = (1 - t) * p[0].X + t * p[1].X;

            return new PointF((float)x, (float)y);
        }

        public static Point GetPoint(Point startPoint, Point endPoint, float t)
        {
            var pt = GetPointF(startPoint, endPoint, t);
            return Point.Round(pt);
        }
    }
}
