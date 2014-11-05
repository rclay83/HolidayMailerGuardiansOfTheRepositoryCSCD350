using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Email
{
    public class MailMan
    {
        private string[] _recipients;
        private string _subject;
        private string _body;

        private string _username;
        private string _password;
        private string _sender;

        private string _smtpServer;

        public MailMan(string[] Recipients, string Subject, string Body)
        {
            _recipients = Recipients;
            _subject    = Subject;
            _body       = Body;
            setSMTPserver("smtp.gmail.com");
            setSender("guardiansoftherepository@gmail.com");
            setPassword("supersecurepassword");
            setUsername("guardiansoftherepository@gmail.com");
        }

        public void sendMail() 
        {
            try
            {
                MailMessage mail = new MailMessage();
                SmtpClient SmtpServer = new SmtpClient(_smtpServer);

                mail.From = new MailAddress(_sender);
                foreach (string recipient in _recipients)
                {
                    mail.To.Add(recipient);
                }

                mail.Subject = _subject;
                mail.Body    = _body;

                SmtpServer.Port = 587;
                SmtpServer.Credentials = new System.Net.NetworkCredential(_username, _password);
                SmtpServer.EnableSsl = true;

                SmtpServer.Send(mail);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);              
            }
        }

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
    }
}
