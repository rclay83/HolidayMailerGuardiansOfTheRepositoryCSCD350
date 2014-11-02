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



        public ContactDao() { }
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
                verifyTable();
                command.CommandText = "SELECT * FROM AllContacts;";
                try
                {
                    connection.Open();
                    IDataReader dataReader = command.ExecuteReader();
                    IContact contactToAdd;
                    while (dataReader.Read())
                    {
                        contactToAdd = new Contact();
                        contactToAdd.Email = (string)dataReader["email"];
                        contactToAdd.FirstName = (string)dataReader["first_name"];
                        string lastName = (string)dataReader["last_name"];
                        if (null != lastName)
                        {
                            contactToAdd.LastName = lastName;
                        }
                        map.Add(contactToAdd.Email, contactToAdd);
                    }
                    dataReader.Close();
                }
                //catch(Exception err)
                //{
                //todo: implement try/catch logic
                //}
                finally
                {
                    connection.Close();
                }

            }
            return map;
        }

        private void verifyTable()
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

                    verifyTable();
                    connection.Open();
                    command.ExecuteNonQuery();
                    return true;

                }
                catch
                {
                    return false;
                }
                finally
                {
                    connection.Close();
                }

            }
        }


        public bool removeContact(IContact toRemove)
        {
            throw new NotImplementedException();
        }

        public bool updateContact(IContact toUpdate)
        {
            throw new NotImplementedException();
        }
    }

}
