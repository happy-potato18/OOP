using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace classes_library
{
    public class Manager : Worker
    {
        public string _department;
        public int _quantofinfer;
        public string Department
        {
            get { return _department; }

            set
            {
                if ((value.Length > 25 ) || (value.Length == 0) || (!Regex.IsMatch(value, @"^([a-zA-Z]+\s*)+$")))
                {
                    _department = "INCORRECT_DEPARTAMENT_NAME";
                }
                else
                {
                    _department = value;
                }
            }
        }
        public int QuantOfInfer
        {
            get { return _quantofinfer; }

            set
            {
                if ((value > 100000) || (value < 1))
                {
                    _quantofinfer = 0;
                }
                else
                {
                    _quantofinfer = value;
                }

            }
        }

        public override string AddWorker()
        {
            throw new NotImplementedException();
        }

        //public void CallIntoOffice()
        //{
        //    Console.WriteLine("Come to my office, please");
        //}

        public override string ChangeWorkerInfo()
        {
            throw new NotImplementedException();
        }

        public override string DeleteWorker()
        {
            throw new NotImplementedException();
        }
    }

}
