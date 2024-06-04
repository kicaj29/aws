- [Basic IAM entities: Role, User, User group, Policy, Permission](#basic-iam-entities-role-user-user-group-policy-permission)
- [IAM Roles](#iam-roles)
  - [Passing a role to an AWS service](#passing-a-role-to-an-aws-service)
    - [Condition keys for passing roles](#condition-keys-for-passing-roles)
  - [Assume role in AWS Console](#assume-role-in-aws-console)
    - [Introduction](#introduction)
    - [Create IAM role](#create-iam-role)
    - [Create IAM user](#create-iam-user)
    - [Configure aws credentials file](#configure-aws-credentials-file)
    - [Usage @Infra role](#usage-infra-role)
    - [Assuming multiple IAM roles](#assuming-multiple-iam-roles)
  - [IAM PassRole vs IAM AssumeRole](#iam-passrole-vs-iam-assumerole)
- [IAM user groups](#iam-user-groups)
- [IAM Access Analyzer and Access Advisor](#iam-access-analyzer-and-access-advisor)
- [IAM Policy](#iam-policy)
  - [Policy: AWS managed, Customer managed, Inline](#policy-aws-managed-customer-managed-inline)
  - [Access Policies Schema](#access-policies-schema)
  - [Policy types](#policy-types)
    - [Guardrails vs. grants](#guardrails-vs-grants)
  - [Principal types](#principal-types)
  - [Analyzing the authorization context](#analyzing-the-authorization-context)
  - [Attributes and Tagging](#attributes-and-tagging)
    - [Benefits of the ABAC method](#benefits-of-the-abac-method)
  - [IAM Condition Keys](#iam-condition-keys)
- [Explicit and implicit denies](#explicit-and-implicit-denies)
- [Types of AWS credentials](#types-of-aws-credentials)
- [Managing server certificates in IAM](#managing-server-certificates-in-iam)
- [Links](#links)

# Basic IAM entities: Role, User, User group, Policy, Permission

* **Permission**: it is the smallest "unit" in IAM. It's the statement in a policy that allows or denies access. Permissions in AWS are defined within policies.
* **Policy**: A policy is a document that formally states one or more permissions. It is written in JSON format. **Policy can be assigned to users, user groups and role.** In context of a single user, user group and role it is called **Permissions Policies**.
  ![20_permissions_policies.png](./images/20_permissions_policies.png)
  Policy can exist also as **inline policy** which is always in context of single user, user group or role ane cannot be assigned to other users, user groups or roles.
  ![24_inline_policy.png](./images/24_inline_policy.png)
* **User**: An IAM user is an identity with **long-term credentials** that is used to interact with AWS in an account.
* **User group**: A user group is a collection of IAM users. Use groups to **specify permissions (these are statements from policies)** for a collection of users.
* **Role**: An IAM role is an identity you can create that has specific permissions with **credentials that are valid for short durations**. Roles can be assumed by entities that you trust.

# IAM Roles

**Roles are used for temporary permissions - it is easy to assign another role and in this way complete change user permissions.**

**Roles also can be used to map corporate identities to roles - then we even do not have create IAM Users. We can federate corporate identities into AWS account.**

**When someone assumes an IAM role, they abandon all previous permissions that they had under a previous role and assume the permissions of the new role.**

Other important features of IAM roles:
* No static login credentials
* IAM roles are assumed programmatically
* **Credentials are temporary** for a configurable amount of time

Roles can be assumed by:
* An **IAM user or role** in the same or different AWS account that needs the access that the role provides
* Applications on an Amazon Elastic Compute Cloud (Amazon EC2) instance that need access to AWS resources
* An AWS service that needs to call other services on your behalf or create and manage resources in your account.
* An external user authenticated by an identity provider service that is compatible with **SAML 2.0 or OpenID Connect, or a custom-built identity broker**
* If your organization uses multiple accounts, roles will be a key part of your strategy of centrally managing users' access across multiple accounts.
* **NOTE: IAM group cannot assume IAM roles!**

## Passing a role to an AWS service

There are many AWS services that require permissions via a role to perform actions on your behalf. To configure these services, you need to **pass the role to the service only once during setup**. For example, assume that you have an application running on an Amazon EC2 instance that requires access to an Amazon DynamoDB table. The application needs temporary credentials for authentication and authorization to interact with the table. When you set up the application, you must pass a role to Amazon EC2 to use with the instance that provides those credentials. The **application assumes the role every time it needs to perform the actions that the role allows**.

### Condition keys for passing roles

To pass a role to an AWS service, a user must have the proper permissions. This helps to ensure that only approved users can configure a service with a role that grants permissions. In order to allow a user to pass a role to an AWS service, you must first add the `iam:PassRole` action to its IAM policy. You may also add IAM condition keys to your policies to further control how roles are passed

* `iam:PassedToService`: The `iam:PassedToService` key specifies the service principal of the service to which a role can be passed. A service principal is the name of a service that can be specified in the Principal element of a policy in the following format: `SERVICE_NAME_URL.amazonaws.com`. You can use `iam:PassedToService` to restrict your users so that they can pass roles only to specific services and ensure that users create service roles only for the services that you specify. 

  For example, a user might create a service role that trusts Amazon CloudWatch to write log data to an Amazon S3 bucket on their behalf. In this case, the trust policy must specify `cloudwatch.amazonaws.com` in the Principal element. If a user with the policy below attempts to create a service role for Amazon EC2, the operation will fail. The failure occurs because the user does not have permission to pass the role to Amazon EC2.

  ![42_iam_pass_role.png](./images/42_iam_pass_role.png)

* `iam:AssociatedResourceArn`: This condition key specifies the ARN of the resource to which this role will be associated at the destination service.  Use this condition key in a policy to allow an IAM entity (an IAM user or role) to pass a role but only if that role is associated with the specified resource.

  For example, below you have the `iam:PassRole` action with both the `iam:PassedToService` and `iam:AssociatedResourceArn` condition keys. Here you are allowing an IAM entity to pass any role to the Amazon EC2 service to be used with instances in the us-east-1 or us-west-1 Region. The IAM user or role would not be allowed to pass roles to other services, and it doesn't allow Amazon EC2 to use the role with instances in other Regions.

  ![43_iam_pass_role.png](./images/43_iam_pass_role.png)

## Assume role in AWS Console

### Introduction

It is AWS best practice to use roles to grant limited access to certain resources for a limited period of time. Thanks to these IAM users can be deleted and created again to increase security (IAM users credentials are stored in c:\Users\[user-name]\.aws\credentials file).

Additionally if some resources are created using only IAM users (without using role) then we might loose access to these resources if IAM user that created them was deleted. Such resource is EKS then root aws account has to be used to again get access to the EKS but again because of security reasons root aws account should be use as less as possible.

### Create IAM role

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

### Create IAM user

**By default, when you create a new IAM user in AWS, it has no permissions associated with it**

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

### Configure aws credentials file

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

### Usage @Infra role

Next we can use ```@Infra``` role to do some operations in AWS (assuming it has proper permissions).
For example we can deploy AWS lambda.

![15_use_role.png](./images/15_use_role.png)
![16_use_role.png](./images/16_use_role.png)
![17_use_role.png](./images/17_use_role.png)

Next we can delete created stack to clean up:

![18_use_role.png](./images/18_use_role.png)
![19_use_role.png](./images/19_use_role.png)

### Assuming multiple IAM roles

**You cannot apply multiple roles to a single instance.**

https://repost.aws/questions/QU-EiIvq3rTGyctiWqMfrSwg/how-to-access-multiple-roles-from-single-iam-user-simultaneously

*Technically, you can assume multiple IAM roles at the same time but the permissions will not be aggregated. Assuming an IAM role doesn't change who you are or what permissions you have.*

*When you assume a role, you are given a new set of temporary credentials to use, instead of "your" credentials -- the credentials you used to assume the role.*

*When interacting with AWS resources each request can only be associated with a singular principal. So, if you assume role1 and role2 you can make requests as role1 OR role2 but not as both together. So, if you are trying to perform a single action that requires an aggregate of the permissions of multiple roles, that's a not possible.*

https://stackoverflow.com/questions/48876077/assume-multiple-aws-iam-roles-are-a-single-time#:~:text=1%20Answer&text=Technically%2C%20yes%2C%20there%20is%20a,to%20assume%20a%20different%20identity.

## IAM PassRole vs IAM AssumeRole

https://demacia.medium.com/difference-between-iam-passrole-and-iam-assumerole-en-id-3cb1ffd71a36

# IAM user groups

An IAM group is a collection of users. All users in the group inherit the permissions assigned to the group. 
**Permissions can be assigned to user groups but not IAM roles.**

# IAM Access Analyzer and Access Advisor

https://docs.aws.amazon.com/IAM/latest/UserGuide/what-is-access-analyzer.html

*"IAM Access Analyzer helps you identify the resources in your organization and accounts, such as Amazon S3 buckets or IAM roles, shared with an external entity. This lets you identify unintended access to your resources and data, which is a security risk."*

**Access Analyzer** gives some visibility into existing external access but does not offer any insight into if the permissions are excessive and how to remediate the risk if so. Another AWS tool, **Access Advisor, analyzes usage of access permissions to services by IAM objects such as users, groups, roles and policies**.

# IAM Policy

In order to talk about IAM policies, you first need to cover the three main pieces of logic that define what is in the policy and how the policy actually works. These pieces make up the **request context that is authenticated by IAM and authorized accordingly**. You can think of the **principal, action, and resource** as the subject, verb, and object of a sentence, respectively.

* Principal: User, role, external user, or application that sent the request and the policies associated with that principal
* Action: What the principal is attempting to do
* Resource: AWS resource object upon which the actions or operations are performed

## Policy: AWS managed, Customer managed, Inline

* **AWS managed**: AWS manages and creates these types of policies. They can be attached to multiple users, groups, and roles. If you are new to using policies, AWS recommends that you start by using AWS managed policies.
* **Customer managed**: These are policies that you create and manage in your AWS account. This type of policy provides more precise control than AWS managed policies and can also be attached to multiple users, groups, and roles. 
* **Inline**: Inline policies are embedded directly into a single user, group, or role. In most cases, AWS doesn’t recommend using inline policies. This type of policy is useful if you want to maintain a strict one-to-one relationship between a policy and the principal entity that it's applied to. **For example, use this type of policy if you want to be sure that the permissions in a policy are not inadvertently assigned to a principal entity other than the one they're intended for.** 

## Access Policies Schema

https://docs.aws.amazon.com/IAM/latest/UserGuide/access_policies.html#access_policies-json

IAM polices can be applied to AWS identities:
* users
* groups
* roles

* **Effect, Action are required fields.**

Sample policies:

```json
{
  "Version": "2012-10-17",
  "Statement": [
    {
      "Sid": "FirstStatement",
      "Effect": "Allow",
      "Action": ["iam:ChangePassword"],
      "Resource": "*"
    }
  ]
}
```
```json
{
  "Version": "2012-10-17",
  "Statement": [
    {
      "Sid": "DenyS3AccessOutsideMyBoundary",
      "Effect": "Deny",
      "Action": [
        "s3:*"
      ],
      "Resource": "[arn:aws:s3:::myBucket/*]",
      "Condition": {
        "StringNotEquals": {
          "aws:ResourceAccount": [
            "222222222222"
          ]
        }
      }
    }
  ]
}
```

* **Version**: The Version element defines the version of the policy language. It specifies the language syntax rules that are needed by AWS to process a policy. To use all the available policy features, include "Version": "2012-10-17" before the "Statement" element in your policies.

* **Sid**: it is optional and provides a brief description of the policy statement

* **Effect**: The Effect element specifies whether the policy will allow or deny access. In this policy, the Effect is "Allow", which means you’re providing access to a particular resource.

* The **Action** element describes the type of action that should be allowed or denied. In the example policy, the action is "*". This is called a wildcard, and it is used to symbolize every action inside your AWS account.

* The **Resource** element specifies the object or objects that the policy statement covers. In the policy example, the resource is the wildcard "*". This represents all resources inside your AWS console.

* **Condition**: IAM allows you to add conditions to your policy statements. The Condition element is optional and lets you specify conditions for when a policy is in effect. `"Condition" : { "{condition-operator}" : { "{condition-key}" : "{condition-value}" }}`. You can have multiple conditions in a single policy, **which are evaluated using a logical AND.**

## Policy types

* **Identity-based**: Also known as IAM policies, identity-based policies are **managed and inline policies** attached to IAM identities (users, groups to which users belong, or roles). Impacts IAM principal permissions.

* **Resource-based**: These are **inline policies** that are attached to AWS resources. The most common examples of   resource-based policies are Amazon S3 bucket policies and IAM role trust policies. Resource-based policies grant permissions to the principal that is specified in the policy; **hence, the principal policy element is required.**. Grants permission to principals or accounts (same or different accounts).

  The resource-based policy below is attached to an Amazon S3 bucket. According to the policy, only the IAM user carlossalzar can access this bucket.
  
  ![25_resource-based-policy.png](./images/25_resource-based-policy.png)

* **Permissions boundaries**: A permissions boundary sets the maximum permissions that an identity-based policy can grant to an **IAM user or role** (permissions boundaries cannot be assigned to user group). The entity can perform only the actions that are allowed by both its identity-based policies and its permissions boundaries. Permissions boundaries do not grant permissions! **Resource-based policies that specify the user or role as the principal are not limited by the permissions boundary.** 

  For example, assume that one of your IAM users should be allowed to manage only Amazon S3, Amazon CloudWatch, and Amazon EC2. To enforce this rule, you can use the customer-managed policy enclosed in the square to set the permissions boundary for the user. Then, add the condition block below to the IAM user's policy. The user can never perform operations in any other service, including IAM, even if it has a permissions policy that allows it.

  ![26_permission-boundaries.png](./images/26_permission-boundaries.png)

  https://docs.aws.amazon.com/IAM/latest/UserGuide/access_policies_boundaries.html   
  https://www.youtube.com/watch?v=t8P8ffqWrsY

* **AWS Organizations SCPs**: AWS Organizations is a service for grouping and centrally managing AWS accounts. If you enable all features in an organization, then you can apply SCPs to any or all of your accounts. SCPs specify the maximum permissions for an account, or a group of accounts, called an organizational unit (OU). 

  ![27_aws_organizations.png](./images/27_aws_organizations.png)
 
* **ACLs**: Use ACLs to control which principals in other accounts can access the resource to which the ACL is attached.  **ACLs are supported by Amazon S3 buckets and objects.** They are similar to resource-based policies although they are the only policy type that does not use the JSON policy document structure. ACLs are cross-account permissions policies that grant permissions to the specified principal. **ACLs cannot grant permissions to entities within the same account.**

* **Session policies**: **A session policy is an inline permissions policy that users pass in the session when they assume the role**. The permissions for a session are the intersection of the identity-based policies for the IAM entity (user or role) used to create the session and the session policies. Permissions can also come from a resource-based policy. **Session policies limit the permissions that the role or user's identity-based policies grant to the session.**

### Guardrails vs. grants

Some policies are used to restrict permissions while others are used to grant access. Using a combination of different policy types not only improves your overall security posture but also minimizes your blast radius in case an incident occurs.

![28_gg.png](./images/28_gg.png)

## Principal types

Principal is a person, role, or application that can make a request for an action or operation on an AWS resource.

* **AWS account**: When you use an AWS account identifier as the principal in a policy, you delegate authority to the account. Within that account, the permissions in the policy statement can be granted to all identities, including IAM users and roles in that account. When you specify an AWS account, you can use the Amazon Resource Name (ARN) arn:aws:iam::AWS-account-ID:root or a shortened form that consists of the aws: prefix followed by the account ID.

  For example, given an account ID of 123456789012, you can use either one of the following methods to specify that account in the Principal element.

  ![30_principal.png](./images/30_principal.png)

* **IAM user**: You can specify an individual IAM user (or array of users) as the principal, as in the following examples. When you specify more than one principal in the element, **you grant permissions to each principal. This is a logical OR** and not a logical AND because you are authenticated as one principal at a time. In a Principal element, the user name is case-sensitive. When you specify users in a Principal element, **you cannot use a wildcard (*) to mean "all users."** Principals must always name a specific user or users.

  ![31_principal.png](./images/31_principal.png)

* **Federated user**: If you already manage user identities outside AWS, you can use IAM identity providers instead of creating IAM users in your AWS account. With an identity provider (IdP), you can manage your user identities outside AWS and give these external user identities permissions to use AWS resources in your account. IAM supports SAML-based IdPs and web identity providers, such as Login with Amazon, Amazon Cognito, Facebook, or Google. 

  Here are examples of a Principal element used for federated web identity users and for federated SAML users.

  ![32_principal.png](./images/32_principal.png)

* **IAM role**: You can use roles to delegate access to users, applications, or services that don't normally have access to your AWS resources. For example, you might want to grant users in your AWS account access to resources they don't usually have or grant users in one AWS account access to resources in another account.  The entity that assumes the role will lose its original privileges and gain the access associated with the role.

  ![33_principal.png](./images/33_principal.png)

* **AWS Service**: IAM roles that can be assumed by an AWS service are called service roles. **Service roles must include a trust policy, which are resource-based policies that are attached to a role that define which principals can assume the role.** Some service roles have predefined trust policies. However, in some cases, you must specify the service principal in the trust policy. A service principal is an identifier that is used to grant permissions to a service.

  The identifier includes the long version of a service name and is usually in the long_service-name.amazonaws.com format. The following example shows a policy that can be attached to a service role. The policy enables two services—Amazon EMR and AWS Data Pipeline—to assume the role. 

  ![34_principal.png](./images/34_principal.png)

## Analyzing the authorization context

In summary, when a principal tries to use the AWS Management Console, the AWS API, or the AWS CLI, that principal sends a request to AWS. AWS gathers the request information into a request context, which is used to evaluate and authorize the request. During authorization, AWS uses values from the request context to check for policies that apply to the request. It then uses the policies to determine whether to allow or deny the request.

* Example 1: access denied because requested action **iam:GetUser** does not match to the action from the policy **"dynamodb:ListTables"**
  ![35_authorization_context.png](./images/35_authorization_context.png)

* Example 2: access denied because the request tries to access resource **arn:aws:iam::123456789012:user/Carol** but policy says that only resource **arn:aws:iam::123456789012:user/Bob** can be accessed
  ![36_authorization_context.png](./images/36_authorization_context.png)

* Example 3: access granted, request matches policy including the same tag value for tags **aws:ResourceTag/project** and **aws:PrincipalTag/project**
  ![37_authorization_context.png](./images/37_authorization_context.png)

## Attributes and Tagging

Attribute-based access control (ABAC) is an authorization strategy that defines permissions based on attributes. In AWS, these attributes are called tags. Tags can be attached to IAM principals (users or roles) and to AWS resources. 

You can create a single ABAC policy or small set of policies for your IAM principals. These ABAC policies can be designed to allow operations when the principal's tag matches the resource tag. ABAC is helpful in environments that are growing rapidly and helps with situations where policy management becomes cumbersome.

### Benefits of the ABAC method

* **Scalable**: Teams change and grow quickly. It is no longer necessary for an administrator to update existing policies to allow access to new resources because permissions for new resources are automatically granted based on attributes. 

* **Manageable**: Because you don't have to create different policies for different job functions, you create fewer policies. Those policies are easier to manage.

* **Granular permissions**: When you create policies, it's a best practice to grant least privilege. Using traditional RBAC, you must write a policy that allows access to only specific resources. However, when you use ABAC, you can allow actions on all resources but only if the resource tag matches the principal's tag.

## IAM Condition Keys

The `Condition` element in a policy lets you indicate the circumstances for when a policy is in effect. You can use the `Condition` element to compare keys in the request context with key values that you specify in your policy. This gives you granular control over when your JSON policy statements match or don't match an incoming request.

The condition key that you specify can be a service-specific or a global condition key. Service-specific condition keys have the service's prefix. For example, Amazon EC2 lets you write a condition using the `ec2:InstanceType` key, which is unique to that service.

Below you can read about different service-specific keys for IAM:   

https://docs.aws.amazon.com/IAM/latest/UserGuide/reference_policies_elements_condition_operators.html#Conditions_String

* **iam:AWSService**: This condition key is used to control access for a specific service role. Many AWS services require that you use roles to allow the service to access resources in other services on your behalf. A role that a service assumes to perform actions on your behalf is called a service role. Here you are able to specify the service to which the permissions in the policy applies.

  To allow an IAM entity to create a specific service role, add the following policy to the IAM entity that needs to create the service role. This policy allows you to create a service role for the specified service and with a specific name. You can then attach managed or inline policies to that role.

  ![38_iam_condition.png](./images/38_iam_condition.png)

* **iam:OrganizationsPolicyId**: For those accounts that are members of an AWS Organizations unit, this condition key provides the IAM entity access to specific SCPs. The example shown here is an IAM policy that allows viewing service last accessed information for SCP with the p-policy123 ID. This policy also allows the requester to retrieve the data for any Organizations entity in their organization.

  ![39_iam_condition.png](./images/39_iam_condition.png)

* **iam:PermissionsBoundary**: The `iam:PermissionsBoundary` key checks that the specified policy is attached as a permissions boundary on the IAM principal resource.

* **iam:PolicyARN**: This condition key checks the Amazon Resource Name (ARN) of a managed policy in requests that involve that same managed policy. You use this key to control how users can apply AWS managed and customer managed policies. For example, you might create a policy that allows users to attach only the IAMUserChangePassword AWS managed policy to a new IAM user, group, or role.

  In the below policy, the iam:PolicyARN condition ensures that permissions are allowed only when the policy being attached matches the AWS managed policy in the condition. Here, the user is allowed to attach policies to only the groups and roles that include the path /TEAM-A/.

  ![40_iam_condition.png](./images/40_iam_condition.png)

* **iam:ResourceTag**: The iam:ResourceTag condition key checks that the tag attached to the identity resource, either a user or role, matches the specified key name and value provided. You can add custom attributes to a user or role in the form of a key-value pair, such as environment=prod where "environment" is the key and "prod" is the value. An upcoming lesson will talk more about using tags.

  The example shows how you might create a policy that allows deleting users with only the `status=terminated` tag.

  ![41_iam_condition.png](./images/41_iam_condition.png)

# Explicit and implicit denies

A request results in an explicit deny if an applicable policy includes a Deny statement. If policies that apply to a request include an Allow statement and a Deny statement, the Deny statement trumps the Allow statement. The request is explicitly denied.

An implicit denial occurs when there is no applicable Deny statement but also no applicable Allow statement. Because an IAM user, role, or federated user is denied access by default, they must be explicitly allowed to perform an action. Otherwise, they are implicitly denied access.

The flow chart below provides details about how the decision is made as AWS authenticates the principal that makes the request. AWS evaluates the policy types in the following order:

![29_graph.png](./images/29_graph.png)

# Types of AWS credentials

* Username and password   
  **A password policy** is a set of rules that define the type of password an IAM user can set. You should define a password policy for all of your IAM users to enforce strong passwords and to require your users to regularly change their passwords. Password requirements are similar to those found in most secure online environments.

  ![21_password_policy.png](./images/21_password_policy.png)

* Multi-factor authentication   
  Multi-factor authentication (MFA) is an additional layer of security for accessing AWS services. With this authentication method, more than one authentication factor is checked before access is granted, **which consists of a user name, a password, and the single-use code from the MFA device**.

  ![22_mfa.png](./images/22_mfa.png)

* User access keys   
  Users need their own access keys to make programmatic calls to AWS using the AWS CLI or the AWS SDKs, or direct HTTPS calls using the APIs for individual AWS services. Access keys are used to digitally sign API calls made to AWS services. Each access key credential consists of an access key ID and a secret key. **Each user can have two active access keys, which is useful when you need to rotate the user's access keys or revoke permissions.** 

  ![23_access_keys.png](./images/23_access_keys.png)

# Managing server certificates in IAM

https://docs.aws.amazon.com/IAM/latest/UserGuide/id_credentials_server-certs.html

Use IAM as a certificate manager only when you must support HTTPS connections in a Region that is not supported by ACM. IAM securely encrypts your private keys and stores the encrypted version in IAM SSL certificate storage. IAM supports deploying server certificates in all Regions, but you must obtain your certificate from an external provider for use with AWS. You cannot upload an ACM certificate to IAM. Additionally, you cannot manage your certificates from the IAM Console.

# Links
https://aws-blog.de/2021/08/iam-what-happens-when-you-assume-a-role.html   
https://docs.aws.amazon.com/cli/latest/userguide/cli-configure-role.html