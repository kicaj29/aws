- [VPC](#vpc)
- [Security Groups](#security-groups)
- [Routing table](#routing-table)
  - [Typical route table (public and private subnet)](#typical-route-table-public-and-private-subnet)
- [Network access control list](#network-access-control-list)
- [Subnet](#subnet)
- [Internet Gateway](#internet-gateway)
- [Create VPC with 2 public subnets](#create-vpc-with-2-public-subnets)
  - [Create EC2 instances that will be connected to created VPC](#create-ec2-instances-that-will-be-connected-to-created-vpc)
  - [Create and assign a public IP address to EC2 instance](#create-and-assign-a-public-ip-address-to-ec2-instance)
  - [Connect to the EC2 Instance via SSH](#connect-to-the-ec2-instance-via-ssh)
    - [Run PuTTYGent to convert pem file to ppk](#run-puttygent-to-convert-pem-file-to-ppk)
  - [Update EC2 instance and deploy the app](#update-ec2-instance-and-deploy-the-app)
    - [Update already installed components (in ssh console)](#update-already-installed-components-in-ssh-console)
    - [Install nodejs](#install-nodejs)
    - [Upload the app using WinSCP](#upload-the-app-using-winscp)
  - [Scaling EC2 instances](#scaling-ec2-instances)
    - [Creating dedicated AMI (image) for EC2 instance](#creating-dedicated-ami-image-for-ec2-instance)
    - [Create load balancer](#create-load-balancer)
    - [Enable instance stickiness on Load Balancer](#enable-instance-stickiness-on-load-balancer)
    - [Creating launch configuration for an auto-scaling group](#creating-launch-configuration-for-an-auto-scaling-group)
    - [Create autoscaling group using created launch configuration](#create-autoscaling-group-using-created-launch-configuration)
    - [Scaling in action](#scaling-in-action)
- [Review of default VPC and default subnets](#review-of-default-vpc-and-default-subnets)
  - [Selected subnets fields](#selected-subnets-fields)
- [Create VPC with public and private subnets](#create-vpc-with-public-and-private-subnets)
  - [Create VPC](#create-vpc)
  - [Create private subnet](#create-private-subnet)
  - [Create public subnet](#create-public-subnet)
  - [Create Internet Gateway](#create-internet-gateway)
  - [Create new route table for public subnet](#create-new-route-table-for-public-subnet)
  - [Enable auto-assign public IPv4](#enable-auto-assign-public-ipv4)
  - [Create EC2 instance in public subnet](#create-ec2-instance-in-public-subnet)
    - [Connect to the created EC2 via SSH to check IP addresses](#connect-to-the-created-ec2-via-ssh-to-check-ip-addresses)
  - [Assigned Elastic IP to ENI](#assigned-elastic-ip-to-eni)
  - [Create seconde EC2 instance in private network](#create-seconde-ec2-instance-in-private-network)
  - [Bastion host (jump box)](#bastion-host-jump-box)
    - [Create bastion host](#create-bastion-host)
    - [Connect with the bastion host](#connect-with-the-bastion-host)
    - [Connect to private EC2 instance from bastion host](#connect-to-private-ec2-instance-from-bastion-host)
- [Dual-Homed Instance](#dual-homed-instance)
- [resources](#resources)


# VPC
* is a logical data center
* local to a single region
* a vpc can span multiple AZ`s
* good for separating public and private resources
* you can create a hardware VPN between your data center  and your VPC
* can have only one Internet Gateway (IGW is completely managed AWS including HA and scaling)

# Security Groups

A security group defines a collection of IPs that are allowed to connect to your instance and IPs that instance is allowed to connect to. **Security groups are attached at the instance level (EC2) to interfaces (ENI) and can be shared among many instances**. You can even set security group rules to allow access from other security groups instead of by IP. **Security groups are stateful firewalls (it has persistency and can remember traffic from the past)**.

For example security group can be defined in EC2 wizard as one of its steps:
![vpc-21-aws-create-ec2-instance.png](images/vpc-21-aws-create-ec2-instance.png)

# Routing table

Each VPC has one **routing table**, which declares attempted destination IPs and where they should be routed to. For instance, if you want to run all outgoing traffic through a proxy, you can set a routing table rule to automatically send traffic to that IP. This can be a powerful tool for security as you can inspect outgoing traffic and only whitelist safe destinations for traffic.   

Router exist across availability zones and there is no single point of hardware failure that would take this router down.   

**Route tables can be assigned also to subnets.**

## Typical route table (public and private subnet)

![vpc-93-route-table.png](images/vpc-93-route-table.png)

![vpc-94-route-table2.png](images/vpc-94-route-table2.png)

# Network access control list

Another tool the VPCs use for networking control is the **network access control list**. Each VPC has one access control list, and this acts as an IP filtering table saying which IPs are allowed through the VPC for both incoming and outgoing traffic. Access control lists are kind of like super-powered security groups that apply rules for the entire VPC. **ACL is stateless firewall.**

# Subnet

A VPC defines a private, logically isolated area for instances, but a **subnet** is where things really get interesting. Instances cannot be launched into just a VPC. They must also be launched into a subnet that is inside a VPC. A subnet is an additional isolated area that has its own CIDR block, routing table, and access control list. Subnets enable you to create different behavior in the same VPC. For instance, creating a public subnet that can be accessed and have access to the public internet and a private subnet that is not accessible from the internet and must go through a NAT gateway to access the outside world. With subnets, you can achieve very secure infrastructure for your instances.

![vpc-01-subnet.png](images/vpc-01-subnet.png)
![vpc-02-subnet-priv-public.png](images/vpc-02-subnet-priv-public.png)

* the first four addresses are reserved within each subnet
* the highest address is reserved
* for example: 10.0.1.0/24
  * reserved addresses are: 10.0.1.0 (subnet address), 10.0.1.1 (reserved for default gateway), 10.0.1.2 (default DNS server), 10.0.1.3 (future use by AWS), and 10.0.1.255 (broad case address)

# Internet Gateway

**External IP is not visible from level of EC2 instance!** It sees only 10.0.1.23 address.   
**Internet GW is performing source NAT and destination NAT**.   

IGW is automatically HA, it is managed by AWS.

If it will receive package with source 10.0.1.23 and destination 8.8.8.8 it will replace 10.0.1.23 by public IP 59.54.23.52. Similar when it will receive incoming packet from Internet it will replace address 59.54.23.52 by 10.0.1.23.

Source nat:

![vpc-97-source-nat.png](images/vpc-97-source-nat.png)

Destination nat:

![vpc-98-dest-nat.png](images/vpc-98-dest-nat.png)


# Create VPC with 2 public subnets

![vpc-03-create-subnet.png](images/vpc-03-create-subnet.png)

![vpc-04-aws-console-create-vpc.png](images/vpc-04-aws-console-create-vpc.png)

![vpc-05-aws-console-create-vpc.png](images/vpc-05-aws-console-create-vpc.png)

![vpc-06-aws-console-create-vpc.png](images/vpc-06-aws-console-create-vpc.png)

Update routing table to make possible to connect with Internet EC2 instances from this subnet.

![vpc-07-aws-console-create-vpc.png](images/vpc-07-aws-console-create-vpc.png)

By default there is only one route that says that any IP address referencing our local VPC CIDR block should resolve locally.

![vpc-08-aws-console-create-vpc.png](images/vpc-08-aws-console-create-vpc.png)

0.0.0.0/0 means anywhere.

![vpc-09-aws-console-create-vpc.png](images/vpc-09-aws-console-create-vpc.png)

Select pre-created internet gateway for your VPC. It will allow outgoing traffic to reach the outside world.
![vpc-10-aws-console-create-vpc.png](images/vpc-10-aws-console-create-vpc.png)

![vpc-11-aws-console-create-vpc.png](images/vpc-11-aws-console-create-vpc.png)

Create second subnet in second AZ - **single subnet can exist only in one AZ**. Use different CIDR to avoid collisions with already created subnet.

![vpc-12-aws-console-create-vpc.png](images/vpc-12-aws-console-create-vpc.png)

![vpc-13-aws-console-create-vpc.png](images/vpc-13-aws-console-create-vpc.png)

![vpc-14-aws-console-create-vpc.png](images/vpc-14-aws-console-create-vpc.png)

## Create EC2 instances that will be connected to created VPC

![vpc-15-aws-create-ec2-instance.png](images/vpc-15-aws-create-ec2-instance.png)

![vpc-16-aws-create-ec2-instance.png](images/vpc-16-aws-create-ec2-instance.png)

![vpc-17-aws-create-ec2-instance.png](images/vpc-17-aws-create-ec2-instance.png)

![vpc-18-aws-create-ec2-instance.png](images/vpc-18-aws-create-ec2-instance.png)

![vpc-19-aws-create-ec2-instance.png](images/vpc-19-aws-create-ec2-instance.png)

![vpc-20-aws-create-ec2-instance.png](images/vpc-20-aws-create-ec2-instance.png)

![vpc-21-aws-create-ec2-instance.png](images/vpc-21-aws-create-ec2-instance.png)

![vpc-22-aws-create-ec2-instance.png](images/vpc-22-aws-create-ec2-instance.png)

Create key pair to be able use SSH and download it (ec2-jacek-west1-keys.pem).

![vpc-23-aws-create-ec2-instance.png](images/vpc-23-aws-create-ec2-instance.png)

Click launch instances:

![vpc-24-aws-create-ec2-instance.png](images/vpc-24-aws-create-ec2-instance.png)

## Create and assign a public IP address to EC2 instance

![vpc-25-aws-elastic-IP.png](images/vpc-25-aws-elastic-IP.png)
![vpc-26-aws-elastic-IP.png](images/vpc-26-aws-elastic-IP.png)
![vpc-27-aws-elastic-IP.png](images/vpc-27-aws-elastic-IP.png)
![vpc-28-aws-elastic-IP.png](images/vpc-28-aws-elastic-IP.png)
![vpc-29-aws-elastic-IP.png](images/vpc-29-aws-elastic-IP.png)
![vpc-30-aws-elastic-IP.png](images/vpc-30-aws-elastic-IP.png)
Now we can see that EC2 instance has public IP Address:
![vpc-31-aws-elastic-IP.png](images/vpc-31-aws-elastic-IP.png)

## Connect to the EC2 Instance via SSH

### Run PuTTYGent to convert pem file to ppk

Click load to select pem file:

![vpc-32-aws-putty.png](images/vpc-32-aws-putty.png)
![vpc-33-aws-putty.png](images/vpc-33-aws-putty.png)   
Select save private key to save if in ppk format.

Apply the ppk file in putty:

![vpc-34-aws-putty.png](images/vpc-34-aws-putty.png)

> :warning: from unknown reasons I had to generate ppk file 2 times, file that was created first time did not work.

To check user name that should be provided in putty select the instance in click connect:

![vpc-34a-aws-putty.png](images/vpc-34a-aws-putty.png)
![vpc-34b-aws-putty.png](images/vpc-34b-aws-putty.png)

Connect to the EC2 instance:

![vpc-35-aws-putty.png](images/vpc-35-aws-putty.png)

![vpc-36-aws-putty.png](images/vpc-36-aws-putty.png)

## Update EC2 instance and deploy the app

### Update already installed components (in ssh console)
```
sudo yum update
```
![vpc-37-aws-suod-yum-update.png](images/vpc-37-aws-suod-yum-update.png)

### Install nodejs
```
curl -sL https://rpm.nodesource.com/setup_14.x | sudo bash -
```
![vpc-38-aws-nodejs-install.png](images/vpc-38-aws-nodejs-install.png)

```
sudo yum install -y nodejs
```
![vpc-39-aws-nodejs-install.png](images/vpc-39-aws-nodejs-install.png)

Next we can check version

```
[ec2-user@ip-10-0-0-152 ~]$ node -v
v14.14.0
[ec2-user@ip-10-0-0-152 ~]$ npm -v
6.14.8
[ec2-user@ip-10-0-0-152 ~]$
```

### Upload the app using WinSCP

Install WinSCP and import defined session from PuTTY.   
Next upload the app to /home/ec2-user/sample-app. Before doing it remove **node_modules** folder from local app, these files will be download by EC2 instance later.

![vpc-40-aws-winscp.png](images/vpc-40-aws-winscp.png)

Next install necessary npms:

![vpc-41-aws-npm-install.png](images/vpc-41-aws-npm-install.png)

Next run the app:

```
[ec2-user@ip-10-0-0-152 sample-app]$ npm start

> pizza-luvrs@1.0.0 start /home/ec2-user/sample-app
> node index.js

Server running at: http://ip-10-0-0-152.us-west-1.compute.internal:3000
```

To run it in web browsers use public IP address because the EC2 instance does not have interface with public IP (it looks that it exist only in Elastic IP component).

Open in web browser this address ```http://52.53.108.154:3000/``` to see working app.

## Scaling EC2 instances

### Creating dedicated AMI (image) for EC2 instance

It is possible to created a snapshot of existing instance. Such snapshot can be saved and replicated.

Such AMI can be automatically scaled using **Auto Scaling Group**. Auto Scaling Group expands or shrinks a pool of instances based on pre-defined rules.

![vpc-42-ec2-scaling.png](images/vpc-42-ec2-scaling.png)

![vpc-43-ec2-scaling.png](images/vpc-43-ec2-scaling.png)

Created AMI (Amazon Machine Images) is visible on the AMI list:
![vpc-44-ec2-scaling.png](images/vpc-44-ec2-scaling.png)

### Create load balancer

![vpc-45-ec2-load-balancer.png](images/vpc-45-ec2-load-balancer.png)

![vpc-46-ec2-load-balancer.png](images/vpc-46-ec2-load-balancer.png)

![vpc-47-ec2-load-balancer.png](images/vpc-47-ec2-load-balancer.png)

![vpc-48-ec2-load-balancer.png](images/vpc-48-ec2-load-balancer.png)

![vpc-49-ec2-load-balancer.png](images/vpc-49-ec2-load-balancer.png)

![vpc-50-ec2-load-balancer.png](images/vpc-50-ec2-load-balancer.png)

![vpc-51-ec2-load-balancer.png](images/vpc-51-ec2-load-balancer.png)

![vpc-52-ec2-load-balancer.png](images/vpc-52-ec2-load-balancer.png)

Click register to add the instance to LB targets:
![vpc-53-ec2-load-balancer.png](images/vpc-53-ec2-load-balancer.png)

![vpc-54-ec2-load-balancer.png](images/vpc-54-ec2-load-balancer.png)

![vpc-55-ec2-load-balancer.png](images/vpc-55-ec2-load-balancer.png)

![vpc-56-ec2-load-balancer.png](images/vpc-56-ec2-load-balancer.png)

### Enable instance stickiness on Load Balancer

We need to ensure that if a user logs into our app in one EC2 instance that they continue to connect to that same instance wit subsequent requests. If we didn't do this, there's a possibility that they would hit an instance where they don't have a session. To avoid this situation entirely, let's enable stickiness on the load balancer. The load balancer will use a cookie to keep track of which users should be sent to which instances.

>NOTE: it is needed only if we deal with stateful app.

![vpc-57-ec2-target-group.png](images/vpc-57-ec2-target-group.png)

![vpc-58-ec2-target-group.png](images/vpc-58-ec2-target-group.png)

![vpc-59-ec2-target-group.png](images/vpc-59-ec2-target-group.png)

### Creating launch configuration for an auto-scaling group

![vpc-60-ec2-auto-scaling-group.png](images/vpc-60-ec2-auto-scaling-group.png)


![vpc-61-ec2-auto-scaling-group.png](images/vpc-61-ec2-auto-scaling-group.png)
>NOTE: search in drop down AMI does not work well, typing *sample* does not find the AMI, I had to select it directly from the drop down.

![vpc-62-ec2-auto-scaling-group.png](images/vpc-62-ec2-auto-scaling-group.png)

![vpc-63-ec2-auto-scaling-group.png](images/vpc-63-ec2-auto-scaling-group.png)

![vpc-64-ec2-auto-scaling-group.png](images/vpc-64-ec2-auto-scaling-group.png)

![vpc-65-ec2-auto-scaling-group.png](images/vpc-65-ec2-auto-scaling-group.png)

### Create autoscaling group using created launch configuration

![vpc-66-ec2-auto-scaling-group.png](images/vpc-66-ec2-auto-scaling-group.png)

![vpc-67-ec2-auto-scaling-group.png](images/vpc-67-ec2-auto-scaling-group.png)

Select both sub-networks. Thx to this load balancer will balance the traffic across AZ.

![vpc-68-ec2-auto-scaling-group.png](images/vpc-68-ec2-auto-scaling-group.png)

Select the target group that was created in load balancer wizard.

![vpc-69-ec2-auto-scaling-group.png](images/vpc-69-ec2-auto-scaling-group.png)

Scaling policies will be defined after creation of the auto scaling group.

![vpc-70-ec2-auto-scaling-group.png](images/vpc-70-ec2-auto-scaling-group.png)

![vpc-71-ec2-auto-scaling-group.png](images/vpc-71-ec2-auto-scaling-group.png)

![vpc-72-ec2-auto-scaling-group.png](images/vpc-72-ec2-auto-scaling-group.png)

After 30 seconds from creation of auto scaling group new instances will be launched in this group:

![vpc-73-ec2-auto-scaling-group.png](images/vpc-73-ec2-auto-scaling-group.png)

In the load balancer/target group we can see 3 instances. One instance was created at the very beginning and 2 other instances are from the create auto scaling group:

![vpc-74-ec2-auto-scaling-group.png](images/vpc-74-ec2-auto-scaling-group.png)
>NOTE: it took some time to see **healthy** in this status column.

Go to load balancer to check its DNS name. The ideas is that then you would get a normal URL like sample-app.com and create a DNS CNAME record to point to this load balancer DNS name. This part is not presented here.

![vpc-75-ec2-auto-scaling-group.png](images/vpc-75-ec2-auto-scaling-group.png)

Type ```http://jacek-app-load-balancer-west1-366905866.us-west-1.elb.amazonaws.com/``` in the web browser to see working app.

We can also see that there is cookie created by the [Enable instance stickiness on Load Balancer](#enable-instance-stickiness-on-load-balancer) mechanism.

![vpc-76-ec2-auto-scaling-group.png](images/vpc-76-ec2-auto-scaling-group.png)

Improve security group for the EC2 instances because now it is enough to allow inbound traffic from the load balancer and SSH.

Change from:
![vpc-77-ec2-auto-scaling-group.png](images/vpc-77-ec2-auto-scaling-group.png)

To:
![vpc-78-ec2-auto-scaling-group.png](images/vpc-78-ec2-auto-scaling-group.png)
The above configuration allows on inbound traffic from any instance that is in security group **jacek-lb-sg-west1**.

We can see also that ```http://jacek-app-load-balancer-west1-366905866.us-west-1.elb.amazonaws.com/``` still works fine in the web browser.

### Scaling in action

![vpc-79-ec2-scaling-in-action.png](images/vpc-79-ec2-scaling-in-action.png)

Add policy:
![vpc-80-ec2-scaling-in-action.png](images/vpc-80-ec2-scaling-in-action.png)


![vpc-81-ec2-scaling-in-action.png](images/vpc-81-ec2-scaling-in-action.png)

![vpc-82-ec2-scaling-in-action.png](images/vpc-82-ec2-scaling-in-action.png)

Typical alarm metrics:
* CPU Utilization
* Disk reads
* Disk writes
* Disk read operations
* Disk write operations
* Network in
* Network out

![vpc-83-ec2-scaling-in-action.png](images/vpc-83-ec2-scaling-in-action.png)

![vpc-84-ec2-scaling-in-action.png](images/vpc-84-ec2-scaling-in-action.png)

Set *greater than 100 bytes* to easily simulate adding more instances.

![vpc-85-ec2-scaling-in-action.png](images/vpc-85-ec2-scaling-in-action.png)

Remove notifications it is not needed now:   
![vpc-86-ec2-scaling-in-action.png](images/vpc-86-ec2-scaling-in-action.png)

It is common practice to create alarms that will trigger adding more instances and alarms that will trigger removing instances.

From unknown reasons I was not able to select auto scaling group from the drop down. This chapter will be finalized later:

![vpc-87-ec2-scaling-in-action.png](images/vpc-87-ec2-scaling-in-action.png)

# Review of default VPC and default subnets

Every AWS region has default VPC created. Thx to this we can start creating for example EC2 instances just after sign in to AWS console, if there would be no default VPC it would have to be created first, it is like pre-requisite step.

> NOTE: if needed it is possible to delete default VPC. If we deleted default VPC by mistake it is possible to create it once again, go to VPC view and next Actions -> Create default VPC.

## Selected subnets fields

* Auto-assign public IPv4 address (Yes/No): in case of Yes instances created in this subnet will get public IP address - publicly routable IP address. Someone in Internet could connect with this EC2 instance using this public IP if other AWS configuration (like security group) allows this.
To have public subnet we need value ```Yes``` for this field and Internet GW in the subnet.
  
* Route table
  ![vpc-88-route-table.png](images/vpc-88-route-table.png)
  First row (172.31.0.0/16): local route, it routes traffic locally within particular VPC. Thx this this instances from this subnet can communicate with instances from other subnets in this VPC.   
  Second row (0.0.0.0/0): if the traffic is anything other than network 172.31.0.0/16 then send this traffic to Internet Gateway. This also causes that EC2 instances will be accessible **from Internet.** It makes this subnet public.

* Network ACL
  ![vpc-89-acl.png](images/vpc-89-acl.png)   
  By default it allows all traffic from any source and any port for inbound and outbound traffic. It means that by default this ACL blocks nothing! What does it mean * on this screen???

# Create VPC with public and private subnets

## Create VPC

![vpc-91-create-vpc.png](images/vpc-91-create-vpc.png)

Automatically will be also created main route for this VPC and default ACL for this VPC.

![vpc-91-create-vpc-main-route.png](images/vpc-91-create-vpc-main-route.png)
Route with target local allows communication between all sub-networks within this VPC.


![vpc-91-create-vpc-default-acl.png](images/vpc-91-create-vpc-default-acl.png)
Default ACL allows for all incoming and all outgoing traffic.   
What does it mean * on this screen???

## Create private subnet

![vpc-92-create-priv-subnet.png](images/vpc-92-create-priv-subnet.png)

Instances that will run in this subnet will not have public IP.
It also has the same route table and ACL that "parent" VPC.

![vpc-92-create-priv-subnet-route-table.png](images/vpc-92-create-priv-subnet-route-table.png)

![vpc-92-create-priv-subnet-acl.png](images/vpc-92-create-priv-subnet-acl.png)

## Create public subnet

![vpc-92-create-public-subnet.png](images/vpc-92-create-public-subnet.png)

Again this subnet will by default will have the same route table and ACL as its VPC. At this moment this subnet is not yet public because instances in this subnet will not get public IP and there is missing route to Internet Gateway.

![vpc-92-create-public-subnet-no-ip.png](images/vpc-92-create-public-subnet-no-ip.png)

![vpc-92-create-public-subnet-default-route-table.png](images/vpc-92-create-public-subnet-default-route-table.png)

![vpc-92-create-public-subnet-default-acl.png](images/vpc-92-create-public-subnet-default-acl.png)

## Create Internet Gateway

Internet gateway is fully managed service it means that HA and auto-scaling works out of the box.

![vpc-92-create-public-subnet-igw.png](images/vpc-92-create-public-subnet-igw.png)

Next attach IGW to the created VPC:

![vpc-92-create-public-subnet-igw-attach-to-vpc.png](images/vpc-92-create-public-subnet-igw-attach-to-vpc.png)

## Create new route table for public subnet

![vpc-92-create-public-route-table.png](images/vpc-92-create-public-route-table.png)

Next add route for Internet traffic:

![vpc-92-create-public-subnet-route-igw.png](images/vpc-92-create-public-subnet-route-igw.png)

Next attach this route to public network:

![vpc-92-public-route-subnet-association.png](images/vpc-92-public-route-subnet-association.png)

## Enable auto-assign public IPv4

In public subnet enable public IPv4:

![vpc-92-public-subnet-ip.png](images/vpc-92-public-subnet-ip.png)

**Since now public network is really public because instances from this network will have public ID and there is route to Internet Gateway**.

## Create EC2 instance in public subnet

![vpc-99-public-subnet-EC2-create.png](images/vpc-99-public-subnet-EC2-create.png)

![vpc-99-public-subnet-EC2-create-eth0.png](images/vpc-99-public-subnet-EC2-create-eth0.png)

![vpc-99-public-subnet-EC2-create-sg.png](images/vpc-99-public-subnet-EC2-create-sg.png)

![vpc-99-public-subnet-EC2-key-pair.png](images/vpc-99-public-subnet-EC2-key-pair.png)

Download pem file and next connect to the EC2 instance

Created instances has one ENI interface:
![vpc-99-public-subnet-EC2-interfaces.png](images/vpc-99-public-subnet-EC2-interfaces.png)

### Connect to the created EC2 via SSH to check IP addresses

We can see that in the EC2 instance we do not have public IP address.
Public IP address is managed by [Internet Gateway](#internet-gateway).

```
[ec2-user@ip-10-1-101-13 ~]$ ifconfig
eth0: flags=4163<UP,BROADCAST,RUNNING,MULTICAST>  mtu 9001
        inet 10.1.101.13  netmask 255.255.255.0  broadcast 10.1.101.255
        inet6 fe80::403:21ff:fe67:47a6  prefixlen 64  scopeid 0x20<link>
        ether 06:03:21:67:47:a6  txqueuelen 1000  (Ethernet)
        RX packets 36907  bytes 51476363 (49.0 MiB)
        RX errors 0  dropped 0  overruns 0  frame 0
        TX packets 11180  bytes 693870 (677.6 KiB)
        TX errors 0  dropped 0 overruns 0  carrier 0  collisions 0

lo: flags=73<UP,LOOPBACK,RUNNING>  mtu 65536
        inet 127.0.0.1  netmask 255.0.0.0
        inet6 ::1  prefixlen 128  scopeid 0x10<host>
        loop  txqueuelen 1000  (Local Loopback)
        RX packets 8  bytes 648 (648.0 B)
        RX errors 0  dropped 0  overruns 0  frame 0
        TX packets 8  bytes 648 (648.0 B)
        TX errors 0  dropped 0 overruns 0  carrier 0  collisions 0

```

**Also this assigned public IP is not permanent. If we stop and start the EC2 instance it will get new public IP address!**.

![vpc-99-public-subnet-EC2-public-IP1.png](images/vpc-99-public-subnet-EC2-public-IP1.png)

![vpc-99-public-subnet-EC2-public-IP2.png](images/vpc-99-public-subnet-EC2-public-IP2.png)

## Assigned Elastic IP to ENI

To have permanent public IP address EIP has to be used and assigned to EC2 instance or to interface from EC2 instance.

Create elastic IP:
![vpc-100-create-elastic-IP.png](images/vpc-100-create-elastic-IP.png)

Review created EIP:
![vpc-100-elastic-IP.png](images/vpc-100-elastic-IP.png)

Associate EIP to ENI:

![vpc-100-elastic-associate.png](images/vpc-100-elastic-associate.png)

Next we can see that the EC2 instance got new public IP which is permanent (restart will not change it). Also previous public IP is unassigned from the instance.

![vpc-100-elastic-IP-ec2.png](images/vpc-100-elastic-IP-ec2.png)

EIP is also not visible from EC2 instance - ```ifconfig``` command.

## Create seconde EC2 instance in private network

![vpc-100-ec2-in-private-network.png](images/vpc-100-ec2-in-private-network.png)

## Bastion host (jump box)

Bastion host allows connection to instance that runs in private network.

### Create bastion host

![vpc-101-bastion-host-ami.png](images/vpc-101-bastion-host-ami.png)

![vpc-101-bastion-host-t3.png](images/vpc-101-bastion-host-t3.png)

![vpc-101-bastion-host-vpc.png](images/vpc-101-bastion-host-vpc.png)

![vpc-101-bastion-host-sg.png](images/vpc-101-bastion-host-sg.png)

### Connect with the bastion host

![vpc-101-bastion-host-connect.png](images/vpc-101-bastion-host-connect.png)

![vpc-101-bastion-host-get-password.png](images/vpc-101-bastion-host-get-password.png)

![vpc-101-bastion-host-pem.png](images/vpc-101-bastion-host-pem.png)

![vpc-101-bastion-host-password.png](images/vpc-101-bastion-host-password.png)   
Bq5zN6h.Xu

Next connect RDP client to connect with the bastion host:

![vpc-101-bastion-host-connected.png](images/vpc-101-bastion-host-connected.png)

Next copy to Windows instance ppk and putty installer:

![vpc-101-bastion-host-copy-files.png](images/vpc-101-bastion-host-copy-files.png)

Install putty and import ppk file.

### Connect to private EC2 instance from bastion host

![vpc-101-bastion-host-connect-to-private-EC2.png](images/vpc-101-bastion-host-connect-to-private-EC2.png)

![vpc-101-bastion-host-connected-to-private-EC2.png](images/vpc-101-bastion-host-connected-to-private-EC2.png)

# Dual-Homed Instance

It is possible to create second interface (ENI) and in this way EC2 instance be accessed from private and public subnet.

![vpc-96-dual-homed-instance.png](images/vpc-96-dual-homed-instance.png)

# resources
https://acloud.guru/forums/aws-certified-cloud-practitioner/discussion/-Lmu_Iq2Zrc_ojEYoN4d/I%20got%20a%20putty%20fatal%20error:%20No%20supported%20authentication%20methods%20available%20(server%20sent:publickey,gssapi-keyex,gssapi-with-mic)%20%20How%20do%20I%20resolve%20this%20issue%3F   
https://app.pluralsight.com/library/courses/aws-developer-getting-started/table-of-contents (chapters related with EC2 and putty)   
https://techviewleo.com/how-to-install-nodejs-on-amazon-linux/   
https://www.udemy.com/course/awsnetworking/   
https://www.youtube.com/watch?v=gMvXruavqDI&t=189s   