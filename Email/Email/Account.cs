using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Email
{
    public class Account : I_Account
    {
        private string _username;
        private string _password;
        private string _sender;
        private string _smtpServer;
        private int _port;
        private bool _enableSSL;

        public Account()
        {
            setSMTPserver("smtp.gmail.com");
            setSender("guardiansoftherepository@gmail.com");
            setPassword("supersecurepassword");
            setUsername("guardiansoftherepository@gmail.com");
        }

        public SmtpClient getSMTPClient()
        {
            SmtpClient SmtpServer = new SmtpClient(getSMTPServer());

            SmtpServer.Port = getPort();
            SmtpServer.Credentials = new System.Net.NetworkCredential(getUsername(), getPassword());
            SmtpServer.EnableSsl = _enableSSL;

            return SmtpServer;
        }

        /**
         * 
         * Getters
         * 
         **/

        public string getSender()
        {
            if (_sender == null)
                throw new Exception("Sender is null");
            return _sender;
        }

        private int getPort()
        {
            if (_port == null)
                throw new Exception("Port is null");
            return _port;
        }

        private string getSMTPServer()
        {
            if (_smtpServer == null)
                throw new Exception("SMTP is null");
            return _smtpServer;
        }

        private string getPassword()
        {
            if (_password == null)
                throw new Exception("Password is null");
            return _password;
        }

        private string getUsername()
        {
            if (_username == null)
                throw new Exception("Username is null");
            return _username;
        }

        /**
         * 
         * Setters
         * 
         **/

        public void setSender(string sender)
        {
            if (sender == null)
                throw new Exception("sender can't be null");
            _sender = sender;
        }

        public void setPassword(string password)
        {
            if (password == null)
                throw new Exception("password can't be null");
            _password = password;
        }

        public void setUsername(string username)
        {
            if (username == null)
                throw new Exception("username can't be null");
            _username = username;
        }

        public void setSMTPserver(string smtpServer)
        {
            if (smtpServer == null)
                throw new Exception("smtp server can't be null");
            _smtpServer = smtpServer;
        }

        public void setPort(int port)
        {
            if (_port == null)
                throw new Exception("port can't be null");
            _port = port;
        }

        public void setSSL(bool enabled)
        {
            _enableSSL = enabled;
        }

    }
}
