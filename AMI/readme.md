# AMI process for creating custom AMIs

* Start an EC2 instance and customize it
* Stop the instance (for data integrity)
* Build an AMI - this will also create EBS snapshot
* Launc instances from other AMIs

![001-create-custom-ami.png](./images/001-create-custom-ami.png)

* In first EC2 instance we add a script that installs some packages

  ```sh
  #!/bin/bash
  # Use this for your user data (script from top to bottom)
  # install httpd (Linux 2 version)
  yum update -y
  yum install -y httpd
  systemctl start httpd
  systemctl enable httpd
  ```  

* Next launch the instance
* Next create a custom image from this EC2 instance

  ![002-create-custom-ami.png](./images/002-create-custom-ami.png)

* When the image will be available run the instance from it.
  This time http server will be available much faster because it will not be installed during system bootstrap

  ![003-create-custom-ami.png](./images/003-create-custom-ami.png)

  Now in user data we do not have to install http server and we can just past http files.

  ```sh
  echo "<h1>Hello World from $(hostname -f)</h1>" > /var/www/html/index.html
  ```

# EC2 Image builder

* Used to automate the creation of Virtual Machines or container images
* => Automate the creation, maintain, validate and test EC2 AMIs
* Can be run on a schedule
* Free service (only pay for the underlying resources)

![004-ec2-image-builder.png](./images/004-ec2-image-builder.png)

## EC2 Image builder hands-on screens

![005-ec2-image-builder.png](./images/005-ec2-image-builder.png)

![006-ec2-image-builder.png](./images/006-ec2-image-builder.png)

![007-ec2-image-builder.png](./images/007-ec2-image-builder.png)

![008-ec2-image-builder.png](./images/008-ec2-image-builder.png)

![009-ec2-image-builder.png](./images/009-ec2-image-builder.png)

![010-ec2-image-builder.png](./images/010-ec2-image-builder.png)

![011-ec2-image-builder.png](./images/011-ec2-image-builder.png)

We can select from a list which components should be installed on the new custom image.
If needed we can create also own custom components.
![012-ec2-image-builder.png](./images/012-ec2-image-builder.png)

![013-ec2-image-builder.png](./images/013-ec2-image-builder.png)

![014-ec2-image-builder.png](./images/014-ec2-image-builder.png)

Next we can select how we want test the new image.

![015-ec2-image-builder.png](./images/015-ec2-image-builder.png)

![016-ec2-image-builder.png](./images/016-ec2-image-builder.png)

![017-ec2-image-builder.png](./images/017-ec2-image-builder.png)

![018-ec2-image-builder.png](./images/018-ec2-image-builder.png)

If we want create custom infra and own IAM Role then this role must have a required policies which are listed in default infra. configuration
![019-ec2-image-builder.png](./images/019-ec2-image-builder.png)

![021-ec2-image-builder.png](./images/021-ec2-image-builder.png)

![022-ec2-image-builder.png](./images/022-ec2-image-builder.png)

![023-ec2-image-builder.png](./images/023-ec2-image-builder.png)

In last step we select AMI distribution settings.
![024-ec2-image-builder.png](./images/024-ec2-image-builder.png)

Next we can run created pipeline.
![025-ec2-image-builder.png](./images/025-ec2-image-builder.png)

During this time we can see that EC2 instance for building a new image is running.
![026-ec2-image-builder.png] (./images/026-ec2-image-builder.png)
![027-ec2-image-builder.png](./images/027-ec2-image-builder.png)

Next status of the pipeline is testing.
![028-ec2-image-builder.png](./images/028-ec2-image-builder.png)

We can see now that the builder instance is terminating and new test instance is created.
![029-ec2-image-builder.png](./images/029-ec2-image-builder.png)

Last step is distribution the new image.
![030-ec2-image-builder.png](./images/030-ec2-image-builder.png)