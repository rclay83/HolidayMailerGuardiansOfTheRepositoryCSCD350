using System.Linq;
using System.Windows;
using ContactData;
using System.Collections.Generic;
using System;
using Microsoft.Win32;
using System.Windows.Input;
using Email;
using System.Reflection;
using System.Collections.ObjectModel;
using System.Windows.Controls;

/*  Holiday Mailer
 *  Guardians of the Repository
 * 
 *  Author: Marcus Sanchez
 *  Last revision:  12/2/2014
 *  
 *  MainWindow class is the main GUI for the mail client.
 *  User interation with contact database and mail sending occurs here.
 *  
 *  TO DO:
 *      -If contact is deleted/selected in contacts/search tab update other
 */

namespace HolidayMailler
{
    public partial class MainWindow : Window
    {
        private IContactDao contactsDB;
        private IAccountDao accountsDB;

        private List<IContact> contactList;
        private ObservableCollection<IContact> selectedContacts;
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
            this.contactsTable.ItemsSource = contactList;

            this.selectedContacts = new ObservableCollection<IContact>();
            this.selectedContacts.CollectionChanged += SelectedContactsUpdated;

            this.sendAsBox.ItemsSource = this.accounts;
            this.sendAsBox.DisplayMemberPath = "Username";

            this.attachments = new List<string>();
        }

        private void SelectedContactsUpdated (object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            this.selectionCountLabel.Content = this.selectedContacts.Count + " Contacts Selected";
            this.selectionCountLabel.Content = this.selectedContacts.Count + " Contacts Selected";
            this.resultsLabel.Content = this.selectedContacts.Count + " Contacts Selected";

            if (this.selectedContacts.Count > 0)
            {
                this.removeContactMenu.IsEnabled = true;
                this.removeButton.IsEnabled = true;
                this.searchRemoveButton.IsEnabled = true;
            }
            else
            {
                this.removeContactMenu.IsEnabled = false;
                this.removeButton.IsEnabled = false;
                this.searchRemoveButton.IsEnabled = false;
            }

            UpdateRecipientField();
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

                this.contactsTable.CommitEdit();
                this.contactsTable.Items.Refresh();
            }
            
            if (this.selectedContacts.Count == 0)
            {
                this.removeContactMenu.IsEnabled = false;
                this.removeButton.IsEnabled = false;
            }
        }

        private void aboutMenu_Click (object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Holiday Mailer\n\nTeam: Guardians of the Repository\nVersion: 0.5");
        }

        private void OnContactChecked (object sender, RoutedEventArgs e)
        {
            IContact selected = this.contactsTable.SelectedItem as IContact;

            if (!this.selectedContacts.Contains(selected))
            {
                this.selectedContacts.Add(selected);
            }
        }

        private void OnContactUnchecked (object sender, RoutedEventArgs e)
        {
            var table = sender as DataGrid;
            this.selectedContacts.Remove(this.resultsTable.SelectedItem as IContact);
        }

        private void OnResultChecked (object sender, RoutedEventArgs e)
        {
            IContact selected = this.resultsTable.SelectedItem as IContact;

            if (!this.selectedContacts.Contains(selected))
            {
                this.selectedContacts.Add(selected);
            }
        }

        private void OnResultUnchecked (object sender, RoutedEventArgs e)
        {
            var table = sender as DataGrid;
            this.selectedContacts.Remove(this.contactsTable.SelectedItem as IContact);
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
            if (this.selectedContacts.Count == 0)
            {
                this.errorLabel.Content = "You must select contacts before sending mail.";
            }
            else if (this.subjectField.Text.Length == 0 || this.bodyField.Text.Length == 0)
            {
                this.errorLabel.Content = "A subject and body are required";
            }
            else
            {
                string[] recipients = new string[this.selectedContacts.Count];
                int contactIndex = 0;

                foreach (IContact contact in this.selectedContacts)
                {
                    recipients[contactIndex++] = contact.Email;
                }

                I_Account sendingAccount = this.sendAsBox.SelectedItem as I_Account;

                try
                {
                    if (sendingAccount == null)
                    {
                        this.errorLabel.Content = "You must add a sending account before sending mail.";
                    }
                    else
                    {
                        if (sendingAccount.Password == "")
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

                        this.sendAsBox.SelectedItem = null;
                        this.sendAsBox.SelectedIndex = -1;
                    }
                }
                catch (System.Net.Mail.SmtpException ex)
                {
                    sendingAccount.Password = "";
                    MessageBox.Show("Authentication error. Check account credentials.\n" + ex.Message);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("An error occured while sending the mail");
                }
            }
        }

        private void CleanMail ()
        {
            this.attachments.Clear();
            this.attachmentsListBox.ItemsSource = this.attachments;
            this.subjectField.Text = "Happy Holidays!";
            this.bodyField.Text = "Happy holidays from Guardians of the Repository!";
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

            else if (this.removeContactMenu.IsEnabled && (Keyboard.IsKeyDown(Key.LeftCtrl) && Keyboard.IsKeyDown(Key.R)))
            {
                removeContactMenu_Click(sender, e);
            }

            else if (Keyboard.IsKeyDown(Key.LeftAlt) && Keyboard.IsKeyDown(Key.H))
            {
                useMenu_Click(sender, e);
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
                    if (CheckUniqueAccount(acc))
                    {
                        this.accounts.Add(acc);
                        this.accountsDB.addAccount(acc);
                    }
                    else
                    {
                        MessageBox.Show("That email address is already in the accounts list.");
                    }

                }
                catch (ContactDataExcpetion ex)
                {
                    MessageBox.Show(ex.Message);
                }

                this.sendAsBox.Items.Refresh();
            }
        }

        private bool CheckUniqueAccount (I_Account toAdd)
        {
            foreach (I_Account account in this.accounts)
            {
                if (account.Username == toAdd.Username)
                {
                    return false;
                }
            }
            return true;
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

            if (sendingAccount != null && !string.IsNullOrEmpty(sendingAccount.Password))
            {
                this.senderPasswordBox.IsEnabled = false;
                this.passwordLabel.IsEnabled = false;
                this.senderPasswordBox.Clear();
            }
            else
            {
                this.senderPasswordBox.Clear();
                this.senderPasswordBox.IsEnabled = true;
                this.passwordLabel.IsEnabled = true;
            }
        }

        private void useMenu_Click(object sender, RoutedEventArgs e)
        {
            string useage =
                "Email Accounts:\n\n" +
                "Accounts provided by Gmail, Yahoo, and Hotmail are currently supported.  " +
                "The password for a sending account must be provided once during each run of the program, as passwords are not saved permanently.  " +
                
                "\n\nContacts:\n\n" +
                "Contacts can be added via the Add Contact menu.  To remove one or more contacts, ensure all contacts to be removed have a check in the selected column.  " +
                "When all contacts to be removed have been selected, press the Remove Selected button to permanently delete them from the database.  " +

                "\n\nSending Mail:\n\n" +
                "To send an email, at least one contact and one sending email account must be added.  " +
                "Adding contacts and email accounts can be done via their respecitve menus.  " +
                "Click the Select column next to a given contact to send mail to that contact.  " +
                "Once all the desired contacts are selected, click the Compose Mail button to transition to the email creation page.  " +
                "";

            MessageBox.Show(useage);
        }
    }
}
