using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.IO;
using System.Configuration;
using System.Web;

namespace SCMS.Reports
{
    public class WkHtml2Pdf
    {
        private static byte[] GeneratePdfBytes(string url, List<String> options = null)
        {
            var fileName = " - ";
            var wkhtmlDir = ConfigurationManager.AppSettings["wkhtml"];
            var wkhtml = wkhtmlDir + "wkhtmltopdf.exe";

            var p = new Process();

            p.StartInfo.CreateNoWindow = true;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.RedirectStandardError = true;
            p.StartInfo.RedirectStandardInput = true;
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.FileName = wkhtml;
            p.StartInfo.WorkingDirectory = wkhtmlDir;

            //string footerFile = "file:///" + HttpContext.Current.Request.PhysicalApplicationPath + "/Content/report_templates/sections/general-footer.html?";

            string switches = "";
            switches += "--page-size Letter ";

            //String numOfCopies = "copy_count=1";

            if (options != null) {
                foreach (String opt in options)
                {
                    switches += " " + opt + " ";
                    //if (opt.Contains("--copies"))
                    //{
                    //    String tmp = opt.Trim();
                    //    tmp = tmp.Replace("--copies", "").Trim();

                    //    int copies = new int();

                    //    if (int.TryParse(tmp, out copies)) {
                    //        numOfCopies = "copy_count=" + copies;
                    //    }

                    //}
                }
            }


            //switches += " --footer-html " + footerFile + numOfCopies;

            p.StartInfo.Arguments = switches + " " + url + " " + fileName;
            p.Start();

            //read output
            byte[] buffer = new byte[32768];
            byte[] file;
            using (var ms = new MemoryStream())
            {
                while (true)
                {
                    int read = p.StandardOutput.BaseStream.Read(buffer, 0, buffer.Length);

                    if (read <= 0)
                    {
                        break;
                    }
                    ms.Write(buffer, 0, read);
                }
                file = ms.ToArray();
            }

            // wait or exit
            p.WaitForExit(60000);

            // read the exit code, close process
            int returnCode = p.ExitCode;
            p.Close();

            return file;
           // return returnCode == 0 ? file : null;
        }

        public static byte[] CreateReport(Dictionary<String, String> data, String HTMLTemplateFile, List<String> options=null) {

            var uri = new UriBuilder(HttpContext.Current.Request.Url.Scheme, HttpContext.Current.Request.Url.Host, HttpContext.Current.Request.Url.Port);

            string url = uri.ToString();

            string siteUrl = url;

            string templatePath = HttpContext.Current.Request.PhysicalApplicationPath
                    + "/Content/report_templates/" + HTMLTemplateFile;

            string tmpl = System.IO.File.ReadAllText(templatePath);

            string tmpFilename =Guid.NewGuid().ToString() + ".html";

            string filepath = HttpContext.Current.Request.PhysicalApplicationPath;
            filepath += "/Content/tmp_reports/" + tmpFilename;

            url += "Content/tmp_reports/" + tmpFilename;

            //tmpl = tmpl.Replace("{TODAY}", url);

            // add css
            tmpl = tmpl.Replace("{STYLE}", siteUrl + "Content/css/reporting.css");

            //add logos
            tmpl = tmpl.Replace("{LOGO1}", siteUrl + "Content/images/reports/drc.png");
            tmpl = tmpl.Replace("{LOGO2}", siteUrl + "Content/images/reports/ddg.png");

            foreach (var pair in data) {
                tmpl = tmpl.Replace(pair.Key, pair.Value);
            }

            System.IO.File.WriteAllText(filepath, tmpl);

            return WkHtml2Pdf.GeneratePdfBytes(url, options);
        }


        public static String CreatePersistedReport(Dictionary<String, String> data, String HTMLTemplateFile, List<String> options = null)
        {

            var uri = new UriBuilder(HttpContext.Current.Request.Url.Scheme, HttpContext.Current.Request.Url.Host, HttpContext.Current.Request.Url.Port);

            string url = uri.ToString();

            string siteUrl = url;

            string templatePath = HttpContext.Current.Request.PhysicalApplicationPath
                    + "/Content/report_templates/" + HTMLTemplateFile;

            string tmpl = System.IO.File.ReadAllText(templatePath);

            string tmpFilename = Guid.NewGuid().ToString() + ".html";

            string filepath = HttpContext.Current.Request.PhysicalApplicationPath;
            filepath += "/Content/tmp_reports/" + tmpFilename;

            url += "Content/tmp_reports/" + tmpFilename;

            //tmpl = tmpl.Replace("{TODAY}", url);

            // add css
            tmpl = tmpl.Replace("{STYLE}", siteUrl + "Content/css/reporting.css");

            //add logos
            tmpl = tmpl.Replace("{LOGO1}", siteUrl + "Content/images/reports/drc.png");
            tmpl = tmpl.Replace("{LOGO2}", siteUrl + "Content/images/reports/ddg.png");

            foreach (var pair in data)
            {
                tmpl = tmpl.Replace(pair.Key, pair.Value);
            }

            System.IO.File.WriteAllText(filepath, tmpl);

            byte[] pdf = WkHtml2Pdf.GeneratePdfBytes(url, options);

            tmpFilename = Guid.NewGuid().ToString() + ".pdf";

            String pdfUrl = HttpContext.Current.Request.PhysicalApplicationPath + "/Content/tmp_reports/" + tmpFilename;

            System.IO.File.WriteAllBytes(pdfUrl, pdf);

            return tmpFilename;

        }

    }
}
