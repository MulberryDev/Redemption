using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace Redemption
{
    public abstract class Watcher
    {
        private FileSystemWatcher fileSystemWatcher;
        protected string sourcePath;
        protected string destinationPath;

        public Watcher(string sourcePath, string destinationPath)
        {
            this.sourcePath = sourcePath;
            this.destinationPath = destinationPath;
            try
            {
                if (!Directory.Exists(sourcePath)) Directory.CreateDirectory(sourcePath);
            }
            catch (IOException ex)
            {
                Logger.WriteLine("Unable to create source directory at {0}: {1}", sourcePath, ex);
                return;
            }

            fileSystemWatcher = new FileSystemWatcher();
            fileSystemWatcher.Path = sourcePath;
            fileSystemWatcher.NotifyFilter = NotifyFilters.LastAccess
                                           | NotifyFilters.LastWrite
                                           | NotifyFilters.FileName
                                           | NotifyFilters.DirectoryName;

            fileSystemWatcher.Changed += new FileSystemEventHandler(OnChanged);
            fileSystemWatcher.Created += new FileSystemEventHandler(OnChanged);
            fileSystemWatcher.Deleted += new FileSystemEventHandler(OnChanged);
            fileSystemWatcher.EnableRaisingEvents = true;
        }

        protected abstract void OnChanged(object source, FileSystemEventArgs e);

        public void Stop()
        {
            fileSystemWatcher.EnableRaisingEvents = false;
            fileSystemWatcher.Dispose();
        }
    }
}
