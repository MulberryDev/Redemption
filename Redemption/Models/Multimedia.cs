using ImageMagick;
using PetaPoco;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Redemption
{
    [PetaPoco.TableName("Multimedia")]
    [PetaPoco.PrimaryKey("ID")]
    public class Multimedia
    {
        public int ID { get; set; }
        public int? ProductID { get; set; }
        public int MultimediaTypeID { get; set; }
        public string Name { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public int Version { get; set; }
        public bool IsActive { get; set; }

        [PetaPoco.Ignore]
        public Size Size { get; set; }

        [PetaPoco.Ignore]
        public bool IsValid { get; private set; }

        private FileInfo fileInfo;
        private string ruleMessage;

        public Multimedia()
        {  
        }

        public Multimedia(FileInfo fileInfo)
        {
            this.MultimediaTypeID = 1;
            this.fileInfo = fileInfo;
            this.Name = Path.GetFileNameWithoutExtension(fileInfo.Name);
            this.FileName = fileInfo.Name;
            this.IsActive = true;
            populateProductID();
            populateVersionNumber();
            // Requires version number to be populated before calculation.
            this.FilePath = Path.Combine(fileInfo.Name.Substring(0, 2), fileInfo.Name.Substring(2, 4), fileInfo.Name.Substring(7, 3), fileInfo.Name.Substring(10, 4), this.Version.ToString());
            this.IsValid = false;

            int retry = 0;
            while (retry < 5)
            {
                try
                {
                    // Out of memory exception when handling multiple 6K by 6K images using Image.FromFile
                    using (FileStream fs = new FileStream(fileInfo.FullName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                        using (Image image = Image.FromStream(fs))
                            this.Size = new Size(image.Height, image.Width);
                    retry = 5;
                }
                catch (IOException ex)
                {
                    Logger.WriteLine("An error occured trying to read the image size of {0}, Retry Number:{1} : {2}", this.Name, retry, ex);
                    Thread.Sleep(1000);
                    retry++;
                }
                catch (ArgumentException ex)
                { 
                    Logger.WriteLine("An argument exception occured trying to read the image size of {0}", this.Name, retry, ex);
                    this.ruleMessage = "NameNotValid";
                    return;
                }
            }

            populateProductID();
            populateVersionNumber();

            this.ApplyRules();
        }

        private void ApplyRules()
        { 
            IRule[] rules = new IRule[] { new HasValidImageSize(), new CorrectNamingConvention(), new ProductLinkExists() };
            foreach (IRule rule in rules)
                if (!rule.ApplyRule(this, out ruleMessage))
                {
                    this.IsValid = false;
                    return;
                }

            this.IsValid = true;
        }

        private void populateProductID()
        {
            Database db = new Database("Database");
            int? productID = db.ExecuteScalar<int?>("SELECT TOP 1 ID FROM Product WHERE code = @0", this.Name.Substring(0, 14).Replace("_", "/"));

            this.ProductID = productID;
        }

        private void populateVersionNumber()
        { 
            Database db = new Database("Database");
            int versionCount = db.ExecuteScalar<int>("SELECT (SELECT COUNT(ID) FROM Multimedia WHERE name = @0) + (SELECT COUNT(ID) FROM MultimediaArchive WHERE name = @0)", this.Name);

            this.Version = (versionCount != 0) ? versionCount : 0;
        }

        public void RenamePhysicalFileToError()
        {
            if (File.Exists(Path.Combine(ConfigurationManager.AppSettings["sourceFolder"], fileInfo.Name + ruleMessage))) File.Delete(Path.Combine(ConfigurationManager.AppSettings["sourceFolder"], fileInfo.Name + ruleMessage));
            File.Move(fileInfo.FullName, Path.Combine(ConfigurationManager.AppSettings["sourceFolder"], fileInfo.Name + ruleMessage));
        }

        public bool Archive()
        {
            try
            {
                Database db = new Database("Database");

                foreach(var archiveMultimedia in db.Query<Multimedia>("SELECT ID, ProductID, MultimediaTypeID, Name, FileName, FilePath, Version FROM Multimedia WHERE Name = @0", this.Name))
                {
                   archiveMultimedia.IsActive = false;
                   new Database("Database").Update(archiveMultimedia);
                }
             
                return true;
            }
            catch (Exception ex)
            {
                Logger.WriteLine("Failed to archive file {0}: {1}", this.Name, ex);
                return false;
            }
        }

        public bool Save()
        {
            try
            {
                Database db = new Database("Database");
                
                db.Insert(this);

                string destPath = Path.Combine(ConfigurationManager.AppSettings["destinationFolder"], this.FilePath, this.FileName);
                if (!Directory.Exists(Path.GetDirectoryName(destPath))) Directory.CreateDirectory(Path.GetDirectoryName(destPath));
                fileInfo.MoveTo(destPath);
                // Read from file
                using (MagickImage image = new MagickImage(destPath))
                {
                    MagickGeometry size = new MagickGeometry(200, 200);
                    size.IgnoreAspectRatio = true;
                    image.Resize(size);
                    image.Write(destPath + "_small");
                }

                using (MagickImage image = new MagickImage(destPath))
                {
                    MagickGeometry size = new MagickGeometry(800, 800);
                    size.IgnoreAspectRatio = true;
                    image.Resize(size);
                    image.Write(destPath + "_medium");
                }


                return true;
            }
            catch (Exception ex)
            {
                Logger.WriteLine("Could not insert {0} into the database: {1}", this.Name, ex);
                return false;
            }
        }
    }
}
