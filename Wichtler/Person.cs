using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wichtler
{
    internal sealed class Person
    {
        public string Name { get; set; }

        private string fullName;
        public string FullName
        {
            get
            {
                if (string.IsNullOrEmpty(fullName))
                    return Name;
                return fullName;
            }
            set { fullName = value; }
        }
        public string Family { get; set; }
        public string Email { get; set; }
    }
}
