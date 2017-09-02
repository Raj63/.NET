using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceConverter.Source.Entity
{
    public class Property
    {
        public string Name { get; set; }
        public string TypeFullName { get; set; }
        public string TypeName { get; set; }
        public string Type { get; set; }

        public bool IsGeneric { get; set; }
        public string GenericName { get; set; }

        public string AccessType { get; set; }
        public bool IsReadOnly { get; set; }
        public bool HasDefaultvalue { get; set; }
        public dynamic defaultValue { get; set; }
    }
}
