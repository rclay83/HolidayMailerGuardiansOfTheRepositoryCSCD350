using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mail
{
    interface IMail
    {
        public string[] Recipients { get; set; }

        public string Subject { get; set; }

        public string Body { get; set; }

        public string Sender { get; set; }
    }
}
