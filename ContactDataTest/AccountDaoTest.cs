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
            Assert.IsTrue(this.Contains(accounts, this.account));
        }

        [TestMethod]
        public void AddMultipleAccounts()
        {
            I_Account testAccount = new CDAccount()
            {
                Username = this.account.Username,
                Password = this.account.Password,
                Port = this.account.Port,
                Sender = this.account.Sender,
                SmtpServer = this.account.SmtpServer,
                SSL = this.account.SSL
            };

            this.accountDao.addAccount(this.account);
            testAccount.Username = "Modified";
            this.accountDao.addAccount(testAccount);
            testAccount.Username = "Modified2";
            this.accountDao.addAccount(testAccount);
            Assert.IsTrue(this.accountDao.getAccounts().Count == 3);
        }

        [TestMethod]
        public void TestGetAllAccountsEmptyTable()
        {
            IList<I_Account> returnedAccounts = this.accountDao.getAccounts();
            Assert.IsTrue(returnedAccounts.Count == 0);
        }

        [TestMethod]
        public void TestRemoveAccount()
        {
            this.accountDao.addAccount(this.account);
            this.accountDao.removeAccount(this.account);
            IList<I_Account> allAccounts = this.accountDao.getAccounts();
            Assert.IsFalse(this.Contains(allAccounts, this.account));
        }

        [TestMethod]
        public void TestRemoveNonExistingAccount()
        {
            this.accountDao.removeAccount(this.account);
            IList<I_Account> allAccounts = this.accountDao.getAccounts();
            Assert.IsFalse(this.Contains(allAccounts, this.account));
        }
        
        private bool Contains(IList<I_Account> list, I_Account target)
        {
            foreach(I_Account act in list)
            {
                if(act.Username.Equals(target.Username))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
