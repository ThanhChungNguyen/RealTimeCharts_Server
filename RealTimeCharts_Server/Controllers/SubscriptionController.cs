using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Amazon.SimpleNotificationService.Util;
using RestSharp;
using Microsoft.AspNetCore.SignalR;
using AlertRealTimeServer.HubConfig;
using AlertRealTimeServer.DataStorage;
using AlertRealTimeServer.Models;
using Telegram.Bot;

namespace AlertRealTimeServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubscriptionController : ControllerBase
    {
        [HttpPost]
        public async Task PostAsync()
        {
            try
            {
                var json = "";
                var req = Request;

                // Allows using several time the stream in ASP.Net Core
                req.EnableBuffering();

                // Arguments: Stream, Encoding, detect encoding, buffer size 
                // AND, the most important: keep stream opened
                using (StreamReader reader = new StreamReader(req.Body, Encoding.UTF8, true, 1024, true))
                {
                    json = reader.ReadToEndAsync().Result;
                }

                // Rewind, so the core is not lost when it looks the body for the request
                req.Body.Position = 0;

                var message = Message.ParseMessage(json);
                if (message.IsSubscriptionType)
                {
                    CheckAndConfirm(message);
                    Console.WriteLine("Successful confirm subscription by visit SubscribeURL provied by AWS message.");
                    return;
                }
                if (message.IsNotificationType)
                {
                    TranferToListener(message);
                    SendMessageToTelegramAsync(message);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Cannot handle SNS message: " + ex.Message);
                return;
            }
        }

        private async Task SendMessageToTelegramAsync(Message message)
        {
            var userIds = new List<int>() { 965257084 };
            var token = "942063387:AAFplGvIyyOfd36X6UeKH9r59sb5mcqIwwg";
            var telegramBotClient = new TelegramBotClient(token);
            foreach (var userId in userIds)
            {
                await telegramBotClient.SendTextMessageAsync(userId, message.MessageText);
            }
        }

        private void TranferToListener(Message message)
        {
            //if(message.TopicArn != "arn:aws:sns:us-east-2:217837847749:Dev-Alert-Topic")
            //{
            //    throw new Exception("Message is not come from expected Alert topic. Ignore this message.");
            //}

            DataManager.AlertModel = new AlertModel { Message = message };
            DataManager.HaveMessageToTransfer = true;
        }

        private void CheckAndConfirm(Message message)
        {
            //if(message.TopicArn != "arn:aws:sns:us-east-2:217837847749:Dev-Alert-Topic")
            //{
            //    throw new Exception("Message is not come from expected Alert topic. Ignore this message.");
            //}

            var client = new RestClient();
            var request = new RestRequest(message.SubscribeURL, Method.GET);

            IRestResponse response = client.Execute(request);
            if (!response.IsSuccessful)
            {
                throw new Exception("Fail in confirming subscription by visiting SubscribeURL from AWS message.");
            }
        }
    }
}