using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ContactData
{
    public class Contact:IContact
    {
        private string _email;
        private string _lastName;
        private string _firstName;
        private bool _gotMail;

        public Contact() { }
        public Contact(string firstName, string lastName, string email)
        {
            this._firstName = firstName;
            this._lastName = lastName;
            this._email = email;
        }

        public String FirstName
        {
            get { return this._firstName; }
            set { this._firstName = value; }
        }

        public String LastName
        {
            get { return this._lastName; }
            set { this._lastName = value; }
        }

        public String Email
        {
            get { return this._email; }
            set { this._email = value; }
        }

        public bool GotMail
        {
            get { return this._gotMail; }
            set { this._gotMail = value; }
        }
    }
}
