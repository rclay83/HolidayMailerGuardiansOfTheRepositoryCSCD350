using System.Windows;
using ContactData;
using System.Text.RegularExpressions;
using System.Windows.Input;

/*  Holiday Mailer
 *  Guardians of the Repository
 * 
 *  Author: Marcus Sanchez
 *  Last revision:  12/2/2014
 *  
 *  AddContactWindow is responsible for prompting the user for new contact information.
 *  
 */

namespace HolidayMailler
{
    public partial class AddContactWindow : Window
    {
        private IContact contact;

        public AddContactWindow (IContact newContact)
        {
            InitializeComponent();
            this.firstNameField.Focus();
            this.contact = newContact;
        }

        /*  Regex taken from MSDN Site: http://msdn.microsoft.com/en-us/library/01escwtf%28v=vs.110%29.aspx
         */

        private void addButton_Click (object sender, RoutedEventArgs e)
        {
            if (Regex.IsMatch(this.firstNameField.Text, @"^[a-zA-Z]*$") && Regex.IsMatch(this.lastNameField.Text, @"^[a-zA-Z]*$"))
            {
                string emailRegex = @"^(?("")("".+?(?<!\\)""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
                                  @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-\w]*[0-9a-z]*\.)+[a-z0-9][\-a-z0-9]{0,22}[a-z0-9]))$";

                if (Regex.IsMatch(this.emailField.Text, emailRegex, RegexOptions.IgnoreCase))
                {
                    this.contact.FirstName = this.firstNameField.Text;
                    this.contact.LastName = this.lastNameField.Text;
                    this.contact.Email = this.emailField.Text;
                    this.contact.GotMail = (bool)this.mailCheckBox.IsChecked;

                    this.Close();
                }
                else
                {
                    this.errorMessageLabel.Content = "Invalid email";
                }
            }
            else
            {
                this.errorMessageLabel.Content = "Invalid first name or last name";
            }
        }

        private void fieldKeyDown (object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                addButton_Click(sender, e);
            }
        }
    }
}
