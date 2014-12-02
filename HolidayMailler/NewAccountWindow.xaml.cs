using Email;
using System.Text.RegularExpressions;
using System.Windows;

/*  Holiday Mailer
 *  Guardians of the Repository
 * 
 *  Author: Marcus Sanchez
 *  Last revision:  11/20/2014
 *  
 *  NewAccountWindow class is responsible for providing fields for the user
 *  to specify new account information
 */

namespace HolidayMailler
{
    internal enum MailService
    {
        Gmail = 0,
        Yahoo = 1,
        Hotmail = 2
    }

    public partial class NewAccountWindow : Window
    {
        private I_Account account;

        public NewAccountWindow (I_Account account)
        {
            this.account = account;
            InitializeComponent();
        }

        /*  Mail services utilize same ports and SSL, only unique attribute is SmtpServer
         *  
         *  Regex taken from MSDN Site: http://msdn.microsoft.com/en-us/library/01escwtf%28v=vs.110%29.aspx
         */

        private void addButton_Click (object sender, RoutedEventArgs e)
        {
            string emailRegex = @"^(?("")("".+?(?<!\\)""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
                                  @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-\w]*[0-9a-z]*\.)+[a-z0-9][\-a-z0-9]{0,22}[a-z0-9]))$";

            if (Regex.IsMatch(this.accountField.Text, emailRegex, RegexOptions.IgnoreCase))
            {
                this.account.Username = this.accountField.Text;
                this.account.Sender = this.account.Username;
                this.account.SSL = true;
                this.account.Port = 587;
                this.account.Password = "";

                switch (this.serviceBox.SelectedIndex)
                {
                    case ((int)MailService.Gmail):
                        this.account.SmtpServer = "smtp.gmail.com";
                        break;

                    case ((int)MailService.Yahoo):
                        this.account.SmtpServer = "smtp.mail.yahoo.com";
                        break;

                    case ((int)MailService.Hotmail):
                        this.account.SmtpServer = "smtp.live.com";
                        break;
                }

                this.Close();
            }
        }
    }
}
