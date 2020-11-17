using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace IMySerialization
{

    public class MyTextSerialization : IMySerialization
    {
        static Assembly ClassesAssembly;
        static List<Type> ClassesTypesLib = new List<Type>();
        private const string OBJECT_DEFENITION = "Object type: ";
        private const int OBJECT_DEFENITION_BEGINING = 13;
       // private const string OBJECT_END_STRING_MARK = "END.";

        private static void WriteFields(object currentObject, StreamWriter sw)
        {

            var properties = currentObject.GetType().GetProperties();


            foreach (var property in properties)
            {
                sw.WriteLine(property.Name + ": " + property.GetValue(currentObject));
                if ((property.PropertyType.IsClass)
                    && (property.PropertyType != typeof(String)))
                {
                    WriteFields(property.GetValue(currentObject), sw);
                }

            }



        }


        private static object ReadFields(Type currentClass, StreamReader sr)
        {
            var obj = ClassesAssembly.CreateInstance(currentClass.FullName);
            var properties = currentClass.GetProperties();

            foreach (var property in properties)
            {
                string info = sr.ReadLine();
                string value = info.Substring(property.Name.Length + ": ".Length);
                if (property.PropertyType == typeof(int))
                {
                    try
                    {
                        property.SetValue(obj, Int32.Parse(value));
                    }
                    catch
                    {

                    }

                }

                else if (property.PropertyType == typeof(String))
                {
                    try
                    {
                        property.SetValue(obj, value);
                    }
                    catch
                    {

                    }
                }

                else if (property.PropertyType == typeof(char))
                {
                    try
                    {
                        property.SetValue(obj, value[0]);
                    }
                    catch
                    {

                    }
                }


                else if (property.PropertyType.IsEnum)
                {
                    try
                    {
                        property.SetValue(obj, value);
                    }
                    catch
                    {

                    }
                }

                else if (property.PropertyType.IsClass)
                {

                    var agreg = ReadFields(property.PropertyType, sr);
                    property.SetValue(obj, agreg);
                }

            }

            return obj;

        }



        
        public string MySerialize(List<object> workers, string fileName)
        {
            try
            {

                using (StreamWriter sw = new StreamWriter(fileName, false, System.Text.Encoding.Default))
                {
                    foreach (var worker in workers)
                    {
                        string objectType = worker.GetType().FullName;
                        sw.WriteLine(OBJECT_DEFENITION + objectType);
                        WriteFields(worker, sw);
                        //sw.WriteLine(OBJECT_END_STRING_MARK);
                        
                    }

                }
                return "OK";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }


        }

        public List<object> MyDeserialize(string fileName)
        {

            ClassesAssembly = Assembly.LoadFile(@"D:\УНИК\4 сем\OOP\Lab_3\classes\class_library\bin\Debug\class_library.dll");
            ClassesTypesLib = ClassesAssembly.GetTypes().Where(type => type.IsClass).ToList();
            List<object> receivedObjects = new List<object>();

            using (StreamReader sr = new StreamReader(fileName))
            {
                while (!sr.EndOfStream)
                {
                    string info = sr.ReadLine();
                   
                    if (info.Contains(OBJECT_DEFENITION))
                    {
                        info = info.Substring(OBJECT_DEFENITION_BEGINING);
                        IEnumerable<Type> objectType = ClassesTypesLib.Where<Type>(tp => tp.FullName == info);
                        object worker = ReadFields(objectType.ToList()[0],sr);
                        receivedObjects.Add(worker);
                    }
                }
            }



            return receivedObjects;

        }
    


    }
}
