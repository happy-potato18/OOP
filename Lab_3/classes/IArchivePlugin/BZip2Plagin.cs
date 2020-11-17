using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using ICSharpCode.SharpZipLib.BZip2;

namespace IArchivePlugin
{
    public class BZip2 : IPlugin
    {
        public byte marker
        {
            get
            {
                return 2;
            }
        }
        public void Compress(List<object> workers, string filename)
        {

            BinaryFormatter formatter = new BinaryFormatter();
            using (FileStream objectsFile = new FileStream("buffer", FileMode.OpenOrCreate))
            {
                formatter.Serialize(objectsFile, workers);
            }

            FileInfo fileToBeZipped = new FileInfo("buffer");
            FileInfo zipFileName = new FileInfo(filename);
            using (FileStream objectsFile = fileToBeZipped.OpenRead())
            {
                using (FileStream compressedFileStream = zipFileName.Create())
                {
                    compressedFileStream.WriteByte((byte)marker);
                    ICSharpCode.SharpZipLib.BZip2.BZip2.Compress(objectsFile, compressedFileStream, true, 4096);
                  
                }
            }

           
            System.IO.File.Delete("buffer");

        }

        public List<object> Decompress(string filename)
        {
            using (FileStream originalFileStream = new FileStream(filename, FileMode.Open))
            {
                originalFileStream.ReadByte();
                using (FileStream decompressedStream = File.Create("buffer"))
                {
                  ICSharpCode.SharpZipLib.BZip2.BZip2.Decompress(originalFileStream, decompressedStream, true);
                  
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
