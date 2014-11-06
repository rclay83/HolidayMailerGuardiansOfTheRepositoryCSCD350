using System.Windows;
using ContactData;
using System.Text.RegularExpressions;

/*  Holiday Mailer
 *  Guardians of the Repository
 * 
 *  Author: Marcus Sanchez
 *  Last revision:  11/4/2014
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

        // to do: regex for email, first name, last name
        private void addButton_Click (object sender, RoutedEventArgs e)
        {
            // just a temporary check, will add regex
            if (this.firstNameField.Text.Length > 1 && this.lastNameField.Text.Length > 1)
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
                this.errorMessageLabel.Content = "Please fill out all fields";
            }
        }
    }
}
