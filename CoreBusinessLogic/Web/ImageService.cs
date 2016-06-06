using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;

namespace SCMS.CoreBusinessLogic.Web
{
    public class ImageService : IImageService
    {
        public bool IsImage(Stream stream)
        {
            try
            {
                var image = Image.FromStream(stream);
                image.Dispose();
                return true;
            }
            catch (Exception) { }
            return false;
        }
    }
}
