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
    public class MockContactDao : IContactDao
    {
        private IDbConnection connection;



        public MockContactDao() { }
        public MockContactDao(IDbConnection conn)
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

            for (int index = 1; index <= 5; index++ )
            {
                string email = "mock@allMocks.com" + index;
                map.Add(email, new MockContact() { FirstName = "mock" + index, LastName = "mockLast", Email = email });
            }
            return map;
        }

        public void verifyTable(IDbConnection connection)
        {
            throw new NotImplementedException("No table to verify. This is a mock dao!");
        }


        public bool addContact(IContact toAdd)
        {
           
            if(null == toAdd.FirstName || null == toAdd.Email)
            {
                throw new ContactDataException("Missing required table field");
            }
            return true;
        }


        public bool removeContact(IContact toRemove)
        {
            if (null == toRemove.FirstName || null == toRemove.Email)
            {
                throw new ContactDataException("Missing required table field");
            }
            return true;
        }

        public bool updateContact(IContact toUpdate)
        {
            if (null == toUpdate.FirstName || null == toUpdate.Email)
            {
                throw new ContactDataException("Missing required table field");
            }
            return true;
        }
    }

}
