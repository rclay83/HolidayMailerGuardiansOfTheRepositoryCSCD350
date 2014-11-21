using Email;
using System.Text.RegularExpressions;
using System.Windows;

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

        private void addButton_Click (object sender, RoutedEventArgs e)
        {
            if (Regex.IsMatch(this.accountField.Text, @"^\w+@[a-zA-Z_]+?\.[a-zA-Z]{2,3}$") && this.passwordField.Password.Length > 0)
            {
                this.account.Username = this.accountField.Text;
                this.account.Sender = this.account.Username;
                this.account.Password = this.passwordField.Password;
                this.account.SSL = true;

                switch (this.serviceBox.SelectedIndex)
                {
                    case ((int)MailService.Gmail):
                        this.account.SmtpServer = "smtp.gmail.com";
                        this.account.Port = 587;
                        break;

                    case ((int)MailService.Yahoo):
                        this.account.SmtpServer = "smtp.mail.yahoo.com";
                        this.account.Port = 465;
                        break;

                    case ((int)MailService.Hotmail):
                        this.account.SmtpServer = "smtp.live.com";
                        this.account.Port = 587;
                        break;
                }

                this.Close();
            }
        }
    }
}
