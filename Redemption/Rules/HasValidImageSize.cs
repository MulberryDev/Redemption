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
            return (multimedia.Size.Height == 6000 && multimedia.Size.Width == 6000);
        }
    }
}
