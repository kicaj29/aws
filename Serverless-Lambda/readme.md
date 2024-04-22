# Introduction for Serverless

* No Server Management
* Flexible Scaling
* **Automated HA and Fault Tolerance**
* No Idle Capacity

# Integration with SQS

The message is stored on the SQS queue durably. The Lambda service polls the Amazon SQS queue. If Lambda finds new messages, it invokes the Lambda function the queue is connected to, passing the message as a parameter and including the message ID for identifying the message.

![002-sqs-inegration.png](./images/002-sqs-inegration.png)

It’s important here to distinguish between the Lambda service and your individual Lambda functions.

* The Lambda service uses long polling to poll the SQS queue, waiting for messages to appear.
* The Lambda service reads messages in batches and invokes your Lambda **function once per batch**.

![003-sqs-inegration.png](./images/003-sqs-inegration.png)

![004-sqs-inegration.png](./images/004-sqs-inegration.png)

By default, Lambda reads up to five batches at a time and sends each batch to an invocation of your function. Each invocation of the Lambda function gets an event object that contains the messages in a batch.   
The messages in that batch become hidden to other consumers for the duration of the queue’s visibility timeout setting.   
**When your Lambda function successfully processes a batch, the Lambda service deletes that batch of messages from the queue. If your function returns an error or doesn’t respond, the messages in that batch become visible again.**   

![005-sqs-inegration.png](./images/005-sqs-inegration.png)

If any of the messages in the batch fail, all items in the batch become visible on the queue again. This means that some messages will be processed more than once. **You want to include code in your Lambda functions to handle this kind of partial failure.** Your Lambda function should delete each message from the queue after successfully processing it. That way if the batch fails, only the unsuccessful messages reappear in the queue.

![006-sqs-inegration.png](./images/006-sqs-inegration.png)

When a message continues to fail, send it to a dead-letter queue. A dead-letter queue is another SQS queue that you use to process failed messages. This is optional, but recommended.   
Make sure that you configure the dead-letter queue on the source queue versus configuring the dead-letter queue option on the Lambda function.

# Notes from AWS PartnerCast

![001-question.png](./images/001-question.png)

* https://docs.aws.amazon.com/lambda/latest/dg/configuration-envvars.html