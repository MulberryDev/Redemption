﻿using System;
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

            var ext = new List<string> {".jpg", ".gif", ".png"};
            var files = Directory.GetFiles(base.sourcePath, "*", SearchOption.AllDirectories).Where(s => ext.Any(i => s.EndsWith(i)));

            foreach (string file in files)
            {
                FileInfo fileInfo = new FileInfo(file);
                string fullTargetPath = Path.Combine(base.destinationPath, fileInfo.Name);

                Multimedia multimedia = new Multimedia(fileInfo);

                if (!multimedia.Save()) continue;

                try
                {
                    if (File.Exists(fullTargetPath)) File.Delete(fullTargetPath);
                    File.Move(file, fullTargetPath);
                }
                catch (IOException ex)
                {
                    Logger.WriteLine("Failed to move a file in {0}: {1}", base.sourcePath, ex);
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
                    if (folderInfo.EnumerateFiles().Any()) return;
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