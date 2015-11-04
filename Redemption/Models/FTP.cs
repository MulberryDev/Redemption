using Redemption;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary1.Models
{
    class FTP
    {
        private static readonly string ftpServer = "ftp://162.13.68.153/apiImages/";
        private static readonly string ftpUsername = "mulberryftp";
        private static readonly string ftpPassword = "33ZqgU5WuqcXaQx";


        public static bool UploadFile(Multimedia multimedia)
        {
            using (WebClient client = new WebClient())
            {
                client.Credentials = new NetworkCredential(ftpUsername, ftpPassword);
                try
                {
                    client.UploadFile(ftpServer + multimedia.FileName, "STOR", Path.Combine(ConfigurationManager.AppSettings["destinationFolder"], multimedia.FilePath, multimedia.FileName));
                    return true;
                }
                catch (Exception ex)
                {
                    Logger.WriteLine(ex.Message);
                    return false;
                }
            }
        }
    }
}
