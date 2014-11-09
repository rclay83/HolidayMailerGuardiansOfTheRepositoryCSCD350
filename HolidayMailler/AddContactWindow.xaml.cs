using System.Windows;
using ContactData;
using System.Text.RegularExpressions;
using System.Windows.Input;

/*  Holiday Mailer
 *  Guardians of the Repository
 * 
 *  Author: Marcus Sanchez
 *  Last revision:  11/8/2014
 *  
 *  AddContactWindow is responsible for prompting the user for new contact information.
 *  
 */

namespace HolidayMailler
{
    public partial class AddContactWindow : Window
    {
        private Contact contact;

        public AddContactWindow (Contact newContact)
        {
            InitializeComponent();
            this.contact = newContact;
        }

        private void addButton_Click (object sender, RoutedEventArgs e)
        {
            if (Regex.IsMatch(this.firstNameField.Text, @"^[a-zA-Z]*$") && Regex.IsMatch(this.lastNameField.Text, @"^[a-zA-Z]*$"))
            {
                if (Regex.IsMatch(this.emailField.Text, @"^\w+@[a-zA-Z_]+?\.[a-zA-Z]{2,3}$"))
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
