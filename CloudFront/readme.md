# AWS CloudFront basics

* Content Delivery Network (CDN)
* Improves read performance, content is cached at the edge location(s)
* Improves user experience
* 216 Point of Presence globally (edge locations)
* DDOoS protection (because worldwide)
* Integration with Shield, AWS Web Application Firewall


# CloudFront origins

* S3 bucket
  * For distributing files and caching them at the edge
  * Enhanced security with CloudFront Origin Access Control (OAC)
    * OAC is replacing Origin Access Identity (OAI)
  * **CloudFront can be used as an ingress (to upload files to S3)**
    * Is it about "S3 Transfer Acceleration" ? TBD
* Custom Origin (HTTP)
  * ALB
  * EC2 instance
  * S3 website
  * Any HTTP backend you want

# CloudFront architecture at a high level 

>NOTE: a client has to use DNS name created by the CloudFront

![01-cloudFront-arch.png](./images/01-cloudFront-arch.png)


# S3 as origin

![02-cloudFront-arch.png](./images/02-cloudFront-arch.png)

## CloudFront vs S3 Cross Region Replication

* CloudFront
  * Global Edge network
  * Files are cached for a TTL (~day?)
  * Great for static content that must be available everywhere

* S3 Cross Region Replication
  * Must be setup for each region you want replication to happen
  * Files are updated in near real-time
  * Read only
  * Great for dynamic content that needs to be available at low-latency in few regions

