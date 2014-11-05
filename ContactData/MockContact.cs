using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ContactData
{
    public class MockContact:IContact
    {
        private bool _gotMail;
        public string FirstName
        {
            get
            {
                return "Mock First Name";
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public string LastName
        {
            get
            {
                return "Mock Last Name";
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public string Email
        {
            get
            {
                return "mock@mockemail.com";
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public bool GotMail
        {
            get
            {
                return this._gotMail;
            }
            set
            {
                this._gotMail = value;
            }
        }
    }
}
