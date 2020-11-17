using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMySerialization
{
    public interface IMySerialization
    {
        string MySerialize(List<object> workers, string fileName);
        List<object> MyDeserialize(string fileName);
    }
}
