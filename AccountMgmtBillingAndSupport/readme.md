# AWS Organizations

* Global Service
* Allows to manage multiple AWS accounts
* The main account is the master account
* Cost benefits
  * Consolidated billing across all accounts - single payment method
  * Pricing benefits from aggregated usage (volume discount for EC2, S3...)
  * Pooling of Reserved EC2 instances for optimal savings
* API is available to automate AWS account creation
* **Restrict account privileges using Service Control Policies (SCP)**

## Multi Account Strategies

* Creates accounts per department, per cost center, per dev/test/prod, based on regulatory restrictions (using SCP) for better resource isolation (ex:VPC), to have per-account service limits, isolated account for logging.
* Multi Account vs One Account Multi VPC
* Use tagging standards for billing purpose
* Enable CloudTrail on all accounts, send logs to central S3 account
* Send CloudWatch logs to central logging account
* Organizational units (OU) - examples
  ![01-organizations.png](./images/01-organizations.png)
