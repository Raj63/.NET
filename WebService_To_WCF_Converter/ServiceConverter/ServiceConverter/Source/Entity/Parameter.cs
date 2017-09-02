using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceConverter.Source.Entity
{
    public class Parameter
    {
        public string Name { get; set; }
        public string TypeFullName { get; set; }
        public string Type { get ; set; }

        public bool IsInputParameter { get; set; }
        public bool HasDefaultValueImpl { get; set; }
        public bool IsRef { get; set; }
        public bool IsGeneric { get; set; }
        public string GenericName { get; set; }

    }
}
