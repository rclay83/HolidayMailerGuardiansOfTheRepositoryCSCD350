using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ContactData
{
    public class AccountDao : IAccountDao
    {
        public void addAccount(Email.I_Account toAdd)
        {
            throw new NotImplementedException();
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
            throw new NotImplementedException();
        }
    }
}
