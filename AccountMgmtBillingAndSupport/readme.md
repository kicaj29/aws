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

## Service Control Policies (SCP)

* Whitelist or blacklist IAM actions
* Applied at the OU or Account level
* **Does not apply to the master account**
* SCP is applied to all the Users and Roles of the Account, including Root
* The SCP does not affect service-linked roles
  * Service-linked roles enable other AWS services to integrate with AWS Organizations and cannot be restricted by SCPs.
* SCP must have an explicit Allow (does not allow anything by default)
* **Deny takes precedence before Allow!**
* Use cases
  * Restrict access to certain services (for example cannot use EMR)
  * Enforce PCI compliance by explicitly disabling services
* Sample SCP hierarchy
  ![02-scp.png](images/02-scp.png)
* Sample blacklist and whitelist strategies
  ![03-scp.png](images/03-scp.png)

## AWS Organizations Hands On

* Sample organization
  ![04-sample-org.png](images/04-sample-org.png)

* Use SCP to create restrictions
  ![05-scp.png](images/05-scp.png)
  ![06-scp.png](images/06-scp.png)
  ![07-scp.png](images/07-scp.png)
  ![08-scp.png](images/08-scp.png)
  ![09-scp.png](images/09-scp.png)
  ![10-scp.png](images/10-scp.png)
  ![11-scp.png](images/11-scp.png)

## Organizations Consolidated Billing

* When you enabled, provides you with
  * Combined Usage - combine the usage across all AWS accounts in the AWS Organization to share the volume pricing, Reserved Instances and Saving Plans
  * One Bill - get on bill for all AWS accounts in the AWS Organization
* The management account can turn off Reserved Instances discount sharing for any account in the AWS Organization, including itself

![12-reserved-instances.png](./images/12-reserved-instances.png)

