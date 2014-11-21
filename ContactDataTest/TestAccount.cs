using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Email;

namespace ContactDataTest
{
    class TestAccount : I_Account
    {
        public string Password
        {
            get
            {
                return "supersecurepassword";
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public int Port
        {
            get
            {
                return 465;
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public bool SSL
        {
            get
            {
                return true;
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public string Sender
        {
            get
            {
                return "guardiansoftherepository@gmail.com";
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public string SmtpServer
        {
            get
            {
                return "smtp.gmail.com";
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public string Username
        {
            get
            {
                return "guardiansoftherepository@gmail.com";
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public override bool Equals(object obj)
        {
            TestAccount that = obj as TestAccount;
            if (that == null)
            {
                return false;
            }


            return this.Username == that.Username;
        }

        public override int GetHashCode()
        {
            return (this.Username == null ? 17 : this.Username.GetHashCode()) ^ (this.Password == null ? 17 : this.Password.GetHashCode());
        }
    }
}
