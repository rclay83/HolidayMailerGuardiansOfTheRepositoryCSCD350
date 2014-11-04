using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ContactData
{
    public class ContactDataExcpetion : Exception
    {
        private string message;

        public ContactDataExcpetion(string message)
        {
            
            this.message = message;
        }
    }
}
