using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceConverter.Source.Entity
{
    public class Class
    {
        public string ClassName { get; set; }
        public string NameSpace { get; set; }
        public string NewNameSpace { get; set; }
        public string AccessType { get; set; }
        public ClassType Type { get; set; }
        public string BaseType { get; set; }
        public HashSet<string> UsingList { get; set; }

        public IList<Method> Methods { get; set; }
        public IList<Property> Properties { get; set; }

        public string FileLocation { get; set; }
    }   
}
