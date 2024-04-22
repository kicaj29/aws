# Standard vs. FIFO SQS queues as Lambda event sources

* Record Order
  * Standard: Order is not guaranteed
  * FIFO: Order is guaranteed per group ID
* Delivery
  * Standard: Messages may be delivered more than once
  * FIFO: Messages are delivered only once. There are no duplicate messages introduced to the queue
* Transaction Throughput
  * Standard: Nearly unlimited messages per second
  * FIFO: FIFO queues support up to 300 messages per second, per API action without batching, or 3,000 with batching

# sqs localstack

## how to change visibility parameter

```
C:\>aws sqs set-queue-attributes --endpoint-url http://localhost:4566 --queue-url http://localhost:4566/000000000000/MyQueue --attribute VisibilityTimeout=3600

C:\>aws sqs get-queue-attributes --endpoint-url http://localhost:4566 --queue-url http://localhost:4566/000000000000/MyQueue --attribute-names All
{
    "Attributes": {
        "ApproximateNumberOfMessages": "0",
        "ApproximateNumberOfMessagesDelayed": "0",
        "ApproximateNumberOfMessagesNotVisible": "0",
        "CreatedTimestamp": "1629184776.743441",
        "DelaySeconds": "0",
        "LastModifiedTimestamp": "1629274571.382143",
        "MaximumMessageSize": "262144",
        "MessageRetentionPeriod": "345600",
        "QueueArn": "arn:aws:sqs:us-east-1:000000000000:MyQueue",
        "ReceiveMessageWaitTimeSeconds": "0",
        "VisibilityTimeout": "3600"
    }
}
```

Usually the queues are created by a system during its startup so it means that first we have to run the system to create the queues.


It looks that we have to set some default fake credentials, only then the cli will work correctly:

C:\Users\[USER]\.aws\credentials

```
[default]
aws_access_key_id=sfsdfdsf
aws_secret_access_key=sdfsasdf
aws_session_token=asdasdasd
```

From some reason it did not work when the values were set on n/a !

C:\Users\[USER]\.aws\config
```
[default]
region = us-east-1
```

## SQS.SampleApp example

Currently this example requires creation of the queue upfront.

# Differences between Amazon SQS, Amazon MQ, and Amazon SNS

https://docs.aws.amazon.com/AWSSimpleQueueService/latest/SQSDeveloperGuide/sqs-difference-from-amazon-mq-sns.html