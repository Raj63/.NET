using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceConverter.Source.Entity
{
    public static class DataTypeMapper
    {

        public static Dictionary<string, string> List = new Dictionary<string, string>
        { {"Public", "public"}, { "String", "string"}, { "Int32", "int"}, {"Long", "long"}, { "Void", "void"}, { "Boolean", "bool"} };

        public static string GetPrimitiveType(string input)
        {
            if (List.ContainsKey(input))
            {
                var outPut = "";
                List.TryGetValue(input, out outPut);
                return outPut;
            }

            return input;
        }
    }
}
