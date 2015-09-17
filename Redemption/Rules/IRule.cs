using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Redemption
{
    public interface IRule
    {
        bool ApplyRule(Multimedia multimedia);
    }
}
