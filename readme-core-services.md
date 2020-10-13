- [Interacting with AWS](#interacting-with-aws)
  - [AWS Console](#aws-console)
  - [AWS CLI](#aws-cli)
  - [AWS SDK](#aws-sdk)
- [EC2 (Elastic Compute)](#ec2-elastic-compute)
  - [EC2 instance types](#ec2-instance-types)
  - [Root device type](#root-device-type)
  - [Amazon Machine Image (AMI)](#amazon-machine-image-ami)
  - [Amazon EC2 purchase options](#amazon-ec2-purchase-options)
  - [Launching EC2 instances](#launching-ec2-instances)
- [AWS Elastic Beanstalk](#aws-elastic-beanstalk)
  - [Launching an App on Elastic Beanstalk](#launching-an-app-on-elastic-beanstalk)
- [AWS Lambda](#aws-lambda)
- [Content and Network Delivery Services](#content-and-network-delivery-services)
  - [Amazon VPS](#amazon-vps)
  - [Direct Connect](#direct-connect)
  - [Amazon Route 53](#amazon-route-53)
  - [Elastic load balancing](#elastic-load-balancing)
    - [Scaling on Amazon EC2](#scaling-on-amazon-ec2)
  - [Amazon CloudFront](#amazon-cloudfront)
  - [API Gateway](#api-gateway)
- [File Storage Services](#file-storage-services)
  - [S3](#s3)
  - [S3 Non-archival Storage Classes](#s3-non-archival-storage-classes)
  - [S3 Lifecycle policies](#s3-lifecycle-policies)
  - [S3 Transfer Acceleration](#s3-transfer-acceleration)
  - [Hosting static web site on S3](#hosting-static-web-site-on-s3)
  - [S3 Glacier](#s3-glacier)
  - [Elastic Block Store](#elastic-block-store)
  - [Elastic File System](#elastic-file-system)
  - [Snowball](#snowball)
  - [Snowmobile](#snowmobile)
- [resources](#resources)

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

# EC2 (Elastic Compute)

Amazon EC2 is a web service that provides resizable compute capacity in the cloud. It is designed to make web-scale computing easier for developers.

>NOTE: https://www.quora.com/Is-Amazon-EC2-IaaS-or-PaaS "Having said that, EC2 is IaaS and is probably the only AWS offering that falls into the IaaS category. Using EC2, AWS users can provision compute, networking and storage just by calling various APIs. Users can access the EC2 instances and their volumes as if they were provisioned in a non-cloud environment."


Can be used to:
* Web application hosting
* Batch processing
* Web services endpoint
* Desktop in the cloud

## EC2 instance types

* defines the processor, memory, and storage type
* cannot be changed without downtime
* provided in the following categories
  * general purpose
  * compute, memory and storage optimized
  * accelerated computing (for example machine learning)
* pricing is based on instance type

For example at 2020, they can be also different in different 
regions:
![aws-02-instance-type-pricing.png](images/core-services/aws-02-instance-type-pricing.png)


## Root device type

* Instance store: ephemeral storage that is physically attached to the host the virtual server is running on
* Elastic Block Store (EBS): persistent storage that exists separately from the host the virtual server is running.

Instance store - if you shout down your server all data will go away, EBS - if you shout down your server all data is still there.

## Amazon Machine Image (AMI)

**Template for an EC2 instance including configuration, operating system and data.**  

AWS provides many AMI`s that can be leveraged.

Custom AMI`s can be created based on your configuration.   

Commercial AMI`s are available in the AWS Marketplace.

## Amazon EC2 purchase options

* On-Demand: you pay the second for the instances that are launched.
* Reserved: you purchase at a discount for instances in advanced for 1-3 years.
  * All Upfront: entire cost for the 1 or 3 year period is paid upfront. It gives **maximum savings**.
  * Partial Upfront: part of 1 or 3 year cost is paid upfront along with a reduced monthly cost
  * No Upfront: no upfront payment is made but there will be a reduced monthly cost. It gives **minimum upfront cost**.
* Spot: you can leverage unused EC2 capacity in a region for a large discount.
  * Can provide up to 90% discount over on-demand pricing.
  * When you request instances, if your bid is higher than Spot price they will launch.
  * If the Spot price grows to exceed your bid, the instances will be terminated
  * Spot instances can be notified 2 minutes prior to termination

![aws-03-purchase-options.png](images/core-services/aws-03-purchase-options.png)

## Launching EC2 instances

![aws-04-ec2-launch.png](images/core-services/aws-04-ec2-launch.png)

![aws-05-ec2-launch.png](images/core-services/aws-05-ec2-launch.png)

![aws-06-ec2-launch.png](images/core-services/aws-06-ec2-launch.png) (click on the bottom *Next: Configure Instance Details*)

![aws-07-ec2-launch.png](images/core-services/aws-07-ec2-launch.png)

![aws-08-ec2-launch.png](images/core-services/aws-08-ec2-launch.png)

![aws-09-ec2-launch.png](images/core-services/aws-09-ec2-launch.png)

![aws-10-ec2-launch.png](images/core-services/aws-10-ec2-launch.png)

![aws-11-ec2-launch.png](images/core-services/aws-11-ec2-launch.png)

![aws-12-ec2-launch.png](images/core-services/aws-12-ec2-launch.png)

![aws-13-ec2-launch.png](images/core-services/aws-13-ec2-launch.png)

![aws-14-ec2-launch.png](images/core-services/aws-14-ec2-launch.png)

![aws-15-ec2-launch.png](images/core-services/aws-15-ec2-launch.png)

![aws-16-ec2-launch.png](images/core-services/aws-16-ec2-launch.png)

![aws-17-ec2-launch.png](images/core-services/aws-17-ec2-launch.png)

# AWS Elastic Beanstalk

* Automates the process of deploying and scaling workloads on EC2 **(PaaS)**
* Supports a specific set of technologies
* Leverages existing AWS services
* Only pay for the other services you leverage

Under the hood is used EC2 but Elastic Beanstalk manages provisioning, load balancing, scaling and monitoring.

Supported Application Platforms: Java, .NET, PHP, Node.js, Python, Ruby, Go, Docker.

Elastic Beanstalk Features:
* Monitoring
* Deployment
* Scaling
* EC2 Customization
  
Use Cases:
* Deploy an application with minimal knowledge of other services
* Reduce the overall maintenance needed for the application
* Few EC2 customizations are required

## Launching an App on Elastic Beanstalk

From https://docs.aws.amazon.com/elasticbeanstalk/latest/dg/tutorials.html download sample nodejs application.

![aws-18-elastic-beanstalk-launch.png](images/core-services/aws-18-elastic-beanstalk-launch.png)

![aws-19-elastic-beanstalk-launch.png](images/core-services/aws-19-elastic-beanstalk-launch.png)

![aws-20-elastic-beanstalk-launch.png](images/core-services/aws-20-elastic-beanstalk-launch.png)

![aws-21-elastic-beanstalk-launch.png](images/core-services/aws-21-elastic-beanstalk-launch.png)

![aws-22-elastic-beanstalk-launch.png](images/core-services/aws-22-elastic-beanstalk-launch.png)

![aws-23-elastic-beanstalk-launch.png](images/core-services/aws-23-elastic-beanstalk-launch.png)

![aws-24-elastic-beanstalk-launch.png](images/core-services/aws-24-elastic-beanstalk-launch.png)

![aws-25-elastic-beanstalk-launch.png](images/core-services/aws-25-elastic-beanstalk-launch.png)

![aws-26-elastic-beanstalk-launch.png](images/core-services/aws-26-elastic-beanstalk-launch.png)

![aws-27-elastic-beanstalk-launch.png](images/core-services/aws-27-elastic-beanstalk-launch.png)

# AWS Lambda

AWS Lambda lets you run code without provisioning or managing servers. You pay only for the compute time you consume. You can run code for virtually any type of application or backend service - all with zero administration.

* Enables the running of code **without provisioning infrastructure**.
* Can configure available memory from 128 MB to 3008 MB.   
* It is integrated with many AWS services.
* Enables event-driven workflows.
* Primary service for serverless architecture.
  

Advantages:
* Reduced maintenance requirements
* Enables fault tolerance without additional work - **multiple availability zones is done automatically**
* Scales based on demand
* Pricing is based on usage

# Content and Network Delivery Services

## Amazon VPS
Amazon Virtual Private Cloud (VPC) - is logically isolated section of the AWS Cloud where you can launch AWS resources in a virtual network that you define.

* it supports public and private subnets
* can utilize NA|T for private subnets
* enabled a connection to your data center
* can connect to other VPC
* supports private connections to many AWS services

## Direct Connect
A cloud service solution that makes it easy to establish a dedicated network connection from your data center to AWS.
It means that the traffic does not have to go through the public Internet.

## Amazon Route 53

* DNS: DNS changes are not instantaneous. Changes have to be propagated to all server over the globe and it can take even couple of hours. It manages public DNS records.
* Global AWS service (not regional) - all changes are saved globally
* HA: we can route users to different region in main region is not available
* enables global resource routing: send request to specific server based on what country they coming from or send request to the server that responds the fastest.

## Elastic load balancing

Elasticity: the ability for the infrastructure supporting an application to grow and contract based on how much it is used at a point in time.

* distributes traffic across multiple targets
* integrated with EC2, ECS and Lambda
* **supports one or more AZ`s (Availability Zones) in a region**
* types:
  * application load balancer (ALB)
  * network load balancer (NLB)
  * classic load balancer

### Scaling on Amazon EC2
* vertical - scale up (e.g. adding faster CPU), system has to be shutted down to do the change.
* horizontal - scale out (e.g. adding more the same CPUs), system does not have to be shutted down to do the change. 

## Amazon CloudFront
* CDN
* enables users to get content from the server closest to them
* supports static and dynamic content
* utilizes AWS edge locations
* includes advanced security features
  * AWS Shield for DDoS
  * AWS WAF

## API Gateway
* fully managed API management service
* directly integrates with multiple AWS services
* provides monitoring & metrics on API calls
* supports VPC and on-premise private applications

# File Storage Services

## S3
* store files as objects in buckets. Bucket is unit of organization in S3.
* provides different storage classes for different use cases.
* stores data across multiple availability zones
* enabled URL access for files
* configurable rules for data lifecycle
* can serve as a static website host

## S3 Non-archival Storage Classes
* S3 Standard: default storage class and is for frequently accessed data
* S3 Intelligent-Tiering: automatically will move your data to the correct storage class based on usage
  * moves between frequent and infrequent access
  * same performance as S3-Standard
* S3 Standard-IA (infrequent access):  infrequently accessed data with the standard resilience
* S3 One Zone-IA is for infrequently access data that is only stored in one AZ

## S3 Lifecycle policies
* objects in a bucket can transition or expire based on your criteria
* transitions can enable objects to move to another storage class **based on time** (but you cannot move something based on usage - it is only available in *S3 Intelligent-Tiering*)
* expiration can delete object based on age
* policies can also factor in versions of a specific object in the bucket (for example delete all versions that are not current version and are older then 7 days)

## S3 Transfer Acceleration
Feature the can be enabled per bucket that allows for optimized uploading of data using AWS Edge Locations as a part of Amazon CloudFront.

## Hosting static web site on S3

![aws-28-s3-hosting-website.png](images/core-services/aws-28-s3-hosting-website.png)
![aws-29-s3-hosting-website.png](images/core-services/aws-29-s3-hosting-website.png)
![aws-30-s3-hosting-website.png](images/core-services/aws-30-s3-hosting-website.png)
![aws-31-s3-hosting-website.png](images/core-services/aws-31-s3-hosting-website.png)
![aws-32-s3-hosting-website.png](images/core-services/aws-32-s3-hosting-website.png)
Upload sample index.html file:
![aws-33-s3-hosting-website.png](images/core-services/aws-33-s3-hosting-website.png)
![aws-34-s3-hosting-website.png](images/core-services/aws-34-s3-hosting-website.png)
![aws-35-s3-hosting-website.png](images/core-services/aws-35-s3-hosting-website.png)
Use encryption to make sure that during uploading to S3 data is encrypted:
![aws-36-s3-hosting-website.png](images/core-services/aws-36-s3-hosting-website.png)
![aws-37-s3-hosting-website.png](images/core-services/aws-37-s3-hosting-website.png)
![aws-38-s3-hosting-website.png](images/core-services/aws-38-s3-hosting-website.png)
Try to open logo.png, it will fail. We have to configure permissions.
![aws-39-s3-hosting-website.png](images/core-services/aws-39-s3-hosting-website.png)
![aws-40-s3-hosting-website.png](images/core-services/aws-40-s3-hosting-website.png)
![aws-41-s3-hosting-website.png](images/core-services/aws-41-s3-hosting-website.png)
![aws-42-s3-hosting-website.png](images/core-services/aws-42-s3-hosting-website.png)
![aws-43-s3-hosting-website.png](images/core-services/aws-43-s3-hosting-website.png)
![aws-44-s3-hosting-website.png](images/core-services/aws-44-s3-hosting-website.png)
Add missing permissions for index.html:
![aws-45-s3-hosting-website.png](images/core-services/aws-45-s3-hosting-website.png)
![aws-46-s3-hosting-website.png](images/core-services/aws-46-s3-hosting-website.png)

## S3 Glacier
## Elastic Block Store
## Elastic File System
## Snowball
## Snowmobile

# resources
https://app.pluralsight.com/library/courses/understanding-aws-core-services/table-of-contents