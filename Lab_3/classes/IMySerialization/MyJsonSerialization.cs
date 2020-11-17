using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.IO;
using System.Buffers;
using Newtonsoft.Json;
using JsonSerializer = Newtonsoft.Json.JsonSerializer;
using classes_library;

namespace IMySerialization
{
    public class MyJsonSerialization : IMySerialization
    {


        public string MySerialize(List<object> workers, string fileName)
        {
            try
            {

                using (StreamWriter sw = new StreamWriter(fileName, false, System.Text.Encoding.Default))
                {
                        foreach(var worker in workers)
                        {
                            
                            var settings = new JsonSerializerSettings()
                            {
                                TypeNameHandling = TypeNameHandling.All
                            };
                            var jsonString = JsonConvert.SerializeObject(worker, settings);
                            sw.WriteLine(jsonString);
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
                    var settings = new JsonSerializerSettings()
                    {
                        TypeNameHandling = TypeNameHandling.All
                    };

                    var deserialized = JsonConvert.DeserializeObject(JsonString, settings);
                    receivedObjects.Add(deserialized);
                }
            }

         
            
            return receivedObjects;
            
        }
    }
}
