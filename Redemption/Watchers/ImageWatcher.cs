using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Threading;

namespace Redemption
{
    public class ImageWatcher : Watcher
    {
        public ImageWatcher() : base(ConfigurationManager.AppSettings["sourceFolder"], ConfigurationManager.AppSettings["destinationFolder"]) { }

        protected override void OnChanged(object source, FileSystemEventArgs e)
        {
            if (!Directory.Exists(base.destinationPath))
                Directory.CreateDirectory(base.destinationPath);

            var ext = new List<string> {".jpg", ".gif", ".png", ".jpeg"};
            var files = Directory.GetFiles(base.sourcePath, "*", SearchOption.AllDirectories).Where(s => ext.Any(i => s.EndsWith(i)));

            foreach (string file in files)
            {
                int retry = 0;
                while (retry < 5)
                {
                    try
                    {
                        // Out of memory exception when handling multiple 6K by 6K images using Image.FromFile
                        FileInfo fileInfo = new FileInfo(file);
                        Multimedia multimedia = new Multimedia(fileInfo);
                        if (!multimedia.IsValid)
                        {
                            multimedia.RenamePhysicalFileToError();
                            retry = 5;
                            continue;
                        }
                        try
                        {
                            if (multimedia.Version > 0) multimedia.Archive();
                            multimedia.Save();
                        }
                        catch (IOException ex)
                        {
                            Logger.WriteLine("Failed to move a file in {0}: {1}", base.sourcePath, ex);
                        }
                        retry = 5;
                    }
                    catch (Exception ex)
                    {
                        Logger.WriteLine("An error occured trying to read or move the file, Retry Number:{0} : {1}", retry, ex);
                        Thread.Sleep(1000);
                        retry++;
                    }
                }
            }

            DeleteEmptyFolders(base.sourcePath);
        }

        private void DeleteEmptyFolders(string path)
        {
            try
            {
                foreach (var folder in Directory.EnumerateDirectories(path))
                {
                    DirectoryInfo folderInfo = new DirectoryInfo(folder);
                    foreach (var fileInfo in folderInfo.EnumerateFiles())
                    {
                        if (File.Exists(Path.Combine(path, fileInfo.Name))) File.Delete(Path.Combine(path, fileInfo.Name));
                        File.Move(fileInfo.FullName, Path.Combine(path, fileInfo.Name));
                    }

                    if (folderInfo.EnumerateDirectories().Any()) DeleteEmptyFolders(folder);

                    Directory.Delete(folder);
                }
            }
            catch (IOException ex)
            {
                Logger.WriteLine("An error occured while trying to delete empty folders in {0}: {1}", path, ex);
            }
        }
    }
}
