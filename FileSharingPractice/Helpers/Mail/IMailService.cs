using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FileSharingPractice.Helpers.Mail
{
    public interface IMailService
    {
        void SendEmail(InputEmailMessage model);
    }
}
