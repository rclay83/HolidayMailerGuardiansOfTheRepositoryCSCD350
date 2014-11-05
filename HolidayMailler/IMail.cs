using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/*  Holiday Mailer
 *  Guardians of the Repository
 * 
 *  Author: Marcus Sanchez
 *  Last revision:  11/4/2014
 *  
 */

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
