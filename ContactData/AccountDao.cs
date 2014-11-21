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
            if (String.IsNullOrEmpty(toAdd.Username) || String.IsNullOrEmpty(toAdd.SmtpServer))
            {
                throw new ContactDataExcpetion("Missing required table field");
            }
            
            using (IDbCommand command = connection.CreateCommand())
            {
                try
                {
                    // Accounts schema
                    //"accountName VARCHAR(50) 
                    //"smtpHost VARCHAR(50) 
                    //"portNo int(5) 
                    //"isSSL BOOLEAN 

                    string commandText = "INSERT INTO Accounts(accountName,smtpHost,portNo,isSSL)" +
                        "VALUES(@userName,@smtpHost,@portNo,@SSL)";

                    command.CommandText = commandText;

                    command.Parameters.Add(new SQLiteParameter("@userName") { Value = toAdd.Username });
                    command.Parameters.Add(new SQLiteParameter("@smtpHost") { Value = toAdd.SmtpServer });
                    command.Parameters.Add(new SQLiteParameter("@portNo") { Value = toAdd.Port });
                    command.Parameters.Add(new SQLiteParameter("@SSL") { Value = toAdd.SSL });

                    verifyTable(this.connection);
                    connection.Open();
                    command.ExecuteNonQuery();
                    

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
            if (String.IsNullOrEmpty(toRemove.Username))
            {
                throw new ContactDataExcpetion("Missing required table field");
            }

            using (IDbCommand command = connection.CreateCommand())
            {
                try
                {
                    string commandText = "DELETE FROM Accounts " +
                        "WHERE accountName = @userName;";

                    command.CommandText = commandText;

                    command.Parameters.Add(new SQLiteParameter("@userName") { Value = toRemove.Username });

                    verifyTable(this.connection);
                    connection.Open();
                    command.ExecuteNonQuery();
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
                        "accountName VARCHAR(50) NOT NULL PRIMARY KEY," +
                        "smtpHost VARCHAR(50)," +
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
                            accountToAdd.Username = (string)dataReader["accountName"];
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
