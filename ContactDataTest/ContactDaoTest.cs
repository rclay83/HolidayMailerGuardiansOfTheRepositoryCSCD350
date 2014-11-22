using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ContactData;
using System.Data.SQLite;
using System.Collections.Generic;
using System.Data;



namespace ContactDataTest
{
    [TestClass]
    public class ContactDaoTest
    {

        SQLiteConnection conn;
        IContact contact;
        IContactDao contactDao;

        public ContactDaoTest()
        {
            this.conn = new SQLiteConnection("Data Source=Contacts.db;Version=3;New=True;Compress=True;");

        }


        [TestInitialize]
        public void setupDb()
        {
            // test contact for insertions, deletions, etc...
            this.contact = new Contact()
            {
                FirstName = "Peter",
                LastName = "Parker",
                Email = "guardiansoftherepository@gmail.com",
                GotMail = true
            };

            this.contactDao = new ContactDao();

            // clean up database before any tests    
            using (IDbCommand cmd = conn.CreateCommand())
            {
                string ctext = "DELETE FROM AllContacts;";
                cmd.CommandText = ctext;
                try
                {
                    this.contactDao.verifyTable(this.conn);
                    conn.Open();
                    cmd.ExecuteNonQuery();

                }
                finally
                {
                    conn.Close();
                }
            }
        }



        [TestMethod]
        public void TestAddContact()
        {
            IContact toAdd = new MockContact();
            Assert.AreEqual(true, this.contactDao.addContact(toAdd));

        }


        [TestMethod]
        public void TestAddContactWithMail()
        {
            IContact toAdd = new Contact()
            {
                FirstName = "Peter",
                LastName = "Parker",
                Email = "spider@spidermail.com",
                GotMail = true
            };
            this.contactDao.addContact(toAdd);
            IDictionary<string, IContact> allContacts = this.contactDao.getAllContacts();
            IContact contactReturned = allContacts[toAdd.Email];
            Assert.AreEqual(toAdd, contactReturned);

        }
        [TestMethod]
        public void TestAddContactWithoutLastName()
        {
            IContact mock = new MockContact();
            IContact toAdd = new Contact() { FirstName = mock.FirstName, Email = mock.Email };

            Assert.AreEqual(true, this.contactDao.addContact(toAdd));

        }

        [TestMethod]
        public void TestGetAllContacts()
        {
            IContact mock = new MockContact();
            this.contactDao.addContact(mock);
            IDictionary<string, IContact> map = this.contactDao.getAllContacts();
            Assert.AreEqual(mock.FirstName, map[mock.Email].FirstName);
        }
        [TestMethod]
        [ExpectedException(typeof(ContactDataExcpetion))]
        public void TestInsertContactNoEmail()
        {
            IContact toAdd = new Contact() { LastName = "some name last", FirstName = "some first name" };
            this.contactDao.addContact(toAdd);

        }

        [TestMethod]
        [ExpectedException(typeof(ContactDataExcpetion))]
        public void TestInsertContactNoFirstName()
        {
            IContact toAdd = new Contact() { LastName = "some name last", Email = "some email" };
            this.contactDao.addContact(toAdd);

        }

        [TestMethod]
        public void TestContactEqualsFalse()
        {
            Contact c1 = new Contact() { FirstName = "John", LastName = "Doe", Email = "john@testmail.com", GotMail = true };
            Contact c2 = new Contact() { FirstName = "Jane", LastName = "Doe", Email = "john@testmail.com", GotMail = true };
            Assert.AreNotEqual(c1, c2);
        }

        [TestMethod]
        public void TestContactEquals()
        {
            Contact c1 = new Contact() { FirstName = "John", LastName = "Doe", Email = "john@testmail.com", GotMail = true };
            Contact c2 = new Contact() { FirstName = "John", LastName = "Doe", Email = "john@testmail.com", GotMail = true };
            Assert.AreEqual(c1, c2);
        }

        [TestMethod]
        public void TestContactEqualsNull()
        {
            Contact c1 = new Contact() { FirstName = "John", LastName = "Doe", Email = "john@testmail.com", GotMail = true };
            Contact c2 = null;
            Assert.AreNotEqual(c1, c2);
        }

        [TestMethod]
        public void TestRemoveContact()
        {
            this.contactDao.addContact(this.contact);
            this.contactDao.removeContact(this.contact);
            IDictionary<string, IContact> allContacts = this.contactDao.getAllContacts();
            Assert.IsTrue(!allContacts.Values.Contains(this.contact));
        }

        [TestMethod]
        public void TestRemoveNonExistingContact()
        {
            this.contactDao.removeContact(this.contact);
            IDictionary<string, IContact> allContacts = this.contactDao.getAllContacts();
            Assert.IsTrue(!allContacts.Values.Contains(this.contact));
        }

        [TestMethod]
        public void TestUpdateContact()
        {
            this.contactDao.addContact(this.contact);
            this.contact.FirstName = "Modified";
            this.contactDao.updateContact(this.contact);
            IDictionary<string, IContact> map = this.contactDao.getAllContacts();
            Assert.AreEqual(this.contact.FirstName, map[this.contact.Email].FirstName);
        }

        [TestMethod]
        public void TestUpdateNonExistentContact()
        {
            this.contactDao.updateContact(this.contact);
            Assert.IsTrue(contactDao.getAllContacts().Count == 0);
        }
    }
}





