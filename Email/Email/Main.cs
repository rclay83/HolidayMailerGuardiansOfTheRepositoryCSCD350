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
            I_Account account   = new Account();
            account.SmtpServer  = @"smtp.gmail.com";
            account.Sender      = @"guardiansoftherepository@gmail.com";
            account.Password    = @"supersecurepassword";
            account.Username    = @"guardiansoftherepository@gmail.com";
            account.Port        = 587;
            account.SSL         = true;

            I_MailMan mail = new MailMan(account, recipients, "this is the subject", "this is the body");
            string [] files = new string[] { @"C:\Users\Rodrigo Marliere\Desktop\iMapp-cache\cacheIndex.txt" };
            mail.setAttachment(files);
            mail.sendMail();
        }
    }
}
