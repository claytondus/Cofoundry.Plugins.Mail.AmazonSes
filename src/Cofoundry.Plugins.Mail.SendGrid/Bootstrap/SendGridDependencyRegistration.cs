using Cofoundry.Core.DependencyInjection;
using Cofoundry.Core.Mail;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cofoundry.Plugins.Mail.SendGrid
{
    public class SendGridDependencyRegistration : IDependencyRegistration
    {
        public void Register(IContainerRegister container)
        {
            if (container.Configuration.GetValue<bool>("Cofoundry:Plugins:SendGrid:Disabled")) return;

            var overrideOptions = RegistrationOptions.Override();

            container
                .Register<IMailDispatchService, SendGridMailDispatchService>(overrideOptions)
                ;
        }
    }
}
