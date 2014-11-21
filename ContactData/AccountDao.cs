using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SQLite;
using System.Data;
using Email;

namespace ContactData
{
    public class AccountDao : IAccountDao
    {
        private IDbConnection connection;


        public AccountDao()
        {
            this.connection = new SQLiteConnection("Data Source=Contacts.db;Version=3;New=True;Compress=True;");
        }

        public void addAccount(Email.I_Account toAdd)
        {
            if (String.IsNullOrEmpty(toAdd.Sender) || String.IsNullOrEmpty(toAdd.SmtpServer))
            {
                throw new ContactDataExcpetion("Missing required table field");
            }
            if (null == toAdd.Port)
            {
                throw new ContactDataExcpetion("Missing required table field");
            }
            using (IDbCommand command = connection.CreateCommand())
            {
                try
                {
                    string commandText = "INSERT INTO AllContacts(first_name,last_name,email,got_mail)" +
                        "VALUES(@first,@last,@email,@got_mail)";

                    command.CommandText = commandText;

                    command.Parameters.Add(new SQLiteParameter("@first") { Value = toAdd.FirstName });
                    command.Parameters.Add(new SQLiteParameter("@last") { Value = toAdd.LastName });
                    command.Parameters.Add(new SQLiteParameter("@email") { Value = toAdd.Email });
                    command.Parameters.Add(new SQLiteParameter("@got_mail") { Value = toAdd.GotMail });

                    verifyTable(this.connection);
                    connection.Open();
                    command.ExecuteNonQuery();
                    return true;

                }
                catch (SQLiteException ex)
                {
                    throw new ContactDataExcpetion("Unexpected SQLException: " + ex.Message);
                }
                finally
                {
                    connection.Close();
                }

            }
        }

        public void removeAccount(Email.I_Account toRemove)
        {
            throw new NotImplementedException();
        }

        public void updateAccount(Email.I_Account toUpdate)
        {
            throw new NotImplementedException();
        }

        public void verifyTable(System.Data.IDbConnection connection)
        {

            if (null != connection)
            {
                using (IDbCommand cmd = connection.CreateCommand())
                {

                    cmd.CommandText = "CREATE TABLE IF NOT EXISTS Accounts(" +
                        "accountName VARCHAR(50) NOT NULL," +
                        "smtpHost VARCHAR(50) PRIMARY KEY," +
                        "portNo int(5) NOT NULL," +
                        "isSSL BOOLEAN DEFAULT 'false' NOT NULL);";
                    try
                    {
                        if (connection.State != ConnectionState.Open)
                        {
                            connection.Open();
                        }
                        cmd.ExecuteNonQuery();
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
            }

        }

        public IList<I_Account> getAccounts()
        {
            IList<I_Account> accountList = new List<I_Account>();
            using (IDbCommand command = connection.CreateCommand())
            {
                verifyTable(this.connection);
                command.CommandText = "SELECT * FROM Accounts;";
                IDataReader dataReader = null;
                try
                {
                    connection.Open();
                    dataReader = command.ExecuteReader();
                    I_Account accountToAdd;
                    while (dataReader.Read())
                    {
                        accountToAdd = new CDAccount();

                        // Accounts schema
                        //"accountName VARCHAR(50) 
                        //"smtpHost VARCHAR(50) 
                        //"portNo int(5) 
                        //"isSSL BOOLEAN 

                        if (null != dataReader["accountName"])
                        {
                            accountToAdd.Sender = (string)dataReader["accountName"];

                        }
                        if (null != dataReader["smtpHost"])
                        {
                            accountToAdd.SmtpServer = (string)dataReader["smtpHost"];
                        }

                        if (null != dataReader["portNo"])
                        {
                            accountToAdd.Port = (Int32)dataReader["portNo"];
                        }
                        accountToAdd.SSL = (bool)dataReader["isSSL"];
                        accountList.Add(accountToAdd);
                    }
                }
                catch (SQLiteException err)
                {
                    throw new ContactDataExcpetion(
                        "Cannot return Accounts. Unexpected SQLite exception occured: " +
                    err.Message);
                }
                finally
                {
                    if (null != dataReader)
                    {
                        dataReader.Close();
                    }
                    connection.Close();
                }
            }
            return accountList;
        }
    }
}
