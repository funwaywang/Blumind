using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using Blumind.Core;
using Blumind.Core.Settings;

namespace Blumind.Controls
{
    class ColorBackImage
    {
        private Image OrgImage;
        private Image _Image;

        public ColorBackImage(Image orgImage)
        {
            OrgImage = orgImage;

            _Options.Current.UISettingChanged += new EventHandler(Options_UISettingChanged);
        }

        public Image Image
        {
            get 
            {
                if (_Image == null && OrgImage != null)
                {
                    GeneralImage();
                }

                return _Image; 
            }

            private set { _Image = value; }
        }

        private void GeneralImage()
        {
            if (OrgImage == null)
                return;

            //Image image = OrgImage;
            //if (_Options.Current.Appearance.PreferredBackColor.IsEmpty)
            //{
            //    Image = image;
            //}
            //else
            //{
            //    Image = PaintHelper.AdjustImageColor(image, _Options.Current.Appearance.StandardBackColor, _Options.Current.Appearance.PreferredBackColor);
            //}
        }

        private void Options_UISettingChanged(object sender, EventArgs e)
        {
            if (_Image != null)
            {
                if (_Image != OrgImage)
                    _Image.Dispose();
                _Image = null;
            }
        }
    }
}
