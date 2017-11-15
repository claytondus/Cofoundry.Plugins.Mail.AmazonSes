using Cofoundry.Core;
using Cofoundry.Core.Mail;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Plugins.Mail.SendGrid
{
    public class SendGridMailDispatchSession : IMailDispatchSession
    {
        private readonly Queue<SendGridMessage> _mailQueue = new Queue<SendGridMessage>();
        private readonly Core.Mail.MailSettings _mailSettings;
        private readonly SendGridSettings _sendGridSettings;
        private readonly SendGridClient _sendGridClient;
        private readonly DebugMailDispatchSession _debugMailDispatchSession;

        public SendGridMailDispatchSession(
            Core.Mail.MailSettings mailSettings,
            SendGridSettings sendGridSettings,
            IPathResolver pathResolver
            )
        {
            _mailSettings = mailSettings;
            _sendGridSettings = sendGridSettings;

            if (_mailSettings.SendMode == MailSendMode.LocalDrop)
            {
                _debugMailDispatchSession = new DebugMailDispatchSession(mailSettings, pathResolver);
            }
            else
            {
                _sendGridClient = new SendGridClient(_sendGridSettings.ApiKey);
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
                    await _sendGridClient.SendEmailAsync(mailItem);
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

        private SendGridMessage FormatMessage(MailMessage message)
        {
            if (message == null) throw new ArgumentNullException(nameof(message));

            var messageToSend = new SendGridMessage();

            var toAddress = GetMailToAddress(message);
            messageToSend.AddTo(toAddress);
            messageToSend.Subject = message.Subject;
            if (message.From != null)
            {
                messageToSend.SetFrom(CreateMailAddress(message.From.Address, message.From.DisplayName));
            }
            else
            {
                messageToSend.SetFrom(CreateMailAddress(_mailSettings.DefaultFromAddress, _mailSettings.DefaultFromAddressDisplayName));
            }

            SetMessageBody(messageToSend, message.HtmlBody, message.TextBody);

            return messageToSend;
        }

        private EmailAddress GetMailToAddress(MailMessage message)
        {
            EmailAddress toAddress;
            if (_mailSettings.SendMode == MailSendMode.SendToDebugAddress)
            {
                if (string.IsNullOrEmpty(_mailSettings.DebugEmailAddress))
                {
                    throw new Exception("MailSendMode.SendToDebugAddress requested but Cofoundry:Mail:DebugEmailAddress setting is not defined.");
                }
                toAddress = CreateMailAddress(_mailSettings.DebugEmailAddress, message.To.DisplayName);
            }
            else
            {
                toAddress = new EmailAddress(message.To.Address, message.To.DisplayName);
            }
            return toAddress;
        }

        private EmailAddress CreateMailAddress(string email, string displayName)
        {
            // In other libraries we catch validation exceptions here, but SendGrid does not throw any so it is ommited
            if (string.IsNullOrEmpty(displayName))
            {
                return new EmailAddress(email);
            }

            return new EmailAddress(email, displayName);
        }

        private void SetMessageBody(SendGridMessage message, string bodyHtml, string bodyText)
        {
            var hasHtmlBody = !string.IsNullOrWhiteSpace(bodyHtml);
            var hasTextBody = !string.IsNullOrWhiteSpace(bodyText);
            if (!hasHtmlBody && !hasTextBody)
            {
                throw new ArgumentException("An email must have either a html or text body");
            }

            message.HtmlContent = bodyHtml;
            message.PlainTextContent = bodyText;
        }
    }
}
