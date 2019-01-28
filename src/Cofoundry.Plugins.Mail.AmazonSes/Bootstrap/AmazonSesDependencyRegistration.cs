using Cofoundry.Core.DependencyInjection;
using Cofoundry.Core.Mail;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cofoundry.Plugins.Mail.AmazonSes
{
    public class AmazonSesDependencyRegistration : IDependencyRegistration
    {
        public void Register(IContainerRegister container)
        {
            if (container.Configuration.GetValue<bool>("Cofoundry:Plugins:AmazonSes:Disabled")) return;

            var overrideOptions = RegistrationOptions.Override();

            container
                .Register<IMailDispatchService, AmazonSesMailDispatchService>(overrideOptions)
                ;
        }
    }
}
