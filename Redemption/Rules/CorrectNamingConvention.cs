using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Redemption
{
    class CorrectNamingConvention : IRule
    {
        public bool ApplyRule(Multimedia multimedia, out string ruleMessage)
        {
            ruleMessage = "NameNotValid";
            if (multimedia.Name.Length != 14 && multimedia.Name.Length != 16) return false;
            return new Regex("^[A-Z]{2}[0-9]{4}_[0-9]{3}[A-Z]{1}[0-9]{3}(_[2-9]|_L)?$").IsMatch(multimedia.Name);
        }
    }
}
