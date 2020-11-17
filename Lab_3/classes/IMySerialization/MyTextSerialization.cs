using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMySerialization
{
    public class MyTextSerialization
    {
        public string MySerialize(List<object> workers, string fileName)
        {
            try
            {

                using (StreamWriter sw = new StreamWriter(fileName, false, System.Text.Encoding.Default))
                {
                    foreach (var worker in workers)
                    {
                        var properties = worker.GetType().GetProperties();
                        List<object> values = new List<object>();
                        string objectType = worker.GetType().FullName;
                        sw.WriteLine("Object type: " + objectType);
                        foreach(var property in properties)
                        {
                            object value = new object();
                            worker.GetType().GetProperty(property.Name).GetValue(value);
                            values.Add(value);
                            sw.WriteLine(property.PropertyType.FullName +": " + value.ToString());
                        }
                        
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
            List<object> receivedObjects = new List<object>();

            using (StreamReader sr = new StreamReader(fileName))
            {
                while (!sr.EndOfStream)
                {
                    string JsonString = sr.ReadLine();
                   
                    var deserialized = "" ;
                    receivedObjects.Add(deserialized);
                }
            }



            return receivedObjects;

        }
    


    }
}
