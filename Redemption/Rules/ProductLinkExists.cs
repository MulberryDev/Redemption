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
        public bool ApplyRule(Multimedia multimedia)
        {
            Database db = new Database("Database");
            return db.ExecuteScalar<int>("SELECT TOP 1 COUNT(ID) FROM Product WHERE code = @0", multimedia.Code.Substring(0, 14).Replace("_", "/")) == 1;
        }
    }
}
