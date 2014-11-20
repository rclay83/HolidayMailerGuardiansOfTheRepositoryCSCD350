using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Email
{
    interface I_Account
    {
        void setSender(string sender);

        void setPassword(string password);

        void setUsername(string username);

        void setSMTPserver(string smtpServer);

        void setPort(int port);

        void setSSL(bool enabled);
    }
}
