# sqs localstack

## how to change visibility parameter

```
C:\>aws sqs set-queue-attributes --endpoint-url http://localhost:4566 --queue-url http://localhost:4566/000000000000/MyQueue --attribute VisibilityTimeout=3600

C:\>aws sqs get-queue-attributes --endpoint-url http://localhost:4566 --queue-url http://localhost:4566/000000000000/MyQueue
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