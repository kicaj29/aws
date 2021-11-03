# Basic example

## Run localstack

* Sometimes to download localstack image from docker hub first you have to sign in:

```
PS D:\GitHub\kicaj29\aws\Lambda\Simple.Lambda.DotNet\src\Simple.Lambda.DotNet> docker login -u [USR] -p [PASS]
WARNING! Using --password via the CLI is insecure. Use --password-stdin.
Login Succeeded
```

* Run localstack on Linux docker, no vpn

```
PS D:\GitHub\kicaj29\aws\Lambda\Simple.Lambda.DotNet\src\Simple.Lambda.DotNet> docker run --name localstack_for_lambda -p 4567:4566 --privileged -v //var/run/docker.sock:/var/run/docker.sock -d -e SERVICES=sqs,sns,logs,lambda,iam -e LAMBDA_EXECUTOR=docker localstack/localstack:latest
0c6b125c51f53776cfc3f8d6d5a6d9ad2acfc18da47793027cb953d3ea777e9a
PS D:\GitHub\kicaj29\aws\Lambda\Simple.Lambda.DotNet\src\Simple.Lambda.DotNet> docker ps
CONTAINER ID   IMAGE                   COMMAND                  CREATED         STATUS         PORTS                                        NAMES
0c6b125c51f5   localstack/localstack   "docker-entrypoint.sh"   5 seconds ago   Up 3 seconds   4571/tcp, 0.0.0.0:4566->4566/tcp, 5678/tcp   localstack_for_lambda
```

>NOTE: because port 4566 is on list with excluded port ranges we use different host port. To check excluded port ranges run `netsh interface ipv4 show excludedportrange protocol=tcp`

* Other runs
```
docker network create -d nat localstack_net
PS D:\GitHub\kicaj29\aws\Lambda\Simple.Lambda.DotNet\src\Simple.Lambda.DotNet> docker run --network localstack_net --name localstack_for_lambda -p 4566:4566 --restart=on-failure:10 --platform linux -d -e SERVICES=sqs,sns,logs,lambda,iam -e LAMBDA_EXECUTOR=docker localstack/localstack:latest
0c6b125c51f53776cfc3f8d6d5a6d9ad2acfc18da47793027cb953d3ea777e9a
PS D:\GitHub\kicaj29\aws\Lambda\Simple.Lambda.DotNet\src\Simple.Lambda.DotNet> docker ps
CONTAINER ID   IMAGE                   COMMAND                  CREATED         STATUS         PORTS                                        NAMES
0c6b125c51f5   localstack/localstack   "docker-entrypoint.sh"   5 seconds ago   Up 3 seconds   4571/tcp, 0.0.0.0:4566->4566/tcp, 5678/tcp   localstack_for_lambda
```

* To remove container run
```
docker rm -f localstack_for_lambda
```

## Configure IAM

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

## Run aws lambda in localstack

* Install neccesary CLI (it might require admin to execute these commands):

```
PS D:\GitHub\kicaj29\aws\Lambda> dotnet tool install --add-source https://api.nuget.org/v3/index.json -g Amazon.Lambda.Tools
You can invoke the tool using the following command: dotnet-lambda
Tool 'amazon.lambda.tools' (version '5.2.0') was successfully installed.
```

>NOTE: it looks that param `--add-source` does not work correctly because I had to manually modify `nuget.config` (`%appdata%\nuget\nuget.config`) to remove broken sources that were blocking installation.

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

# Links
https://docs.aws.amazon.com/lambda/latest/dg/csharp-package-cli.html   
https://www.alexhyett.com/contact-form-lambda-dotnet-core/   
https://gitlab.com/sunnyatticsoftware/sandbox/localstack-sandbox/-/tree/master/03-lambda-dotnet-empty   
https://awscli.amazonaws.com/v2/documentation/api/latest/reference/lambda/create-function.html
https://awscli.amazonaws.com/v2/documentation/api/latest/reference/lambda/invoke.html
https://docs.aws.amazon.com/sdkref/latest/guide/setting-global-cli_binary_format.html