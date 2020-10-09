# Interacting with AWS

## AWS Console
UI in web browser.

## AWS CLI
Command line access for administering AWS resources.   

To use CLI first you have to generate Access Key ID and Secret Access Key in AWS IAM console.

![aws-01-cli-user.png](images/core-services/aws-01-cli-user.png)

Next run aws configure to store credentials locally:

```
C:\>aws configure
AWS Access Key ID [****************5OF5]:
AWS Secret Access Key [****************JZG0]:
Default region name [us-east-1]: us-east-2
Default output format [json]:
```

Next we can start using aws cli, for example

```
C:\>aws ec2 describe-instance-status --instance-id i-0fdd9ef82fa12
{
    "InstanceStatuses": [
        {
            "AvailabilityZone": "us-east-2c",
            "InstanceId": "i-0fdd9ef82fa12",
            "InstanceState": {
                "Code": 16,
                "Name": "running"
            },
```

In AWS CLI we can use profiles: https://docs.aws.amazon.com/cli/latest/userguide/cli-configure-profiles.html. You can configure additional profiles by using aws configure with the --profile option. A named profile is a collection of settings and credentials that you can apply to a AWS CLI command.
All AWS CLI credentials and settings are stored in files:   
   
%USERPROFILE%\.aws\credentials   
%USERPROFILE%\.aws\config

## AWS SDK
Programmatic access to manage AWS resources - supported by multiple languages.

# resources
https://app.pluralsight.com/library/courses/understanding-aws-core-services/table-of-contents