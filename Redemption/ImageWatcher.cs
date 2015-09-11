using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Redemption
{
    class ImageWatcher : Watcher
    {
        public ImageWatcher() : base(ConfigurationManager.AppSettings["sourceFolder"], ConfigurationManager.AppSettings["destinationFolder"]) { }

        protected override void OnChanged(object source, FileSystemEventArgs e)
        {
            if (!Directory.Exists(base.destinationPath))
                Directory.CreateDirectory(base.destinationPath);

            string[] files = Directory.GetFiles(base.sourcePath);

            foreach (string file in files)
            {
                FileInfo fileInfo = new FileInfo(file);
                string fullTargetPath = Path.Combine(base.destinationPath, fileInfo.Name);
                try
                {
                    if (File.Exists(fullTargetPath)) File.Delete(fullTargetPath);
                    File.Move(file, fullTargetPath);
                }
                catch (Exception ex)
                {
                    Logger.WriteLine("Failed to move a file in {0}: {1}", base.sourcePath, ex);
                }

                Logger.WriteLine("Found: {0}, Moved To: {1}", file, Path.Combine(base.destinationPath, fileInfo.Name));
            }
        }
    }
}
