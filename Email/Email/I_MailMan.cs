﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Email
{
    public interface I_MailMan
    {
        void sendMail();

        void setAttachment(string[] attachment);
    }
}
