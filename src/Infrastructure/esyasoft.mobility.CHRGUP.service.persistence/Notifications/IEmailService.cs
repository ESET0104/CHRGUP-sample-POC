using System;
using System.Collections.Generic;
using System.Text;

namespace esyasoft.mobility.CHRGUP.service.persistence.Notifications
{
    public interface IEmailService
    {
        Task SendAsync(string to, string subject, string body);
    }
}
