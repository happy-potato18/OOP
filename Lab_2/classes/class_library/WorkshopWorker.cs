using System;
using System.Collections.Generic;
using System.Text;

namespace classes_library
{
    public class WorkshopWorker : Worker
    {
        public enum Shifts
        {
            Morning,
            Afternoon,
            Evening,
            Night
        }


       
        public Shifts Sft { get; set; }
       

        
        //public void HaveSmokeBreak()
        //{
        //    Console.WriteLine("I am smoking now...");
        //}

        //public void EndSmokeBreak()
        //{
        //    Console.WriteLine("I have ended smoking...");
        //}

        public override string AddWorker()
        {
            throw new NotImplementedException();
        }

        public override string DeleteWorker()
        {
            throw new NotImplementedException();
        }

        public override string ChangeWorkerInfo()
        {
            throw new NotImplementedException();
        }
    }

}
