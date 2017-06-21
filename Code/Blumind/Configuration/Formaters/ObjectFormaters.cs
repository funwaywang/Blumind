using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Blumind.Configuration.Formaters
{
    static class ObjectFormaters
    {
        static List<IObjectFormater> Formaters;

        static ObjectFormaters()
        {
            Formaters = new List<IObjectFormater>();

            Formaters.Add(new FontFormater());
        }

        public static IObjectFormater GetFormater(Type type)
        {
            foreach (var f in Formaters)
                if (f.SupportType(type))
                    return f;

            return null;
        }

        public static IObjectFormater GetFormater(string typeName)
        {
            foreach (var f in Formaters)
            {
                if (StringComparer.OrdinalIgnoreCase.Equals(f.Name, typeName))
                    return f;
            }

            return null;
        }
    }
}
