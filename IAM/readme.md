# Assume role

## Introduction

It is AWS best practice to use roles to grant limited access to certain resources for a limited period of time. Thanks to these IAM users can be deleted and created again to increase security (IAM users credentials are stored in c:\Users\[user-name]\.aws\credentials file).

Additionally if some resources are created using only IAM users (without using role) then we might loose access to these resources if IAM user that created them was deleted. Such resource is EKS then root aws account has to be used to again get access to the EKS but again because of security reasons root aws account should be use as less as possible.

## Create role

**Roles are used for temporary permissions - it is easy to assign another role and in this way complete change user permissions.**

**Roles also can be used to map corporate identities to roles - then we even do have create IAM Users. We can federate corporate identities into AWS account.**

**When an identity assumes a role it abandons all previous permissions that it has and it assumes permission of that role.**

Typically this role is created using AWS Console and not using IaC because at this point in time usually we have ony root aws account and usually we do not want store root aws account credentials locally because of security but if there are some reasons it can be also created using IaC.
IAM user that will assume this role will be used in IaC.

![01_create_role.png](./images/01_create_role.png)

Select permissions which will be need (best practice is to select only these which are really needed):

![02_create_role.png](./images/02_create_role.png)

![03_create_role.png](./images/03_create_role.png)

![04_create_role.png](./images/04_create_role.png)

Next we have to add policy that allows the role to be assumed by any IAM user in the aws account 123456789012 (fake number), **if the administrator of that account explicitly grants the ```sts:assumerole``` permission to the user.**

```
{
  "Version": "2012-10-17",
  "Statement": [
    {
      "Effect": "Allow",
      "Principal": {
        "AWS": "arn:aws:iam::123456789012:root"
      },
      "Action": "sts:AssumeRole"
    }
  ]
}
```

>NOTE: It seems that if on a role we specify exact IAM user then it is not need to add ```sts:assumerole``` on this user.

```
{
  "Version": "2012-10-17",
  "Statement": [
    {
      "Sid": "",
      "Effect": "Allow",
      "Principal": {
        "AWS": [
          "arn:aws:iam::593685711111:user/infrastructure-admin"
        ]
      },
      "Action": "sts:AssumeRole"
    }
  ]
}
```

![05_create_role.png](./images/05_create_role.png)

![06_create_role.png](./images/06_create_role.png)

## Create IAM user

![07_create_user.png](./images/07_create_user.png)
![08_create_user.png](./images/08_create_user.png)
![09_create_user.png](./images/09_create_user.png)
![10_create_user.png](./images/10_create_user.png)
Next we should copy credentials and placed them in aws ```credentials``` file on local machine.
![11_create_user.png](./images/11_create_user.png)

Next we have to add policy that allows the user to assume only the ```@Infra``` role.

>NOTE: Like explained in chapter create role this step is needed only if IAM role has policy assume role specified using root user!

```
{
  "Version": "2012-10-17",
  "Statement": [
    {
      "Effect": "Allow",
      "Action": "sts:AssumeRole",
      "Resource": "arn:aws:iam::123456789012:role/@Infra"
    }
  ]
}
```

![12_create_user.png](./images/12_create_user.png)
![13_create_user.png](./images/13_create_user.png)
![14_create_user.png](./images/14_create_user.png)

>NOTE: pay attention that this IAM user has only this one permission and this is the key point because then all permissions are controlled on the role level. 

## Configure aws credentials file

Next we have to configure credential files:

```
[infra]
role_arn = arn:aws:iam::123456789012:role/@Infra
source_profile = deployer-do-not-use-this-profile-directly

[deployer-do-not-use-this-profile-directly]
aws_access_key_id = [KEY]
aws_secret_access_key = [ACCESS_KEY]
output=json
region=eu-central-1
```

>NOTE: in case of terraform assume role we have to use different way of configuring credentials: do not use `source_profile` and simply set only `aws_access_key_id` and `aws_secret_access_key` for the IAM user and next use such profile in terraform. Example of using user profile is available in terraform repository (without assuming a role):
https://github.com/kicaj29/Terraform/tree/master/aws/terraform-enhanced-backend

## Usage @Infra role

Next we can use ```@Infra``` role to do some operations in AWS (assuming it has proper permissions).
For example we can deploy AWS lambda.

![15_use_role.png](./images/15_use_role.png)
![16_use_role.png](./images/16_use_role.png)
![17_use_role.png](./images/17_use_role.png)

Next we can delete created stack to clean up:

![18_use_role.png](./images/18_use_role.png)
![19_use_role.png](./images/19_use_role.png)


# Links
https://aws-blog.de/2021/08/iam-what-happens-when-you-assume-a-role.html   
https://docs.aws.amazon.com/cli/latest/userguide/cli-configure-role.html