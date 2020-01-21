using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace VeloNSK.HelpClass.Style
{
    class Size
    {
        public double GetHeightSize()
        {
            return Device.Info.ScaledScreenSize.Height;
        }
        public double GetWidthSize()
        {
            return Device.Info.ScaledScreenSize.Width;
        }
    }
}
