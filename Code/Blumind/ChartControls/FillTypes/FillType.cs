using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using Blumind.Canvas;

namespace Blumind.ChartControls.FillTypes
{
    abstract class FillType
    {
        static FillType()
        {
            DefaultFillType = new Solid();

            GeneralFillTypes = new Dictionary<string, FillType>(StringComparer.OrdinalIgnoreCase);
            GeneralFillTypes.Add("Solid", DefaultFillType);
            GeneralFillTypes.Add("Modern", new Modern());
        }

        public static FillType DefaultFillType { get; private set; }

        public static Dictionary<string, FillType> GeneralFillTypes { get; private set; }

        public abstract IBrush CreateBrush(IGraphics graphics, Color backColor, Rectangle rectangle);
        
        public static FillType GetFillType(string name)
        {
            if (name != null && GeneralFillTypes.ContainsKey(name))
                return GeneralFillTypes[name];
            else
                return DefaultFillType;
        }
    }
}
