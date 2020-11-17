using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.IO.Compression;
using IMySerialization;
using System.Runtime.Serialization.Formatters.Binary;


namespace IArchivePlugin
{
    public class Deflate : IPlugin
    {
        public byte marker
        {
            get
            {
                return 1;
            }
        }
        public void Compress(List<object> workers, string filename)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            using (FileStream objectsFile = new FileStream("buffer", FileMode.OpenOrCreate))
            {
                formatter.Serialize(objectsFile, workers);
            }

           

            using (FileStream objectsFile = new FileStream("buffer", FileMode.Open))
            {
                using (FileStream compressedFileStream = File.Create(filename))
                {
                    compressedFileStream.WriteByte(marker);
                    using(DeflateStream compressionStream = new DeflateStream(compressedFileStream, CompressionMode.Compress))
                    {
                        objectsFile.CopyTo(compressionStream);
                    }
                }
            }

            System.IO.File.Delete("buffer");




        }

        public List<object> Decompress(string filename)
        {
            using (FileStream originalFileStream = new FileStream(filename, FileMode.Open))
            {
                originalFileStream.ReadByte();
                using (FileStream decompressedFileStream = File.Create("buffer"))
                {
                    using (DeflateStream decompressionStream = new DeflateStream(originalFileStream, CompressionMode.Decompress))
                    {
                        decompressionStream.CopyTo(decompressedFileStream);
                    }
                }
            }


            List<object> receivedObjects = new List<object>();


            BinaryFormatter formatter = new BinaryFormatter();
            using (FileStream fs = new FileStream("buffer", FileMode.Open))
            {
                
                var deserialized = formatter.Deserialize(fs);
                foreach (var obj in (deserialized as List<object>))
                {
                    receivedObjects.Add(obj);
                }

            }


            System.IO.File.Delete("buffer");

            return receivedObjects;




        }
    }
    
}
