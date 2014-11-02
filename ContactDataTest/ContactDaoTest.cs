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
        SQLiteConnection conn = new SQLiteConnection("Data Source=Contacts.db;Version=3;New=True;Compress=True;");

        [TestMethod]
        public void TestGetAllContacts()
        {
            IContactDao cdao = new ContactDao();
            IContact expected = new MockContact();
            cdao.Connection = conn;

            string sql = "INSERT INTO AllContacts(first_name, last_name, email, got_mail) ";
                sql += "VALUES ('" + expected.FirstName + "', '" + expected.LastName + "',";
            sql+= "'"+expected.Email+"','";
            sql+= expected.GotMail +"');";
            
            try
            {
                IDbCommand command = conn.CreateCommand();
                command.CommandText = sql;
                conn.Open();
                command.ExecuteNonQuery();
               
            }
            finally
            {
                conn.Close();
            }

            IDictionary<String, IContact> map = cdao.getAllContacts();

            Assert.AreEqual(expected.FirstName, map[expected.Email].FirstName);
        }


        [TestMethod]
        public void TestInsertContact()
        {
            IContactDao cdao = new ContactDao(conn);
            IContact toAdd = new MockContact();
            cdao.addContact(toAdd);

            using(IDbCommand cmd = conn.CreateCommand())
            {
                try
                {
                    cmd.CommandText = "SELECT * FROM AllContacts WHERE email = '";
                    cmd.CommandText+= toAdd.Email + "';";
                    conn.Open();
                    IDataReader dr = cmd.ExecuteReader();
                    dr.Read();
                    Assert.AreEqual(toAdd.FirstName, (string)dr["first_name"]);
                }
                finally
                {
                    conn.Close();
                }
            }
        }

        [TestInitialize]
        public void setupDb()
        {
            IContact mock = new MockContact();
            using (IDbCommand cmd = conn.CreateCommand())
            {
                string ctext = "DELETE FROM AllContacts WHERE first_name = '"+mock.FirstName+"';";
                cmd.CommandText = ctext;
                try
                {
                    conn.Open();
                    cmd.ExecuteNonQuery();

                }
                finally
                {
                    conn.Close();
                }
            }
        }

    }
}
