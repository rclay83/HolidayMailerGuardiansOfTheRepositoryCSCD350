using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Email
{
    class Account : I_Account
    {
        private string _username;
        private string _password;
        private string _sender;
        private string _smtpServer;
        private int _port;
        private bool _enableSSL;

        public string Username
        {
            get
            {
                if (_username == null)
                    throw new ArgumentNullException("Username can't be null.");
                return _username;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("Username can't be null.");
                _username = value;
            }
        }
        public string Password
        {
            get
            {
                if (_password == null)
                    throw new ArgumentNullException("Password can't be null");
                return _password;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("Password can't be null");
                _password = value;
            }
        }

        public string Sender
        {
            get
            {
                if (_sender == null)
                    throw new ArgumentNullException("Sender can't be null.");
                return _sender;
            }

            set
            {
                if (value == null)
                    throw new ArgumentNullException("Sender can't be null.");
                _sender = value;
            }
        }

        public string SmtpServer
        {
            get
            {
                if (_smtpServer == null)
                    throw new ArgumentNullException("SMTP server can't be null.");
                return _smtpServer;
            }

            set
            {
                if (value == null)
                    throw new ArgumentNullException("SMTP server can't be null.");
                _smtpServer = value;
            }
        }
        public int Port
        {
            get
            {
                if (_port == 0)
                    throw new ArgumentNullException("Port can't be null.");
                return _port;
            }

            set
            {
                if (value == 0)
                    throw new ArgumentNullException("Port can't be null.");
                _port = value;
            }
        }
        public bool SSL
        {
            get
            {
                return _enableSSL;
            }
            set
            {
                _enableSSL = value;
            }
        }

        public Account()
        {

        }
    }
}
