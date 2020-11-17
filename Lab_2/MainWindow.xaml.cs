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

namespace Lab_2
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        static List<object> workers = new List<object>();
        static Assembly ClassesAssembly;
        static List<Type> ClassesTypesLib = new List<Type>();
        static int offset = 1;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void App_Loaded(object sender, RoutedEventArgs e)
        {

            ClassesAssembly = Assembly.LoadFile(@"D:\УНИК\4 сем\OOP\Lab_2\classes\class_library\bin\Debug\class_library.dll");
            ClassesTypesLib = ClassesAssembly.GetTypes().Where(type => type.IsClass).ToList();

            foreach (var currentClass in ClassesTypesLib)
            {
                if ((!currentClass.IsAbstract) && (!currentClass.IsSealed))
                    cmbClasses.Items.Add(currentClass.Name);
            }

            if (cmbClasses.Items.Count > 0)
            {
                cmbClasses.SelectedIndex = 0;
            }



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
                        tbShowing.AppendText(String.Format("Incorrect data at field {0}" + Environment.NewLine,property.Name));
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
                        property.SetValue(obj,((TextBox)pnlBlocks.Children[index]).Text[0]);
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
            //pnlBlocks.Children.Clear(); 
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
                    var newComboBox = new ComboBox();
                    newComboBox.Margin = new Thickness(leftMargin, 5 * offset, 0, 0);
                    newComboBox.Name = "ComboBox" + property.Name;
                    newComboBox.Text = property.Name;

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
                        for(int i = 0; i < workers.Count; i++)
                        {
                            var property = workers[i].GetType().GetProperty("PassNum");
                            if (property.GetValue(workers[i]).ToString() == tbDeleting.Text)
                            {
                                workers.RemoveAt(i);
                                tbShowing.AppendText("Item was deleted successfully!" + Environment.NewLine);
                                //break;
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
                    tbShowing.AppendText("\t" + property.Name+ ": " +property.GetValue(currentObject)
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
                                    + currentObject.GetType().Name + " ):" + Environment.NewLine );
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
    }
}
