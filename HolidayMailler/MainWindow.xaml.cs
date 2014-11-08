using System.Linq;
using System.Windows;
using ContactData;
using System.Collections.Generic;
using System.Data.SQLite;
using Email;
using System;

/*  Holiday Mailer
 *  Guardians of the Repository
 * 
 *  Author: Marcus Sanchez
 *  Last revision:  11/7/2014
 *  
 *  MainWindow class is the main GUI for the mail client.
 *  User interation with contact database and mail sending occurs here.
 *  
 *  To do:
 *      Add mouse over messges
 *      Create Regex for new contact validation
 *      "How to use" option in Help
 *      Check/uncheck all contacts
 */

namespace HolidayMailler
{

    public partial class MainWindow : Window
    {
        private ContactDao contactsDB;
        private List<IContact> contactList;
        private List<IContact> selectedContacts;

        public MainWindow ()
        {
            InitializeComponent();
            contactsDB = new ContactDao();
            this.contactList = this.contactsDB.getAllContacts().Values.ToList();
            this.selectedContacts = new List<IContact>();
            this.contactsTable.ItemsSource = contactList;
        }

        private void addContactMenu_Click (object sender, RoutedEventArgs e)
        {
            Contact toAdd = new Contact();
            AddContactWindow contactWindow = new AddContactWindow(toAdd);
            contactWindow.ShowDialog();

            if (toAdd.FirstName != null)
            {
                try
                {
                    this.contactsDB.addContact(toAdd);
                    this.contactList.Add(toAdd);
                    this.contactsTable.Items.Refresh();
                }
                catch (SQLiteException ex)
                {
                    MessageBox.Show("There is already a contact in the databse with that email.");
                }
            }
        }

        private void aboutMenu_Click (object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Holiday Mailer\n\nTeam: Guardians of the Repository\nVersion: 0.00000001");
        }

        private void OnContactChecked (object sender, RoutedEventArgs e)
        {
            this.selectedContacts.Add((Contact)this.contactsTable.SelectedItem);
            this.selectionCountLabel.Content = this.selectedContacts.Count + " contacts selected";
            this.composeButton.IsEnabled = true;

            UpdateRecipientField();
        }

        private void OnContactUnchecked (object sender, RoutedEventArgs e)
        {
            this.selectedContacts.Remove((Contact)this.contactsTable.SelectedItem);
            this.selectionCountLabel.Content = this.selectedContacts.Count + " contacts selected";

            if (this.selectedContacts.Count == 0)
            {
                this.composeButton.IsEnabled = false;
                this.mailTab.IsEnabled = false;
            }

            UpdateRecipientField();
        }

        private void exitMenu_Click (object sender, RoutedEventArgs e)
        {
            var decision = MessageBox.Show("Are you sure you want to exit?", "Confirm", MessageBoxButton.YesNo);

            if (decision == MessageBoxResult.Yes)
            {
                this.Close();
            }
        }

        private void UpdateRecipientField ()
        {
            this.sendToField.Text = "";

            foreach (Contact contact in this.selectedContacts)
            {
                this.sendToField.Text += contact.Email + ", ";
            }
            this.sendToField.Text = this.sendToField.Text.Substring(0, this.sendToField.Text.Length - 2);
        }

        private void composeButton_Click (object sender, RoutedEventArgs e)
        {
            this.subjectField.Text = "";
            this.bodyField.Text = "";

            this.tabs.SelectedIndex = 1;
            this.mailTab.IsEnabled = true;
        }

        private void sendButton_Click (object sender, RoutedEventArgs e)
        {
            if (this.subjectField.Text.Length > 0 && this.bodyField.Text.Length > 0)
            {
                string[] recipients = new string[this.selectedContacts.Count];
                int contactIndex = 0;

                foreach (Contact contact in this.selectedContacts)
                {
                    recipients[contactIndex++] = contact.Email;
                }

                try
                {
                    MailMan mailman = new MailMan(recipients, this.subjectField.Text, this.bodyField.Text);
                    mailman.sendMail();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("An error occured while attempting to send the message.");
                }

                MessageBox.Show("Message sent to " + this.selectedContacts.Count + " contacts.", "Success");
                this.tabs.SelectedIndex = 0;
                this.mailTab.IsEnabled = false;
            }
            else
            {
                this.errorLabel.Content = "A subject and body are required";
            }
        }
    }
}
