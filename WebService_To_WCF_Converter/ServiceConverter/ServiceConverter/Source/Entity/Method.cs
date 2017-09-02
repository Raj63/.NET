using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceConverter.Source.Entity
{
    public class Method
    {
        public string Name { get; set; }
        public string SignatureFullName { get; set; }


        public bool IsStatic { get; set; }
        public string AccessType { get; set; }
        public bool IsConstructor { get; set; }

        public IList<Parameter> InputParameters { get; set; }
        public Parameter OutputParameter { get; set; }

        public IList<CustomAttribute> CustomAttributes { get; set; }
        public bool IsOperationExposed { get; set; }

        public string MethodBody { get; set; }
    }
}
