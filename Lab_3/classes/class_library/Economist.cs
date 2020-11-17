using System;
using System.Collections.Generic;
using System.Text;

namespace classes_library
{
    [Serializable]
    public class Economist : Clerk
    {
        public enum CalcBrand
        {
            Casio,
            Rebell,
            Citizen,
            Canon,
            inFormat

        }
        public CalcBrand Calculator { get; set; }
       

        //public void DoReport()
        //{
        //    Console.WriteLine("I am doing report");
        //}

        //public void PassReport()
        //{
        //    Console.WriteLine("I have finished doing report");
        //}



    }

}
