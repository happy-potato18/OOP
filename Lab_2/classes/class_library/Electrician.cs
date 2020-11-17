using System;
using System.Collections.Generic;
using System.Text;

namespace classes_library
{
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
