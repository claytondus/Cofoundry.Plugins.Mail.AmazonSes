using Cofoundry.Core.Configuration;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Cofoundry.Plugins.Mail.AmazonSes
{
    public class AmazonSesSettings : PluginConfigurationSettingsBase
    {
        /// <summary>
        /// Indicates whether the plugin should be disabled, which means services
        /// will not be bootstrapped. Defaults to false.
        /// </summary>
        public bool Disabled { get; set; }

        /// <summary>
        /// The AWS IAM access key used when authenticating with the SES API.
        /// </summary>
        [Required]
        public string AccessKey { get; set; }
        
        /// <summary>
        /// The AWS IAM secret key used when authenticating with the SES API.
        /// </summary>
        [Required]
        public string SecretKey { get; set; }
        
        /// <summary>
        /// The AWS region.
        /// </summary>
        [Required]
        public string AwsRegion { get; set; }
    }
}
