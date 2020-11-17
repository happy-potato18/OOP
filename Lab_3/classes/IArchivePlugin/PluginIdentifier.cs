using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace IArchivePlugin
{
    public class PluginIdentifier
    {
        public static byte FindPluginMarker(string filename)
        {
            byte marker = 255;
            using (FileStream originalFileStream = new FileStream(filename, FileMode.Open))
            {
               marker =(byte)originalFileStream.ReadByte();
               
            }
            return marker;
        }
    }
}
