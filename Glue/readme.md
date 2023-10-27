# Glue Data Catalog

* Metadata repository for all your tables
  * Automated schema interface
  * Schemas are versioned
* Integrates with Athena or Redshift Spectrum (schema and data discovery)
* Glue Crawlers can help build the Glue Data Catalog

![01.png](./images/01.png)

## Glue Data Catalog - Crawlers

* Crawlers go through your data to infer schemas and partitions
* Works JSON, Parquet, CSV, relation store
* Crawlers work for S3, Amazon Redshift, Amazon RDS
* Run the Crawler on a Schedule or On Demand
* Need an IAM role / credentials to access the data stores

# Glue and S3 Partitions

* Glue crawler will extract partitions based on how you S3 data is organized
* Think up front about how you will be querying your data lake in S3
* Example: devices and sensor data every hour
* Do you query primarily by time ranges?
  * If so, organize you buckets as `s3://my-bucket/dataset/yyyy/mm/dd/device`
* Do you query primarily by device?
  * If so, organize your buckets as `s://my-bucket/dataset/device/yyyy/mm/dd/device`
  ![02.png](./images/02.png)

# Glue ETL

* Transform data, Clean Data, Enrich Data (before doing analysis)
  * Generate ETL code in Python or Scala, you can modify the code
  * Can provide your own Spark or PySpark scripts
  * Target can be S3, JDBC (RDS, Redshift), or in GLue Data Catalog
* Fully managed, cost effective, pay only for the resources consumed
* Jobs are run on a serverless Spark platform

* Glue Scheduler to schedule the jobs
* Glue Triggers to automate job runs based on the events

## Glue ETL Transformations

* Bundled Transformation
  * DropFields, DropNullFields - remove (null) fields
  * Filter - specify a function to filter records
  * Join - to enrich data
  * Map - add fields, delete fields, perform external lookups

* **Machine Learning Transformation**
  * FindMatches ML: identify duplicate or matching records in your dataset, even when the records do not have a common unique identifier and no fields match exactly.

* Format conversions: CSV, JSON, Avro, Parquet, ORC, XML
* Apache Spark transformation (example: K-Means)