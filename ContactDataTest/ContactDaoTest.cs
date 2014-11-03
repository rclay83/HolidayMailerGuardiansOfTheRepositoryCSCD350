using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ContactData;
using System.Data.SQLite;
using System.Collections.Generic;
using System.Data;


//todo: write test where lastname is null
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
            IContact mock = new MockContact();
            using (IDbCommand cmd = conn.CreateCommand())
            {
                string ctext = "DELETE FROM AllContacts WHERE first_name = '" + mock.FirstName + "';";
                cmd.CommandText = ctext;
                try
                {
                    IContactDao cdao = new ContactDao();
                    cdao.Connection = this.conn;
                    cdao.verifyTable();
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
            IContactDao cdao = new ContactDao(conn);
            IContact toAdd = new MockContact();
            Assert.AreEqual(true, cdao.addContact(toAdd));
           
        }

        [TestMethod]
        public void TestAddContactWithoutLastName()
        {
            IContactDao cdao = new ContactDao();
            cdao.Connection = this.conn;
            IContact mock = new MockContact();
            IContact toAdd = new Contact() { FirstName = mock.FirstName, Email = mock.Email };

            Assert.AreEqual(true, cdao.addContact(toAdd));

        }

        [TestMethod]
        public void TestGetAllContacts()
        {
            IContactDao cdao = new ContactDao(conn);
            IContact mock = new MockContact();
            cdao.addContact(mock);
            IDictionary<string, IContact> map = cdao.getAllContacts();
            Assert.AreEqual(mock.FirstName, map[mock.Email].FirstName);
        }
    }
}
       

       


