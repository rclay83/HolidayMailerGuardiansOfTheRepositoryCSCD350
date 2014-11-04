using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mail
{
    interface IMail
    {
        string[] Recipients { get; set; }

        string Subject { get; set; }

        string Body { get; set; }

        string Sender { get; set; }
    }
}
