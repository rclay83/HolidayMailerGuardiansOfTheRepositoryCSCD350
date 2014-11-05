using System.Linq;
using System.Windows;
using ContactData;
using System.Collections.Generic;
using System.Data.SQLite;

/*  Holiday Mailer
 *  Guardians of the Repository
 * 
 *  Author: Marcus Sanchez
 *  Last revision:  11/4/2014
 *  
 *  MainWindow class is the main GUI for the mail client.
 *  User interation with contact database and mail sending occurs here.
 *  
 *  To do:
 *      Add mouse over messges
 *      Create Regex for new contact validation
 *      "How to use" option in Help
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
                this.contactList.Add(toAdd);                

                try
                {
                    this.contactsDB.addContact(toAdd);
                }
                catch (SQLiteException ex)
                {
                    MessageBox.Show("There is already a contact in the databse with that email.");
                }

                this.contactsTable.Items.Refresh();
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
            this.sendButton.IsEnabled = true;
        }

        private void OnContactUnchecked (object sender, RoutedEventArgs e)
        {
            this.selectedContacts.Remove((Contact)this.contactsTable.SelectedItem);
            this.selectionCountLabel.Content = this.selectedContacts.Count + " contacts selected";

            if (this.selectedContacts.Count == 0)
            {
                this.sendButton.IsEnabled = false;
            }
        }

        private void exitMenu_Click (object sender, RoutedEventArgs e)
        {
            var decision = MessageBox.Show("Are you sure you want to exit?", "Confirm", MessageBoxButton.YesNo);

            if (decision == MessageBoxResult.Yes)
            {
                this.Close();
            }
        }
    }
}
