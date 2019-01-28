using Cofoundry.Core;
using Cofoundry.Core.Mail;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Plugins.Mail.AmazonSes
{
    public class AmazonSesMailDispatchService : IMailDispatchService
    {
        private readonly MailSettings _mailSettings;
        private readonly AmazonSesSettings _sesSettings;
        private readonly IPathResolver _pathResolver;

        public AmazonSesMailDispatchService(
            MailSettings mailSettings,
            AmazonSesSettings sesSettings,
            IPathResolver pathResolver
            )
        {
            _mailSettings = mailSettings;
            _sesSettings = sesSettings;
            _pathResolver = pathResolver;
        }

        public IMailDispatchSession CreateSession()
        {
            return new AmazonSesMailDispatchSession(_mailSettings, _sesSettings, _pathResolver);
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
