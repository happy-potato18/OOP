using System;
using System.Collections.Generic;
using System.Text;

namespace classes_library
{

    public static class ElectrisianExtension
    {
        public static int CountTools(this Electrician el)
        {
            return 0;
        }
    }
        
    [Serializable]
    public class Electrician : Clerk
    {
        public enum Tools
        {
            Screwdriver,
            Hammer,
            Nails,
            Lantern,
            Drill

        }
        public Tools Tool { get; set; }
        

    }

}
