# CloudFormation

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

# AWS Cloud Development Kit (CDK)

* Define your cloud infra. using a familiar language
  * JS/TS, Python, Java and .NET
* The code is "compiled" into a CloudFormation template (JSON/YAML)
* You can therefor deploy infra and application runtime code together
  * Great for Lambda Functions
  * Great for Docker containers in ECS/EKS

![02-cloud-formation-cdk.png](./images/02-cloud-formation-cdk.png)

# Elastic Beanstalk

## Typical architecture

![03-web-app-3-tier.png](./images/03-web-app-3-tier.png)

* Developer problems on AWS
  * Manage infra
  * Deploying code
  * Configuring all the databases, load balancers erc.
  * Scaling concerns

* Most web apps have the same architecture (ALB + ASG)
* All devs want is for their code to tun!
* Possibly, consistently across different applications and envs.

## Beanstalk Overview

* Elastic Beanstalk is a developer centric view of deploying an application to AWS
* It uses many components: EC2, ASG, ELB, RDS, etc...
* But it is all on one view that is easy to make sense of
* We still have full control over the configuration
* **It is PaaS**
* Beanstalk is free but you pay for the underlying instances
* Managed service
  * Instances configuration / OS is handled by Beanstalk
  * Deployment strategy is configurable but performed by Elastic Beanstalk
  * Capacity provisioning
  * Load balancing and auto-scaling
  * Application health-monitor and responsiveness
* Supports 3 architecture models
  * Single Instance deployment: good for dev
  * LB + ASG: great for production or pre-production web applications
  * ASG only: great for non-web apps in production (workers, etc...)
* Supports: Go, Java SE, .NET, Docker etc... Can also write custom platform
* Beanstalk uses **CloudFormation**
* Health Monitoring

![04-beanstalk.png](./images/04-beanstalk.png)

# AWS CodeDeploy

* We want to deploy our application
* Works with EC2 instances
* Works with On-Premises Servers
* Servers/Instances must be provisioned and configured ahead of time with the CodeDeploy Agent

![05-code-deploy.png](./images/05-code-deploy.png)

# AWS CodeCommit

* AWS competing product for GitHub is CodeCommit
* It hosts Git-based repositories
* Fully managed
* Scalable & high available
* Private, Secured, Integrated with AWS 

# AWS CodeBuild

* Code building service in the cloud (name is obvious)
* Compiles source code, run tests, and produces packages that are ready to be deployed (by CodeDeploy for example)

![06-code-build.png](./images/06-code-build.png)

* Benefits
  * Fully managed, serverless
  * Continuously scalable & high available
  * Secure
  * Pay-as-you-go pricing - only pay for the build time

# AWS CodePipeline

* Orchestrate the different steps to have the code automatically pushed to production
  * Code => Build => Test => Provision => Deploy
  * Basics for CICD
* Benefits
  * Fully managed, compatible with CodeCommit, CodeBuild, CodeDeploy, Elastic Beanstalk
  * Fast delivery & rapid updates

![07-code-pipeline.png](./images/07-code-pipeline.png)

# AWS CodeArtifact

* Software packages depend on each other to be built (also called code dependencies), and new onces are created
* Storing and retrieving these dependencies is called artifact management
* Traditional you need to setup your own artifact management system
* **CodeArtifact** is a secure, scalable, and code-effective artifact management for software development
* Works with common deps tools such as Maven, npm, yarn, pip, NuGet
* Developers and CodeBuild can then retrieve deps straight from CodeArtifact

# AWS CodeStar

* Unified UI to easily manage software development activities in one place

![08-code-star.png](./images/08-code-star.png)

* "Quick way" to get started to correctly set-up CodeCommit, CodePipeline, CodeBuild, CodeDeploy, Elastic Beanstalk, EC2, etc...
* Can edit the code "in-the-cloud" using **AWS Cloud9**

# AWS Cloud9

* It is cloud IDE for writing, running and debugging code
* "Classic" IDE are downloaded on a computer before being used
* A cloud IDE can be used within a web browser, meaning you can work on your projects with no setup necessary
* Allows for code collaboration in real-time (pair-programming)

![09-code-star.png](./images/09-code-star.png)


# CodeStart & Cloud9 Hands On

