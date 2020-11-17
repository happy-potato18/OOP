using System;
using System.Collections.Generic;
using System.Text;

namespace classes_library
{
    public class Director : Manager
    {
        public int _shareprocent;
        public int ShareProcent
        {
            get { return _shareprocent; }

            set
            {
                if ( (value < 1) || (value > 100) )
                {
                    _shareprocent = 0;
                }
                else
                {
                    _shareprocent = value;
                }
            }

        }

        //public void SackWorker()
        //{
        //    Console.WriteLine("Good luck in looking for new job");
        //}

        //public void EmployWorker()
        //{
        //    Console.WriteLine("Congrats, you have became our new worker");
        //}

    }

}
