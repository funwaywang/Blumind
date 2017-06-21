using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Blumind.Controls.Paint
{
    struct BezierControlPoint
    {
        int _Angle;
        int _Length;

        public BezierControlPoint(int angle, int length)
        {
            _Angle = angle;
            _Length = length;
        }

        public int Angle
        {
            get { return _Angle; }
            set { _Angle = value; }
        }

        public int Length
        {
            get { return _Length; }
            set { _Length = value; }
        }

        public static BezierControlPoint Empty
        {
            get { return new BezierControlPoint(); }
        }

        public bool IsEmpty
        {
            get { return _Angle == 0 && _Length == 0; }
        }
    }
}
