using Cofoundry.Core;
using Cofoundry.Core.Mail;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Plugins.Mail.AmazonSes
{
    public class SendGridMailDispatchService : IMailDispatchService
    {
        private readonly MailSettings _mailSettings;
        private readonly SendGridSettings _sendGridSettings;
        private readonly IPathResolver _pathResolver;

        public SendGridMailDispatchService(
            MailSettings mailSettings,
            SendGridSettings sendGridSettings,
            IPathResolver pathResolver
            )
        {
            _mailSettings = mailSettings;
            _sendGridSettings = sendGridSettings;
            _pathResolver = pathResolver;
        }

        public IMailDispatchSession CreateSession()
        {
            return new SendGridMailDispatchSession(_mailSettings, _sendGridSettings, _pathResolver);
        }

        public async Task DispatchAsync(MailMessage message)
        {
            using (var session = CreateSession())
            {
                session.Add(message);
                await session.FlushAsync();
            }
        }
    }
}
