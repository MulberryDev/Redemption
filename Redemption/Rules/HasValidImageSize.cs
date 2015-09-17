using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Redemption
{
    public class HasValidImageSize : IRule
    {
        public bool ApplyRule(Multimedia multimedia)
        {
            return (true);
        }
    }
}
