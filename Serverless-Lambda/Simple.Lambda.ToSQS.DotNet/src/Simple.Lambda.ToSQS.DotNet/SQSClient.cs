using Amazon.Runtime;
using Amazon.SQS;
using Amazon.SQS.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Simple.Lambda.ToSQS.DotNet
{
    public class SQSClient
    {
        private AmazonSQSClient _sqs;
        public void Init()
        {
            // integration with localstack only
            //string serviceUrl = string.Format($"http://localhost:4566");
            // USE host.docker.internal to route the traffic to the docker host!!!
            string serviceUrl = string.Format($"http://host.docker.internal:4566");
            AmazonSQSConfig config = new AmazonSQSConfig
            {
                ServiceURL = serviceUrl,
            };

            AWSCredentials credentials = new BasicAWSCredentials("fake", "fake");

            this._sqs = new AmazonSQSClient(credentials, config);
        }

        public async Task SendMessageAsync(string message)
        {
            SendMessageRequest sqsSendMessageRequest = new SendMessageRequest
            {
                QueueUrl = "http://localhost:4566/000000000000/BatchExported",
                MessageBody = message
            };

            var result = await this._sqs.SendMessageAsync(sqsSendMessageRequest);
            Console.WriteLine(result.MessageId);
        }

        public async Task ReceiveMessage()
        {
            ReceiveMessageRequest receiveMessageRequest = new ReceiveMessageRequest();
            receiveMessageRequest.QueueUrl = "http://localhost:4566/000000000000/BatchExported";
            ReceiveMessageResponse receiveMessageResponse = await _sqs.ReceiveMessageAsync(receiveMessageRequest);
            foreach (Message message in receiveMessageResponse.Messages)
            {
                Console.WriteLine("Body: {0}", message.Body);
            }
        }
    }
}
