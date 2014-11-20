﻿using System.Linq;
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
 *  Last revision:  11/19/2014
 *  
 *  MainWindow class is the main GUI for the mail client.
 *  User interation with contact database and mail sending occurs here.
 */

namespace HolidayMailler
{

    public partial class MainWindow : Window
    {
        private ContactDao contactsDB;
        private List<IContact> contactList;
        private List<IContact> selectedContacts;

        private Account sender;
        private MailMan message;
        private List<string> attachments;

        public MainWindow ()
        {
            InitializeComponent();
            contactsDB = new ContactDao();
            this.contactList = this.contactsDB.getAllContacts().Values.ToList();
            this.selectedContacts = new List<IContact>();
            this.contactsTable.ItemsSource = contactList;
            this.sender = new Account();

            this.attachments = new List<string>();
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
                foreach (Contact contact in this.selectedContacts)
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

                foreach (Contact contact in this.selectedContacts)
                {
                    recipients[contactIndex++] = contact.Email;
                }

                try
                {
                    this.message = new MailMan(this.sender, recipients, this.subjectField.Text, this.bodyField.Text);

                    if (this.attachments.Count > 0)
                    {
                        this.message.setAttachment(this.attachments.ToArray());
                    }

                    this.message.sendMail();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("An error occured while attempting to send the message.");
                }

                MessageBox.Show("Message sent to " + this.selectedContacts.Count + " contacts.", "Success");

                CleanMail();
                this.tabs.SelectedIndex = 0;
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
        }

        private void Window_Closing (object sender, System.ComponentModel.CancelEventArgs e)
        {
            var decision = MessageBox.Show("Are you sure you want to exit?", "Confirm", MessageBoxButton.YesNo);

            e.Cancel = (MessageBoxResult.No == decision);
        }
    }
}
