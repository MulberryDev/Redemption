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
        public int MultimediaTypeID { get; set; }
        public string Code { get; set; }
        public string FilePath { get; set; }
        public int Version { get; set; }

        [PetaPoco.Ignore]
        public Size Size { get; set; }

        private FileInfo fileInfo;
        private string ruleMessage;

        public Multimedia()
        { 
        
        }

        public Multimedia(FileInfo fileInfo)
        {
            this.MultimediaTypeID = 1;
            this.fileInfo = fileInfo;
            this.Code = Path.GetFileNameWithoutExtension(fileInfo.Name);
            this.FilePath = Path.Combine(ConfigurationManager.AppSettings["destinationFolder"], fileInfo.Name);

            // Out of memory exception when handling multiple 6K by 6K images using Image.FromFile
            try
            {
                using (FileStream fs = new FileStream(fileInfo.FullName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    using (Image image = Image.FromStream(fs))
                        this.Size = new Size(image.Height, image.Width);
            }
            catch (Exception ex)
            {
                Logger.WriteLine("An error occured trying to read the image size of {0}: {1}", this.Code, ex);
            }
        }

        public bool ApplyRules()
        { 
            IRule[] rules = new IRule[] { new HasValidImageSize(), new CorrectNamingConvention(), new ProductLinkExists() };
            foreach (IRule rule in rules)
                if (rule.ApplyRule(this, out ruleMessage) == false)
                    return false;
  
            return true;
        }

        private void populateProductID()
        {
            Database db = new Database("Database");
            int? productID = db.ExecuteScalar<int?>("SELECT TOP 1 ID FROM Product WHERE code = @0", this.Code.Substring(0, 14).Replace("_", "/"));

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
                if (File.Exists(fileInfo.FullName + ruleMessage)) File.Delete(fileInfo.FullName + ruleMessage);
                File.Move(fileInfo.FullName, fileInfo.FullName + ruleMessage);
                return false;
            }

            populateProductID();
            populateVersionNumber();

            try
            {
                Database db = new Database("Database");

                if (this.Version != 0)
                {
                    Multimedia archiveMultimedia = db.SingleOrDefault<Multimedia>("SELECT ID, ProductID, MultimediaTypeID, Code, FilePath, Version FROM Multimedia WHERE code = @0", this.Code);
                    db.Delete(archiveMultimedia);
                    db.Insert("MultimediaArchive", "ID", false, archiveMultimedia);
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
