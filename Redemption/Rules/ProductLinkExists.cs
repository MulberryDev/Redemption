using PetaPoco;
using Redemption;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Redemption
{
    class ProductLinkExists : IRule
    {
        public bool ApplyRule(Multimedia multimedia, out string ruleMessage)
        {
            ruleMessage = "ProductNotExists";
            Database db = new Database("Database");
            return db.ExecuteScalar<int?>("SELECT TOP 1 ID FROM Product WHERE code = @0", multimedia.Name.Substring(0, 14).Replace("_", "/")) != null;
        }
    }
}
