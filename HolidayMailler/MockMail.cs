using Mail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HolidayMailler
{
    class MockMail : IMail
    {
        public string[] GetRecipients ()
        {
            return new string[] { "GuardiansOfTheRepository@gmail.com" };
        }

        public string GetSubject ()
        {
            return "test mail";
        }

        public string GetBody ()
        {
            return "this is a test message";
        }

        public string Sender
        {
            get
            {
                return "GuardiansOfTheRepository@gmail.com";
            }
            set
            {
                throw new NotImplementedException();
            }
        }
    }
}
