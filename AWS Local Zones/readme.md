# AWS Local Zones

* Places AWS compute, storage, database, and other selected AWS services **closer to end users to run latency-sensitive applications like for example games**
* Extend your VPC to more locations - "Extension of an AWS Region"
* Compatible with EC2, RDS, ECS, EBS, ElastiCache, Direct Connect...
* Example
  * AWS Region: N.Virginia (us-east-1)
  * AWS Local Zones: Boston, Chicago, Dallas, Houston, Miami,...

![01.png](./images/01.png)

# Hands On

* To check local zones click "Zones" link

![02.png](./images/02.png)

* There are regions which do not have local zones, there are only availability zones

![03.png](./images/03.png)

* And there are regions which have local zones (availability zones are always)

![04.png](./images/04.png)

and also Wavelength Zones

![05.png](./images/05.png)

* If we want reduce latency for users from Boston we can enable local zone

![06.png](./images/06.png)

* Next if we want launch EC2 we can create a new subnet for selected VPC that will be hosted in Boston

![07.png](./images/07.png)

![08.png](./images/08.png)

* Next we can select this subnet for the EC2 instance

![09.png](./images/09.png)