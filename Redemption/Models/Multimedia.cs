using PetaPoco;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Redemption
{
    public class Multimedia
    {
        public int ID { get; set; }
        public int? ProductID { get { return null; } }
        public string Code { get; set; }
        public string FilePath { get; set; }
        public int Version { get; set; }

        public Multimedia(string code, string filePath)
        {
            this.Code = code;
            this.FilePath = filePath;
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
            Database db = new Database("Database");
            db.Query<Multimedia>("SELECT * FROM Multimedia");
            return true;
        }
    }
}
