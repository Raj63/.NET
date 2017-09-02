using System;
using System.Collections.Generic;
using System.Linq;
using ServiceConverter.Source.Entity;
using ServiceConverter.Source.resources.Templates;
using System.IO;
using System.Diagnostics;
using System.Windows.Controls;
using System.Text.RegularExpressions;

namespace ServiceConverter.Source
{
    public class ConverterAction
    {
        public readonly int x;
        private readonly string projectPath = Path.GetDirectoryName(Path.GetDirectoryName(System.IO.Directory.GetCurrentDirectory()));
        private readonly string destinationPath = @"C:\WcfConverter\" + DateTime.Now.ToString("yyyy_MM_dd_HHmmss") + @"\WcfService\";
        private readonly string dataContractPath = "DataContract\\Entities\\";
        private readonly string serviceHosttPath = "WcfServiceHost\\";
        private readonly string serviceImplPath = "WcfServiceHost\\Implementation\\";
        private readonly string templatePath = @"\Source\resources\Templates\WcfService";
        private List<KeyValuePair<String, String>> nameSpaceMappingDataContract;
        private List<KeyValuePair<String, String>> nameSpaceMappingService;
        private List<KeyValuePair<String, String>> nameSpaceMappingServiceImpl;

        private ListBox console;
        private TabControl tabControl;

        public ConverterAction(ListBox console, TabControl tabControl)
        {
            this.console = console;
            this.tabControl = tabControl;
        }

        private void WriteToConsole(ListBoxEntity entity)
        {
            tabControl.SelectedIndex = 1;
            console.Items.Add(entity);
        }

        public void ConvertToWcf(IList<Class> classes, string codeImplPath)
        {
            string basePath = destinationPath;

            if (classes != null && classes.Count > 0)
            {
                nameSpaceMappingDataContract = new List<KeyValuePair<string, string>>();
                nameSpaceMappingService = new List<KeyValuePair<string, string>>();
                nameSpaceMappingServiceImpl = new List<KeyValuePair<string, string>>();

                Copy(projectPath + templatePath, destinationPath);

                Dictionary<string, string> codeImplFileList = null;
                if (!string.IsNullOrWhiteSpace(codeImplPath))
                {
                    codeImplFileList = DirSearch(codeImplPath);
                }

                foreach (var c in classes)
                {
                    var nameSpace = GetNameSpace(c.NameSpace);

                    if (c.Type.Equals(ClassType.DataContract))
                    {
                        c.NewNameSpace = "DataContract.Entities";
                        c.FileLocation = basePath + dataContractPath;

                        if (!string.IsNullOrWhiteSpace(nameSpace))
                        {
                            c.FileLocation = c.FileLocation + nameSpace.Replace('.', '\\') + "\\";
                            c.NewNameSpace = c.NewNameSpace + "." + nameSpace;
                        }

                        nameSpaceMappingDataContract.Add(new KeyValuePair<string, string>(c.NameSpace, c.NewNameSpace));

                        System.IO.Directory.CreateDirectory(c.FileLocation);
                    }
                    else if (c.Type.Equals(ClassType.Service))
                    {
                        c.NewNameSpace = "WcfServiceHost";
                        c.FileLocation = basePath + serviceHosttPath;
                        if (!string.IsNullOrWhiteSpace(nameSpace))
                        {
                            c.FileLocation = c.FileLocation + nameSpace.Replace('.', '\\') + "\\";
                            c.NewNameSpace = c.NewNameSpace + "." + nameSpace;
                        }

                        nameSpaceMappingService.Add(new KeyValuePair<string, string>(c.NameSpace, c.NewNameSpace));

                        System.IO.Directory.CreateDirectory(c.FileLocation);

                        var key = c.ClassName + ".asmx.cs";
                        var output = "";
                        if (codeImplFileList != null && codeImplFileList.Count > 0 && codeImplFileList.ContainsKey(key))
                        {
                            codeImplFileList.TryGetValue(key, out output);
                            ParseMethodImpl(c, output);
                        }
                    }
                    else if (c.Type.Equals(ClassType.Other))
                    {
                        c.NewNameSpace = "WcfServiceHost.Implementation";
                        c.FileLocation = basePath + serviceImplPath;
                        if (!string.IsNullOrWhiteSpace(nameSpace))
                        {
                            c.FileLocation = c.FileLocation + nameSpace.Replace('.', '\\') + "\\";
                            c.NewNameSpace = c.NewNameSpace + "." + nameSpace;
                        }
                        nameSpaceMappingServiceImpl.Add(new KeyValuePair<string, string>(c.NameSpace, c.NewNameSpace));

                        System.IO.Directory.CreateDirectory(c.FileLocation);
                        var key = c.ClassName + ".cs";
                        var output = "";
                        if (codeImplFileList != null && codeImplFileList.Count > 0 && codeImplFileList.ContainsKey(key))
                        {
                            codeImplFileList.TryGetValue(key, out output);
                            ParseMethodImpl(c, output);
                        }
                    }

                    //System.IO.Directory.CreateDirectory(c.FileLocation);
                }



                foreach (var c in classes)
                {
                    if (c.Type.Equals(ClassType.DataContract))
                    {
                        var writePath = "";
                        writePath = c.FileLocation + c.ClassName + ".cs";
                        //GenerateDataMembers(c.Properties, true);
                        var dataContractTemplate = new DataContractTemplate { C = c, IsDataContract = true, NameSpaceMapping = nameSpaceMappingDataContract };
                        var output = dataContractTemplate.TransformText();
                        System.IO.File.WriteAllText(writePath, output);


                        //Now add these files to the Csproj compile list
                        var format = "<Compile Include = \"{0}\" />\n\t\t<!--//compileList-->";
                        var fileName = basePath + "DataContract\\DataContract.csproj";

                        Uri path1 = new Uri(fileName);
                        Uri path2 = new Uri(c.FileLocation);
                        Uri diff = path1.MakeRelativeUri(path2);

                        var relativeLocation = diff.OriginalString.Replace(@"/", @"\") + c.ClassName + ".cs";
                        string old = File.ReadAllText(fileName);
                        string updatedFile = old.Replace("<!--//compileList-->", string.Format(format, relativeLocation));
                        File.WriteAllText(fileName, updatedFile);

                    }
                    else if (c.Type.Equals(ClassType.Service))
                    {
                        var iWritePath = "";
                        iWritePath = c.FileLocation + "I" + c.ClassName + ".cs";
                        //GenerateDataMembers(c.Properties, true);
                        var iWcfServiceTemplate = new IWcfServiceTemplate { C = c, IsServiceContract = true, NameSpaceMapping = nameSpaceMappingService };
                        var iOutput = iWcfServiceTemplate.TransformText();
                        System.IO.File.WriteAllText(iWritePath, iOutput);



                        var writePath = "";
                        writePath = c.FileLocation + c.ClassName;

                        var serviceSVC = new ServiceHostSVC { ServiceName = c.NewNameSpace + "." + c.ClassName, ServiceClassName = c.ClassName + ".svc.cs" };
                        var outputSvc = serviceSVC.TransformText();
                        System.IO.File.WriteAllText(writePath + ".svc", outputSvc);

                        var wcfServiceTemplate = new WcfServiceTemplate { C = c, IsServiceContract = true, NameSpaceMapping = nameSpaceMappingService };
                        var output = wcfServiceTemplate.TransformText();
                        System.IO.File.WriteAllText(writePath + ".svc.cs", output);

                        var fileName = c.FileLocation + "WcfServiceHost.csproj";
                        //Now add these files to the Csproj compile list
                        string old = File.ReadAllText(fileName);
                        string updatedFile = old.Replace("ServicePlaceHolder", c.ClassName);
                        File.WriteAllText(fileName, updatedFile);
                    }
                    else if (c.Type.Equals(ClassType.Other))
                    {
                        var writePath = "";
                        writePath = c.FileLocation + c.ClassName + ".cs";
                        //GenerateDataMembers(c.Properties, true);
                        var serviceImpl = new ImplementationTemplate { C = c, NameSpaceMapping = nameSpaceMappingDataContract };
                        var output = serviceImpl.TransformText();
                        System.IO.File.WriteAllText(writePath, output);


                        //Now add these files to the Csproj compile list
                        var format = "<Compile Include = \"{0}\" />\n\t\t<!--//compileList-->";
                        var fileName = basePath + serviceHosttPath + "WcfServiceHost.csproj";

                        Uri path1 = new Uri(fileName);
                        Uri path2 = new Uri(c.FileLocation);
                        Uri diff = path1.MakeRelativeUri(path2);

                        var relativeLocation = diff.OriginalString.Replace(@"/", @"\") + c.ClassName + ".cs";
                        string old = File.ReadAllText(fileName);
                        string updatedFile = old.Replace("<!--//compileList-->", string.Format(format, relativeLocation));
                        File.WriteAllText(fileName, updatedFile);
                    }

                }

                Process.Start(destinationPath);
            }
        }

        private void ParseMethodImpl(Class c, string filePath)
        {
            var x = @"private\s+void\s+btnDelete_Click\s*\(object\s+sender,\s*EventArgs\s+e\)\s*\{.+;\s+\}\s*";


            foreach (var method in c.Methods)
            {
                var count = method.InputParameters.Count + 1;
                var dataType = new string[count];
                var key = 0;
                var inputParam = "";
                var output = method.OutputParameter;
                var returnType = "";

                if (output != null)
                {
                    if (output.IsGeneric)
                    {
                        dataType[key] = output.Type;
                        returnType = output.GenericName.Substring(0, output.GenericName.IndexOf('`')) + "<{" + key + "}>";
                        key = key + 1;
                    }
                    else
                    {
                        dataType[key] = (output.Type.Equals("Void") ? "void" : output.Type);
                        key = key + 1;
                    }
                }

                foreach (var input in method.InputParameters)
                {
                    //dataType[key] = GetType(input, "");
                    if (input != null)
                    {
                        var response = "";

                        if (!string.IsNullOrWhiteSpace(inputParam))
                            inputParam = inputParam + @"\s*,\s*";

                        if (input.IsRef)
                            response = @"ref\s+";

                        if (input.IsGeneric)
                        {
                            dataType[key] = input.Type;
                            response = response + input.GenericName.Substring(0, input.GenericName.IndexOf('`')) + "<{" + key + "}>";
                            inputParam = inputParam + response + @"\s+" + input.Name + (input.HasDefaultValueImpl ? " = null" : "");
                            key = key + 1;
                        }
                        else
                        {
                            response = response + (input.Type.Equals("Void") ? "void" : input.Type);
                            dataType[key] = response;
                            inputParam = inputParam + "{" + key + "}" + @"\s+" + input.Name + (input.HasDefaultValueImpl ? " = null" : "");
                            key = key + 1;
                        }
                    }
                    //return response;
                }

                //var searchPattern = string.Format(dataMemberFormat, method.AccessType, returnType, method.Name, inputParam);
                method.MethodBody = FindMatch(method.AccessType, method.Name, filePath, inputParam, dataType, returnType);
            }

        }



        private List<string> GetAllProbability(string format, string[] dataType, int start)
        {
            var list = new List<string>();

            if (start <= dataType.Length)
            {
                var convertedFormat = string.Copy(format);
                for (var i = 0; i < start; i++)
                {
                    //convert
                    convertedFormat = convertedFormat.Replace("{" + i + "}", DataTypeMapper.GetPrimitiveType(dataType[i]));
                }

                for (var i = start; i < dataType.Length; i++)
                {
                    //without conversion
                    convertedFormat = convertedFormat.Replace("{" + i + "}", dataType[i]);
                }

                list.Add(convertedFormat);
                list.AddRange(GetAllProbability(format, dataType, start+1));
            }

            return list;
        }

        private string FindMatch(string accessType, string name, string filePath, string inputParam, string[] dataType, string returnType)
        {
            var dataMemberFormat = @"\s+{0}\s+{1}\s*\(\s*{2}\s*\)\s*";
            var inputFile = File.ReadAllText(filePath);
            //var returnType = "void";
            //var inputParam = "";
            var standardType = string.Copy(dataMemberFormat);

            standardType = accessType + standardType;
            if (!string.IsNullOrWhiteSpace(returnType))
            {
                standardType = standardType.Replace("{0}", returnType);
            }
            standardType = standardType.Replace("{1}", name);
            standardType = standardType.Replace("{2}", inputParam);

            foreach (var pattern in GetAllProbability(standardType, dataType, 0))
            {
                var eachPattern = pattern + @"\r\n*\s*(?<body>\{(?<DEPTH>)(?>(?<DEPTH>)\{|\}(?<-DEPTH>)|(?(DEPTH)[^\{\}]*|))*\}(?<-DEPTH>)(?(DEPTH)(?!)))";
                //Regex regex = new Regex(eachPattern);
                //Match match = regex.Match(inputFile);
                foreach(Match match in Regex.Matches(inputFile, eachPattern, RegexOptions.IgnorePatternWhitespace))
                {
                    if (match.Success)
                    {
                        return match.Groups[1].Value;
                    }
                }
                
            }

            return "";
        }


        private string GetType(Parameter p, string defaultVal)
        {
            var response = defaultVal;

            if (p != null)
            {
                if (p.IsGeneric)
                {
                    response = p.GenericName.Substring(0, p.GenericName.IndexOf('`')) + "<" + p.Type + ">";
                }
                else
                {
                    response = (p.Type.Equals("Void") ? "void" : p.Type);
                }

                if (p.IsRef)
                    response = @"ref\s+" + response;
            }

            return response;
        }

        private Dictionary<String, String> DirSearch(string sDir)
        {
            var files = new Dictionary<String, String>();
            try
            {
                foreach (string f in Directory.GetFiles(sDir))
                {
                    var name = Path.GetFileName(f);
                    files.Add(name, f);
                }

                foreach (string d in Directory.GetDirectories(sDir))
                {
                    files.Concat(DirSearch(d)).ToDictionary(x => x.Key, x => x.Value);
                }
            }
            catch (System.Exception excpt)
            {
                WriteToConsole(new ListBoxEntity { Content = "Exception while parsing files in the repository folder" });
            }

            return files;
        }

        private void AddClassFile(String path, string solutionName, string className)
        {
            //create .cs file in the solution file path 
            //then add the class file to the .csproj for compilation
            /* *
             replace <!--NewFiles--> with <Compile Include="{path}\{className}.cs" />
             * */
        }


        //Gets the namespace after removing the project name from it
        public string GetNameSpace(string nameSpace)
        {
            var response = "";

            if (!string.IsNullOrWhiteSpace(nameSpace) && nameSpace.Contains("."))
            {
                response = nameSpace.Substring(nameSpace.IndexOf('.') + 1);
            }

            return response;
        }

        public static void Copy(string sourceDirectory, string targetDirectory)
        {
            DirectoryInfo diSource = new DirectoryInfo(sourceDirectory);
            DirectoryInfo diTarget = new DirectoryInfo(targetDirectory);

            CopyAll(diSource, diTarget);
        }

        public static void CopyAll(DirectoryInfo source, DirectoryInfo target)
        {
            Directory.CreateDirectory(target.FullName);

            // Copy each file into the new directory.
            foreach (FileInfo fi in source.GetFiles())
            {
                Console.WriteLine(@"Copying {0}\{1}", target.FullName, fi.Name);
                fi.CopyTo(Path.Combine(target.FullName, fi.Name), true);
            }

            // Copy each subdirectory using recursion.
            foreach (DirectoryInfo diSourceSubDir in source.GetDirectories())
            {
                DirectoryInfo nextTargetSubDir =
                    target.CreateSubdirectory(diSourceSubDir.Name);
                CopyAll(diSourceSubDir, nextTargetSubDir);
            }
        }


        //test method to generate the datamembers for a datacontract class
        public string GenerateDataMembers(IList<Property> Properties, bool isDataContract)
        {
            var response = "" + Environment.NewLine;
            var dataMemberFormat = "{0}\t{1}{2}\t{3};";
            var signature = "[DataMember]";
            foreach (var property in Properties)
            {
                var eachProperty = string.Format(dataMemberFormat, property.AccessType.ToString(), (property.IsReadOnly ? "readonly\t" : ""),
                                property.Type, property.Name);

                //var member = property.AccessType.ToString() + "\t" + (property.IsReadOnly ? "readonly\t" : "") + "\t" +
                //    property.TypeName + "\t" + property.Name;
                if (!string.IsNullOrWhiteSpace(response))
                    response = response + Environment.NewLine + Environment.NewLine;

                response = response + string.Format("{0}{1}{2}", signature, Environment.NewLine, eachProperty);

            }
            return response;
        }

    }
}
