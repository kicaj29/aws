# ECS

* ECS = Elastic Container Service
  * https://explore.skillbuilder.aws/learn/course/internal/view/elearning/915/building-and-deploying-containers-using-amazon-elastic-container-service
* Launch Docker containers on AWS
* You must provision & maintain the infrastructure (EC2 instances)
* AWS takes care of starting/stopping containers
* Has integrations with the Application Load Balancer
* ECS vs EKS: https://www.clickittech.com/aws/amazon-ecs-vs-eks/

![01-ECS-service.png](./images/01-ECS-service.png)

# Fargate

* Launch Docker containers on AWS
* You do not have to provision the infrastructure (no EC2 instances to manage)
* Serverless offering - AWS just runs containers for you based on the CPU/RAM you need
* **Can be used for both ECS and EKS**

![02-Fargate.png](./images/02-Fargate.png)