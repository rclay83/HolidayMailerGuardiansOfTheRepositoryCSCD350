using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Email
{
    public interface I_Account
    {
        string Sender { get; set; }

        string Password { get; set; }

        string Username { get; set; }

        string SmtpServer { get; set; }

        int Port { get; set; }

        bool SSL { get; set; }
    }
}
