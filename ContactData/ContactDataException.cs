using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ContactData
{
    public class ContactDataException : Exception
    {
        private string message;

        public ContactDataException(string message)
        {
            
            this.message = message;
        }
    }
}
