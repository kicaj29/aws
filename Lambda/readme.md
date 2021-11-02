# Basic example

## Run localstack

* Sometimes to download localstack image from docker hub first you have to sign in:

```
PS D:\GitHub\kicaj29\aws\Lambda\Simple.Lambda.DotNet\src\Simple.Lambda.DotNet> docker login -u [USR] -p [PASS]
WARNING! Using --password via the CLI is insecure. Use --password-stdin.
Login Succeeded
```

```
PS D:\GitHub\kicaj29\aws\Lambda\Simple.Lambda.DotNet\src\Simple.Lambda.DotNet> docker run --name localstack_for_lambda -p 4566:4566 --platform linux -d -e SERVICES=sqs,sns,logs,lambda,iam localstack/localstack   
0c6b125c51f53776cfc3f8d6d5a6d9ad2acfc18da47793027cb953d3ea777e9a
PS D:\GitHub\kicaj29\aws\Lambda\Simple.Lambda.DotNet\src\Simple.Lambda.DotNet> docker ps
CONTAINER ID   IMAGE                   COMMAND                  CREATED         STATUS         PORTS                                        NAMES
0c6b125c51f5   localstack/localstack   "docker-entrypoint.sh"   5 seconds ago   Up 3 seconds   4571/tcp, 0.0.0.0:4566->4566/tcp, 5678/tcp   localstack_for_lambda
```

* To remove container run
```
docker rm -f localstack_for_lambda
```

* Create IAM role in the localstack

--endpoint-url http://localhost:4566 param points localstack instance running locally

```
aws iam --endpoint-url http://localhost:4566 create-role --role-name lambda-dotnet-ex --assume-role-policy-document '{"Version": "2012-10-17", "Statement": [{ "Effect": "Allow", "Principal": {"Service": "lambda.amazonaws.com"}, "Action": "sts:AssumeRole"}]}'
```
This command should generate result like this:
```
{
    "Role": {
        "Path": "/",
        "RoleName": "lambda-dotnet-ex",
        "RoleId": "ivy2j1bp82atq32uolyz",
        "Arn": "arn:aws:iam::000000000000:role/lambda-dotnet-ex",
        "CreateDate": "2021-11-02T10:34:45.200000+00:00",
        "AssumeRolePolicyDocument": "{Version: 2012-10-17, Statement: [{ Effect: Allow, Principal: {Service: lambda.amazonaws.com}, Action: sts:AssumeRole}]}",
        "MaxSessionDuration": 3600
    }
}
```

* Attach `AWSLambdaBasicExecutionRole` policy to role
```
aws iam --endpoint-url http://localhost:4566 attach-role-policy --role-name lambda-dotnet-ex --policy-arn arn:aws:iam::aws:policy/service-role/AWSLambdaBasicExecutionRole
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
aws lambda --endpoint-url http://localhost:4566 create-function --function-name lambda-dotnet-function --zip-file fileb://function.zip --handler Sample.Lambda.DotNet::Sample.Lambda.DotNet.Function::FunctionHandler --runtime dotnetcore3.1 --role arn:aws:iam::000000000000:role/lambda-dotnet-ex
```

# Links
https://docs.aws.amazon.com/lambda/latest/dg/csharp-package-cli.html   
https://www.alexhyett.com/contact-form-lambda-dotnet-core/   
https://gitlab.com/sunnyatticsoftware/sandbox/localstack-sandbox/-/tree/master/03-lambda-dotnet-empty   
https://awscli.amazonaws.com/v2/documentation/api/latest/reference/lambda/create-function.html
