using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Redemption
{
    public class Multimedia
    {
        public int productID { get; set; }
        public int multimediaTypeID { get; set; }
        public string Code { get; set; }
        public string FilePath { get; set; }
        public Size Size { get; set; }
        public int Version { get; set; }

        public Multimedia(string code, string filePath)
        {
            this.Code = code;
            this.FilePath = filePath;
        }

        private bool ApplyRules()
        { 
            IRule[] rules = new IRule[] { new HasValidImageSize() };
            foreach (IRule rule in rules)
                if (rule.ApplyRule(this) == false) return false;

            return true;
        }

        public bool Save()
        {
            return true;
        }
    }
}
