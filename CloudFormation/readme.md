# Introduction

It is declarative way of outlining your AWS Infra, for any resources (most of them are supported).
CloudFormation creates those for you, in the right order, with the exact configuration that you specify.

* Infrastructure as code
* Cost
  * Each resources withing the stack is tagged with an identifier so you can easily see how much a stack costs you
  * You can estimate the costs of your resources using the CloudFromation template
  * Saving strategy, for example: In Dev, you could automation deletion of templates at 5 PM and recreated at 8 AM, safely
* Productivity
  * Ability to destroy and re-create as infra on the cloud on the fly
  * Automated generation of Diagram for your templates
  * Declarative programming (no need to figure out ordering and orchestration)
* Don`t re-invent the wheel
  * Leverage existing templates in the web
  * Leverage the documentation
* Supports (almost) all AWS resources
  * You can use "custom resources" for resources that are not supported

![01-cloud-formation-designer.png](./images/01-cloud-formation-designer.png)

# Information about your stack

If CloudFormation fails to create, update, or delete your stack, you can view error messages or logs to help you learn more about the issue. The following screenshots describe the types of information available for troubleshooting a CloudFormation issue.

## Stack info

The Stack info tab shows basic information about each stack: stack ID, status, description, and when it was last created, deleted, or updated.

![02-stack.png](./images/02-stack.png)

## Resources

The Resources tab shows information about all the resources grouped by this stack. You can view many of the resources through links to the AWS Management Console.

![02-resources.png](./images/02-resources.png)

## Events

The Events tab shows all the events that have been generated for the stack, with their timestamp. This is very useful when trying to figure out what exactly is happening if something goes wrong.

![03-events.png](./images/03-events.png)

## Other information

The AWS CloudFormation console will also show you other information about your stack. This includes stack outputs, the template that was used and its parameters, and any change sets.

## How can I obtain information about my stack using the AWS CLI?

The AWS CLI can show you all the stack information. The following commands are useful, and take a stack name as argument (--stack-name "...").

* `aws cloudformation describe-stacks`

  This AWS CLI command shows the basic information about the stack, similar to the Stack info tab in the AWS Management Console. For more information, check the [documentation](https://docs.aws.amazon.com/cli/latest/reference/cloudformation/describe-stacks.html).

  ![04-cli-stacks.png](./images/04-cli-stacks.png)

* `aws cloudformation describe-stack resources`

  This command shows information about the resources belonging to a stack, similar to the Resources tab in the AWS Management Console. For more information, check the [documentation](https://docs.aws.amazon.com/cli/latest/reference/cloudformation/describe-stack-resources.html).   

  ![05-cli-stack-resources.png](./images/05-cli-stack-resources.png)

* `aws cloudformation describe-stack-events`

  This command shows information about all the events generated for a stack, in reverse chronological order, similar to the Events tab in the AWS Management Console. For more information, check the [documentation](https://docs.aws.amazon.com/cli/latest/reference/cloudformation/describe-stack-events.html).

  ![06-cli-stack-events.png](./images/06-cli-stack-events.png)