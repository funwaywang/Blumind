using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace Blumind.Canvas.Svg
{
    class SvgGraphicsPath : IGraphicsPath
    {
        enum Command
        {
            M, // moveto
            L, // lineto
            H, // horizontal lineto
            V, // vertical lineto
            C, // curveto
            S, // smooth curveto
            Q, // quadratic Bézier curve
            T, // smooth quadratic Bézier curveto
            A, // elliptical Arc
            Z, // closepath
        }

        class Node
        {
            public Node(Command command)
            {
                Command = command;
            }

            public Node(Command command, Point point)
            {
                Command = command;
                Points = new Point[] { point };
            }

            public Node(Command command, IEnumerable<Point> points)
            {
                Command = command;
                Points = points.ToArray();
            }

            public Command Command { get; private set; }

            public Point[] Points { get; private set; }
        }

        List<Node> Nodes = new List<Node>();

        public SvgGraphicsPath()
        {
        }

        public object Raw
        {
            get { return this; }
        }

        public void StartFigure()
        {
        }

        public void CloseFigure()
        {
            Nodes.Add(new Node(Command.Z));
        }

        public void AddBezier(Point point1, Point point2, Point point3, Point point4)
        {
            Nodes.Add(new Node(Command.M, point1));
            Nodes.Add(new Node(Command.C, new Point[] { point2, point3, point4 }));
        }

        public void AddLine(Point point1, Point point2)
        {
            Nodes.Add(new Node(Command.M, point1));
            Nodes.Add(new Node(Command.L, point2));
        }

        public void MoveTo(Point point)
        {
            Nodes.Add(new Node(Command.M, point));
        }
        
        public void LineTo(Point point)
        {
            Nodes.Add(new Node(Command.L, point));
        }

        public string Render(System.Xml.XmlElement element)
        {
            var sb = new StringBuilder();
            if (Nodes.Count > 0)
            {
                foreach (var n in Nodes)
                {
                    if (sb.Length > 0)
                        sb.Append(" ");

                    sb.Append(n.Command.ToString());
                    if (n.Points != null)
                    {
                        foreach (var p in n.Points)
                        {
                            sb.AppendFormat("{0},{1} ", p.X, p.Y);
                        }
                    }
                }
            }
            return sb.ToString();
        }
    }
}
