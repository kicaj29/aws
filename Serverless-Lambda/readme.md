- [Run lambda on localstack with dotnet core 3.1](#run-lambda-on-localstack-with-dotnet-core-31)
  - [Run localstack on docker desktop set as linux containers - WORKS OK](#run-localstack-on-docker-desktop-set-as-linux-containers---works-ok)
    - [Configure IAM](#configure-iam)
    - [Run aws lambda in localstack](#run-aws-lambda-in-localstack)
    - [Watch localstack logs](#watch-localstack-logs)
  - [Run localstack on docker desktop set as windows containers - DOES NOT WORK](#run-localstack-on-docker-desktop-set-as-windows-containers---does-not-work)
  - [SAM to host lambda](#sam-to-host-lambda)
    - [Run localstack on docker desktop set as linux containers - WORKS OK](#run-localstack-on-docker-desktop-set-as-linux-containers---works-ok-1)
    - [Run localstack on docker desktop set as windows containers - DOES NOT WORK](#run-localstack-on-docker-desktop-set-as-windows-containers---does-not-work-1)
- [Links](#links)

# Run lambda on localstack with dotnet core 3.1

## Run localstack on docker desktop set as linux containers - WORKS OK

* Run localstack

```
docker run --name localstack_for_lambda -p 4567:4566 --privileged -v //var/run/docker.sock:/var/run/docker.sock -d -e SERVICES=sqs,sns,logs,lambda,iam -e LAMBDA_EXECUTOR=docker localstack/localstack:latest
```

Why to use double dash in mounted path - explained [here](https://stackoverflow.com/a/47229180/15353052).

>NOTE: sometimes (because of some Windows Updates?) port 4566 is on list with excluded port ranges use different host port than default 4566. To check excluded port ranges run `netsh interface ipv4 show excludedportrange protocol=tcp`. It looks that port 4566 is excluded only if docker is run as linux containers!


* To remove container run
```
docker rm -f localstack_for_lambda
```

### Configure IAM

* Make sure that in aws credentials file there are some fake default credentials
```
[default]
aws_access_key_id=test
aws_secret_access_key=test
region=us-east-1
```

* Create IAM role in the localstack

--endpoint-url http://localhost:4567 param points localstack instance running locally

```
aws iam --endpoint-url http://localhost:4567 create-role --role-name lambda-dotnet-ex --assume-role-policy-document '{"Version": "2012-10-17", "Statement": [{ "Effect": "Allow", "Principal": {"Service": "lambda.amazonaws.com"}, "Action": "sts:AssumeRole"}]}'
```
This command should generate result like this:
```
{
    "Role": {
        "Path": "/",
        "RoleName": "lambda-dotnet-ex",
        "RoleId": "d672ux6okrwytxgyygrl",
        "Arn": "arn:aws:iam::000000000000:role/lambda-dotnet-ex",
        "CreateDate": "2021-11-02T10:34:45.200000+00:00",
        "AssumeRolePolicyDocument": "{Version: 2012-10-17, Statement: [{ Effect: Allow, Principal: {Service: lambda.amazonaws.com}, Action: sts:AssumeRole}]}",
        "MaxSessionDuration": 3600
    }
}
```

* Attach `AWSLambdaBasicExecutionRole` policy to role
```
aws iam --endpoint-url http://localhost:4567 attach-role-policy --role-name lambda-dotnet-ex --policy-arn arn:aws:iam::aws:policy/service-role/AWSLambdaBasicExecutionRole
```
If everything will be fine it should generate empty result.

### Run aws lambda in localstack

* Install neccesary CLI (it might require admin to execute these commands):

```
PS D:\GitHub\kicaj29\aws\Lambda> dotnet tool install --add-source https://api.nuget.org/v3/index.json -g Amazon.Lambda.Tools
You can invoke the tool using the following command: dotnet-lambda
Tool 'amazon.lambda.tools' (version '5.2.0') was successfully installed.
```

>NOTE: it looks that param `--add-source` does not work correctly because I had to manually modify `nuget.config` (`%appdata%\nuget\nuget.config`) to remove broken sources that were causing that that package were not installed but also there was no error/warning!

```
dotnet new -i Amazon.Lambda.Templates
```

>NOTE: to see all supported project templates run: ```dotnet new -all```.

* Create sample lambda project

```
dotnet new lambda.EmptyFunction -n Simple.Lambda.DotNet
```

* Compile and publish

First go to folder where lambda is stored: `D:\GitHub\kicaj29\aws\Lambda\Simple.Lambda.DotNet\src\Simple.Lambda.DotNet>`

```
dotnet build
dotnet publish -c Release -o publish
```

* Zip lambda files
```
cd publish
zip -r ../function.zip *
```

* Create lambda function used earlier created role `lambda-dotnet-ex`
```
cd ..
aws lambda --endpoint-url http://localhost:4567 create-function --function-name lambda-dotnet-function --zip-file fileb://function.zip --handler Simple.Lambda.DotNet::Simple.Lambda.DotNet.Function::FunctionHandler --runtime dotnetcore3.1 --role arn:aws:iam::000000000000:role/lambda-dotnet-ex
```

* Invoke lambda using payload in CLI
Payload text must be encoded as base64 and quoted:
```
"{ \"name\": \"Bob\" }"
```

```
aws lambda --endpoint-url http://localhost:4567 invoke --function-name lambda-dotnet-function --payload InsgXCJuYW1lXCI6IFwiQm9iXCIgfSI= response.json --log-type Tail
```

* Invoke lambda using payload from a file

```
aws lambda --endpoint-url http://localhost:4567 invoke --cli-binary-format raw-in-base64-out --function-name lambda-dotnet-function --payload file://sample-payload.json response.json --log-type Tail
```

### Watch localstack logs

We can see that there are no errors during startup and we can see when the lambda has been executed:

```
PS C:\> docker run --name localstack_for_lambda -p 4567:4566 --privileged -v //var/run/docker.sock:/var/run/docker.sock -e SERVICES=sqs,sns,logs,lambda,iam -e LAMBDA_EXECUTOR=docker localstack/localstack:latest
Waiting for all LocalStack services to be ready
2021-11-03 11:20:11,838 CRIT Supervisor is running as root.  Privileges were not dropped because no user is specified in the config file.  If you intend to run as root, you can set user=root in the config file to avoid this message.
2021-11-03 11:20:11,840 INFO supervisord started with pid 15
2021-11-03 11:20:12,843 INFO spawned: 'infra' with pid 21
(. .venv/bin/activate; exec bin/localstack start --host --no-banner)

LocalStack version: 0.12.19.1
LocalStack Docker container id: a64da81fdcc3
LocalStack build date: 2021-11-02
LocalStack build git hash: 4c9b108c

2021-11-03 11:20:14,136 INFO success: infra entered RUNNING state, process has stayed up for > than 1 seconds (startsecs)
Starting edge router (https port 4566)...
2021-11-03T11:20:14:INFO:bootstrap.py: Execution of "prepare_environment" took 507.09ms
[2021-11-03 11:20:14 +0000] [22] [INFO] Running on https://0.0.0.0:4566 (CTRL + C to quit)
2021-11-03T11:20:14:INFO:hypercorn.error: Running on https://0.0.0.0:4566 (CTRL + C to quit)
2021-11-03T11:20:15:INFO:bootstrap.py: Execution of "_load_service_plugin" took 590.37ms
2021-11-03T11:20:15:INFO:bootstrap.py: Execution of "require" took 590.64ms
2021-11-03T11:20:17:INFO:bootstrap.py: Execution of "_load_service_plugin" took 1923.90ms
2021-11-03T11:20:17:INFO:bootstrap.py: Execution of "require" took 1924.08ms
2021-11-03T11:20:17:INFO:localstack.services.infra: Starting mock CloudWatch service on http port 4566 ...
2021-11-03T11:20:17:INFO:localstack.services.motoserver: starting moto server on http://0.0.0.0:56691
2021-11-03T11:20:17:INFO:localstack.services.infra: Starting mock IAM service on http port 4566 ...
2021-11-03T11:20:17:INFO:localstack.services.infra: Starting mock Lambda service on http port 4566 ...
Ready.
2021-11-03T11:20:17:INFO:bootstrap.py: Execution of "require" took 553.25ms
2021-11-03T11:20:17:INFO:localstack.services.infra: Starting mock CloudWatch Logs service on http port 4566 ...
2021-11-03T11:20:17:INFO:localstack.services.infra: Starting mock SNS service on http port 4566 ...
2021-11-03T11:20:17:INFO:localstack.services.infra: Starting mock SQS service on http port 4566 ...
2021-11-03T11:20:17:INFO:bootstrap.py: Execution of "preload_services" took 3438.04ms
2021-11-03T11:21:27:INFO:localstack.services.awslambda.lambda_utils: Determined main container target IP: 172.17.0.2
2021-11-03T11:21:27:INFO:localstack.services.awslambda.lambda_executors: Running lambda: arn:aws:lambda:us-east-1:000000000000:function:lambda-dotnet-function
2021-11-03T11:21:33:INFO:localstack.services.awslambda.lambda_executors: Running lambda: arn:aws:lambda:us-east-1:000000000000:function:lambda-dotnet-function
```

## Run localstack on docker desktop set as windows containers - DOES NOT WORK

In this mode we can enable experimental flag that also allows run linux containers.

* Run localstack

```
docker run --name localstack_for_lambda -p 4567:4566 --platform linux --privileged -v //var/run/docker.sock:/var/run/docker.sock -d -e SERVICES=sqs,sns,logs,lambda,iam -e LAMBDA_EXECUTOR=docker localstack/localstack:latest
```

Unfortunately privileged mode is not supported on Windows even if we specify `--platform linux` flag because this command generates output:
```
docker: Error response from daemon: Windows does not support privileged mode.
```

Let's try run it without privileged mode - then also we have to use `local` as `LAMBDA_EXECUTOR`.
From [docs](https://hub.docker.com/r/localstack/localstack):

`LAMBDA_EXECUTOR` supports values: `local`, `docker` - default, `docker-reuse` but for the last 2 if the localstack is started inside Docker, then the `docker` command needs to be available inside the container and to achieve this we have to the container in privileged mode.

* Try run with `LAMBDA_EXECUTOR` set on `docker` but without privileged mode, there are errors

```
PS C:\WINDOWS\system32> docker run --name localstack_for_lambda -p 4566:4566 --platform linux -e SERVICES=sqs,sns,logs,lambda,iam -e LAMBDA_EXECUTOR=docker localstack/localstack:0.12.18
Waiting for all LocalStack services to be ready
2021-11-03 11:36:06,809 CRIT Supervisor is running as root.  Privileges were not dropped because no user is specified in the config file.  If you intend to run as root, you can set user=root in the config file to avoid this message.
2021-11-03 11:36:06,811 INFO supervisord started with pid 13
2021-11-03 11:36:07,813 INFO spawned: 'infra' with pid 25
(. .venv/bin/activate; exec bin/localstack start --host --no-banner)
2021-11-03 11:36:08,817 INFO success: infra entered RUNNING state, process has stayed up for > than 1 seconds (startsecs)
exception while loading plugin localstack.plugins.cli:pro: plugin localstack.plugins.cli:pro is deactivated

LocalStack version: 0.12.18
LocalStack build date: 2021-09-23
LocalStack build git hash: 2c436bad

Starting edge router (https port 4566)...
2021-11-03T11:36:11:INFO:bootstrap.py: Execution of "load_plugin_from_path" took 1710.89ms
2021-11-03T11:36:11:INFO:bootstrap.py: Execution of "load_plugins" took 1711.66ms
Waiting for all LocalStack services to be ready
Waiting for all LocalStack services to be ready
Starting mock CloudWatch service on http port 4566 ...
2021-11-03T11:36:21:INFO:localstack_ext.bootstrap.install: Unable to download local test SSL certificate from https://cdn.jsdelivr.net/gh/localstack/localstack-artifacts@master/local-certs/server.key to /tmp/localstack/server.test.pem: MyHTTPSConnectionPool(host='cdn.jsdelivr.net', port=443): Max retries exceeded with url: /gh/localstack/localstack-artifacts@master/local-certs/server.key (Caused by NewConnectionError('<urllib3.connection.HTTPSConnection object at 0x7f2d53c4b210>: Failed to establish a new connection: [Errno -3] Try again')) Traceback (most recent call last):
  File "/opt/code/localstack/.venv/lib/python3.7/site-packages/urllib3/connection.py", line 175, in _new_conn
    (self._dns_host, self.port), self.timeout, **extra_kw
  File "/opt/code/localstack/.venv/lib/python3.7/site-packages/urllib3/util/connection.py", line 73, in create_connection
    for res in socket.getaddrinfo(host, port, family, socket.SOCK_STREAM):
  File "/usr/lib/python3.7/socket.py", line 752, in getaddrinfo
    for res in _socket.getaddrinfo(host, port, family, type, proto, flags):
```

It is possible to deploy lambda function but it crashes when we try call it:
```
{"errorMessage":"Lambda process returned with error. Result: . Output:\n","errorType":"InvocationException","stackTrace":["  File \"/opt/code/localstack/localstack/services/awslambda/lambda_api.py\", line 820, in run_lambda\n    lock_discriminator=lock_discriminator,\n","  File \"/opt/code/localstack/localstack/services/awslambda/lambda_executors.py\", line 399, in execute\n    return do_execute()\n","  File \"/opt/code/localstack/localstack/services/awslambda/lambda_executors.py\", line 389, in do_execute\n    return _run(func_arn=func_arn)\n","  File \"/opt/code/localstack/localstack/utils/cloudwatch/cloudwatch_util.py\", line 157, in wrapped\n    raise e\n","  File \"/opt/code/localstack/localstack/utils/cloudwatch/cloudwatch_util.py\", line 153, in wrapped\n    result = func(*args, **kwargs)\n","  File \"/opt/code/localstack/localstack/services/awslambda/lambda_executors.py\", line 376, in _run\n    raise e\n","  File \"/opt/code/localstack/localstack/services/awslambda/lambda_executors.py\", line 362, in _run\n    result = self._execute(lambda_function, inv_context)\n","  File \"/opt/code/localstack/localstack/services/awslambda/lambda_executors.py\", line 659, in _execute\n    result = self.run_lambda_executor(lambda_function=lambda_function, inv_context=inv_context)\n","  File \"/opt/code/localstack/localstack/services/awslambda/lambda_executors.py\", line 587, in run_lambda_executor\n    ) from error\n"]}

```

* Try run with `LAMBDA_EXECUTOR` value `local`, also errors

```
PS C:\WINDOWS\system32> docker run --name localstack_for_lambda -p 4566:4566 --platform linux -e SERVICES=sqs,sns,logs,lambda,iam -e LAMBDA_EXECUTOR=local localstack/localstack:0.12.18
Waiting for all LocalStack services to be ready
2021-11-03 11:41:09,813 CRIT Supervisor is running as root.  Privileges were not dropped because no user is specified in the config file.  If you intend to run as root, you can set user=root in the config file to avoid this message.
2021-11-03 11:41:09,815 INFO supervisord started with pid 14
2021-11-03 11:41:10,818 INFO spawned: 'infra' with pid 26
(. .venv/bin/activate; exec bin/localstack start --host --no-banner)
2021-11-03 11:41:11,823 INFO success: infra entered RUNNING state, process has stayed up for > than 1 seconds (startsecs)

LocalStack version: 0.12.18
LocalStack build date: 2021-09-23
LocalStack build git hash: 2c436bad

exception while loading plugin localstack.plugins.cli:pro: plugin localstack.plugins.cli:pro is deactivated
Starting edge router (https port 4566)...
2021-11-03T11:41:16:INFO:bootstrap.py: Execution of "load_plugin_from_path" took 1635.45ms
2021-11-03T11:41:16:INFO:bootstrap.py: Execution of "load_plugins" took 1635.92ms
Waiting for all LocalStack services to be ready
Waiting for all LocalStack services to be ready
Starting mock CloudWatch service on http port 4566 ...
Starting mock IAM service on http port 4566 ...
Starting mock Lambda service on http port 4566 ...
Starting mock CloudWatch Logs service on http port 4566 ...
Starting mock SNS service on http port 4566 ...
Starting mock SQS service on http port 4566 ...
2021-11-03T11:41:26:INFO:localstack_ext.bootstrap.install: Unable to download local test SSL certificate from https://cdn.jsdelivr.net/gh/localstack/localstack-artifacts@master/local-certs/server.key to /tmp/localstack/server.test.pem: MyHTTPSConnectionPool(host='cdn.jsdelivr.net', port=443): Max retries exceeded with url: /gh/localstack/localstack-artifacts@master/local-certs/server.key (Caused by NewConnectionError('<urllib3.connection.HTTPSConnection object at 0x7fe77ece8710>: Failed to establish a new connection: [Errno -3] Try again')) Traceback (most recent call last):
  File "/opt/code/localstack/.venv/lib/python3.7/site-packages/urllib3/connection.py", line 175, in _new_conn
    (self._dns_host, self.port), self.timeout, **extra_kw
  File "/opt/code/localstack/.venv/lib/python3.7/site-packages/urllib3/util/connection.py", line 73, in create_connection
    for res in socket.getaddrinfo(host, port, family, socket.SOCK_STREAM):
  File "/usr/lib/python3.7/socket.py", line 752, in getaddrinfo
    for res in _socket.getaddrinfo(host, port, family, type, proto, flags):
socket.gaierror: [Errno -3] Try again

During handling of the above exception, another exception occurred:
```

And if we try to create and run lambda we get the following error:

**Note that Node.js, Golang, and .Net Core Lambdas currently require LAMBDA_EXECUTOR=docker**

```
{"errorMessage":"Unable to find executor for Lambda function \"lambda-dotnet-function\". Note that Node.js, Golang, and .Net Core Lambdas currently require LAMBDA_EXECUTOR=docker","errorType":"InvocationException","stackTrace":["  File \"/opt/code/localstack/localstack/services/awslambda/lambda_api.py\", line 820, in run_lambda\n    lock_discriminator=lock_discriminator,\n","  File \"/opt/code/localstack/localstack/services/awslambda/lambda_executors.py\", line 399, in execute\n    return do_execute()\n","  File \"/opt/code/localstack/localstack/services/awslambda/lambda_executors.py\", line 389, in do_execute\n    return _run(func_arn=func_arn)\n","  File \"/opt/code/localstack/localstack/utils/cloudwatch/cloudwatch_util.py\", line 157, in wrapped\n    raise e\n","  File \"/opt/code/localstack/localstack/utils/cloudwatch/cloudwatch_util.py\", line 153, in wrapped\n    result = func(*args, **kwargs)\n","  File \"/opt/code/localstack/localstack/services/awslambda/lambda_executors.py\", line 376, in _run\n    raise e\n","  File \"/opt/code/localstack/localstack/services/awslambda/lambda_executors.py\", line 362, in _run\n    result = self._execute(lambda_function, inv_context)\n","  File \"/opt/code/localstack/localstack/services/awslambda/lambda_executors.py\", line 1201, in _execute\n    raise InvocationException(result, log_output)\n"]}
```

So it looks that it is not possible to execute lambda on localstack when docker desktop is run as Windows containers because **LAMBDA_EXECUTOR=docker** is required but it requires privileged mode and this mode is not supported on Windows!

If we try use latest tag then it is even worst because we cannot create IAM role:

```
PS C:\> docker run --name localstack_for_lambda -p 4566:4566 --platform linux -e SERVICES=sqs,sns,logs,lambda,iam -e LAMBDA_EXECUTOR=local localstack/localstack:latest
Waiting for all LocalStack services to be ready
2021-11-03 11:49:21,833 CRIT Supervisor is running as root.  Privileges were not dropped because no user is specified in the config file.  If you intend to run as root, you can set user=root in the config file to avoid this message.
2021-11-03 11:49:21,835 INFO supervisord started with pid 16
2021-11-03 11:49:22,837 INFO spawned: 'infra' with pid 28
(. .venv/bin/activate; exec bin/localstack start --host --no-banner)
2021-11-03 11:49:23,842 INFO success: infra entered RUNNING state, process has stayed up for > than 1 seconds (startsecs)

LocalStack version: 0.12.19.1
LocalStack build date: 2021-11-02
LocalStack build git hash: 4c9b108c

Starting edge router (https port 4566)...
Waiting for all LocalStack services to be ready
2021-11-03 11:49:32,794 INFO exited: infra (exit status 2; not expected)
2021-11-03 11:49:32,795 INFO spawned: 'infra' with pid 47
Error starting infrastructure: gave up waiting for edge server on 0.0.0.0:4566 Traceback (most recent call last):
  File "/opt/code/localstack/localstack/services/infra.py", line 396, in start_infra
    thread = do_start_infra(asynchronous, apis, is_in_docker)
  File "/opt/code/localstack/localstack/services/infra.py", line 505, in do_start_infra
    thread = start_runtime_components()
  File "/opt/code/localstack/localstack/utils/bootstrap.py", line 91, in wrapped
    return f(*args, **kwargs)
  File "/opt/code/localstack/localstack/services/infra.py", line 498, in start_runtime_components
    f"gave up waiting for edge server on {config.EDGE_BIND_HOST}:{config.EDGE_PORT}"
TimeoutError: gave up waiting for edge server on 0.0.0.0:4566

(. .venv/bin/activate; exec bin/localstack start --host --no-banner)
```

```
PS D:\GitHub\kicaj29\aws\Lambda\Simple.Lambda.DotNet\src\Simple.Lambda.DotNet> aws iam --endpoint-url http://localhost:4566 create-role --role-name lambda-dotnet-ex --assume-role-policy-document '{"Version": "2012-10-17", "Statement": [{ "Effect": "Allow", "Principal": {"Service": "lambda.amazonaws.com"}, "Action": "sts:AssumeRole"}]}'

Could not connect to the endpoint URL: "http://localhost:4566/"
```

* Used commands
```
aws iam --endpoint-url http://localhost:4566 create-role --role-name lambda-dotnet-ex --assume-role-policy-document '{"Version": "2012-10-17", "Statement": [{ "Effect": "Allow", "Principal": {"Service": "lambda.amazonaws.com"}, "Action": "sts:AssumeRole"}]}'
```

```
aws iam --endpoint-url http://localhost:4566 attach-role-policy --role-name lambda-dotnet-ex --policy-arn arn:aws:iam::aws:policy/service-role/AWSLambdaBasicExecutionRole
```

```
aws lambda --endpoint-url http://localhost:4566 create-function --function-name lambda-dotnet-function --zip-file fileb://function.zip --handler Simple.Lambda.DotNet::Simple.Lambda.DotNet.Function::FunctionHandler --runtime dotnetcore3.1 --role arn:aws:iam::000000000000:role/lambda-dotnet-ex
```

```
aws lambda --endpoint-url http://localhost:4566 invoke --cli-binary-format raw-in-base64-out --function-name lambda-dotnet-function --payload file://sample-payload.json response.json --log-type Tail
```

## SAM to host lambda

In this scenario I try to integrate localstack and SAM.

### Run localstack on docker desktop set as linux containers - WORKS OK

In this scenario lambda is run on SAM and it sends a message to SQS from localstack

[Sources](./Simple.Lambda.ToSQS.DotNet)

* Create project

```
dotnet new lambda.EmptyFunction -n Simple.Lambda.ToSQS.DotNet
```

>NOTE: files *Dockerfile* and *serverless.template* are not created with this project and there were added manually after project creation.

* Build lambda

```
sam build -t .\serverless.template --use-container
```

* Start SAM

```
sam local start-lambda
```
result
```
PS D:\GitHub\kicaj29\aws\Lambda\Simple.Lambda.ToSQS.DotNet\src\Simple.Lambda.ToSQS.DotNet> sam local start-lambda
Starting the Local Lambda Service. You can now invoke your Lambda Functions defined in your template through the endpoint.
2021-11-05 15:37:40  * Running on http://127.0.0.1:3001/ (Press CTRL+C to quit)
```

* Call lambda

```
aws --endpoint-url=http://localhost:3001 lambda invoke --function-name TransferFunction --payload InsgXCJuYW1lXCI6IFwiQm9iXCIgfSI= .\res.txt
```

* Check SQS from the localstack

The message send by the lambda is in the queue

```
PS C:\Users\jkowalski> aws sqs receive-message --endpoint-url http://localhost:4566  --queue-url http://localhost:4566/000000000000/MyQueue
{
    "Messages": [
        {
            "MessageId": "be92f69a-7db6-365e-4472-8d01bad572ae",
            "ReceiptHandle": "tpjfacsztbiyvaplyywjqpinytoufelhqbpgdzqwkgnlsbxdvgzkddfwvjkshfcceioyivccgimzzzupcyhoxgvwdfwsggzochemozcxmzmexjmrwlwvrxyztwtvfctsgkjamycoajijkltkctzapgudcmkukoqmxjifbnfiogzucaojuiulmopqk",
            "MD5OfBody": "00d5df7b1ecb9d109cb35ce40c0eb705",
            "Body": "Message sent by lamnda"
        }
    ]
}
```

### Run localstack on docker desktop set as windows containers - DOES NOT WORK

In this scenario I wanted have 2 SQSs in localstack and one lambda in SAM.
This lambda should be triggered by first SQS and should send message to second SQS.
It does not work because it is not possible to build the image and even if the image would be somehow available there is no way to hook the lambda from SAM to 
SQS from localstack because ARN address does not contain IP and PORT, more [here](https://docs.aws.amazon.com/lambda/latest/dg/with-sqs.html#events-sqs-eventsource)

```
aws lambda create-event-source-mapping --function-name my-function --batch-size 5 \
--maximum-batching-window-in-seconds 60 \
--event-source-arn arn:aws:sqs:us-east-2:123456789012:my-queue
```

[Sources](./SQS.Lambda.DotNet)

* Install AWS SAM CLI
  
https://docs.aws.amazon.com/serverless-application-model/latest/developerguide/serverless-sam-cli-install-windows.html

>NOTE: currently there is no official docker image for AWS SAM CLI

* Create project

```
dotnet new lambda.SQS -n SQS.Lambda.DotNet
```

>NOTE: files *Dockerfile* and *serverless.template* are not created with this project and there were added manually after project creation.

* Build lambda

The build fails because there is no way to build linux image on windows and even if the image would somehow would be already available there is no way to instruct SAM to run this image with ```--platform linux``` flag.

https://aws.amazon.com/blogs/compute/using-container-image-support-for-aws-lambda-with-aws-sam/

```
sam build -t .\serverless.template --use-container
```
results:
```
PS D:\GitHub\kicaj29\aws\Lambda\SQS.Lambda.DotNet\src\SQS.Lambda.DotNet> sam build -t .\serverless.template --use-container
Starting Build inside a container
Building codeuri: D:\GitHub\kicaj29\aws\Lambda\SQS.Lambda.DotNet\src\SQS.Lambda.DotNet runtime: None metadata: {'DockerTag': 'dotnetcore-v1', 'Dockerfile': 'SQS.Lambda.DotNet/Dockerfile', 'DockerContext': 'D:\\GitHub\\kicaj29\\aws\\Lambda\\SQS.Lambda.DotNet\\src'} architecture: x86_64 functions: ['TransferFunction']
Building image for TransferFunction function
Setting DockerBuildArgs: {} for TransferFunction function
Step 1/9 : FROM mcr.microsoft.com/dotnet/sdk:3.1 AS build-image
 ---> ec62f0eae4ba
Step 2/9 : RUN mkdir -p /build/build_artifacts
 ---> [Warning] The requested image's platform (linux/amd64) does not match the detected host platform (windows/amd64) and no specific platform was requested
 ---> Running in 2d7e5a5987d0

Build Failed
Error: TransferFunction failed to build: The command '/bin/sh -c mkdir -p /build/build_artifacts' returned a non-zero code: 4294967295: failed to shutdown container: container 2d7e5a5987d0c438c4e350e61ba7d10b9be2d7702208ab566c53ef0c82eeafcb encountered an error during hcsshim::System::waitBackground: failure in a Windows system call: The virtual machine or container with the specified identifier is 
not running. (0xc0370110): subsequent terminate failed container 2d7e5a5987d0c438c4e350e61ba7d10b9be2d7702208ab566c53ef0c82eeafcb encountered an error during hcsshim::System::waitBackground: failure in a Windows system call: The virtual machine or container with the specified identifier is not running. (0xc0370110)
```

# Links
https://docs.aws.amazon.com/lambda/latest/dg/csharp-package-cli.html   
https://www.alexhyett.com/contact-form-lambda-dotnet-core/   
https://gitlab.com/sunnyatticsoftware/sandbox/localstack-sandbox/-/tree/master/03-lambda-dotnet-empty   
https://awscli.amazonaws.com/v2/documentation/api/latest/reference/lambda/create-function.html
https://awscli.amazonaws.com/v2/documentation/api/latest/reference/lambda/invoke.html
https://docs.aws.amazon.com/sdkref/latest/guide/setting-global-cli_binary_format.html