using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Redemption
{
    public class Size
    {
        private int height;
        public int Height { get { return this.height; } }

        private int width;
        public int Width { get { return this.width; } }

        public Size(int height, int width)
        {
            this.height = height;
            this.width = width;
        }
    }
}
