using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SampleWebApplication
{
    public class BondObject
    {
        public string BondNumber;
        public string BondName;
        public string BondLimit;
        public string BondTerm;
        public DateTime BondEffectiveFrom;
        public string AccountNumber;
        public List<string> Bondlist;
    }
}