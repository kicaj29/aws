- [Introduction for Serverless](#introduction-for-serverless)
- [Integration with SQS](#integration-with-sqs)
- [Handling errors](#handling-errors)
  - [Synchronous events](#synchronous-events)
  - [Asynchronous events](#asynchronous-events)
  - [Error handling for stream-based events](#error-handling-for-stream-based-events)
  - [Failed-event destinations](#failed-event-destinations)
    - [Failed-event destinations vs dead-letter queue](#failed-event-destinations-vs-dead-letter-queue)
  - [Error handling with Amazon SQS as an event source](#error-handling-with-amazon-sqs-as-an-event-source)
  - [Error handling summary by execution model](#error-handling-summary-by-execution-model)
    - [API GW (synchronous event source)](#api-gw-synchronous-event-source)
    - [SNS (asynchronous event source)](#sns-asynchronous-event-source)
    - [Kinesis Data Streams (polling a stream as event source)](#kinesis-data-streams-polling-a-stream-as-event-source)
    - [SQS queue (polling a queue as an event source)](#sqs-queue-polling-a-queue-as-an-event-source)
  - [Dead-letter queues for Lambda functions and for SQS source queues](#dead-letter-queues-for-lambda-functions-and-for-sqs-source-queues)
  - [AWS Event Fork Pipelines](#aws-event-fork-pipelines)
  - [Migrating to Serverless](#migrating-to-serverless)
    - [Leapfrog](#leapfrog)
    - [Organic](#organic)
    - [Strangler](#strangler)
- [Serverless IT automation](#serverless-it-automation)
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

**By default, Lambda reads up to five batches at a time** and sends each batch to an invocation of your function. Each invocation of the Lambda function gets an event object that contains the messages in a batch.   
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

![009-asynchronous-events.png](./images/009-asynchronous-events.png)

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

## Failed-event destinations

For both **asynchronous** and **streaming** event sources, you can specify an on-failure destination for a Lambda function.

For **asynchronous sources**, you have the option of an SNS topic, SQS queue, EventBridge event bus, or another Lambda function. 
![028-failed-event-destinations.png](./images/028-failed-event-destinations.png)

For **streaming event sources**, you can specify an SNS topic or an SQS queue.
![029-failed-event-destinations.png](./images/029-failed-event-destinations.png)

The SendOrder Step Functions task kicks off the SNS fulfillment topic, whose subscribers handle additional fulfillment requirements. Let’s say that one of the subscribers is a Lambda function that decides if the order qualifies for a promotional gift. If it does, the function initiates steps to send the gift from a third-party system.

To handle potential failures caused by the third-party system, set the function’s on-failure destination equal to the ARN of an SNS topic that notifies the team responsible for fulfillment.

![030-failed-event-destinations.png](./images/030-failed-event-destinations.png)

### Failed-event destinations vs dead-letter queue

There are a couple of advantages to using an on-failure destination rather than using a dead-letter queue.

* First, the invocation record that is sent to the on-failure destination contains more data than the event object available to a dead-letter queue.
* Second, it provides more flexibility to change or modify the failure behaviors. A dead-letter queue is part of a function’s version-specific configuration.  
* You can also set on-success destinations to route successfully processed events without modifying your Lambda function code.

![031-failed-event-destinations.png](./images/031-failed-event-destinations.png)

## Error handling with Amazon SQS as an event source

For polling event sources that aren’t stream based, for example Amazon SQS, if an invocation fails or times out, the message is available again when the visibility timeout period expires.

Lambda keeps retrying that message until it is either successful or the queue’s **Maxreceivecount** limit has been exceeded.   

As noted earlier, it’s a best practice to set up a dead-letter queue on the source queue to process the failed messages.   

When you’re building serverless applications, you need to execute performance tests, and adjust retries and timeouts to find the optimal combination that allows your processes to complete but doesn’t create bottlenecks that can cascade throughout the system.   

Let’s go back to the connection between an Amazon SQS queue and Lambda as an example of how you manage timeouts across services.

You can set a timeout on your Lambda functions, and you can set the visibility timeout on SQS queues.

You can also set the batch size for the queue from 1 to 10 messages per batch, which can impact both your function timeout and your visibility timeout configurations.

You choose your Lambda timeout to allow the function to complete successfully under most circumstances.

You also want to consider at what point to give up on individual invocation to avoid additional costs or prevent a bottleneck.

**A larger batch size can reduce polling costs and improve efficiency for fast workloads**. **But for longer running functions, you might need a lower batch size so that everything in the batch is processed before the function timeout expires.**

For example, a batch size of 10 would require fewer polling processes and fewer invocations than a batch size of 3.

![032-lambda-sqs.png](./images/032-lambda-sqs.png)

If your function typically can process a message in 2 seconds, then a batch size of 10 would typically process in 20 seconds, and you might use a function timeout of 30 seconds. But if your function takes 2 minutes to process each message, then it would take 20 minutes to process a batch of 10.

However, the maximum timeout for Lambda is 15 minutes, so that batch would fail without processing all of its messages, and any messages in that batch that weren’t deleted by your function would again be visible on the queue. Which brings us back to the visibility timeout setting on the queue.

You need to configure the visibility timeout to allow enough time for your Lambda function to complete a message batch. So if we stick with the example of a batch size of 10 and a Lambda function that takes 20 seconds to process the batch, you need a visibility timeout that is greater than 20 seconds.

![033-lambda-sqs.png](./images/033-lambda-sqs.png)

You also need to leave some buffer in the visibility timeout to account for Lambda invocation retries when the function is getting throttled. You don’t want your visibility timeout to expire before those messages can be processed. **The best practice is to set your visibility timeout to 6 times the timeout you configure for your function.**

**If a Lambda function returns errors when processing messages, Lambda decreases the number of processes polling the queue.**

## Error handling summary by execution model

### API GW (synchronous event source)

* **Timeout considerations** – API Gateway has a 30-second timeout. If the Lambda function hasn't responded to the request within 30 seconds, an error is returned.
* **Retries** – There are no built-in retries if a function fails to execute successfully.
* **Error handling** – Generate the SDK from the API stage, and use the backoff and retry mechanisms it provides.

### SNS (asynchronous event source)

* **Timeout considerations** – Asynchronous event sources do not wait for a response from the function's execution. Requests are handed off to Lambda, where they are queued and invoked by Lambda.
* **Retries** – Asynchronous event sources have built-in retries. If a failure is returned from a function's execution, Lambda will attempt that invocation **two more times** for a **total of three attempts** to execute the function with its event payload. You can use the Retry Attempts configuration to set the retries to 0 or 1 instead.

  If Lambda is unable to invoke the function (for example, if there is not enough concurrency available and requests are getting throttled), Lambda will continue to try to run the function again for up to **6 hours by default**. You can modify this duration with **Maximum Event Age**.

  Amazon SNS has unique retry behaviors among asynchronous events based on its delivery policy for AWS Lambda(opens in a new tab). It will perform 3 immediate tries, 2 at 1 second apart, 10 backing off from 1 second to 20 seconds, and 100,000 at 20 seconds apart.

* **Error handling** – Use the Lambda destinations(opens in a new tab) **OnFailure** option to send failures to another destination for processing. Alternatively, move failed messages to a **dead-letter queue** on the function. When Amazon SNS is the event source, you also have the option to configure a dead-letter queue on the SNS subscription.

### Kinesis Data Streams (polling a stream as event source)

* **Timeout considerations** – When the retention period for a record expires, the record is no longer available to any consumer. The retention period is **24 hours by default**. You can increase the retention period at a cost. As an event source for **Lambda**, you can configure **Maximum Record Age** to tell Lambda to skip processing a data record when it has reached its Maximum Record Age.
* **Retries** – By default, Lambda retries a failing batch until the retention period for a record expires. You can configure Maximum Retry Attempts so that your Lambda function will skip retrying a batch of records when it has reached the Maximum Retry Attempts (or it has reached the Maximum Record Age).
* **Error handling** – Configure an **OnFailure** destination on your Lambda function so that when a data record reaches the Maximum Retry Attempts or Maximum Record Age, you can send its metadata, such as shard ID and stream Amazon Resource Name (ARN), to an SQS queue or SNS topic for further investigation.

  Use **BisectBatchOnFunctionError** to tell Lambda to split a failed batch into two batches. Retry your function invocation with smaller batches to isolate bad records and work around timeout and retry issues.

  For more information on these error handling features, see the blog post AWS Lambda Supports Failure-Handling Features for Kinesis and DynamoDB Event Sources https://aws.amazon.com/about-aws/whats-new/2019/11/aws-lambda-supports-failure-handling-features-for-kinesis-and-dynamodb-event-sources/.

### SQS queue (polling a queue as an event source)

* **Timeout considerations** – When the visibility timeout expires, messages become visible to other consumers on the queue. Set your visibility timeout to 6 times the timeout you configure for your function.
* **Retries** – Use the **maxReceiveCount** on the queue's policy to limit the number of times Lambda will retry to process a failed execution.
* **Error handling** – Write your functions to delete each message as it is successfully processed. Move failed messages to a dead-letter queue configured on the source SQS queue.

## Dead-letter queues for Lambda functions and for SQS source queues

![034-dead-letter.png](./images/034-dead-letter.png)

## AWS Event Fork Pipelines 

AWS Event Fork Pipelines are prebuilt applications, available in the [AWS Serverless Application Repository](https://aws.amazon.com/serverless/serverlessrepo/), that you can use in your serverless applications.

The **Event Replay Pipeline** buffers events from the given Amazon SNS topic into an Amazon SQS queue, so it can replay these events back to another pipeline in a disaster recovery scenario.

![035-event-replay-pipeline.png](./images/035-event-replay-pipeline.png)

## Migrating to Serverless

At a high level, there are three migration patterns that you might follow to migrate your legacy applications to a serverless model.

### Leapfrog

As the name suggests, with the leapfrog pattern, you bypass interim steps and go straight from an on-premises legacy architecture to a serverless cloud architecture.

![036-migration-to-serverless.png](./images/036-migration-to-serverless.png)

### Organic

With the organic pattern, you move on-premises applications to the cloud in more of a **lift-and-shift model**. In this model, existing applications are kept intact, either running on Amazon Elastic Compute Cloud (Amazon EC2) instances or with some limited rewrites to container services such as Amazon Elastic Kubernetes Service (Amazon EKS), Amazon Elastic Container Service (Amazon ECS), or AWS Fargate.

### Strangler

With the strangler pattern, an organization incrementally and systematically decomposes monolithic applications by creating APIs and building event-driven components that gradually replace components of the legacy application.

Distinct API endpoints can point to old compared to new components and safe deployment options (such as canary deployments) let you point back to the legacy version with very little risk.

New feature branches can be serverless first, and legacy components can be decommissioned as they are replaced. This pattern represents a more systematic approach to adopting serverless, **allowing you to move to critical improvements where you see benefit quickly but with less risk and upheaval than the leapfrog pattern**.

# Serverless IT automation

A powerful IT automation pattern is to trigger a Lambda function that assesses whether a configuration change is allowed and deletes the change automatically if it is not allowed. In this example, you will learn how configuration changes can be automated in response to an event.

![037-Automate-response.png](./images/037-Automate-response.png)

* **1 CloudWatch event for security group change**: a CloudWatch event is fired off whenever someone modifies a security group
* **2 Lambda function evaluates the change**: the CloudWatch event triggers a Lambda function through AWS Config that has custom code to review a list of what security group rules are allowed or denied. If the update is not allowed, the function deletes the rule and uses SNS to send an email alert.

# Notes from AWS PartnerCast

![001-question.png](./images/001-question.png)

* https://docs.aws.amazon.com/lambda/latest/dg/configuration-envvars.html