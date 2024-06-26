- [Introduction](#introduction)
- [ElastiCache for Redis](#elasticache-for-redis)
  - [Redis Cluster Modes: Enabled and Disabled](#redis-cluster-modes-enabled-and-disabled)
  - [Create ElastiCache for Redis Serverless](#create-elasticache-for-redis-serverless)
    - [ElastiCache for Redis Serverless `Customize default settings`](#elasticache-for-redis-serverless-customize-default-settings)
  - [Create ElastiCache for Redis, design your own cache - easy create](#create-elasticache-for-redis-design-your-own-cache---easy-create)
  - [Create ElastiCache for Redis, design your own cache, cluster cache, cluster mode disabled](#create-elasticache-for-redis-design-your-own-cache-cluster-cache-cluster-mode-disabled)
  - [Create ElastiCache for Redis, design your own cache, cluster cache, cluster mode enabled](#create-elasticache-for-redis-design-your-own-cache-cluster-cache-cluster-mode-enabled)
- [ElastiCache for Memcached](#elasticache-for-memcached)
  - [Create ElastiCache for Memcached Serverless](#create-elasticache-for-memcached-serverless)
    - [Memcached Serverless `Customize default settings`](#memcached-serverless-customize-default-settings)
  - [Create ElastiCache for Memcached, design your own cache - easy create](#create-elasticache-for-memcached-design-your-own-cache---easy-create)
  - [Create ElastiCache for Memcached, design your own cache - standard create](#create-elasticache-for-memcached-design-your-own-cache---standard-create)
- [Redis vs Memcached](#redis-vs-memcached)
- [Resources](#resources)

# Introduction

* ElastiCache is to get managed **Redis or Memcached**
* Caches are in-memory databases with high performance, low latency
* Helps reduce load off databases for read intensive workloads
* AWS takes care of OS maintenance / patching, optimizations, setup, configuration, monitoring, failure recovery and backups

![01-elastiCache.png](./images/01-elastiCache.png)

* https://aws.amazon.com/elasticache/redis-vs-memcached/
  ![02-redis-vs-mem-cache.png](./images/02-redis-vs-mem-cache.png)

# ElastiCache for Redis

* https://docs.aws.amazon.com/AmazonElastiCache/latest/red-ug/WhatIs.html

## Redis Cluster Modes: Enabled and Disabled

https://docs.aws.amazon.com/whitepapers/latest/database-caching-strategies-using-redis/redis-cluster-modes-enabled-and-disabled.html

*Cluster Mode for Amazon ElastiCache for Redis is a feature that enables you to scale your Redis cluster without any impact on the cluster performance. While you initiate a scale-out operation by adding a specified number of new shards to a cluster, you also initiate scale-up or scale-down operation by selecting a new desired node type, Amazon ElastiCache for Redis a new cluster synchronizing the new nodes with the previous ones. Amazon ElastiCache for Redis supports three cluster configurations:*

* Redis - **single node**
* Redis - **cluster mode disabled**: In a Redis cluster with cluster mode disabled, there is one primary and you can have up to five read replicas in your replication group. Adding or removing replicas incurs no downtime to your application.
* Redis - **cluster mode enabled (sharding)**: With Cluster Mode enabled, your Redis cluster can now scale horizontally (in or out) in addition to scaling vertically (up and down). In a Redis cluster with cluster mode enabled, clusters can have **up to ninety shards by default** (which can be increased if requested) and up to five read replicas in each node group. By default, the hash slots get evenly distributed between shards, but customers can also configure a custom hash slot. 

## Create ElastiCache for Redis Serverless

![03-create-redis-serverless.png](./images/03-create-redis-serverless.png)

![04-create-redis-serverless.png](./images/04-create-redis-serverless.png)

### ElastiCache for Redis Serverless `Customize default settings`

If we select `Customize default settings` then we can select a VPC, AZs, encryption key, security groups, backups and limits.

![31-redis-serverless.png](./images/31-redis-serverless.png)
![32-redis-serverless.png](./images/32-redis-serverless.png)
![33-redis-serverless.png](./images/33-redis-serverless.png)


## Create ElastiCache for Redis, design your own cache - easy create

![05-create-redis-design-own-cache.png](./images/05-create-redis-design-own-cache.png)

![06-create-redis-design-own-cache.png](./images/06-create-redis-design-own-cache.png)

![07-create-redis-design-own-cache.png](./images/07-create-redis-design-own-cache.png)

NOTE: subnet groups can be also created from separated view

![09-create-redis-design-own-cache.png](./images/09-create-redis-design-own-cache.png)

![08-create-redis-design-own-cache.png](./images/08-create-redis-design-own-cache.png)

## Create ElastiCache for Redis, design your own cache, cluster cache, cluster mode disabled

![10-create-redis-design-own-cache.png](./images/10-create-redis-design-own-cache.png)

![11-create-redis-design-own-cache.png](./images/11-create-redis-design-own-cache.png)

![12-create-redis-design-own-cache.png](./images/12-create-redis-design-own-cache.png)

![13-create-redis-design-own-cache.png](./images/13-create-redis-design-own-cache.png)

![14-create-redis-design-own-cache.png](./images/14-create-redis-design-own-cache.png)

![15-create-redis-design-own-cache.png](./images/15-create-redis-design-own-cache.png)

![16-create-redis-design-own-cache.png](./images/16-create-redis-design-own-cache.png)

![17-create-redis-design-own-cache.png](./images/17-create-redis-design-own-cache.png)

## Create ElastiCache for Redis, design your own cache, cluster cache, cluster mode enabled

The same steps like above but here we can specify amount of shards and replicas per shard, plus slots and keyspaces.

![18-create-redis-design-own-cache.png](./images/18-create-redis-design-own-cache.png)

* Equal distribution   
  ![19-create-redis-design-own-cache.png](./images/19-create-redis-design-own-cache.png)

* Custom distribution   
  ![20-create-redis-design-own-cache.png](./images/20-create-redis-design-own-cache.png)

# ElastiCache for Memcached

https://docs.aws.amazon.com/AmazonElastiCache/latest/mem-ug/WhatIs.html

## Create ElastiCache for Memcached Serverless

![21-create-memcached.png](./images/21-create-memcached.png)

![22-create-memcached.png](./images/22-create-memcached.png)

### Memcached Serverless `Customize default settings`

If we select `Customize default settings` then we can select a VPC, AZs, encryption key, security groups, and limits.

![34-memcached-serverless.png](./images/34-memcached-serverless.png)
![35-memcached-serverless.png](./images/35-memcached-serverless.png)
![36-memcached-serverless.png](./images/36-memcached-serverless.png)

## Create ElastiCache for Memcached, design your own cache - easy create

![23-create-memcached.png](./images/23-create-memcached.png)

![24-create-memcached.png](./images/24-create-memcached.png)

## Create ElastiCache for Memcached, design your own cache - standard create

![25-create-memcached.png](./images/25-create-memcached.png)

![26-create-memcached.png](./images/26-create-memcached.png)

![27-create-memcached.png](./images/27-create-memcached.png)

![28-create-memcached.png](./images/28-create-memcached.png)

![29-create-memcached.png](./images/29-create-memcached.png)

![30-create-memcached.png](./images/30-create-memcached.png)

# Redis vs Memcached

* Both have serverless option but only Redis has option `Restore from backup` and only Redis can create backups.
* Both Redis and Memcached always are placed in a VPC also when a serverless option is used.
* Redis has cluster mode which can be enabled or disabled. When cluster mode is enabled then you can specify number ot shards and replicas per shard and slots/keyspaces for shards. Memcached does not have shards with slots/keyspaces but you specify only nodes 
*The number of nodes in this cache cluster. A node is a partition of your data.* without replicas.

# Resources

* https://docs.aws.amazon.com/AmazonElastiCache/latest/mem-ug/Strategies.html