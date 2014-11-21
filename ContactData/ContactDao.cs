using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.Common;
using System.Data.SQLite;

namespace ContactData
{
    public class ContactDao : IContactDao
    {
        private IDbConnection connection;



        public ContactDao()
        {
            this.connection = new SQLiteConnection("Data Source=Contacts.db;Version=3;New=True;Compress=True;");
        }
        public ContactDao(IDbConnection conn)
        {

            this.connection = conn;
        }

        public IDbConnection Connection
        {
            get { return this.connection; }
            set { this.connection = value; }
        }

        public IDictionary<String, IContact> getAllContacts()
        {
            IDictionary<String, IContact> map = new Dictionary<String, IContact>();
            using (IDbCommand command = connection.CreateCommand())
            {
                verifyTable(this.connection);
                command.CommandText = "SELECT * FROM AllContacts;";
                IDataReader dataReader = null;
                try
                {
                    connection.Open();
                    dataReader = command.ExecuteReader();
                    IContact contactToAdd;
                    while (dataReader.Read())
                    {
                        contactToAdd = new Contact();

                        if (null != dataReader["email"])
                        {
                            contactToAdd.Email = (string)dataReader["email"];

                        }
                        if (null != dataReader["first_name"])
                        {
                            contactToAdd.FirstName = (string)dataReader["first_name"];
                        }

                        if (null != dataReader["last_name"])
                        {
                            contactToAdd.LastName = (string)dataReader["last_name"];
                        }
                        contactToAdd.GotMail = (bool)dataReader["got_mail"];
                        map.Add(contactToAdd.Email, contactToAdd);
                    }

                }
                catch (SQLiteException err)
                {
                    throw new ContactDataExcpetion("Cannot return all contacts. unexpected sqlite exception: " +
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
            return map;
        }




        public void verifyTable(IDbConnection connection)
        {
            if (null != connection)
            {
                using (IDbCommand cmd = connection.CreateCommand())
                {
                    cmd.CommandText = "CREATE TABLE IF NOT EXISTS AllContacts(" +
                        "first_name VARCHAR(15) NOT NULL," +
                        "last_name VARCHAR(15)," +
                        "email VARCHAR(40) PRIMARY KEY," +
                        "got_mail BOOLEAN DEFAULT 'false' NOT NULL);";
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


        public bool addContact(IContact toAdd)
        {
            if (null == toAdd.FirstName || null == toAdd.Email)
            {
                throw new ContactDataExcpetion("Missing required table field");
            }
            if (0 == toAdd.FirstName.Length || 0 == toAdd.Email.Length)
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

        /**
         * Remove contact from Contact table where email matches
         * */
        public bool removeContact(IContact toRemove)
        {
            if (null == toRemove.FirstName || null == toRemove.Email)
            {
                throw new ContactDataExcpetion("Missing required table field");
            }
            if (0 == toRemove.FirstName.Length || 0 == toRemove.Email.Length)
            {
                throw new ContactDataExcpetion("Missing required table field");
            }
            using (IDbCommand command = connection.CreateCommand())
            {
                try
                {
                    string commandText = "DELETE FROM AllContacts " +
                        "WHERE email = @email;";

                    command.CommandText = commandText;

                    command.Parameters.Add(new SQLiteParameter("@email") { Value = toRemove.Email });

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

        public bool updateContact(IContact toUpdate)
        {
            throw new NotImplementedException();
        }
    }

}
