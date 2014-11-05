using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Email
{
    class Class
    {
        static void Main(string[] args)
        {
            string[] recipients = new string[] { "rmarliere@gmail.com", "rodrigo@maisapp.com.br" };
            MailMan mail = new MailMan(recipients, "this is the subject", "this is the body");
            mail.sendMail();
        }
    }
}
