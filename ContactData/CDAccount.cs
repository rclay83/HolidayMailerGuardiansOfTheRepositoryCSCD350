using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Email;

namespace ContactData
{
    public class CDAccount : I_Account
    {
        public string Password { get; set; }

        public int Port { get; set; }


        public bool SSL { get; set; }

        public string Sender { get; set; }


        public string SmtpServer { get; set; }

        public string Username { get; set; }

        public override bool Equals(object obj)
        {
            CDAccount that = obj as CDAccount;
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
