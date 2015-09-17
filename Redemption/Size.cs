using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Redemption
{
    public class Size
    {
        public int Height { get; set; }
        public int Width { get; set; }

        public Size(int height, int width)
        {
            this.Height = height;
            this.Width = width;
        }
    }
}
