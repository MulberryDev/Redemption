using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Redemption
{
    class CorrectNamingConvention : IRule
    {
        public bool ApplyRule(Multimedia multimedia)
        {
            return true;
            // Check with RegEx if naming convention is right
        }
    }
}
