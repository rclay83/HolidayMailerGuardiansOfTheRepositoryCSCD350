using System.Linq;
using System.Windows;
using ContactData;
using System.Collections.Generic;
using System.Data.SQLite;
using System;
using Microsoft.Win32;
using System.Windows.Input;
using Email;
using System.Reflection;

/*  Holiday Mailer
 *  Guardians of the Repository
 * 
 *  Author: Marcus Sanchez
 *  Last revision:  11/20/2014
 *  
 *  MainWindow class is the main GUI for the mail client.
 *  User interation with contact database and mail sending occurs here.
 *  
 *  TO DO:
 *      -Genericize redundant event handlers (pressing enter in field call button press)
 *      -If contact is deleted/selected in contacts/search tab update other
 */

namespace HolidayMailler
{

    public partial class MainWindow : Window
    {
        private IContactDao contactsDB;
        private IAccountDao accountsDB;

        private List<IContact> contactList;
        private List<IContact> selectedContacts;

        private List<I_Account> accounts;
        private I_MailMan message;
        private List<string> attachments;

        public MainWindow ()
        {
            InitializeComponent();
            this.contactsDB = new ContactDao();
            this.accountsDB = new AccountDao();

            this.contactList = this.contactsDB.getAllContacts().Values.ToList();
            this.accounts = this.accountsDB.getAccounts() as List<I_Account>;
            this.selectedContacts = new List<IContact>();
            this.contactsTable.ItemsSource = contactList;

            this.sendAsBox.ItemsSource = this.accounts;
            this.sendAsBox.DisplayMemberPath = "Username";

            this.attachments = new List<string>();
        }

        private void addContactMenu_Click (object sender, RoutedEventArgs e)
        {
            IContact toAdd = new Contact();
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
                catch (ContactDataExcpetion ex)
                {
                    MessageBox.Show("There is already a contact in the databse with that email.");
                }
            }
        }

        private void removeContactMenu_Click (object sender, RoutedEventArgs e)
        {
            var decision = MessageBox.Show("Permanently delete the " + this.selectedContacts.Count + " selected contacts?", "Confirm Removal", MessageBoxButton.YesNo);

            if (decision == MessageBoxResult.Yes)
            {
                foreach (IContact toDelete in this.selectedContacts)
                {
                    this.contactsDB.removeContact(toDelete);
                    this.contactList.Remove(toDelete);
                    this.contactsTable.CommitEdit();
                }

                this.selectedContacts.Clear();
            }
            this.contactsTable.CancelEdit();
            this.contactsTable.Items.Refresh();
        }

        private void aboutMenu_Click (object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Holiday Mailer\n\nTeam: Guardians of the Repository\nVersion: 0.00000001");
        }

        private void OnContactChecked (object sender, RoutedEventArgs e)
        {
            IContact selected = (IContact)this.contactsTable.SelectedItem;

            if (!this.selectedContacts.Contains(selected))
            {
                this.selectedContacts.Add(selected);
                this.selectionCountLabel.Content = this.selectedContacts.Count + " Contacts Selected";
                this.resultsLabel.Content = this.selectedContacts.Count + " Contacts Selected";

                this.composeButton.IsEnabled = true;
                this.removeContactMenu.IsEnabled = true;

                UpdateRecipientField();
            }
        }

        private void OnContactUnchecked (object sender, RoutedEventArgs e)
        {
            this.selectedContacts.Remove((IContact)this.contactsTable.SelectedItem);
            this.selectionCountLabel.Content = this.selectedContacts.Count + " Contacts Selected";
            this.resultsLabel.Content = this.selectedContacts.Count + " Contacts Selected";

            if (this.selectedContacts.Count > 0)
            {
                this.removeContactMenu.IsEnabled = false;
            }

            UpdateRecipientField();
        }

        private void OnResultChecked (object sender, RoutedEventArgs e)
        {
            IContact selectedResult = (IContact)this.resultsTable.SelectedItem;

            if (!this.selectedContacts.Contains(selectedResult))
            {
                this.selectedContacts.Add(selectedResult);

                this.selectionCountLabel.Content = this.selectedContacts.Count + " Contacts Selected";
                this.selectionCountLabel.Content = this.selectedContacts.Count + " Contacts Selected";
                this.resultsLabel.Content = this.selectedContacts.Count + " Contacts Selected";
            }
        }

        private void OnResultUnchecked (object sender, RoutedEventArgs e)
        {
            this.selectedContacts.Remove((IContact)this.resultsTable.SelectedItem);
            this.selectionCountLabel.Content = this.selectedContacts.Count + " Contacts Selected";
            this.resultsLabel.Content = this.selectedContacts.Count + " Contacts Selected";

            if (this.selectedContacts.Count > 0)
            {
                this.removeContactMenu.IsEnabled = false;
            }

            UpdateRecipientField();
        }

        private void exitMenu_Click (object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void UpdateRecipientField ()
        {
            this.sendToField.Text = "";

            if (this.selectedContacts.Count > 0)
            {
                foreach (IContact contact in this.selectedContacts)
                {
                    this.sendToField.Text += contact.Email + ", ";
                }

                this.sendToField.Text = this.sendToField.Text.Substring(0, this.sendToField.Text.Length - 2);
            }
        }

        private void composeButton_Click (object sender, RoutedEventArgs e)
        {
            this.tabs.SelectedIndex = 2;
        }

        private void sendButton_Click (object sender, RoutedEventArgs e)
        {
            if (this.subjectField.Text.Length > 0 && this.bodyField.Text.Length > 0)
            {
                string[] recipients = new string[this.selectedContacts.Count];
                int contactIndex = 0;

                foreach (IContact contact in this.selectedContacts)
                {
                    recipients[contactIndex++] = contact.Email;
                }

                try
                {
                    I_Account sendingAccount = this.sendAsBox.SelectedItem as I_Account;

                    if (sendingAccount == null)
                    {
                        this.errorLabel.Content = "You must add a sending account before sending mail.";
                    }
                    else {
                    if (sendingAccount.Password == null)
                    {
                        if (this.senderPasswordBox.Password.Length == 0)
                        {
                            this.errorLabel.Content = "You must provide the password for sending account";
                            return;
                        }
                        else
                        {
                            sendingAccount.Password = this.senderPasswordBox.Password;
                        }
                    }

                    this.message = new MockMailMan(sendingAccount, recipients, this.subjectField.Text, this.bodyField.Text);

                    if (this.attachments.Count > 0)
                    {
                        this.message.setAttachment(this.attachments.ToArray());
                    }

                    this.message.sendMail();
                    MessageBox.Show("Message sent to " + this.selectedContacts.Count + " contacts.", "Success");

                    CleanMail();
                    this.tabs.SelectedIndex = 0;
                    }

                }
                catch (System.Net.Mail.SmtpException ex)
                {
                    MessageBox.Show("Authentication error. Check account credentials. " + ex.Message);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("An error occured while sending the mail");
                }
            }
            else
            {
                if (this.selectedContacts.Count == 0)
                {
                    this.errorLabel.Content = "Contacts must be selected before sending";
                }
                else
                {
                    this.errorLabel.Content = "A subject and body are required";
                }
            }
        }

        private void CleanMail ()
        {
            this.attachments.Clear();
            this.attachmentsListBox.ItemsSource = this.attachments;
            this.subjectField.Text = "";
            this.bodyField.Text = "";
            this.errorLabel.Content = "";
            this.attachmentLabel.Content = "No attachments";
        }

        private void attatchButton_Click (object sender, RoutedEventArgs e)
        {
            OpenFileDialog openWindow = new OpenFileDialog();
            var result = openWindow.ShowDialog();

            if (result == true)
            {
                this.attachmentLabel.Content = "File attatched";
                this.attachments.Add(openWindow.FileName);
                this.attachmentsListBox.ItemsSource = this.attachments;
                this.attachmentsListBox.Items.Refresh();
                this.attachmentLabel.Content = this.attachments.Count + " attchments";
            }
        }

        private void viewAllMenu_Click (object sender, RoutedEventArgs e)
        {
            this.tabs.SelectedIndex = 0;
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

            else if (Keyboard.IsKeyDown(Key.LeftCtrl) && Keyboard.IsKeyDown(Key.E))
            {
                newAccountMenu_Click(sender, e);
            }
        }

        private void Window_Closing (object sender, System.ComponentModel.CancelEventArgs e)
        {
            var decision = MessageBox.Show("Are you sure you want to exit?", "Confirm", MessageBoxButton.YesNo);

            e.Cancel = (MessageBoxResult.No == decision);
        }

        delegate bool SearchConstraint (string data, string searchText);

        private void searchButton_Click (object sender, RoutedEventArgs e)
        {
            if (this.searchText.Text.Length == 0)
            {
                this.resultsTable.ItemsSource = this.contactList;
            }
            else
            {
                string searchText = this.searchText.Text.ToLower();
                SearchConstraint search;

                if (this.constraintBox.SelectedIndex == 0)
                    search = ((data, text) => data.Contains(text));
                else
                    search = ((data, text) => data.StartsWith(text));


                string propertyText = this.fieldBox.SelectionBoxItem.ToString().Replace(" ", "");

                var contactProperties = typeof(Contact).GetProperties().Where(prop => prop.Name == propertyText && prop.PropertyType == typeof(string) && prop.CanRead);

                var results = this.contactList.Where(contact =>
                {
                    foreach (PropertyInfo prop in contactProperties)
                    {
                        string data = (string)prop.GetValue(contact);

                        if (data != null && search(data.ToLower(), searchText))
                        {
                            return true;
                        }
                    }
                    return false;
                });

                List<IContact> contactResults = results.ToList();
                this.resultsTable.ItemsSource = contactResults;
            }
        }

        private void newAccountMenu_Click (object sender, RoutedEventArgs e)
        {
            I_Account acc = new MockAccount();
            acc.Username = "";

            NewAccountWindow addAccountWindow = new NewAccountWindow(acc);
            addAccountWindow.ShowDialog();

            if (acc.Username != "")
            {
                try
                {
                    this.accountsDB.addAccount(acc);
                    this.accounts.Add(acc);
                }
                catch (ContactDataExcpetion ex)
                {
                    MessageBox.Show(ex.Message);
                }

                this.sendAsBox.Items.Refresh();
            }
        }

        private void searchText_KeyDown (object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                searchButton_Click(sender, e);
            }
        }

        private void sendAsBox_SelectionChanged (object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            I_Account sendingAccount = this.sendAsBox.SelectedItem as I_Account;

            if (sendingAccount.Password == null)
            {
                this.senderPasswordBox.Clear();
                this.senderPasswordBox.IsEnabled = true;
                this.passwordLabel.IsEnabled = true;
            }
            else
            {
                this.senderPasswordBox.IsEnabled = false;
                this.passwordLabel.IsEnabled = false;
            }
        }

    }
}
