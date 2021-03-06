﻿using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace JWTAuthentication.API.Services
{
    public static class SendGridMailing
    {
        //sendGrid API Key
        private const string _sendGridAPiKey = "";
        private const string _sentFromEmail = "tahainfo@yahoo.fr";
        private const string _sentFromName = "TAHA BEN AMMAR";

        /// <summary>
        /// Send Email with sendgrid
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        internal static Task SendEmail(SendGridMessage message)
        {
            try
            {
                var client = new SendGridClient(_sendGridAPiKey);
                message.From = new EmailAddress(_sentFromEmail, _sentFromName);
                //var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
                return client.SendEmailAsync(message);
            }
            catch
            {

                return Task.FromResult(0);
            }
        }

    }
}
