using System.Linq;
using System.Windows;
using ContactData;
using System.Collections.Generic;
using System.Data.SQLite;
using Email;
using System;
using Microsoft.Win32;
using System.Windows.Input;

/*  Holiday Mailer
 *  Guardians of the Repository
 * 
 *  Author: Marcus Sanchez
 *  Last revision:  11/8/2014
 *  
 *  MainWindow class is the main GUI for the mail client.
 *  User interation with contact database and mail sending occurs here.
 *  
 *  To do:
 *      Add mouse over messges
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
            else
            {
                UpdateRecipientField();
            }
        }

        private void exitMenu_Click (object sender, RoutedEventArgs e)
        {
            this.Close();
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
        // To Do:   Send the file to mail object, currently does nothing with file
        private void attatchButton_Click (object sender, RoutedEventArgs e)
        {
            OpenFileDialog openWindow = new OpenFileDialog();
            var result = openWindow.ShowDialog();

            if (result == true)
            {
                this.attatchmentLabel.Content = "File attatched";
            }
            else
            {
                this.attatchmentLabel.Content = "No attatchments";
            }
        }

        private void searchCriteriaField_KeyDown (object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {

            }
        }

        private void viewAllMenu_Click (object sender, RoutedEventArgs e)
        {
            this.contactsTable.ItemsSource = this.contactList;
        }

        private void Window_KeyDown (object sender, KeyEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.LeftCtrl) && Keyboard.IsKeyDown(Key.N))
            {
                addContactMenu_Click(sender, e);
            }

            else if (Keyboard.IsKeyDown(Key.LeftCtrl) && Keyboard.IsKeyDown(Key.A))
            {
                viewAllMenu_Click(sender, e);
            }

            else if (Keyboard.IsKeyDown(Key.LeftAlt) && Keyboard.IsKeyDown(Key.A))
            {
                aboutMenu_Click(sender, e);
            }
        }

        private void Window_Closing (object sender, System.ComponentModel.CancelEventArgs e)
        {
            var decision = MessageBox.Show("Are you sure you want to exit?", "Confirm", MessageBoxButton.YesNo);

            e.Cancel = (MessageBoxResult.No == decision);
        }
    }
}
