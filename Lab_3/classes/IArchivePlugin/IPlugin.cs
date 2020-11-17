using System;
using System.Collections.Generic;

namespace IArchivePlugin
{
    public interface IPlugin
    {
        byte marker { get; }
        void Compress(List<object> workers,string filename);
        List<object> Decompress(string filename);

    }
}
