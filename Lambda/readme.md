- [Run lambda on localstack](#run-lambda-on-localstack)
  - [Run localstack on docker desktop set as linux containers](#run-localstack-on-docker-desktop-set-as-linux-containers)
    - [Configure IAM](#configure-iam)
    - [Run aws lambda in localstack](#run-aws-lambda-in-localstack)
    - [Watch localstack logs](#watch-localstack-logs)
  - [Run localstack on docker desktop set as windows containers](#run-localstack-on-docker-desktop-set-as-windows-containers)
- [Links](#links)

# Run lambda on localstack

## Run localstack on docker desktop set as linux containers

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

## Run localstack on docker desktop set as windows containers

In this mode we can enable experimental flag that also allows run linux containers.

* Run localstack

```
docker run --name localstack_for_lambda -p 4567:4566 --platform linux --privileged -v //var/run/docker.sock:/var/run/docker.sock -d -e SERVICES=sqs,sns,logs,lambda,iam -e LAMBDA_EXECUTOR=docker localstack/localstack:latest
```

Unfortunately privileged mode is not supported on Windows even if we specify `--platform linux` flag because this command generates out put:
```
docker: Error response from daemon: Windows does not support privileged mode.
```

Let's try run it without privileged mode - then also we have to use default `LAMBDA_EXECUTOR`.
From [docs](https://hub.docker.com/r/localstack/localstack):

`LAMBDA_EXECUTOR` supports values: `local`, `docker` - default, `docker-reuse` but for the last 2 if the localstack is started inside Docker, then the `docker` command needs to be available inside the container and to achieve this we have to the container in privileged mode.

* Try run with `LAMBDA_EXECUTOR` set on `docker` but without privileged mode, there are errors

```
PS C:\> docker run --name localstack_for_lambda -p 4566:4566 --platform linux -e SERVICES=sqs,sns,logs,lambda,iam -e LAMBDA_EXECUTOR=docker localstack/localstack:latest
Waiting for all LocalStack services to be ready
2021-11-03 10:46:42,816 CRIT Supervisor is running as root.  Privileges were not dropped because no user is specified in the config file.  If you intend to run as root, you can set user=root in the config file to avoid this message.
2021-11-03 10:46:42,819 INFO supervisord started with pid 13
2021-11-03 10:46:43,821 INFO spawned: 'infra' with pid 27
(. .venv/bin/activate; exec bin/localstack start --host --no-banner)
2021-11-03 10:46:44,826 INFO success: infra entered RUNNING state, process has stayed up for > than 1 seconds (startsecs)

LocalStack version: 0.12.19.1
LocalStack build date: 2021-11-02
LocalStack build git hash: 531dc8cf

Starting edge router (https port 4566)...
2021-11-03T10:46:46:INFO:bootstrap.py: Execution of "prepare_environment" took 506.46ms
Waiting for all LocalStack services to be ready
2021-11-03 10:46:51,943 INFO exited: infra (exit status 2; not expected)
2021-11-03 10:46:51,944 INFO spawned: 'infra' with pid 47
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

* Try run with `LAMBDA_EXECUTOR` value `local`, also errors

```
PS C:\> docker run --name localstack_for_lambda -p 4566:4566 --platform linux -e SERVICES=sqs,sns,logs,lambda,iam -e LAMBDA_EXECUTOR=local localstack/localstack:latest
Waiting for all LocalStack services to be ready
2021-11-03 10:39:09,813 CRIT Supervisor is running as root.  Privileges were not dropped because no user is specified in the config file.  If you intend to run as root, you can set user=root in the config file to avoid this message.
2021-11-03 10:39:09,815 INFO supervisord started with pid 15
2021-11-03 10:39:10,817 INFO spawned: 'infra' with pid 26
(. .venv/bin/activate; exec bin/localstack start --host --no-banner)
2021-11-03 10:39:11,822 INFO success: infra entered RUNNING state, process has stayed up for > than 1 seconds (startsecs)

LocalStack version: 0.12.19.1
LocalStack build date: 2021-11-02
LocalStack build git hash: 531dc8cf

Waiting for all LocalStack services to be ready
Starting edge router (https port 4566)...
2021-11-03 10:39:21,820 INFO exited: infra (exit status 2; not expected)
2021-11-03 10:39:21,821 INFO spawned: 'infra' with pid 45
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

LocalStack version: 0.12.19.1
LocalStack build date: 2021-11-02
LocalStack build git hash: 531dc8cf
```

And if we try to create and run lambda we get the following error:


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

# Links
https://docs.aws.amazon.com/lambda/latest/dg/csharp-package-cli.html   
https://www.alexhyett.com/contact-form-lambda-dotnet-core/   
https://gitlab.com/sunnyatticsoftware/sandbox/localstack-sandbox/-/tree/master/03-lambda-dotnet-empty   
https://awscli.amazonaws.com/v2/documentation/api/latest/reference/lambda/create-function.html
https://awscli.amazonaws.com/v2/documentation/api/latest/reference/lambda/invoke.html
https://docs.aws.amazon.com/sdkref/latest/guide/setting-global-cli_binary_format.html