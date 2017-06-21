using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace Blumind.Configuration
{
    class RecentFileInfo
    {
        public RecentFileInfo()
        {
        }

        public RecentFileInfo(string filename)
        {
            Filename = filename;
        }

        public RecentFileInfo(string filename, Image thumbImage)
        {
            Filename = filename;
            ThumbImage = thumbImage;
        }

        public string Filename { get; set; }

        public Image ThumbImage { get; set; }

        public Image Icon { get; set; }

        public string Description { get; set; }
    }
}
