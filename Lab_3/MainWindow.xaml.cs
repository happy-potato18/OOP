using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Reflection;
using System.Reflection.Emit;
using System.Text.RegularExpressions;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Runtime.Serialization;
using IMySerialization;

namespace Lab_2
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        static readonly List<object> workers = new List<object>();
        static Assembly ClassesAssembly;
        static Assembly PluginsAssembly;
        static List<Type> ClassesTypesLib = new List<Type>();
        static List<Type> PluginsLib = new List<Type>();
        static int offset = 1;
        static string SaveInFileName = "";
        static string OpenForFileName = "";
        static string SaveCompressedFileName = "";
        static string OpenCompressedFileName = "";
        static string fileType = "";


        public MainWindow()
        {
            InitializeComponent();

        }

        private void App_Loaded(object sender, RoutedEventArgs e)
        {

            ClassesAssembly = Assembly.LoadFile(@"D:\УНИК\4 сем\OOP\Lab_3\classes\class_library\bin\Debug\class_library.dll");
            ClassesTypesLib = ClassesAssembly.GetTypes().Where(type => type.IsClass).ToList();

            foreach (var currentClass in ClassesTypesLib)
            {
                if ((!currentClass.IsAbstract) && (!currentClass.IsSealed))
                    cmbClasses.Items.Add(currentClass.Name);
            }

            PluginsAssembly = Assembly.LoadFile(@"D:\УНИК\4 сем\OOP\Lab_3\classes\IArchivePlugin\bin\Debug\netstandard2.0\IArchivePlugin.dll");
            PluginsLib = FindAllPlugins(PluginsAssembly, "IPlugin");

            foreach (var currentPlugin in PluginsLib)
            {
                cmbPlugins.Items.Add(currentPlugin.Name);
            }

            if (cmbClasses.Items.Count > 0)
            {
                cmbClasses.SelectedIndex = 0;
            }

            if (cmbPlugins.Items.Count > 0)
            {
                cmbPlugins.SelectedIndex = 0;
            }

            if (cmbSerializationType.Items.Count > 0)
            {
                cmbSerializationType.SelectedIndex = 0;
            }

        }

        private static List<Type> FindAllPlugins(Assembly Dll, string InterfaceName)
        {
            List<Type> Plugins = new List<Type>();
            foreach (var type in Dll.GetTypes())
            {
                if (type.IsPublic == true)
                {

                    if (!type.IsAbstract)
                    {

                        if (type.GetInterface(InterfaceName) != null)
                        {

                            Plugins.Add(type);
                        }

                    }
                }
            }

            return Plugins;
        }


        public object AddFieldsToObject(Type currentClass, int index)
        {
            var obj = ClassesAssembly.CreateInstance(currentClass.FullName);
            var properties = currentClass.GetProperties();

            foreach (var property in properties)
            {
                if (property.PropertyType == typeof(int))
                {
                    try
                    {
                        property.SetValue(obj, Int32.Parse(((TextBox)pnlBlocks.Children[index]).Text));
                    }
                    catch
                    {
                        tbShowing.AppendText(String.Format("Incorrect data at field {0}" + Environment.NewLine, property.Name));
                    }

                }

                else if (property.PropertyType == typeof(String))
                {
                    try
                    {
                        property.SetValue(obj, ((TextBox)pnlBlocks.Children[index]).Text);
                    }
                    catch
                    {
                        tbShowing.AppendText(String.Format("Incorrect data at field {0}" + Environment.NewLine, property.Name));
                    }
                }

                else if (property.PropertyType == typeof(char))
                {
                    try
                    {
                        property.SetValue(obj, ((TextBox)pnlBlocks.Children[index]).Text[0]);
                    }
                    catch
                    {
                        tbShowing.AppendText(String.Format("Incorrect data at field {0}" + Environment.NewLine, property.Name));
                    }
                }


                else if (property.PropertyType.IsEnum)
                {
                    try
                    {
                        property.SetValue(obj, ((ComboBox)pnlBlocks.Children[index]).SelectedIndex);
                    }
                    catch
                    {
                        tbShowing.AppendText(String.Format("Incorrect data at field {0}" + Environment.NewLine, property.Name));
                    }
                }

                else if (property.PropertyType.IsClass)
                {
                    index++;
                    var agreg = AddFieldsToObject(property.PropertyType, index);
                    property.SetValue(obj, agreg);
                    index--;
                }

                index++;

            }

            return obj;

        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {

            var currentClassName = cmbClasses.SelectedItem.ToString();

            foreach (var currentClass in ClassesTypesLib)
            {
                if (currentClassName == currentClass.Name)
                {

                    var newWorker = AddFieldsToObject(currentClass, 0);
                    workers.Add(newWorker);
                    tbShowing.AppendText("Object was added successfully!" + Environment.NewLine);


                }
            }

        }

        private void CreateFieldsOnForm(Type currentClass, int leftMargin)
        {
            var properties = currentClass.GetProperties();
            foreach (var property in properties)
            {
                if ((property.PropertyType.IsPrimitive)
                    || (property.PropertyType == typeof(String)))
                {
                    pnlBlocks.Children.Add
                    (new TextBox()
                    { Margin = new Thickness(leftMargin, 5 * offset, 0, 0), Name = "TextBox" + property.Name, Text = property.Name }
                    );
                }

                else if (property.PropertyType.IsEnum)
                {
                    var newComboBox = new ComboBox
                    {
                        Margin = new Thickness(leftMargin, 5 * offset, 0, 0),
                        Name = "ComboBox" + property.Name,
                        Text = property.Name
                    };

                    foreach (var enumName in property.PropertyType.GetEnumNames())
                        newComboBox.Items.Add(enumName);

                    newComboBox.SelectedIndex = 0;

                    pnlBlocks.Children.Add(newComboBox);
                }

                else if (property.PropertyType.IsClass)
                {
                    pnlBlocks.Children.Add(new System.Windows.Controls.Label()
                    {
                        Content = property.PropertyType.Name,
                        Margin = new Thickness(leftMargin, 5 * offset, 0, 0),

                    });
                    offset++;
                    CreateFieldsOnForm(property.PropertyType, leftMargin + 15);

                }

                offset++;

            }
        }


        private void CmbClasses_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            pnlBlocks.Children.Clear();
            var currentClassName = cmbClasses.SelectedItem.ToString();

            foreach (var currentClass in ClassesTypesLib)
            {
                if (currentClassName == currentClass.Name)
                {
                    offset = 1;
                    CreateFieldsOnForm(currentClass, 5);

                    break;


                }
            }

        }

        private void BtnDel_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                switch (cmbDeletingType.SelectedIndex)
                {
                    case 0:
                        int IndexForDeleting = Int32.Parse(tbDeleting.Text);
                        workers.RemoveAt(IndexForDeleting - 1);
                        tbShowing.AppendText("Item was deleted successfully!" + Environment.NewLine);
                        break;
                    case 1:
                        for (int i = 0; i < workers.Count; i++)
                        {
                            var property = workers[i].GetType().GetProperty("PassNum");
                            if (property.GetValue(workers[i]).ToString() == tbDeleting.Text)
                            {
                                workers.RemoveAt(i);
                                tbShowing.AppendText("Item was deleted successfully!" + Environment.NewLine);

                            }

                        }
                        break;

                }
            }
            catch
            {
                tbShowing.AppendText("Entered index or string is incorrect!" + Environment.NewLine);
            }
        }

        private void ShowAllFields(object currentObject)
        {
            var properties = currentObject.GetType().GetProperties();


            foreach (var property in properties)
            {
                if ((property.PropertyType.IsClass)
                    && (property.PropertyType != typeof(String)))
                {
                    ShowAllFields(property.GetValue(currentObject));

                }
                else
                    tbShowing.AppendText("\t" + property.Name + ": " + property.GetValue(currentObject)
                                           + Environment.NewLine);
            }



        }

        private void BtnShow_Click(object sender, RoutedEventArgs e)
        {

            int number = 1;
            tbShowing.AppendText("Existing workers: " + Environment.NewLine);
            foreach (var currentObject in workers)
            {
                tbShowing.AppendText(number.ToString() + ". ("
                                    + currentObject.GetType().Name + " ):" + Environment.NewLine);
                ShowAllFields(currentObject);
                tbShowing.AppendText(Environment.NewLine);
                number++;


            }
        }


        public void SetValuesOnForm(object currentObject, int index)
        {

            var properties = currentObject.GetType().GetProperties();

            foreach (var property in properties)
            {
                if (property.PropertyType == typeof(int))
                {
                    try
                    {
                        var value = property.GetValue(currentObject);
                        ((TextBox)(pnlBlocks.Children[index])).Text = ((int)value).ToString();
                    }
                    catch
                    {
                        tbShowing.AppendText(String.Format("Incorrect data at field {0}" + Environment.NewLine, property.Name));
                    }

                }

                else if (property.PropertyType == typeof(String))
                {
                    try
                    {
                        var value = property.GetValue(currentObject);
                        ((TextBox)(pnlBlocks.Children[index])).Text = value.ToString();
                    }
                    catch
                    {
                        tbShowing.AppendText(String.Format("Incorrect data at field {0}" + Environment.NewLine, property.Name));
                    }
                }

                else if (property.PropertyType == typeof(char))
                {
                    try
                    {
                        var value = property.GetValue(currentObject);
                        ((TextBox)(pnlBlocks.Children[index])).Text = ((char)value).ToString();
                    }
                    catch
                    {
                        tbShowing.AppendText(String.Format("Incorrect data at field {0}" + Environment.NewLine, property.Name));
                    }
                }


                else if (property.PropertyType.IsEnum)
                {
                    try
                    {
                        var value = property.GetValue(currentObject);

                        ((ComboBox)(pnlBlocks.Children[index])).SelectedItem = value.ToString();
                    }
                    catch
                    {
                        tbShowing.AppendText(String.Format("Incorrect data at field {0}" + Environment.NewLine, property.Name));
                    }
                }

                else if (property.PropertyType.IsClass)
                {
                    index++;
                    var agreg = property.GetValue(currentObject);
                    SetValuesOnForm(agreg, index);
                    index--;
                }

                index++;

            }


        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                switch (cmbDeletingType.SelectedIndex)
                {
                    case 0:
                        int IndexForModifying = Int32.Parse(tbDeleting.Text);
                        var modifiedWorker = workers[IndexForModifying - 1];
                        SetValuesOnForm(modifiedWorker, 0);
                        tbShowing.AppendText("Item is waiting for modifying..." + Environment.NewLine);
                        break;
                    case 1:
                        foreach (var worker in workers)
                        {
                            var property = worker.GetType().GetProperty("PassNum");
                            if (property.GetValue(worker).ToString() == tbDeleting.Text)
                            {
                                modifiedWorker = worker;
                                SetValuesOnForm(modifiedWorker, 0);
                                tbShowing.AppendText("Item is waiting for modifying..." + Environment.NewLine);
                                break;
                            }

                        }
                        break;

                }

            }
            catch
            {
                tbShowing.AppendText("Entered index or string is incorrect!" + Environment.NewLine);
            }
        }



        public object UpdateFieldsInObject(object obj, int index)
        {

            var properties = obj.GetType().GetProperties();

            foreach (var property in properties)
            {
                if (property.PropertyType == typeof(int))
                {
                    try
                    {
                        property.SetValue(obj, Int32.Parse(((TextBox)pnlBlocks.Children[index]).Text));
                    }
                    catch
                    {
                        tbShowing.AppendText(String.Format("Incorrect data at field {0}" + Environment.NewLine, property.Name));
                    }

                }

                else if (property.PropertyType == typeof(String))
                {
                    try
                    {
                        property.SetValue(obj, ((TextBox)pnlBlocks.Children[index]).Text);
                    }
                    catch
                    {
                        tbShowing.AppendText(String.Format("Incorrect data at field {0}" + Environment.NewLine, property.Name));
                    }
                }

                else if (property.PropertyType == typeof(char))
                {
                    try
                    {
                        property.SetValue(obj, Char.Parse(((TextBox)pnlBlocks.Children[index]).Text));
                    }
                    catch
                    {
                        tbShowing.AppendText(String.Format("Incorrect data at field {0}" + Environment.NewLine, property.Name));
                    }
                }


                else if (property.PropertyType.IsEnum)
                {
                    try
                    {
                        property.SetValue(obj, ((ComboBox)pnlBlocks.Children[index]).SelectedIndex);
                    }
                    catch
                    {
                        tbShowing.AppendText(String.Format("Incorrect data at field {0}" + Environment.NewLine, property.Name));
                    }
                }

                else if (property.PropertyType.IsClass)
                {
                    index++;
                    var agreg = UpdateFieldsInObject(property.GetValue(obj), index);
                    property.SetValue(obj, agreg);
                    index--;
                }

                index++;

            }

            return obj;

        }

        private void BtnModify_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                object modifiedWorker;
                switch (cmbDeletingType.SelectedIndex)
                {
                    case 0:
                        int IndexForModifying = Int32.Parse(tbDeleting.Text);
                        modifiedWorker = workers[IndexForModifying - 1];
                        workers[IndexForModifying - 1] = UpdateFieldsInObject(modifiedWorker, 0);
                        tbShowing.AppendText("Item was modifying successfully!" + Environment.NewLine);
                        break;
                    case 1:
                        for (int i = 0; i < workers.Count; i++)
                        {
                            var property = workers[i].GetType().GetProperty("PassNum");
                            if (property.GetValue(workers[i]).ToString() == tbDeleting.Text)
                            {
                                modifiedWorker = workers[i];
                                workers[i] = UpdateFieldsInObject(modifiedWorker, 0);
                                tbShowing.AppendText("Item was modifying successfully!" + Environment.NewLine);
                                break;
                            }

                        }

                        break;

                }

            }
            catch
            {
                tbShowing.AppendText("Entered index is incorrect!" + Environment.NewLine);
            }
        }

        private void CmbClasses_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
            pnlBlocks.Children.Clear();
            var currentClassName = cmbClasses.SelectedItem.ToString();

            foreach (var currentClass in ClassesTypesLib)
            {
                if (currentClassName == currentClass.Name)
                {
                    offset = 1;
                    CreateFieldsOnForm(currentClass, 20);

                    break;


                }
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            tbShowing.Text = "";
        }

        private void CmbSerializationType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void CmbDeletingType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (cmbDeletingType.SelectedIndex)
            {

                case 0:
                    tbDeleting.Text = "Number of object";
                    break;
                case 1:
                    tbDeleting.Text = "Pass number of object";
                    break;


            }

        }

        private HashSet<int> AddIndexesFromRange(string enteredRanges)
        {
            string correctRange = @"\b\d+\s*-\s*\d+\s*\b";
            Regex range = new Regex(correctRange);
            HashSet<int> returnedIndexes = new HashSet<int>();

            foreach (Match r in range.Matches(enteredRanges))
            {
                var currentRange = r.ToString();
                currentRange.Replace(" ", "");
                string correctIndex = @"\b\d+\b";
                Regex index = new Regex(correctIndex);
                MatchCollection indexes = index.Matches(currentRange);
                for (int i = Int32.Parse(indexes[0].ToString()) - 1; i < Int32.Parse(indexes[1].ToString()); i++)
                {
                    returnedIndexes.Add(i);

                }

            }

            return returnedIndexes;
        }

        private HashSet<int> AddCertainIndexes(string enteredRanges)
        {
            HashSet<int> returnedIndexes = new HashSet<int>();
            string correctIndex = @"\b\d+\b";
            Regex index = new Regex(correctIndex);
            MatchCollection indexes = index.Matches(enteredRanges);
            foreach (Match ind in indexes)
            {
                var i = Int32.Parse(ind.ToString());
                returnedIndexes.Add(i - 1);

            }
            return returnedIndexes;

        }



        private void BtnSerialize_Click(object sender, RoutedEventArgs e)
        {
            IMySerialization.IMySerialization Serializator;
            string mes = "";
            switch (cmbSerializationType.SelectedIndex)
            {
                case 0:
                    Serializator = new MyBinarySerialization();
                    mes = Serializator.MySerialize(workers, SaveInFileName);
                    break;
                case 1:
                    Serializator = new MyJsonSerialization();
                    mes = Serializator.MySerialize(workers, SaveInFileName);
                    break;
                case 2:
                    Serializator = new MyTextSerialization();
                    mes = Serializator.MySerialize(workers, SaveInFileName);
                    break;

            }
            if (mes == "OK")
            {
                tbShowing.AppendText("Serialization was ENDED successfully!" + Environment.NewLine);
            }
            else
            {
                tbShowing.AppendText("Error of serialization: " + mes + Environment.NewLine);
            }

        }



        private void BtnDeserialize_Click(object sender, RoutedEventArgs e)
        {
            IMySerialization.IMySerialization Deserializator;
            List<object> receivedObjects = new List<object>();
            switch (cmbSerializationType.SelectedIndex)
            {
                case 0:
                    Deserializator = new MyBinarySerialization();
                    receivedObjects = Deserializator.MyDeserialize(OpenForFileName);

                    break;
                case 1:
                    Deserializator = new MyJsonSerialization();
                    receivedObjects = Deserializator.MyDeserialize(OpenForFileName);
                    break;
                case 2:
                    Deserializator = new MyTextSerialization();
                    receivedObjects = Deserializator.MyDeserialize(OpenForFileName);
                    break;
                    
            }

            if (receivedObjects.Count != 0)
            {
                tbShowing.AppendText("Deserialization was ended successfully!" + Environment.NewLine);

                int number = 1;
                tbShowing.AppendText("Deserialized objects: " + Environment.NewLine);
                foreach (var currentObject in receivedObjects)
                {
                    tbShowing.AppendText(number.ToString() + ". ("
                                        + currentObject.GetType().Name + " ):" + Environment.NewLine);
                    ShowAllFields(currentObject);
                    tbShowing.AppendText(Environment.NewLine);
                    number++;
                }
            }
            else
            {
                tbShowing.AppendText("Error of deserialization! " + Environment.NewLine);
            }

        }



        private void BtnOpenFile_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog
            {
                FileName = "Serialized"
            };
            
            dlg.Filter = IMySerialization.SerializatorsArray.filters;

            bool? result = dlg.ShowDialog();
            if (result == true)
            {
                OpenForFileName = dlg.FileName;
                tbOpenFileName.Text = OpenForFileName;
                fileType = dlg.FileName.Split(new char[] { '.' })[1];
                cmbSerializationType.Text = fileType;
            }


        }

        private void BtnSaveFile_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog
            {
                FileName = "Serialized",

            };
           
            dlg.Filter = IMySerialization.SerializatorsArray.filters;

            bool? result = dlg.ShowDialog();

            if (result == true)
            {
                SaveInFileName = dlg.FileName;
                tbSaveFileName.Text = SaveInFileName;
                fileType = dlg.FileName.Split(new char[] { '.' })[1];
                cmbSerializationType.Text = fileType;
            }
        }

        private void BtnSaveCompressed_Click(object sender, RoutedEventArgs e)
        {

            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog
            {
                FileName = "Compressed.cmp",

            };

            dlg.Filter = "compressed files (*.cmp)|*.cmp";

            bool? result = dlg.ShowDialog();

            if (result == true)
            {
                SaveCompressedFileName = dlg.FileName;
                tbSaveCompressed.Text = SaveCompressedFileName;
              
            }

        }

        private void BtnCompress_Click(object sender, RoutedEventArgs e)
        {
            foreach(var currPlugin in PluginsLib)
            {
                if(currPlugin.Name == cmbPlugins.SelectedItem.ToString())
                {
                    
                    var obj = PluginsAssembly.CreateInstance(currPlugin.FullName);
                    var method = currPlugin.GetMethod("Compress");
                    method.Invoke(obj, new object[] { workers, SaveCompressedFileName });
                    tbShowing.AppendText("Compression was ended successfully!" + Environment.NewLine);
                    break;
                }
            }
            
            

        }

        private void BtnOpenCompressed_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog
            {
                FileName = "Compressed.cmp"
            };

            dlg.Filter = "compressed files (*.cmp)|*.cmp";

            bool? result = dlg.ShowDialog();
            if (result == true)
            {
                OpenCompressedFileName = dlg.FileName;
                tbOpenCompressed.Text = OpenCompressedFileName;
              
            }
        }

        private void BtnDecompress_Click(object sender, RoutedEventArgs e)
        {
            byte marker = (byte)PluginsAssembly.GetType("IArchivePlugin.PluginIdentifier").GetMethod("FindPluginMarker", BindingFlags.Public | BindingFlags.Static).Invoke(null, new object[] { OpenCompressedFileName });
            List<object> receivedObjects = new List<object>();

            foreach(var currPlugin in PluginsLib)
            {
                var obj = PluginsAssembly.CreateInstance(currPlugin.FullName);
                if (marker ==(byte)currPlugin.GetProperty("marker").GetValue(obj))
                {
                    var method = currPlugin.GetMethod("Decompress");
                    receivedObjects = method.Invoke(obj, new object[] { OpenCompressedFileName }) as List<object>;
                    break;
                }
               
            }
          
            if (receivedObjects.Count != 0)
            {
                tbShowing.AppendText("Decompression was ended successfully!" + Environment.NewLine);

                int number = 1;
                tbShowing.AppendText("Decompressed objects: " + Environment.NewLine);
                foreach (var currentObject in receivedObjects)
                {
                    tbShowing.AppendText(number.ToString() + ". ("
                                        + currentObject.GetType().Name + " ):" + Environment.NewLine);
                    ShowAllFields(currentObject);
                    tbShowing.AppendText(Environment.NewLine);
                    number++;
                }
            }
            else
            {
                tbShowing.AppendText("Error of decompression! " + Environment.NewLine);
            }

        }
    }
}