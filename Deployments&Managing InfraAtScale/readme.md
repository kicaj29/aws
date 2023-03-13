- [CloudFormation](#cloudformation)
- [AWS Cloud Development Kit (CDK)](#aws-cloud-development-kit-cdk)
- [Elastic Beanstalk](#elastic-beanstalk)
  - [Typical architecture](#typical-architecture)
  - [Beanstalk Overview](#beanstalk-overview)
- [AWS CodeDeploy](#aws-codedeploy)
- [AWS CodeCommit](#aws-codecommit)
- [AWS CodeBuild](#aws-codebuild)
- [AWS CodePipeline](#aws-codepipeline)
- [AWS CodeArtifact](#aws-codeartifact)
- [AWS CodeStar](#aws-codestar)
- [AWS Cloud9](#aws-cloud9)
- [CodeStart \& Cloud9 Hands On](#codestart--cloud9-hands-on)
- [Systems Manager (SSM)](#systems-manager-ssm)
  - [How Systems Manager work](#how-systems-manager-work)
  - [SSM Session Manager](#ssm-session-manager)
    - [SSM Session Manger Hands On](#ssm-session-manger-hands-on)
- [OpsWorks Overview](#opsworks-overview)
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
* **Works with On-Premises Servers**
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

* Developer tools: please where all developer tools are available but we have to create all of them
![10-code-hands-on.png](./images/10-code-hands-on.png)

* CodeStart creates everything automatically

![11-code-hands-on.png](./images/11-code-hands-on.png)

![12-code-hands-on.png](./images/12-code-hands-on.png)

![13-code-hands-on.png](./images/13-code-hands-on.png)

![14-code-hands-on.png](./images/14-code-hands-on.png)

![15-code-hands-on.png](./images/15-code-hands-on.png)

![16-code-hands-on.png](./images/16-code-hands-on.png)

![17-code-hands-on.png](./images/17-code-hands-on.png)

* After creation we can see that a lot of resources have been created automatically

![18-code-hands-on.png](./images/18-code-hands-on.png)

* Pipeline steps

![19-code-hands-on.png](./images/19-code-hands-on.png)

![20-code-hands-on.png](./images/20-code-hands-on.png)

![21-code-hands-on.png](./images/21-code-hands-on.png)

* If we go to the beanstalk we can  see created application and also there we can click the link to open the app

![22-code-hands-on.png](./images/22-code-hands-on.png)

* Next create Cloud9 environment

![23-code-hands-on.png](./images/23-code-hands-on.png)

![24-code-hands-on.png](./images/24-code-hands-on.png)

![25-code-hands-on.png](./images/25-code-hands-on.png)

* Next we can open Cloud9 IDE

![26-code-hands-on.png](./images/26-code-hands-on.png)

* Push the changes

![27-code-hands-on.png](./images/27-code-hands-on.png)

# Systems Manager (SSM)

* Helps you manage your EC2 and On-Premises systems at scale
* Another **Hybrid AWS service**
* Get operational insights about the state of your infra.
* Suit of 10+ products
* Most important features
  * Patching automation for enhanced compliance
  * Run commands across an entire fleet of servers
  * Store parameter configuration with the SSM Parameter Store
* Works for both Windows and Linux OS

## How Systems Manager work

* We need to install the SSM agent onto the systems we control
* Installed by default on Amazon Linux AMI & some Ubuntu AMIs
* If an instance cannot be controlled with SSM, it is probably an issue with the SSM agent!
* Thanks to the SSM agent, we can run commands, path & configure our servers

![28-ssm.png](./images/28-ssm.png)

## SSM Session Manager

* Allows you to start a secure shell on your EC2 and on-premises servers
* No SSH access, bastion hosts, or SSH keys needed
* No port 22 needed (better security)
* Supports Linux, macOS and Windows
* Send session log data to S3 or CloudWatch Logs

![29-ssm-session-manager.png](./images/29-ssm-session-manager.png)

### SSM Session Manger Hands On

* First we have to launch some EC2 instance

![30-ssm-session-manager-hands-on.png](./images/30-ssm-session-manager-hands-on.png)

* We will not use any key-pair

![31-ssm-session-manager-hands-on.png](./images/31-ssm-session-manager-hands-on.png)

* Also disable SSH traffic

![32-ssm-session-manager-hands-on.png](./images/32-ssm-session-manager-hands-on.png)

* Assign IAM role which can talk to the SSM service

![33-ssm-session-manager-hands-on.png](./images/33-ssm-session-manager-hands-on.png)

![34-ssm-session-manager-hands-on.png](./images/34-ssm-session-manager-hands-on.png)

* Next go to the SSM Service and select **Fleet Manager**

![35-ssm-session-manager-hands-on.png](./images/35-ssm-session-manager-hands-on.png)

Here all EC2 instances which are registered with SSM will appear here:

![36-ssm-session-manager-hands-on.png](./images/36-ssm-session-manager-hands-on.png)

* When the created EC2 instances will start it will appear on the list

![37-ssm-session-manager-hands-on.png](./images/37-ssm-session-manager-hands-on.png)

* Next we can run secure shell using **Session Manager**

![38-ssm-session-manager-hands-on.png](./images/38-ssm-session-manager-hands-on.png)

* Start the session

![39-ssm-session-manager-hands-on.png](./images/39-ssm-session-manager-hands-on.png)

![40-ssm-session-manager-hands-on.png](./images/40-ssm-session-manager-hands-on.png)

![41-ssm-session-manager-hands-on.png](./images/41-ssm-session-manager-hands-on.png)

* We can access also session history

![42-ssm-session-manager-hands-on.png](./images/42-ssm-session-manager-hands-on.png)

# OpsWorks Overview

* Chef & Puppet help you perform server configuration automatically, or repetitive actions.
  * These tools are not created by AWS
* They work great with EC2 & On-Premises VM
* AWS OpsWorks = Managed Chef & Puppet
* It is alternative to the AWS SSM
* Only provision on standard AWS resources
  * EC2, Databases, Load Balancers, EBS volumes
* **The only reason to use OpsWorks is that you were using Chef & Puppet before migration to the cloud and after migration
  you would like to still use Chef & Puppet to re-use existing code**

![43-opswork.png](./images/43-opswork.png)