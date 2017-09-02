using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServiceConverter.Source.Entity;
using System.Windows.Controls;

namespace ServiceConverter.Source
{
    public class ClassAnalyzer
    {
        public ListBox console;
        public TabControl tabControl;

        public ClassAnalyzer(ListBox con, TabControl tab)
        {
            console = con;
            tabControl = tab;
        }

        private void WriteToConsole(ListBoxEntity entity)
        {
            tabControl.SelectedIndex = 1;
            console.Items.Add(entity);
        }

        public IList<Class> GetClasses(Type[] types)
        {
            IList<Class> classList = null;

            foreach (Type type in types)
            {
                if (type != null && type.BaseType != null && !string.IsNullOrWhiteSpace(type.BaseType.FullName))
                {
                    if (classList == null)
                    {
                        classList = new List<Class>();
                    }

                    Class eachClass = GetClass(type);

                    if (eachClass != null)
                    {
                        classList.Add(eachClass);
                    }

                }

                //MemberInfo[] members = type.GetMembers(BindingFlags.Public
                //                                      | BindingFlags.Instance
                //                                      | BindingFlags.InvokeMethod);

            }

            return classList;
        }

        private static Class GetClass(Type type)
        {
            Class eachClass = null;
            if (!string.IsNullOrWhiteSpace(type.Name))
            {
                eachClass = new Class();

                eachClass.ClassName = type.Name;
                eachClass.UsingList = new HashSet<string>();
                //Element inside the namespace cannot be private or protected
                eachClass.AccessType = "public";
                eachClass.NameSpace = type.Namespace;

                var userDefinedClassName = type.FullName;
                if (type.BaseType.FullName.Equals("System.Web.Services.WebService"))
                {
                    //Service class
                    eachClass.Type = ClassType.Service;
                    eachClass.Methods = GetMethods(type, true, userDefinedClassName, eachClass.UsingList);
                    eachClass.Properties = GetProperties(type, eachClass.UsingList);
                }
                else if (type.BaseType.FullName.Equals("System.Object"))
                {
                    //User defined class
                    //if(type.DeclaringMethod == null || type.GetFields().Length > 0)

                    var methods = GetMethods(type, false, userDefinedClassName, eachClass.UsingList);
                    var properties = GetProperties(type, eachClass.UsingList);

                    eachClass.Methods = methods;
                    eachClass.Properties = properties;

                    if ((methods == null || methods.Count == 0) && properties != null && properties.Count > 0)
                    {
                        eachClass.Type = ClassType.DataContract;
                    }
                    else
                    {
                        //these classes will be assigned as datacontracts if they are being used as input/output param
                        //by the operationexposed methods else will be left as the other class
                        eachClass.Type = ClassType.Other;
                    }
                }
                else
                {
                    return null;
                }
            }

            return eachClass;
        }



        private static IList<Method> GetMethods(Type type, bool isServiceClass, string userDefinedClassName, HashSet<string> UsingList)
        {
            IList<Method> methodList = null;

            MethodInfo[] methods = type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            if (methods != null && methods.Count() > 0)
            {
                methodList = new List<Method>();

                foreach (MethodInfo method in methods)
                {
                    var methodInfo = GetMethodInfo(method, isServiceClass, userDefinedClassName, UsingList);
                    if (methodInfo != null)
                    {
                        methodList.Add(methodInfo);
                    }
                }
            }

            return methodList;
        }

        private static Method GetMethodInfo(MethodInfo method, bool isServiceClass, string userDefinedClassName, HashSet<string> UsingList)
        {
            Method eachMethod = null;

            if (method.DeclaringType != null && method.DeclaringType.FullName == userDefinedClassName)
            {
                eachMethod = new Method();

                eachMethod.Name = method.Name;
                eachMethod.IsConstructor = method.IsConstructor;
                eachMethod.IsStatic = method.IsStatic;
                eachMethod.AccessType = GetAccessSpecifier(method.IsPublic, method.IsPrivate);
                eachMethod.InputParameters = GetInputParams(method, UsingList);
                eachMethod.OutputParameter = GetReturnParam(method, UsingList);
                eachMethod.SignatureFullName = method.DeclaringType.FullName + "." + method.Name + "(" +
                                                string.Join(",", eachMethod.InputParameters.Select(i => i.Type + " " + i.Name + (i.HasDefaultValueImpl ? " = null" : ""))) + ")";
                eachMethod.IsOperationExposed = false;

                //eachMethod.SignatureFullName = method.FullName;
                eachMethod.CustomAttributes = null;

                if (isServiceClass)
                {
                    var attrs = GetAttributes(method.CustomAttributes);

                    if (attrs != null)
                    {
                        eachMethod.CustomAttributes = attrs;
                        var x = attrs.Where(i => i.IsWebMethod == true).ToList();
                        eachMethod.IsOperationExposed = x.Count > 0;
                    }
                }

            }
            //System.Diagnostics.Debug.WriteLine(type.Name + "." + method.Name + " - " + method.MemberType);

            return eachMethod;
        }

        private static IList<Parameter> GetInputParams(MethodInfo method, HashSet<string> UsingList)
        {
            IList<Parameter> paramList = null;

            var parameters = method.GetParameters();
            if (parameters != null)
            {
                paramList = new List<Parameter>();
                foreach (var eachparam in parameters)
                {
                    Parameter p = new Parameter();

                    p.Name = eachparam.Name;
                    p.HasDefaultValueImpl = eachparam.HasDefaultValue || eachparam.IsOptional;
                    p.IsInputParameter = true;

                    if (eachparam.ParameterType != null)
                    {
                        p.IsRef = eachparam.ParameterType.IsByRef;

                        if (eachparam.ParameterType.Namespace.Equals("System.Collections.Generic") && eachparam.ParameterType.GenericTypeArguments != null)
                        {
                            p.IsGeneric = true;
                            p.GenericName = eachparam.ParameterType.Name;
                            if (p.IsRef)
                            {
                                p.Type = eachparam.ParameterType.GenericTypeArguments[0].Name.Replace("&", "");
                                p.TypeFullName = eachparam.ParameterType.GenericTypeArguments[0].FullName.Replace("&", "");
                            }
                            else
                            {
                                p.Type = eachparam.ParameterType.GenericTypeArguments[0].Name;
                                p.TypeFullName = eachparam.ParameterType.GenericTypeArguments[0].FullName;
                            }

                            UsingList.Add(eachparam.ParameterType.Namespace +"."+ p.GenericName.IndexOf('`'));
                        }
                        else
                        {
                            if (p.IsRef)
                            {
                                p.Type = eachparam.ParameterType.Name.Replace("&", "");
                                p.TypeFullName = eachparam.ParameterType.FullName.Replace("&", "");
                            }
                            else
                            {
                                p.Type = eachparam.ParameterType.Name;
                                p.TypeFullName = eachparam.ParameterType.FullName;
                            }
                        }





                        UsingList.Add(p.TypeFullName);
                    }

                    paramList.Add(p);

                }
            }

            return paramList;
        }

        private static Parameter GetReturnParam(MethodInfo method, HashSet<string> UsingList)
        {
            Parameter p = new Parameter { IsInputParameter = false, IsRef = false, Type = "void", TypeFullName = "System.void" };

            var param = method.ReturnType;

            if (param != null)
            {
                if (param.Namespace.Equals("System.Collections.Generic") && param.GenericTypeArguments != null)
                {
                    p.IsGeneric = true;
                    p.GenericName = param.Name;
                    p.Type = param.GenericTypeArguments[0].Name;
                    p.TypeFullName = param.GenericTypeArguments[0].FullName;
                    UsingList.Add(param.Namespace + "." + p.GenericName.IndexOf('`'));
                }
                else
                {
                    p.Type = param.Name;
                    p.TypeFullName = param.FullName;
                }
                UsingList.Add(p.TypeFullName);
            }

            return p;
        }

        private static IList<CustomAttribute> GetAttributes(IEnumerable<CustomAttributeData> attrs)
        {
            IList<CustomAttribute> attributes = null;
            if (attrs.Count() > 0)
            {
                attributes = new List<CustomAttribute>();

                foreach (var eachAttr in attrs)
                {
                    var a = new CustomAttribute();
                    a.IsWebMethod = false;
                    if (eachAttr.AttributeType != null)
                    {
                        a.AttributeFullName = eachAttr.AttributeType.FullName;

                        if (eachAttr.AttributeType.FullName.Equals("System.Web.Services.WebMethodAttribute"))
                        {
                            a.IsWebMethod = true;
                            if (eachAttr.NamedArguments != null && eachAttr.NamedArguments[0].TypedValue != null &&
                                eachAttr.NamedArguments[0].TypedValue.Value != null)
                                a.TypedValue = eachAttr.NamedArguments[0].TypedValue.Value.ToString();
                            else
                                a.TypedValue = "";

                        }
                    }


                    attributes.Add(a);
                }
            }

            return attributes;
        }

        private static IList<Property> GetProperties(Type type, HashSet<string> UsingList)
        {
            IList<Property> properties = null;

            FieldInfo[] fields = type.GetFields();
            if (fields.Count() > 0)
            {
                properties = new List<Property>();

                foreach (var field in fields)
                {
                    if (string.Compare(field.MemberType.ToString(), "Field") == 0)
                    {

                        var p = new Property();
                        p.Name = field.Name;
                        p.IsReadOnly = false;
                        p.HasDefaultvalue = false;
                        p.AccessType = GetAccessSpecifier(field.IsPublic, field.IsPrivate);

                        if (field.FieldType != null)
                        {
                            if (field.FieldType.Namespace.Equals("System.Collections.Generic") && field.FieldType.GenericTypeArguments != null)
                            {
                                p.IsGeneric = true;
                                p.GenericName = field.FieldType.Name;
                                p.Type = field.FieldType.GenericTypeArguments[0].Name;
                                p.TypeFullName = field.FieldType.GenericTypeArguments[0].FullName;
                                UsingList.Add(field.FieldType.Namespace + "." + p.GenericName.IndexOf('`'));
                            }
                            else
                            {
                                p.IsGeneric = false;
                                p.Type = field.FieldType.Name;
                                p.TypeFullName = field.FieldType.FullName;
                            }
                            UsingList.Add(p.TypeFullName);

                        }

                        if (field.IsLiteral || field.IsInitOnly)
                        {
                            p.IsReadOnly = true;

                            if (field.IsLiteral)
                            {
                                p.HasDefaultvalue = true;
                                p.defaultValue = field.GetValue(field);
                            }
                        }

                        properties.Add(p);

                    }
                }
            }


            return properties;
        }

        private static string GetAccessSpecifier(bool isPublic, bool isPrivate)
        {
            //AccessSpecifier accessType = AccessSpecifier.Protected;
            var accessType = "protected";
            if (isPublic)
                accessType = "public";
            else if (isPrivate)
                accessType = "private";

            return accessType;
        }
    }
}
