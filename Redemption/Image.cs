using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Redemption
{
    class Image
    {
        public int ID { get; set; }
        public int Name { get; set; }

        public Image(int ID, int name)
        {
            this.ID = ID;
            this.Name = name;
        }
        public bool Save()
        {
            return true;
        }
    }
}
