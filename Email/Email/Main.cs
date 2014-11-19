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
            Account account = new Account();
            account.setSMTPserver("smtp.gmail.com");
            account.setSender("guardiansoftherepository@gmail.com");
            account.setPassword("supersecurepassword");
            account.setUsername("guardiansoftherepository@gmail.com");
            account.setPort(587);
            account.setSSL(true);

            MailMan mail = new MailMan(account, recipients, "this is the subject", "this is the body");
            //string [] files = new string[] { @"C:\Users\Rodrigo Marliere\Desktop\iMapp-cache\cacheIndex.txt" };
            //mail.setAttachment(files);
            mail.sendMail();
        }
    }
}
