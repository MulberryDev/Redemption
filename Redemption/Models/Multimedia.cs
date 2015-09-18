using PetaPoco;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Redemption
{
    [PetaPoco.TableName("Multimedia")]
    [PetaPoco.PrimaryKey("ID")]
    public class Multimedia
    {
        public int ID { get; set; }
        public int? ProductID { get; set; }
        public string Code { get; set; }
        public string FilePath { get; set; }
        public int Version { get; set; }

        [PetaPoco.Ignore]
        public Size Size { get; set; }

        private FileInfo fileInfo;

        public Multimedia()
        { 
        
        }

        public Multimedia(FileInfo fileInfo)
        {
            this.fileInfo = fileInfo;
            this.ProductID = null;
            this.Code = fileInfo.Name;
            this.FilePath = Path.Combine(ConfigurationManager.AppSettings["destinationFolder"], fileInfo.Name);

            using (Image image = Image.FromFile(fileInfo.FullName))
                this.Size = new Size(image.Height, image.Width);
        }

        public bool ApplyRules()
        { 
            IRule[] rules = new IRule[] { new HasValidImageSize(), new CorrectNamingConvention(), new ProductLinkExists() };
            foreach (IRule rule in rules)
                if (rule.ApplyRule(this) == false) return false;
  
            return true;
        }

        private void populateProductID()
        {
            Database db = new Database("Database");
            int? productID = db.ExecuteScalar<int?>("SELECT TOP 1 ID FROM Product WHERE code = @0", this.Code);

            this.ProductID = productID;
        }

        private void populateVersionNumber()
        { 
            Database db = new Database("Database");
            int versionCount = db.ExecuteScalar<int>("SELECT (SELECT COUNT(ID) FROM Multimedia WHERE code = @0) + (SELECT COUNT(ID) FROM MultimediaArchive WHERE code = @0)", this.Code);

            this.Version = (versionCount != 0) ? versionCount : 0;
        }

        public bool Save()
        {
            if (!this.ApplyRules())
            {
                if (File.Exists(fileInfo.FullName + ".bad")) File.Delete(fileInfo.FullName + ".bad");
                File.Move(fileInfo.FullName, fileInfo.FullName + ".bad");
                return false;
            }

            populateProductID();
            populateVersionNumber();

            try
            {
                Database db = new Database("Database");

                if (this.Version != 0)
                {
                    Multimedia archiveMultimedia = db.SingleOrDefault<Multimedia>("SELECT ID, ProductID, Code, FilePath, Version FROM Multimedia WHERE code = @0", this.Code);
                    db.Insert("MultimediaArchive", "ID", archiveMultimedia);
                    db.Delete(archiveMultimedia);
                }
              
                db.Insert(this);
                return true;
            }
            catch (Exception ex)
            {
                Logger.WriteLine("Could not insert {0} into the database: {1}", this.Code, ex);
                return false;
            }
        }
    }
}
