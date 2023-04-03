Amazon Elastic Block Store

# Instance Store Volumes

https://docs.aws.amazon.com/AWSEC2/latest/UserGuide/InstanceStorage.html

An instance store provides temporary block-level storage for your instance. This storage is located on disks that are **physically attached** to the host computer. Instance store is ideal for temporary storage of information that changes frequently, such as buffers, caches, scratch data, and other temporary content

This is also good option when you need storage with very low latency, but you don't need the data to persist when the instance terminates or you can take advantage of fault-tolerant architectures.
# What is an EBS Volume

* It is a network drive you can attach to your instances while they run (it is not a physical drive). Because it is a network drive there might be a bit of latency.
* It allows your instances to persist data, even after their termination.
* They can only be mounted to a single instance at a time.
* **They are bound to a specific availability zone** (it is not regional resource)
  * For example: an EBS in us-east-1a cannot be attached to us-east-1b, unless we create a snapshot - then it is possible.
* Analogy: think ot them as a "network USB stick".
* Size up to 16 TB
* Single file can be stored as many blocks - it allows to update only part of the file, partial file updates are possible, S3 does not support it
* EBS Volumes do not automatically scale if more space is needed
* Both non-root and root volumes can be encrypted

## EBS - delete on termination

* By default, attached EBS volume is not deleted.
![001-delete-on-termination-attr.png](./images/001-delete-on-termination-attr.png)

## EBS - snapshots (backup)

* EBS snapshots are stored on S3
* incremental snapshots
* make a snapshot (backup) of you EBS volume at point in time
* not necessary to detach volume to do snapshot, but recommended
* can copy snapshots across AZ or Region

![002-ebs.png](./images/002-ebs.png)

Features

* EBS snapshot archive
  * move a snapshot to an "archive tier" that is 75% cheaper
  * takes within 24 to 72 hours for restoring the archive
  * snapshots are stored on S3
* Recycle bin for EBS snapshot
  * setup rules to retain deleted snapshots so you can recover the after an accidental deletion
  * specify retention (1 day to 1 year)

# Root device storage concepts

https://docs.aws.amazon.com/AWSEC2/latest/UserGuide/RootDeviceStorage.html

When you launch an instance, the **root device volume contains the image used to boot the instance**. When we introduced Amazon EC2, all AMIs were backed by Amazon EC2 instance store, which means the root device for an instance launched from the AMI is an instance store volume created from a template stored in Amazon S3. After we introduced Amazon EBS, we introduced AMIs that are backed by Amazon EBS. This means that the root device for an instance launched from the AMI is an Amazon EBS volume created from an Amazon EBS snapshot.

You can choose between AMIs backed by Amazon EC2 instance store and AMIs backed by Amazon EBS. **We recommend that you use AMIs backed by Amazon EBS**, because they launch faster and use persistent storage.


## Instance store-backed instances

Any data on the instance store volumes persists as long as the instance is running, but this data is deleted when the instance is terminated (instance store-backed instances do not support the Stop action) or if it fails (such as if an underlying drive has issues).

After an instance store-backed instance fails or terminates, it cannot be restored. If you plan to use Amazon EC2 instance store-backed instances, we highly recommend that you distribute the data on your instance stores across multiple Availability Zones. You should also back up critical data from your instance store volumes to persistent storage on a regular basis.

## Amazon EBS-backed instances

Instances that use Amazon EBS for the root device automatically have an Amazon EBS volume attached. When you launch an Amazon EBS-backed instance, we create an Amazon EBS volume for each Amazon EBS snapshot referenced by the AMI you use. You can optionally use other Amazon EBS volumes or instance store volumes, depending on the instance type.

An Amazon EBS-backed instance can be stopped and later restarted without affecting data stored in the attached volumes. 

# EFS - Elastic File System

* Managed NFS (network file system) that can be mounted on hundreds of EC2 at a time so it makes it **shared filed system**.
* Works only with Linux EC2 and multi-AZ.
* Highly available (multiple Availability Zones), scalable, expensive (3x gp2), pay per use, no capacity planning.
* It is regional resource
* Automatically scales (if more space is needed)
* To access EFS file systems from on-premises, you must have an AWS Direct Connect or AWS VPN connection between your on-premises datacenter and your Amazon VPC. **You mount an EFS file system on your on-premises Linux server** using the standard Linux mount command for mounting a file system

![003-efs.png](./images/003-efs.png)

# EFS Infrequent Access (EFS-IA)

* Storage class that is cost-optimized for files not accessed every day
* Up to 92% lower cost compared of EFS Standard
* EFS will automatically move your files EFS-IA based on the list time there were accessed
  * enable EFS-IA with Lifecycle Policy
  * example: move files that are not accessed for 60 days to EFS-IA
* transparent to the applications accessing EFS

![003-efs-ia.png](./images/003-efs-ia.png)

# EC2 Instance store

* It is a name of hardware that is attached to the phisical server.
* Standard EBS volumes are network drives with good but "limited" performance.
* If you new a high-performance hardware disk, use **EC2 instance store**.
* Better I/O performance
* EC2 Instance Store lose their storage if they are stopped (ephemeral)
* Good for buffer / scratch data / temporary content
* Risk of data loss if hardware fails
* Backups and repolication are your responsibility

# Shared responsibility model for EC2 storage

![004-responsibility-model.png](./images/004-responsibility-model.png)

# Amazon FSx

* launch 3rd party high-performance file systems on AWS
* fully managed service
* 3 options
  * FXs for Lustre
    * for Hight Performance Computing (HPC)
    * the name Lustre is derived from "Linux" and "cluster:
    * ML, analytics, video processing, financial modeling...
    * Scales up to 100s GB/s, millions of IOPS, sub-ms latencies

    ![006-FSx-for-Lustre.png](./images/006-FSx-for-Lustre.png)
    
  * FSx for Windows File Server
    * Windows native file system
    * Built on File Server
    * Supports SMB and Windows NTFS
    * Integrity with Microsoft AD
    * Can be accessed from AWS or your on-premise infrastructure

    ![005-FSx-for-Windows.png](./images/005-FSx-for-Windows.png)

  * FSx for NetApp ONTAP