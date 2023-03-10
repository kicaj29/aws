# Important requirements

S3 allows people to store **objects** (files) in **buckets** (directories - root directories).

*  Buckets must have globally unique name (across all accounts)
*  Buckets are defined at the region level
*  S3 looks like a global service but buckets are created in a region
*  Naming conventions for buckets
   *  No uppercase
   *  No underscore
   *  3-63 length
   *  Not an IP
   *  Must start with lowercase letter or number

# Path

The key is compose of prefix and object name: s3://[BUCKET-NAME]/[FOLDER1]/[FOLDER2]/[FILE-NAME], sample key:
`s3://my-bucket/f1/f2/my_file.txt` - here the prefix is f1/f2

# Limits

* Max object size is 5TB but **S3 has unlimited storage**
* If uploading more than 5TB, must use "multi-part upload".
* Cannot partially update objects - even we change only one bite we have to upload the whole object (such limitation does not exist in EBS - EBS can store a single file in multiple blocks and then only selected blocks are updated).

# Security

* User based - IAM policies
  * NOTE: an IAM principal can access an S3 object if the user IAM permissions allow it OR the resource policy ALLOWS it AND there is no explicit DENY
* Resource based
  * Bucket policies - bucket wide rules from the S3 console - allow cross account
  * Object Access Control List (ACL) - finer grain
  * Bucket ACL - less comment
* Encryption
* Bucket settings for block public access - can be set on account level

# WebSites

URL address can be: `<bucket-name>.s3-website-<AWS-region>.amazonaws.com` or `<bucket-name>.s3-website.<AWS-region>.amazonaws.com`.


# Versioning

* It is enabled at the bucket level
* Same key overwrite will increment the "version": 1,2,3...
* Protects against unintended deletes (ability to restore version)
* Easy roll back to previous version
* **Any file the is not versioned prior to enabling versioning will have version "null"**
* Suspending versioning does not delete previous versions

# Access logs

* For audit purpose, you may want to log all access to S3 buckets
* Any requests made to S3, from any account, authorized pr denied, will be logged into another S3 bucket
* Access logs are also stored on S3 (new bucket has to be created)
* Logs appears after sometime (~1 hour)

# Replication

* Must enable versioning in source and destination
* Cross Region Replication (CRR)
* Same Region Replication (SRR)
* Buckets can be in different accounts
* Copying is asynchronous
* Must give proper IAM permissions to S3
* It is possible to specify prefix in the replication path
* IAM role is necessary
* When enabling we have to specify if already existing objects also should be replicated
* Version IDs are also replicated but only newest versions are replicated

CRR use cases: compliance, lower latency access, replication across accounts
SRR use cases: log aggregation, live replication between production and test accounts

# S3 classes

Can be specified for the whole bucket or per object (file) or per prefix (using lifecycle rules).

## S3 Durability

* High durability (99,999999999%, 11 9s) **of objects across multiple AZ**.
* If you store 10M objects with S3, you can on average expect to incur a loss of single object once every 10 000 years.
* Same for all storage classes.

## S3 Availability

* Measures how readily available a service is.
* Varies depending on storage class.
* Example: S3 standard has 99.99% availability = not available 53 minutes a year. More [here](https://blog.imagekit.io/how-do-you-prepare-for-an-aws-s3-outage-e60052937ef3).

## Classes
Can move between classes manually or using S3 lifecycle configurations.

* Standard - general purpose
  * 99.999_999_999% availability (52 mins) (11 nines) - this is the probability that object will stay intact during 1 year
  * Used for frequently accessed data
  * Sustain 2 concurrent facility failures this is because data is **stored in at least 3 AZs**
  * Use cases: big data analytics, mobile & gaming applications, content distribution...
* Infrequent access (IA)  
  * For data that is less frequent accessed, but requires rapid access when needed
  * Lower cost than S3 standard
  * Standard IA
    * Stores data in at lease 3 AZs
    * 99.9% availability
    * Use cases: disaster recovery, backups
  * One Zone IA
    * Stores data in 1 AZ
    * 99.5% availability
    * High durability (99,999999999%, 11 9s) in a single AZ; data lost when AZ is destroyed
    * Use cases: secondary backup copies on on-prem data, or data you can recreate
* Glacier
  * Low-cost object storage meant for archiving/backup
  * Pricing: price for storage + object retrieval cost
  * Can upload directly or via LifeCycle Policy (also known as Object lifecycle management)
  * Instant Retrieval
    * Millisecond retrieval, **great for data accessed once a quarter**
    * Minimum storage duration of 90 days
  * Flexible Retrieval (formerly Amazon S3 Glacier)
    * Expedited (1 to 5 minutes), Standard (3 to 5 hours), Bulk (5 to 12 hours) - free
    * Minimum storage duration of 90 days
  * Deep Archive - for long term storage
    * Standard (12 hours), bulk (48 hours)
    * Minimum storage duration of 180 days
  * Glacier Instant Retrieval vs Infrequent access (IA): https://allcloud.io/blog/moving-to-s3-glacier-or-infrequent-access-storage/
    * Data stored in the S3 Glacier Instant Retrieval storage class offers a cost savings compared to the S3 Standard-IA storage class, with the same latency and throughput performance as the S3 Standard-IA storage class.
    * S3 Glacier Instant Retrieval has higher data access costs than S3 Standard-IA
    * https://aws.amazon.com/s3/pricing/
* Intelligent-Tiering
  * Ideal for data with unknown or changing access patterns
  * Small monthly monitoring and auto-tiering fee
  * Moves objects automatically between Access Tiers based on usage
  * There are no retrieval charges in S3 Intelligent-Tiering

* Comparison

![Comparison](./images/01-s3-comparison.png)


# S3 Object lock

* S3 object lock: adopt a WORM (Write Once - Read Many) model - possible in Glacier. It is also possible to lock this policy
  so no one can change it in the future.
* Block an object version deletion for a specified amount of time

# S3 Glacier vault lock

* S3 object lock: adopt a WORM (Write Once - Read Many) model
* **Lock the policy for future edits (can no longer be changed)**
  * For example: I want upload and object to s3 and make sure then no one will ever be able to delete this file (even admins cannot delete it)
* Helpful for compliance and data retention

# S3 Encryption

* No encryption
* Server-side encryption (server encrypts the file after receiving it)
* Client-side encryption (user encrypts the file before uploading it)

# Shared responsibility model for S3

* aws
  * Infrastructure (global security, durability, availability, sustain concurrent loss of data in 2 facilities)
  * Configuration and vulnerability analysis
  * Compliance validation
* clients
  * S3 versioning
  * S3 bucket policies
  * S3 replication setup
  * Logging and monitoring
  * S3 storage class
  * Data encryption at rest and in transit

# AWS Snow Family

Highly-secure, portable offline devices to collect and process data at the edge, and migrate data into and out of AWS. Usually it is used to move on-premises data to AWS. In this way we can transfer data to AWS much faster.

## Data migration

* Snowcone
  * Small device, portable, computing anywhere, rugged & secure, withstands harsh environments
  * Light (4.5 pounds, 2.1 kg)
  * Device used for edge computing, storage and data transfer
  * 8 TB of usable storage
  * Must provide own batter/cables
  * Can be sent back to AWS offline, or connect it to Internet and use **AWS DataSync** to send data
* Snowball edge
  * Big device
  * Move TBs or PBs of data in or out of AWS
  * Pay per data transfer job
  * Provide block storage and Amazon S3-compatible object storage
  * Storage Optimized
    * 80 TB of HDD capacity for block volume and S3 compatible object storage
  * Compute Optimized
    * 42 TB of HDD capacity for block volume and S3 compatible object storage
  * Use case: large data cloud migration, DC decommission, disaster recovery
  * No **AWS DataSync** agent
  * Up to 15 nodes
  * **Natively supports EC2 instances** - you can run Amazon EC2 compute instances hosted on a Snowball Edge with the sbe1, sbe-c, and sbe-g instance types.
* Snowmobile
  * It is truck (real truck)
  * 100 PB
  * GPS
  * 24/7 video surveillance
  * No **AWS DataSync** agent

## Edge computing

Process data while it`s being created **on an edge location**. For example: a truck on the road, a ship on the sea, mining station underground, ...
These locations my have:

* Limited / no Internet access
* Limited / no easy access to computing powe

Use cases:
* pre-process data
* machine learning at the edge
* transcoding media streams

### Edge computing devices

* Snowcone
  * 2 CPU
  * 4GB of memory
  * Wired or wireless access
  * USB-C power using a cord of the optional better
* Snowball edge
  * Storage Optimized
    * Up to 40 vCPU, 80 GiB of RAM
    * Object storage clustering available
    * Better than Compute Optimized for data transfer (because of supporting clusters ?)
  * Compute Optimized
    * 52 vCPUs, 208 GiB of RAM
    * Optional GPU (useful for video processing or machine learning)
    * 42 TB usable storage
* All: can run EC2 instances & AWS lambda functions (using AWS IoT Greengrass)

## AWS OpsHub

Historically, to use Snow Family devices, you needed a CLI.
Today, you can use AWS OpsHub a software you instal on your computer to manage your Snow Family Devices (it has UI).

# Storage gateway - hybrid cloud

* AWS is pushing for "hybrid cloud"
  * Part of infra is on-prem
  * Part of infra is on the cloud
* This can be due to
  * Long cloud migrations
  * Security requirements
  * Compliance requirements
  * IT strategy
* S3 is a proprietary storage technology (unlike EFS/NFS), so how do you expose the S3 data on-premise?
  * **Use AWS Storage Gateway**

AWS Storage Gateway is a bridge between on-premise data and cloud data in S3. Hybrid storage service to allow on-prem to seamlessly use the AWS Cloud.

Use cases: disaster recovery, backup and restore, tiered storage

Types of storage gateways:
* File - provides a virtual on-premises **file server,** which enables you to store and retrieve files as objects in Amazon S3
* Volume - the volume gateway represents the family of gateways that support **block-based volumes**, previously referred to as gateway-cached and gateway-stored mode
* Tape - Gateway Virtual Tape Library **can be used with popular backup software such as** NetBackup, Backup Exec and Veeam. Uses a virtual media changer and tape drives.


# S3 Transfer Acceleration

* Increase transfer speed by transferring file to an AWS edge location which will forward the data to the S3 bucket in the target region

![02-s3-transfer-acceleration.png](./images/02-s3-transfer-acceleration.png)

* Can be tested using page https://s3-accelerate-speedtest.s3-accelerate.amazonaws.com/en/accelerate-speed-comparsion.html





