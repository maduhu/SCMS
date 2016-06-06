using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SCMS.CoreBusinessLogic.Web
{
    public interface IImageService
    {
        bool IsImage(Stream stream);
    }
}
