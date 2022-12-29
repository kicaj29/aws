- [General Guiding Principles](#general-guiding-principles)
- [Design Principles](#design-principles)
- [Well architected framework 6 pillars](#well-architected-framework-6-pillars)
  - [1 Operational excellence](#1-operational-excellence)
  - [2 Security](#2-security)
  - [3 Reliability](#3-reliability)
  - [4 Performance efficiency](#4-performance-efficiency)
  - [5 Cost optimization](#5-cost-optimization)
  - [6 Sustainability](#6-sustainability)
- [AWS well-architected tool](#aws-well-architected-tool)
- [Right sizing](#right-sizing)
- [AWS Ecosystem - free resources](#aws-ecosystem---free-resources)
- [AWS Knowledge Center](#aws-knowledge-center)
- [AWS IQ](#aws-iq)
- [AWS re:Post](#aws-repost)

# General Guiding Principles

* Stop guessing your capacity needs (use auto scaling)
* Test systems at production scale
* Automate to make architectural experimentation easier (IaC)
* Allow for evolutionary architectures
  * Design based on changing requirements
* Drive architectures using data
* Improve through game days
  * Simulate applications for flash sale days

# Design Principles

* Scalability: vertical and horizontal
* Disposable Resources: servers should be disposable & easily configured
* Automation: Serverless, Infrastructure as a Service, Auto Scaling...
* Loos coupling
  * Monolith are applications that do more and more over time, become bigger
  * Break it down into smaller, loosely coupled components
  * A change or a failure in one component should not cascade to other components
* Services, not Server
  * Do not use just EC2
  * Use managed services, databases, serverless, etc!

# Well architected framework 6 pillars

## 1 Operational excellence

* Includes the ability to run and monitor systems to deliver business value and to continually improve supporting processes and procedures
* Design principles
  * Perform operations as code - IaC
  * Annotate documentation - automate the creation of annotated docs after every build
  * Make frequent, small, reversible changes - so that in case of any failure, you can reverse it
  * Refine operations procedures frequently - and ensure that team members are familiar with it
  * Anticipate failure
  * Learn from all operational failures

![01-operational-excellence.png](./images/01-operational-excellence.png)

## 2 Security

* Includes the ability to protect information, systems, and assets while delivering business value through risk assessments and mitigation strategies
* Design principles
  * Implement a string identity foundation - centralize privilege management and reduce (or even eliminate) reliance on long-term credentials - principle of least privilege - IAM
  * Enable traceability - integrated logs and metrics with systems to automatically respond and take action
  * Apply security at all layers - like edge network, VPC, subnet, load balancer, every instance, operating system, and application
  * Auto security best practices
  * Protect data in transit and at rest - encryption, tokenization, and access control
  * Keep people away from data - reduce or eliminate the need for direct access or manual processing of data
  * Prepare for security events - run incident response simulations and use tools with automation to increase your speed for detection, investigation, and recover

![02-security.png](./images/02-security.png)

## 3 Reliability

* Ability of a system to recover from infra. or service disruptions, dynamically acquire computing resources to meed demand, and mitigate disruptions such as misconfigurations or transient network issues
* Design Principles
  * Test recovery procedures - use automation to simulate different failures or to recreate scenarios that led to failures before
  * Automatically recover from failure - anticipate and remediate failures before they occur
  * Scale horizontally to increase aggregate system availability - distribute requests across multiple, smaller resources to ensure that they do not share a common point of failure
  * Stop guessing capacity - maintain the optimal level to satisfy demand without over or under provisioning - use auto scaling
  * Manage change in automation - use automation to make changes to infrastructure

![03-reliability.png](./images/03-reliability.png)

## 4 Performance efficiency

* Includes the ability to use computing resources efficiently to meet system requirements, and to maintain that efficiency as demand changes and technologies evolve
* Design Principles
  * **Democratize advanced technologies - advanced technologies become services and hence you can focus on product development**
  * Go global in minutes - easy deployment in multiple regions
  * User serverless architectures - avoid burden of managing servers
  * Experiment more often - easy to carry out comparative testing
  * Mechanical sympathy - be aware of all AWS services
  
![04-performance-efficiency.png](./images/04-performance-efficiency.png)

## 5 Cost optimization

* Includes the ability to run systems to deliver business value at the lowest price point
* Design principles
  * Adopt a consumption mode - pay only for what you use
  * Measure overall efficiency - use CloudWatch
  * Stop spending money on data center operations - AWS does the infra. part and enables customer to focus on organization projects
  * Analyze and attribute expenditure - accurate identification of system usage and costs, helps measure return on investment (ROI) - make sure to use tags
  * **Use managed and application level services to reduce costs of ownership** - as managed service operate at cloud scale, they can offer a lower cost per transaction or service

![05-performance-efficiency.png](./images/05-performance-efficiency.png)

## 6 Sustainability

* The sustainability pillar focuses in minimizing the environmental impacts of running cloud workloads
* Design principles
  * Understand your impact - establish performance indicators, evaluate improvements
  * Establish sustainability goals - set long term goals for each workload, model return on investment (ROI)
  * Maximize utilization - right size each workload to maximize the energy efficiency of the underlying hardware and minimize idle resources
  * Anticipate and adopt new, more efficient hardware and software offerings - and design for flexibility to adopt new technologies over time
  * Use managed services - shared services reduce the amount of infrastructure; managed services help automate sustainability best practices as moving infrequent accessed data to cold storage and adjusting compute capacity
  * Reduce the downstream impact of your cloud workloads - reduce the amount of energy or resources required to use your services and reduce the need for your customers to upgrade their devices
  * Sustainability AWS Services

![06-Sustainability.png](./images/06-Sustainability.png)

# AWS well-architected tool

Free tool to review your architectures against the 6 pillars well-architected framework and adopt architectural best practices

![07-tool.png](./images/07-tool.png)

# Right sizing

* EC2 has many instance types, but choosing the most powerful instance type is not the best choice, because the cloud is **elastic**
* Right sizing is the process of matching instance types and sizes to your workload performance and capacity requirements **at the lowest possible cost**
* Scaling up is easy so always start small
* It is also the process of looking at deployed instances and identifying opportunities to eliminate or downsize without compromising capacity or other requirements, which results in lower costs
* It is important to right size especially in 2 moments
  * before a cloud migration
  * continuously after the cloud onboarding process (requirements change over time)
* Tools which can help: CloudWatch, Cost Explorer, Trusted Advisor, 3rd party tools can help also

# AWS Ecosystem - free resources

* AWS blogs: https://aws.amazon.com/blogs/aws
* AWS forum: https://repost.aws/
* AWS Whitepapers & guides: https://aws.amazon.com/whitepapers
* AWS Quick Starts: https://aws.amazon.com/quickstart
* AWS Solutions: https://aws.amazon.com/solutions/
* AWS Marketplace
  * Digital catalog with thousands of software listings from independent software vendors
  * Examples: Custom AMIs, CloudFormation templates, Software as Service, Containers
  * If you buy through the AWS Marketplace, it goes into your AWS bill
  * You can sell your own solutions on the AWS Marketplace

* AWS Training
  * AWS Digital (online) and Classroom Training (in-person or virtual)
  * AWS Private Training (for your organization)
  * Training and Certification for the U.S Government
  * Training and Certification for the Enterprise
  * AWS Academy: helps universities teach AWS

* AWS Professional Services & Partner Network
  * The AWS Professional Services is a global team of experts
  * They work alongside your team and a chosen member of the APN (AWS Partner Network)
  * APN Technology Partners: providing hardware, connectivity, and software
  * APN Consulting Partners: professional services firm to help build on AWS
  * APN Training Partner: find who can help you learn AWS
  * AWS Competency Program: AWS Competencies are granted to APN Partners who have demonstrated technical proficiency and proven customer success in specialized areas.
  * AWS Navigate Program: help Partners become bette Partners

# AWS Knowledge Center

https://aws.amazon.com/premiumsupport/knowledge-center/

# AWS IQ

* Quickly find professional help for your AWS projects
* Engage and pay AWS Certified 3rd party experts for on-demand project work
* Video-conferencing, contract mgmt, secure collaboration, integrated billing
* For customers
![08-tool.png](./images/08-tool.png)
* For experts
![09-tool.png](./images/09-tool.png)


# AWS re:Post

It replaces AWS forum.

https://repost.aws/

* Part of the AWS free tier
* Community members can earn reputation points to build up their community expert status bby providing accepted answers
* Questions from AWS Premium Support customers that do not receive a response from the community are passed on to AWS Support Engineers
* It is not intended to be used for questions that are time-sensitive or involve any proprietary information