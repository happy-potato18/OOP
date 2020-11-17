using System;
using System.Collections.Generic;

namespace classes_library
{
    [Serializable]
    abstract public class Worker
    {
        public string _passnum;
        public string PassNum
        {
            get { return _passnum; }
            set
            {
                if ((value.Length != 8) || (!Int32.TryParse(value, out int i) ))
                {
                    _passnum = "00000000";
                }
                else
                {
                    _passnum = value;
                }
            }
        
                
        }
        public Profile Profile { get; set; }


       // public void ComeToWork()
       // {
       //     Console.WriteLine("I have already coming!");
       // }

        //public void GetAwayFromWork()
        //{
        //    Console.WriteLine("I have already been going home!");
        //}

        abstract public string AddWorker();

        abstract public string DeleteWorker();

        abstract public string ChangeWorkerInfo();

    }
 
}
