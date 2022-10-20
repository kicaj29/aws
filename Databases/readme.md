- [NoSQL Databases (non relational databases)](#nosql-databases-non-relational-databases)
  - [DynamoDB](#dynamodb)
    - [DynamoDB Accelerator - DAX](#dynamodb-accelerator---dax)
    - [DynamoDB Global Tables](#dynamodb-global-tables)
- [RDS (Relational Database Service)](#rds-relational-database-service)
  - [Advantage over using RDS versus deploying DB on EC2](#advantage-over-using-rds-versus-deploying-db-on-ec2)
    - [Aurora (part of RDS)](#aurora-part-of-rds)
  - [RDS Deployments](#rds-deployments)
- [ElastiCache](#elasticache)
- [Redshift](#redshift)
- [Amazon EMR](#amazon-emr)
- [Athena](#athena)
- [QuickSight](#quicksight)
- [DocumentDB](#documentdb)


# NoSQL Databases (non relational databases)
## DynamoDB

* Fully managed HA with replication across 3 AZ
* Scales to massive workloads, distributed "serverless" database
* Millions of requests per seconds, trillions of row, 100s of TB of storage
* Fast and consistent in performance
* Single-digit millisecond latency - low latency retrieval
* Integrated with IAM for security, authorization and administration
* Low cost and auto scaling capabilities
* Standard and Infrequent Access (IA) Table Class
* Key/Value DB
![07-dynamoDB.png](./images/07-dynamoDB.png)

### DynamoDB Accelerator - DAX

* Fully managed in-memory cache for DynamoDB
* It is not ElastiCache
* 10x performance improvement - single digit millisecond latency to microseconds latency - when accessing your DynamoDB tables
* Difference with ElastiCache at the CCP level: DAX is only used for and is integrated with DynamoDB, while ElastiCache can be used for other databases

![08-dynamoDB.png](./images/08-dynamoDB.png)

### DynamoDB Global Tables

* Make a DynamoDB table accessible with low latency in multiple-regions
* Active-Active replication (read/write to any AWS region)

![09-dynamoDB-globalTables.png](./images/09-dynamoDB-globalTables.png)

# RDS (Relational Database Service)
* PostgresSQL
* MySQL
* MariaDB
* Oracle
* Microsoft SQL Server

## Advantage over using RDS versus deploying DB on EC2

RDS is managed service:
* Automated provisioning, OS patching
* Continuous backups and restore to specific timestamp (Point in Time Restore)
* Monitoring dashboards
* Read replicas for improved performance
* Multi AZ setup for DR (Disaster Recovery)
* Scaling capability (vertical and horizontal)
* Storage backed by EBS (gp2 or io 1)

**But you can`t SSH into your instances.**

![01-rds.png](./images/01-rds.png)

### Aurora (part of RDS)
Aurora is AWS Proprietary database, not open source.

![02-rds.png](./images/02-rds.png)

* **PostgresSQL and MySQL** are supported as Aurora DB.
* It is "AWS cloud optimized" and claims 5x performance improvement over MySQL on RDS, over 3x the performance of Postgres on RDS.
* Storage will automatically grow in increments of 10GB, up to 64TB
* Aurora costs more than RDS (20% more) - but is more efficient
* Not in the free tier

## RDS Deployments

* Read replicas
  * Can create up to 5 read replicas
  * Data is only written to the main DB
![03-replicas.png](./images/03-replicas.png)
* Multi-AZ
  * Failover in case AZ outage (HA)
  * Only one AZ as failover
![04-multiAZ.png](./images/04-multiAZ.png)
* Multi-region (read replicas)
  * Disaster recover in case of region issue
  * Local performance for global reads
  * Replication cost
![05-multiRegion.png](./images/05-multiRegion.png)

# ElastiCache

* The same way RDS is to get managed Relational Databases
* ElastiCache is to get managed Redis or Memcached
* Caches are in-memory databases with high performance, low latency
* Helps reduce load off databases for read intensive workloads
* AWS takes care of OS maintenance / patching, optimizations, setup, configuration, monitoring, failure recovery and backups

![06-elastiCache.png](./images/06-elastiCache.png)

# Redshift

* Is based on PostgresSQL, but it is not used for OLTP (Online transaction processing)
* It is OLAP - online **analytical** processing (analytics and data warehousing)
* Load data once every hour, not every second
* 10x better performance than other data warehouses, scales to PBs of data
* Columnar storage of data (instead of row based)
* Massively Parallel Query Execution (MPP), HA
* Pay as you go based on the instances provisioned
* Has a SQL interface for performing the queries
* BI tools such as AWS Quicksight or Tableau integrate with it

# Amazon EMR

* Stands for "Elastic MapReduce"
* EMR helps creating **Hadoop** clusters (Big Data) to analyze and process vast amount of data
* The clusters can be made of hundreds of EC2 instances
* Also supports Apache, Spark, HBase, Presto, Flink,...
* EMR takes care of all the provisioning configuration
* Auto-scaling and integrated with Spot instances
* Use cases: data processing, machine learning, web indexing, big data

# Athena

* Serverless query service to perform analytics again S3 objects
* Use standard SQL language to query the files
* Supports: CSV, JSON, ORC, Avro and Parquet (build on Presto)
* Pricing: $500 per TB of data scanned
* Use compressed or columnar data for cost savings (less scan)
* Use cases: business intelligence, analytics, reporting, analyze & query VPC Flow Logs, ELB Logs, CloudTrails etc.
*** Exam tip: analyze data in S3 using serverless SQL, use Athena**

# QuickSight
* Serverless machine learning-powered business intelligence service to create interactive dashboards
* Fast, automatically scalable, embeddable
* Use cases
  * BA
  * Building visualizations
  * Perform ad-hoc analysis
  * Get business insights using data
* Integrated with: RDS, Aurora, Athena, Redshift, S3...
![10-quickSight.png](./images/10-quickSight.png)

# DocumentDB

* Aurora is as "AWS-implementation" of PostgresSQL/MySQL
* DocumentDB is the same for **MongoDB**
* Fully managed, HA with replication across 3 AZ
* Storage automatically grows in increments of 10GB, up to 64TB
* Automatically scales to workloads with millions of requests per seconds