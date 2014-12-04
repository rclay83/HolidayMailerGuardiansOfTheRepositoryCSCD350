using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ContactData
{
    public interface IContact
    {
        String FirstName { get; set; }
        String LastName { get; set; }
        String Email { get; set; }
        bool GotMail { get; set; }
        bool Selected { get; set; }
    }
}
