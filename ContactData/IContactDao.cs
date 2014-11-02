using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace ContactData
{
    public interface IContactDao
    {
        IDictionary<String, IContact> getAllContacts();

        IDbConnection Connection { get; set; }

        bool addContact(IContact toAdd);
        bool removeContact(IContact toRemove);
        bool updateContact(IContact toUpdate);
    }
}
