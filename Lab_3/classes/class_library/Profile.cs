using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;




namespace classes_library
{
    [Serializable]
    sealed public class Profile
   {
        public string _name;

        public string Name
        {
            get { return _name; }

            set
            {
                if ((value.Length > 40) || (value.Length == 0) || (!Regex.IsMatch(value, @"^([a-zA-Z]+\s*)+$")))
                {
                    _name = "INCORRECT_NAME";
                }
                else
                {
                    _name = value;
                }
            }

        }
        public string _surname;

        public string Surname
        {
            get { return _surname; }

            set
            {
                if ((value.Length > 40) || (value.Length == 0) || (!Regex.IsMatch(value, @"^([a-zA-Z]+\s*)+$")))
                {
                    _surname = "INCORRECT_SURNAME";
                }
                else
                {
                    _surname = value;
                }
            }

        }

        public int _age;
        public int Age
        {
            get
            {
                return _age;
            }

            set
            {
                if ((value < 18) || (value > 75) )
                {
                    _age = 0;
                }
                else
                {
                    _age = value;
                }
            }
        }

        public char _sex;
        public char Sex
        {
            get { return _sex; }

            set
            {
                if ((char.ToUpper(value) != 'M') && (char.ToUpper(value) != 'F') )
                {
                    _sex = '?';
                }
                else
                {
                    _sex = value;
                }
            }
        }


        public string _university;

        public string University
        {
            get { return _university; }

            set
            {
                if ((value.Length > 80) || (value.Length == 0) || (!Regex.IsMatch(value, @"^([a-zA-Z]+\s*)+$")))
                {
                    _university = "INCORRECT_UNIVERSITY_NAME";
                }
                else
                {
                    _university = value;
                }
            }

        }

        public int Salary { get; set; }
               
    }
}
