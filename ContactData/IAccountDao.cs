using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Email;
using System.Data;

namespace ContactData
{
    public interface IAccountDao
    {
        List<I_Account> getAccounts();
        void addAccount(I_Account toAdd);
        void removeAccount(I_Account toRemove);
        void updateAccount(I_Account toUpdate);
        void verifyTable(IDbConnection connection);

    }
}
