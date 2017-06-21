using System;
using System.Collections;
using System.Collections.Generic;

namespace Blumind.Model
{
    class PictureLibrary : Dictionary<string, Picture>
    {
        public PictureLibrary()
            : base(StringComparer.OrdinalIgnoreCase)
        {
        }
    }
}
