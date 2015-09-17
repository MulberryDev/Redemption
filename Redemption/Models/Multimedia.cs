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
        public int? ProductID { get { return null; } }
        public string Code { get; set; }
        public string FilePath { get; set; }
        public int Version { get; set; }

        [PetaPoco.Ignore]
        public Size Size { get; set; }

        public Multimedia(FileInfo fileInfo)
        {
            this.Code = fileInfo.Name;
            this.FilePath = Path.Combine(ConfigurationManager.AppSettings["destinationFolder"], fileInfo.Name);

            using (Image image = new Bitmap(fileInfo.FullName))
                this.Size = new Size(image.Height, image.Width);
        }

        public bool ApplyRules()
        { 
            IRule[] rules = new IRule[] { new HasValidImageSize(), new CorrectNamingConvention() };
            foreach (IRule rule in rules)
                if (rule.ApplyRule(this) == false) return false;
  
            return true;
        }

        public bool Save()
        {
            if (!this.ApplyRules()) return false;

            try
            {
                Database db = new Database("Database");
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
