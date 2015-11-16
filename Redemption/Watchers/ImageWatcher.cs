using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Threading;
using ClassLibrary1.Models;
using System.Collections.ObjectModel;

namespace Redemption
{
    public class ImageWatcher : Watcher
    {
        public ImageWatcher() : base(ConfigurationManager.AppSettings["sourceFolder"], ConfigurationManager.AppSettings["destinationFolder"]) { }
        private List<string> ext = new List<string> { ".jpg", ".gif", ".png", ".jpeg" };
        private List<string> ruleNames = new List<string> { "ProductNotExists", "SizeNotValid", "NameNotValid" };
        private IEnumerable<string> files;

        protected override void OnChanged(object source, FileSystemEventArgs e)
        {
            Helper.Retry(() => PopulateImageList(), TimeSpan.FromSeconds(1));
            if (files != null || files.Count() != 0) ProcessImageList();
        }

        private void PopulateImageList()
        {
            files = Directory.GetFiles(base.sourcePath, "*", SearchOption.AllDirectories).Where(s => ext.Any(i => s.EndsWith(i)) && !ruleNames.Any(x => s.Contains(x)));
        }

        private void ProcessImageList()
        {
            Parallel.ForEach<string>(files, file => Helper.Retry(() => ProcessImage(file), TimeSpan.FromSeconds(1)));

            PopulateImageList();
            if (files.Count() != 0) ProcessImageList();

            DeleteEmptyFolders(base.sourcePath);
        }

        private void ProcessImage(string file)
        {
            FileInfo fileInfo = new FileInfo(file);
            Multimedia multimedia = new Multimedia(fileInfo);
            if (!multimedia.IsValid)
            {
                multimedia.RenamePhysicalFileToError();
                return;
            }

            if (multimedia.Version > 0) multimedia.Archive();
            multimedia.Save();
        }

        private void DeleteEmptyFolders(string path)
        {
            try
            {
                foreach (var folder in Directory.EnumerateDirectories(path))
                {
                    DirectoryInfo folderInfo = new DirectoryInfo(folder);

                    foreach (var fileInfo in folderInfo.EnumerateFiles().Where(x => ext.Any(i => x.Extension != i)))
                    {
                        if (File.Exists(Path.Combine(path, fileInfo.Name))) File.Delete(Path.Combine(path, fileInfo.Name));
                        File.Move(fileInfo.FullName, Path.Combine(path, fileInfo.Name));
                    }

                    if (folderInfo.EnumerateFiles().Any()) return;
                    if (folderInfo.EnumerateDirectories().Any()) DeleteEmptyFolders(folder);

                    Directory.Delete(folder);
                    DeleteEmptyFolders(folder);
                }
            }
            catch (IOException ex)
            {
                Logger.WriteLine("An error occured while trying to delete empty folders in {0}: {1}", path, ex);
            }
        }
    }
}
