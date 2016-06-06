using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.IO;
using System.Web;

namespace SCMS.Utils.File
{
    public class ImageUtility
    {
        public static Bitmap GetLogosImage()
        {
            string logoPath = HttpContext.Current.Request.PhysicalApplicationPath
                    + "Content/reports/logos-90.png";

            return new Bitmap(logoPath);
        }


        public static int Pixel2MTU(int pixels)
        {
            int mtus = pixels * 9525;
            return mtus;
        }
    }
}
