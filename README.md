# Cofoundry.Plugins.Mail.AmazonSes

[![Build status](https://ci.appveyor.com/api/projects/status/wquf931go22ibjg6?svg=true)](https://ci.appveyor.com/project/Cofoundry/cofoundry-plugins-mail-sendgrid)
[![NuGet](https://img.shields.io/nuget/v/Cofoundry.Plugins.Mail.AmazonSes.svg)](https://www.nuget.org/packages/Cofoundry.Plugins.Mail.AmazonSes/)
[![Gitter](https://img.shields.io/gitter/room/cofoundry-cms/cofoundry.svg)](https://gitter.im/cofoundry-cms/cofoundry)


This library is a plugin for [Cofoundry](https://www.cofoundry.org/). For more information on getting started with Cofoundry check out the [Cofoundry repository](https://github.com/cofoundry-cms/cofoundry).

## Overview

This plugin allows you to send mail using the [Amazon Simple Email Service (SES)](https://sendgrid.com/) API. By referencing this package your Cofoundry project will automatically replace the default IMailDispatchService implementation with one that uses Amazon SES. Use the following settings to configure the service:

- ***Cofoundry:Plugins:AmazonSes:AccessKey*** The access key to use when authenticating with the Amazon SES API.
- ***Cofoundry:Plugins:AmazonSes:SecretKey*** The secret key to use when authentication with the Amazon SES API.
- ***Cofoundry:Plugins:AmazonSes:AwsRegion*** The AWS region.
- ***Cofoundry:Plugins:AmazonSes:Disabled*** Indicates whether the plugin should be disabled, which means services will not be bootstrapped. Defaults to false.





