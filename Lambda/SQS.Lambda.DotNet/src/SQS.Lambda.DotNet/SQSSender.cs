using Amazon.SQS;
using System;
using System.Collections.Generic;
using System.Text;

namespace SQS.Lambda.DotNet
{
    public class SQSSender
    {
        private AmazonSQSClient _sqs;
        public void Init()
        {
            string serviceUrl = string.Format($"http://localhost:4566");
            AmazonSQSConfig config = new AmazonSQSConfig
            {
                ServiceURL = serviceUrl,
            };
            this._sqs = new AmazonSQSClient(null, config);
        }
    }
}
