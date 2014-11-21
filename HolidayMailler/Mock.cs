using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Email;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;

namespace HolidayMailler
{
    public class MockMailMan : Email.I_MailMan
    {
        private string[] _recipients;
        private string _subject;
        private string _body;

        private I_Account _account;
        private MailMessage _mail;

        public MockMailMan (I_Account account, string[] Recipients, string Subject, string Body)
        {
            _recipients = Recipients;
            _subject = Subject;
            _body = Body;
            _account = account;
            _mail = new MailMessage();
        }

        public void sendMail ()
        {
            try
            {
                SmtpClient SmtpServer = getSMTPClient();

                _mail.From = new MailAddress(_account.Sender);
                foreach (string recipient in _recipients)
                {
                    _mail.To.Add(recipient);
                }

                _mail.Subject = _subject;
                _mail.Body = _body;

                SmtpServer.Send(_mail);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void setAttachment (string[] files)
        {
            foreach (string file in files)
            {
                Attachment data = new Attachment(file, MediaTypeNames.Application.Octet);

                // Add time stamp information for the file.
                ContentDisposition disposition = data.ContentDisposition;
                disposition.CreationDate = System.IO.File.GetCreationTime(file);
                disposition.ModificationDate = System.IO.File.GetLastWriteTime(file);
                disposition.ReadDate = System.IO.File.GetLastAccessTime(file);
                // Add the file attachment to this e-mail message.

                _mail.Attachments.Add(data);
            }
        }

        private SmtpClient getSMTPClient ()
        {
            SmtpClient SmtpClient = new SmtpClient(_account.SmtpServer);

            SmtpClient.Port = _account.Port;
            SmtpClient.Credentials = new System.Net.NetworkCredential(_account.Username, _account.Password);
            SmtpClient.EnableSsl = _account.SSL;

            return SmtpClient;
        }
    }

    public class MockAccount : I_Account
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
    }
}


