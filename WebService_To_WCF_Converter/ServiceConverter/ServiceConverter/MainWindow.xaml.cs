using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows;
using System.Linq;
using ServiceConverter.Source;
using ServiceConverter.Source.Entity;
using System.Windows.Forms;

namespace ServiceConverter
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public IList<Class> classes;
        public System.Windows.Controls.ListBox console;

        public MainWindow()
        {
            InitializeComponent();
            console = listBoxConsole; 
        }


        private void WriteToConsole(ListBoxEntity entity)
        {
            tabControl.SelectedIndex = 1;
            console.Items.Add(entity);
        }

        private void button_browsePath_Click(object sender, RoutedEventArgs e)
        {

            WriteToConsole(new ListBoxEntity { Content = "Browsing DLL path: Progress...", ContentTextColor = "Black" });
            var dialog = new System.Windows.Forms.OpenFileDialog();
            dialog.Filter = "Component Files|*.dll";
            System.Windows.Forms.DialogResult result = dialog.ShowDialog();

            if (result == System.Windows.Forms.DialogResult.OK && !string.IsNullOrWhiteSpace(dialog.FileName))
            {
                textBox_assemblyPath.Text = dialog.FileName;
                WriteToConsole(new ListBoxEntity { Content = "Selected DLL path: " + dialog.FileName, ContentTextColor = "Green" });
            }
        }

        private void button_analyze_Click(object sender, RoutedEventArgs e)
        {
            WriteToConsole(new ListBoxEntity { Content = "Analyze the input DLL:", ContentTextColor = "Black" });
            string dllLocation = textBox_assemblyPath.Text;
            if (!string.IsNullOrWhiteSpace(dllLocation))
            {
                WriteToConsole(new ListBoxEntity { Content = "Start...", ContentTextColor = "Black" });
                ReadAssembly(dllLocation);
                WriteToConsole(new ListBoxEntity { Content = "DLL Analsis completed!", ContentTextColor = "Green" });
            }
            else
            {
                WriteToConsole(new ListBoxEntity { Content = "Invalid DLL path - NUll/Empty", ContentTextColor = "Red" });
            }
        }

        private void button_convert_Click(object sender, RoutedEventArgs e)
        {
            var action = new ConverterAction(console, tabControl);
            action.ConvertToWcf(classes, (checkBoxCodeImpl.IsChecked == true ? labelCodeImplPath.Content.ToString() : ""));
        }

        private IEnumerable<Type> GetAssemblyTypes(Assembly curAssembly)
        {
            var returnAssemTypes = new List<Type>();

            returnAssemTypes.AddRange(curAssembly.GetTypes());

            return returnAssemTypes;
        }

        private List<DataGridEntity> ConvertToGridEntity(IList<Class> classes)
        {
            List<DataGridEntity> list = new List<DataGridEntity>();
            WriteToConsole(new ListBoxEntity { Content = "Write DLL info to the Grid:", ContentTextColor = "Black" });

            foreach (var c in classes)
            {
                WriteToConsole(new ListBoxEntity { Content = "Class : "+ c.ClassName, ContentTextColor = "Blue" });

                foreach (var m in c.Methods)
                {
                    var x = new DataGridEntity();

                    x.Class = c.ClassName;
                    x.MethodName = m.Name;
                    WriteToConsole(new ListBoxEntity { Content = "Method : " + m.Name, ContentTextColor = "Blue" });

                    x.HasRef = m.InputParameters.Where(i => i.IsRef == true).ToList().Count > 0;
                    WriteToConsole(new ListBoxEntity { Content = "Has any ref : " + x.HasRef, ContentTextColor = x.HasRef ? "Red":"Blue" });

                    if (m.OutputParameter != null && m.OutputParameter.IsGeneric)
                    {
                        x.ReturnTypes = m.OutputParameter.GenericName.Substring(0, m.OutputParameter.GenericName.IndexOf('`')) + "<" +
                            m.OutputParameter.Type + ">";
                    }
                    else
                    {
                        x.ReturnTypes = m.OutputParameter.Type;
                    }

                    WriteToConsole(new ListBoxEntity { Content = "Return type : " + x.ReturnTypes, ContentTextColor = "BlueViolet" });

                    x.InputParams = "(" + string.Join(",", m.InputParameters.Select(i => formatInputType(i) + " " + i.Name + (i.HasDefaultValueImpl ? " = null" : ""))) + ")";

                    WriteToConsole(new ListBoxEntity { Content = "Input paramters : " + x.InputParams, ContentTextColor = "Green" });
                    list.Add(x);
                }

            }

            return list;

        }

        private string formatInputType(Parameter p)
        {
            var response = "void";
            if (p != null && p.IsGeneric)
            {
                response = p.GenericName.Substring(0, p.GenericName.IndexOf('`')) + "<" +
                    p.Type + ">";
            }
            else
            {
                response = p.Type;
            }

            return response;
        }

        private void ReadAssembly(String dllLocation)
        {
            //string path = System.IO.Path.GetFileNameWithoutExtension(dllLocation);
            Assembly a = Assembly.LoadFrom(dllLocation);
            WriteToConsole(new ListBoxEntity { Content = "Loading the assembly from the input path..", ContentTextColor = "Blue" });

            Type[] types = a.GetExportedTypes();
            WriteToConsole(new ListBoxEntity { Content = "Extracting exported Types information from the loaded DLL", ContentTextColor = "Black" });

            var analyzer = new ClassAnalyzer(console, tabControl);
            WriteToConsole(new ListBoxEntity { Content = "Initializing the Class Analyzer", ContentTextColor = "Black" });

            classes = analyzer.GetClasses(types);

            WriteToConsole(new ListBoxEntity { Content = "Loading the Data Grid list", ContentTextColor = "Black" });
            dataGrid_Metadata.ItemsSource = ConvertToGridEntity(classes);
            WriteToConsole(new ListBoxEntity { Content = "Successfully loaded the Grid with the new Data.", ContentTextColor = "Black" });

        }

        private void checkBoxCodeImpl_Click(object sender, RoutedEventArgs e)
        {
            var checkbox = (System.Windows.Controls.CheckBox) sender;
            if (checkbox.IsChecked == true)
            {
                WriteToConsole(new ListBoxEntity { Content = "Browsing Code repository path: Progress...", ContentTextColor = "Black" });
                FolderBrowserDialog folderDlg = new FolderBrowserDialog();
                folderDlg.ShowNewFolderButton = true;
                // Show the FolderBrowserDialog.
                DialogResult result = folderDlg.ShowDialog();
                if (result == System.Windows.Forms.DialogResult.OK)
                {
                    labelCodeImplPath.Content = folderDlg.SelectedPath;
                    Environment.SpecialFolder root = folderDlg.RootFolder;
                    WriteToConsole(new ListBoxEntity { Content = "Selected repository path: " + folderDlg.SelectedPath, ContentTextColor = "Green" });
                }                 
            }
            else
            {
                labelCodeImplPath.Content = "";
                WriteToConsole(new ListBoxEntity { Content = "Removing selected repository path", ContentTextColor = "Red" });
            }
        }
    }
}
