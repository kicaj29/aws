# Important requirements

S3 allows people to store objects (files) in buckets (directories - root directories).

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

* Max object size is 5TB.
* If uploading more than 5TB, must use "multi-part upload".

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

## S3 Durability

* High durability (99,999999999%, 11 9s) of objects across multiple AZ.
* If you store 10M objects with S3, you can on average expect to incur a loss of single object once every 10 000 years.
* Same for all storage classes.

## S3 Availability

* Measures how readily available a service is.
* Varies depending on storage class.
* Example: S3 standard has 99.99% availability = not available 53 minutes a year. More [here](https://blog.imagekit.io/how-do-you-prepare-for-an-aws-s3-outage-e60052937ef3).

## Classes
Can move between classes manually or using S3 lifecycle configurations.

* Standard - general purpose
  * 99.99% availability (52 mins)
  * Used for frequently accessed data
  * Sustain 2 concurrent facility failures
  * Use cases: big data analytics, mobile & gaming applications, content distribution...
* Infrequent access (IA)  
  * For data that is less frequent accessed, but requires rapid access when needed
  * Lower cost than S3 standard
  * Standard IA
    * 99.9% availability
    * Use cases: disaster recovery, backups
  * One Zone IS
    * 99.5% availability
    * High durability (99,999999999%, 11 9s) in a single AZ; data lost when AZ is destroyed
    * Use cases: secondary backup copies on on-prem data, or data you can recreate
* Glacier
  * Low-cost object storage meant for archiving/backup
  * Pricing: price for storage + object retrieval cost
  * Instant Retrieval
    * Millisecond retrieval, great for data accessed once a quarter
    * Minimum storage duration of 90 days
  * Flexible Retrieval (formerly Amazon S3 Glacier)
    * Expedited (1 to 5 minutes), Standard (3 to 5 hours), Bulk (5 to 12 hours) - free
    * Minimum storage duration of 90 days
  * Deep Archive - for long term storage
    * Standard (12 hours), bulk (48 hours)
    * Minimum storage duration of 180 days
* Intelligent-Tiering
  * Small monthly monitoring and auto-tiering fee
  * Moves objects automatically between Access Tiers based on usage
  * There are no retrieval charges in S3 Intelligent-Tiering

* Comparison

![Comparison](./images/01-s3-comparison.png)
