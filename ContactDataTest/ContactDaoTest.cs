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

        public ContactDaoTest()
        {
            this.conn = new SQLiteConnection("Data Source=Contacts.db;Version=3;New=True;Compress=True;");

        }


        [TestInitialize]
        public void setupDb()
        {
            
            using (IDbCommand cmd = conn.CreateCommand())
            {
                string ctext = "DELETE FROM AllContacts;";
                cmd.CommandText = ctext;
                try
                {
                    IContactDao cdao = new ContactDao();
                    cdao.verifyTable(this.conn);
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
            IContactDao cdao = new ContactDao();
            IContact toAdd = new MockContact();
            Assert.AreEqual(true, cdao.addContact(toAdd));
           
        }

            
        [TestMethod]
        public void TestAddContactWithMail()
        {
            IContactDao cdao = new ContactDao();
            IContact toAdd = new Contact() 
            { 
                FirstName = "Peter", 
                LastName = "Parker", 
                Email = "spider@spidermail.com", 
                GotMail = true };
            cdao.addContact(toAdd);
            IDictionary<string, IContact> allContacts = cdao.getAllContacts();
            IContact contactReturned = allContacts[toAdd.Email];
            Assert.AreEqual(toAdd, contactReturned);

        }
        [TestMethod]
        public void TestAddContactWithoutLastName()
        {
            IContactDao cdao = new ContactDao();
   
            IContact mock = new MockContact();
            IContact toAdd = new Contact() { FirstName = mock.FirstName, Email = mock.Email };

            Assert.AreEqual(true, cdao.addContact(toAdd));

        }

        [TestMethod]
        public void TestGetAllContacts()
        {
            IContactDao cdao = new ContactDao();
            IContact mock = new MockContact();
            cdao.addContact(mock);
            IDictionary<string, IContact> map = cdao.getAllContacts();
            Assert.AreEqual(mock.FirstName, map[mock.Email].FirstName);
        }
        [TestMethod]
       [ExpectedException(typeof(ContactDataExcpetion))]
        public void TestInsertContactNoEmail()
        {
            IContactDao cdao = new ContactDao();
            IContact toAdd = new Contact() { LastName = "some name last", FirstName = "some first name" };
            cdao.addContact(toAdd);

        }

        [TestMethod]
        [ExpectedException(typeof(ContactDataExcpetion))]
        public void TestInsertContactNoFirstName()
        {
            IContactDao cdao = new ContactDao();
            IContact toAdd = new Contact() { LastName = "some name last", Email = "some email" };
            cdao.addContact(toAdd);

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
    }
}
       

       


