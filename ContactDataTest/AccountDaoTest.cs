using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ContactData;
using System.Data.SQLite;
using System.Data;
using Email;
using System.Collections.Generic;

namespace ContactDataTest
{
    [TestClass]
    public class AccountDaoTest
    {
        SQLiteConnection conn;
        I_Account account;
        IAccountDao accountDao;

        public AccountDaoTest()
        {
            this.conn = new SQLiteConnection("Data Source=Contacts.db;Version=3;New=True;Compress=True;");

        }


        [TestInitialize]
        public void setupDb()
        {
            // test contact for insertions, deletions, etc...
            this.account = new TestAccount();
            this.accountDao = new AccountDao();
            // clean up database before any tests    
            using (IDbCommand cmd = conn.CreateCommand())
            {
                string ctext = "DELETE FROM Accounts;";
                cmd.CommandText = ctext;
                try
                {
                    this.accountDao.verifyTable(this.conn);
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
        public void GetAllAccountsTest()
        {
            this.accountDao.addAccount(this.account);
            IList<I_Account> accounts = this.accountDao.getAccounts();
            Assert.IsTrue(accounts.Contains(this.account));
        }

        [TestMethod]
        public void AddAccountTest()
        {

        }
    }
}
