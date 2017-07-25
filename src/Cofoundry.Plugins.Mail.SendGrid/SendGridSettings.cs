using Cofoundry.Core.Configuration;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Cofoundry.Plugins.Mail.SendGrid
{
    public class SendGridSettings : PluginConfigurationSettingsBase
    {
        /// <summary>
        /// Indicates whether the plugin should be disabled, which means services
        /// will not be bootstrapped. Defaults to false.
        /// </summary>
        public bool Disabled { get; set; }

        /// <summary>
        /// The api key use when authenticating with the SendGrid api.
        /// </summary>
        [Required]
        public string ApiKey { get; set; }
    }
}
