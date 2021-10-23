# Assume role

## Introduction

It is AWS best practice to use roles to grant limited access to certain resources for a limited period of time. Thanks to these IAM users can be deleted and created again to increase security (IAM users credentials are stored in c:\Users\[user-name]\.aws\credentials file).

Additionally if some resources are created using only IAM users (without using role) then we might loose access to these resources if IAM user that created them was deleted. Such resource is EKS then root aws account has to be used to again get access to the EKS but again because of security reasons root aws account should be use as less as possible.


# Links
https://aws-blog.de/2021/08/iam-what-happens-when-you-assume-a-role.html