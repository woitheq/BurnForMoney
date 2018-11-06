﻿using System.IO;
using System.Linq;
using System.Threading.Tasks;
using BurnForMoney.Functions.Configuration;
using BurnForMoney.Functions.Shared.Queues;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using SendGrid.Helpers.Mail;

namespace BurnForMoney.Functions.Functions
{
    public static class NotificationsGatewayFunc
    {
        private static string _emailTemplate;

        [FunctionName(FunctionsNames.NotificationsGateway)]
        public static async Task SendEmail([QueueTrigger(AppQueueNames.NotificationsToSend)] Notification notification, ILogger log, ExecutionContext context,
            [SendGrid(ApiKey = "SendGrid:ApiKey")] IAsyncCollector<SendGridMessage> messageCollector)
        {
            var configuration = ApplicationConfiguration.GetSettings(context);

            var message = new SendGridMessage
            {
                From = new EmailAddress(configuration.Email.SenderEmail, "Burn for Money")
            };

            message.AddTos(notification.Recipients.Select(email => new EmailAddress(email)).ToList());   
            message.Subject = notification.Subject;
            message.HtmlContent = ApplyTemplate(notification.HtmlContent, context);

            log.LogInformation($"Sending message to: [{string.Join(", ", notification.Recipients)}].");
            await messageCollector.AddAsync(message);
        }

        private static string ApplyTemplate(string content, ExecutionContext context)
        {
            if (string.IsNullOrWhiteSpace(_emailTemplate))
            {
                var path = Path.Combine(context.FunctionAppDirectory + "\\Resources\\", "email_template.txt");
                _emailTemplate = File.ReadAllText(path);
            }

            return _emailTemplate.Replace("%%%content%%%", content);
        }
    }
}