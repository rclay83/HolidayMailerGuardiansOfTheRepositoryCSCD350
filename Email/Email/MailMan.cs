using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;

namespace Email
{
    class MailMan : I_MailMan
    {
        private string[] _recipients;
        private string _subject;
        private string _body;

        private I_Account _account;
        private MailMessage _mail;

        public MailMan(I_Account account, string[] Recipients, string Subject, string Body)
        {
            _recipients = Recipients;
            _subject    = Subject;
            _body       = Body;
            _account    = account;
            _mail = new MailMessage();
        }

        public void sendMail() 
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
                _mail.Body    = _body;

                SmtpServer.Send(_mail);
            }
            catch (Exception ex)
            {
                throw ex;        
            }
        }

        public void setAttachment(string[] files)
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

        private SmtpClient getSMTPClient()
        {
            SmtpClient SmtpClient = new SmtpClient(_account.SmtpServer);

            SmtpClient.Port = _account.Port;
            SmtpClient.Credentials = new System.Net.NetworkCredential(_account.Username, _account.Password);
            SmtpClient.EnableSsl = _account.SSL;

            return SmtpClient;
        }
    }
}
