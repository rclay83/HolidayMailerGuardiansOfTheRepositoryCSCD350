using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HolidayMailler
{
    interface IMail
    {
        public string[] GetRecipients ();

        public string GetSubject ();

        public string GetBody ();

        public string Sender
        {
            get;
            set;
        }
    }
}
