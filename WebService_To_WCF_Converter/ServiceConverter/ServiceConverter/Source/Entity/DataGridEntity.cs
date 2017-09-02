using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceConverter.Source.Entity
{
    public class DataGridEntity
    {
        public string Class { get; set; }
        public string MethodName { get; set; }
        public string ReturnTypes { get; set; }
        public string InputParams { get; set; }
        public bool HasRef { get; set; }
    }
}
