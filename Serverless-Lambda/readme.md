- [Introduction for Serverless](#introduction-for-serverless)
- [Integration with SQS](#integration-with-sqs)
- [Handling errors](#handling-errors)
  - [Synchronous events](#synchronous-events)
  - [Asynchronous events](#asynchronous-events)
  - [Error handling for stream-based events](#error-handling-for-stream-based-events)
- [Notes from AWS PartnerCast](#notes-from-aws-partnercast)

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

# Handling errors

When you invoke a function, two types of errors can occur:

* **Function errors**: You get a function error if Lambda successfully hands off an event to your function, but the function throws an error or times out before completing.
  ![007-function-error.png](./images/007-function-error.png)

* **Invocation errors**: You get an invocation error if the request is rejected before your function receives it. For example, an oversized payload or lack of permissions will cause an invocation error. You also get an invocation error if the function is getting throttled.
  ![008-invokation-error.png](./images/008-invokation-error.png)

## Synchronous events

In a synchronous invocation, like between API Gateway and Lambda, no retries are built in. You have to write code to handle errors and retries for all error types.  

![009-synchronous-events.png](./images/009-synchronous-events.png)

## Asynchronous events

![009-asynchronous-events.png](./images/![009-asynchronous-events.png])

With asynchronous event sources, like Amazon S3, **Lambda provides built-in retry behaviors**.   

When Lambda gets an asynchronous event, the lambda service returns a success to the event source and **puts the request in its own internal queue**. Then it sends invocation requests from its queue to your function.   
If the invocation request returns invocation errors, Lambda retries that **request two more times by default**. You can configure this retry value between 0 and 2.   

If the invocation request returns invocation errors, Lambda retries that invocation for **up to 6 hours**. You can decrease the default duration using the maximum age of event setting. 

To handle events that continue to fail beyond the retry or maximum age settings, you can configure a **dead-letter queue** on the Lambda function. Alternatively, you can set a **failed-event destination**.

## Error handling for stream-based events

Stream-based event sources (like Kinesis Data Streams or DynamoDB Streams) need to maintain **record order per shard**. So, by default, if an error occurs while Lambda is processing a batch of records, Lambda won’t process any new records from that shard until the batch succeeds or expires.You can use the Iterator-Age metric to detect blocked shards.  

![010-stream-events.png](./images/010-stream-events.png)


Prior to December 2019, if you wanted to bypass stream failures, you needed to write code into your function to return a success back to the stream after some number of attempts, and then write the error record to something like Amazon SQS queue or CloudWatch Logs for offline analysis. 

![011-stream-events.png](./images/011-stream-events.png)

A better way to manage failures is to modify the default behaviors using four configuration options introduced in 2019:

* **Bisect batch on function error**: tells Lambda to split a failing batch into two and retry each batch separately.
* **Maximum retry attempts**: let you limit the number of retries on a failed batch.
* **Maximum record age**: let you limit the duration of retries on a failed batch.
* **On-failure destination**: lets you send failed records to an SNS topic or SQS queue to be handled offline without having to add additional logic into your function.

Here’s an illustration of how these options work together. In this example, **BisectOnFunctionError = True, MaximumRetryAttempts = 2**, and DestinationConfig includes an **OnFailure Destination** that points to an **SNS topic**.

![012-stream-events.png](./images/012-stream-events.png)

Assume you have a batch of 10 records, and the third record in this batch of 10 returns a function error. When the function returns an error, Lambda splits the batch into two, and then sends those to your function separately, still maintaining record order. Lambda also resets the retry and max age values whenever it splits a batch.

![013-stream-events.png](./images/013-stream-events.png)

Now you’ve got two batches of five. Lambda sends the first batch of five and it fails. So the splitting process repeats. Lambda splits that failing batch yielding a batch of two and a batch of three records. Lambda resets the retry and max age values, and sends the first of those two batches for processing.

![014-stream-events.png](./images/014-stream-events.png)
![015-stream-events.png](./images/015-stream-events.png)

This time, the batch of two records processes successfully. So Lambda sends the batch of three to the function. That batch fails. Lambda splits it, and now it has a batch with one record (the bad one) and another with two records.

![016-stream-events.png](./images/016-stream-events.png)
![017-stream-events.png](./images/017-stream-events.png)

Lambda sends the batch with a bad record and it fails, but there’s nothing left to split.

So now the max retry and max age settings come into play. In this example, the function retries the record twice, and when it continues to fail, sends it to the SNS topic configured for the on-failure destination.   

With the erroring record out of the way, Lambda works its way back through each of the smaller batches it created, always maintaining record order.

![018-stream-events.png](./images/018-stream-events.png)
![019-stream-events.png](./images/019-stream-events.png)
![020-stream-events.png](./images/020-stream-events.png)
![021-stream-events.png](./images/021-stream-events.png)

So Lambda is going to process the unprocessed batch of two, then the unprocessed batch of five. At that point, the original batch of 10 is marked as successful, and Lambda moves the pointer on the stream to the start of the next batch of records.

![022-stream-events.png](./images/022-stream-events.png)
![023-stream-events.png](./images/023-stream-events.png)
![024-stream-events.png](./images/024-stream-events.png)

These options provide flexible error handling, but they also introduce **the potential for a record to be processed multiple times**. In the example, the first two records are processed before the function returns an error.

![025-stream-events.png](./images/025-stream-events.png)

Then they’re processed a second time in the smaller batch of five records, which also fails, and then they are processed a third time in their own batch.

This means you have to handle idempotency in your function rather than assuming only-once record processing.

![026-stream-events.png](./images/026-stream-events.png)
![027-stream-events.png](./images/027-stream-events.png)

# Notes from AWS PartnerCast

![001-question.png](./images/001-question.png)

* https://docs.aws.amazon.com/lambda/latest/dg/configuration-envvars.html