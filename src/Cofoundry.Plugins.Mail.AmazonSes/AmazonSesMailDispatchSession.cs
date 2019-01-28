using Cofoundry.Core;
using Cofoundry.Core.Mail;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Amazon;
using Amazon.SimpleEmail;
using Amazon.SimpleEmail.Model;

namespace Cofoundry.Plugins.Mail.AmazonSes
{
    public class AmazonSesMailDispatchSession : IMailDispatchSession
    {
        private readonly Queue<SendEmailRequest> _mailQueue = new Queue<SendEmailRequest>();
        private readonly Core.Mail.MailSettings _mailSettings;
        private readonly AmazonSesSettings _AmazonSesSettings;
        private readonly AmazonSimpleEmailServiceClient _AmazonSesClient;
        private readonly DebugMailDispatchSession _debugMailDispatchSession;

        public AmazonSesMailDispatchSession(
            Core.Mail.MailSettings mailSettings,
            AmazonSesSettings AmazonSesSettings,
            IPathResolver pathResolver
            )
        {
            _mailSettings = mailSettings;
            _AmazonSesSettings = AmazonSesSettings;

            if (_mailSettings.SendMode == MailSendMode.LocalDrop)
            {
                _debugMailDispatchSession = new DebugMailDispatchSession(mailSettings, pathResolver);
            }
            else
            {
                _AmazonSesClient = new AmazonSimpleEmailServiceClient(_AmazonSesSettings.AccessKey,
                    _AmazonSesSettings.SecretKey,
                    RegionEndpoint.GetBySystemName(_AmazonSesSettings.AwsRegion));
            }
        }

        public void Add(MailMessage mailMessage)
        {
            var messageToSend = FormatMessage(mailMessage);
            _mailQueue.Enqueue(messageToSend);
        }

        public async Task FlushAsync()
        {
            if (_debugMailDispatchSession != null)
            {
                await _debugMailDispatchSession.FlushAsync();
                return;
            }

            while (_mailQueue.Count > 0)
            {
                var mailItem = _mailQueue.Dequeue();
                if (mailItem != null && _mailSettings.SendMode != MailSendMode.DoNotSend)
                {
                    await _AmazonSesClient.SendEmailAsync(mailItem);
                }
            }
        }

        public void Dispose()
        {
            if (_debugMailDispatchSession != null)
            {
                _debugMailDispatchSession.Dispose();
            }
        }

        private SendEmailRequest FormatMessage(MailMessage message)
        {
            if (message == null) throw new ArgumentNullException(nameof(message));

            var messageToSend = new SendEmailRequest();

            messageToSend.Destination = GetDestination(message);
            messageToSend.Message = new Message();
            if (message.From != null)
            {
                messageToSend.Source = $"{message.From.DisplayName} <{message.From.Address}>";
            }
            else
            {
                messageToSend.Source = $"{_mailSettings.DefaultFromAddressDisplayName} <{_mailSettings.DefaultFromAddress}>";
            }

            messageToSend.Message = GetMessage(message.HtmlBody, message.TextBody, message.Subject);

            return messageToSend;
        }

        private Destination GetDestination(MailMessage message)
        {
            var destination = new Destination();
            destination.ToAddresses = new List<string>();
            if (_mailSettings.SendMode == MailSendMode.SendToDebugAddress)
            {
                if (string.IsNullOrEmpty(_mailSettings.DebugEmailAddress))
                {
                    throw new Exception("MailSendMode.SendToDebugAddress requested but Cofoundry:Mail:DebugEmailAddress setting is not defined.");
                }
                destination.ToAddresses.Add($"{message.To.DisplayName} <{_mailSettings.DebugEmailAddress}>");
            }
            else
            {
                destination.ToAddresses.Add($"{message.To.DisplayName} <{message.To.Address}>");
            }
            return destination;
        }

        private Message GetMessage(string bodyHtml, string bodyText, string subject)
        {
            var hasHtmlBody = !string.IsNullOrWhiteSpace(bodyHtml);
            var hasTextBody = !string.IsNullOrWhiteSpace(bodyText);
            var hasSubject = !string.IsNullOrWhiteSpace(subject);
            if (!hasHtmlBody && !hasTextBody)
            {
                throw new ArgumentException("An email must have either a html or text body and a subject");
            }

            return new Message
            {
                Subject = new Content
                {
                    Data = subject
                },
                Body = new Body
                {
                    Html = new Content(bodyHtml),
                    Text = new Content(bodyText)
                }
            };
        }
    }
}
