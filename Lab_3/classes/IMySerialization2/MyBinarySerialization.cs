using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace IMySerialization
{
    public class MyBinarySerialization : IMySerialization
    {
        public string MySerialize(List<object> workers,string fileName)
        {
            try
            {
                BinaryFormatter formatter = new BinaryFormatter();
                using (FileStream objectsFile = new FileStream(fileName, FileMode.OpenOrCreate))
                {
                        formatter.Serialize(objectsFile, workers);
                }

                return "OK";
            }
            catch(Exception ex)
            {
                return ex.Message;
            }
           

        }

        public List<object> MyDeserialize(string fileName)
        {
           List<object> receivedObjects = new List<object>(); 

          
            BinaryFormatter formatter = new BinaryFormatter();
            using (FileStream fs = new FileStream(fileName, FileMode.Open))
            {
               
                var deserialized = formatter.Deserialize(fs);
                foreach(var obj in (deserialized as List<object>))
                {
                    receivedObjects.Add(obj);
                }

            }

            return receivedObjects ;
          


        }
    }
}
