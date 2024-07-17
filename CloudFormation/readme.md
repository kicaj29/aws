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