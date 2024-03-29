# Introduction

* DynamoDB is designed for OLTP

# Consistency

Consistency is the ability to read data with the understanding that all prior writes will be reflected in the results returned. **Reads can be “strongly” consistent or “eventually” consistent**.  Let's  walk through an example of the difference with the diagram below.

* The client writes an update to Key1, and it is durable persisted. The copy in AZ A is one of those written to immediately. The copu in AZ B has not yet been replicated.

    ![001-consistency.png](./images/001-consistency.png)

* When a client wants to read the item an **eventually consistency read request (this is default behavior)**, DynamoDB may choose to route the request to either of AZ copies shown - the returned result may be the updated value "B", or it may be the stale value "A".

    ![002-consistency.png](./images/002-consistency.png)

* In the strong consistent read request scenario, DynamoDB ensures that the request is routed to an AZ copy that is known to have the lates updates.

    ![003-consistency.png](./images/003-consistency.png)

    It’s tempting to want to use strongly consistent reads all the time – but DynamoDB charges more for them because the work is concentrated on a smaller number of replicated copies. Also, there is the chance that in some failure scenarios, SC reads may be briefly unavailable. Finally, a SC read may not inherently provide the kinds of assurances you think it might – it is not a locking mechanism. If you make a SC read and then come back to write to the same item based on the data you read, you might find that another client made a different update in the interim.

# Read and Write Capacity Units

You must specify read and write throughput values when you create a table. DynamoDB reserves the necessary resources to handle your throughput requirements and divides the throughput evenly among partitions. 

* **Read** capacity units (RCU) - the number of strongly consistent reads **per second** of items up to 4 KB in size
  * Eventually consistency consumes half as many RCUs
* **Write** capacity units (WCU) -  the number of 1 KB writes **per second**
  * Note that updating a **single attribute in an item requires writing the entire item**. Your throughput is generally evenly divided among your partitions – so it is important to design for requests which are evenly distributed across your keys.

DynamoDB has features called **Burst** (which is like carry-over minutes on a cellular plan), and **Adaptive Capacity** (which is intelligent compensation allowing you to “borrow” unused throughput from less active keys to cover the needs of a key which is busier).

Be aware that a **single item can never be read at more than 3000 RCU**, **or written at more than 1000 WCU** (or a linear combination of the two).

# Basic Item Requests

* Write
  * **PutItem** – Write item to specified primary key.
  * **UpdateItem** – Change attributes for item with specified primary key.
  * **BatchWriteItem** – Write bunch of items to the specified primary keys.
  * **DeleteItem** – Remove item associated with specified primary key.
* Read
  * **GetItem** – Retrieve item associated with specified primary key.
  * **BatchGetItem** – Retrieve items with this bunch of specified primary keys.
  * **Query** – For specified **partition key**, retrieve items matching **sort key** expression (forward/reverse order).
  * **Scan** – Give me every item in my table.

# Secondary Indexes

## Local Secondary Indexes

* Index is local to a partition key. An LSI **always has the same partition key as the base table**. They live on the same partitions with the base table, and they share the provisioned capacity of the base table.
  * The partition key is the same as the table’s partition key. The sort key can be any scalar attribute.
* **Allows you to query items with the same partition key – specified with the query.** All the items with a particular partition key in the table and the items in the corresponding local secondary index (together known as an item collection) are stored on the same partition. The total size of an item collection cannot exceed 10 GB.
* Can only be created when a table is created and cannot be deleted.
* Supports eventual consistency and strong consistency.
* Does not have its own provisioned throughput.
* **Queries can return attributes that are not projected into the index.**

![004-indexes.png](./images/004-indexes.png)

## Global Secondary Indexes

* Index is across all partition keys
  * Think of a GSI as another completely separate table that DynamoDB replicates to from the base table.
* **Allows you to query over the entire table, across all partitions**
* Can have a partition key and optional sort key that are different from the partition key and sort key of the original table.
* Key values do not need to be unique.
* Can be created when a table is created or can be added to an existing table and it can be deleted.
* **Supports eventual consistency only.**
* Has its own provisioned throughput settings for read and write operations.
* **Queries only return attributes that are projected into the index.**

![005-indexes.png](./images/005-indexes.png)

# DynamoDB Streams

A DynamoDB stream is an ordered flow of information about changes to a table. The records in the stream are strictly in the order in which the changes occurred. Each change contains exactly one stream record. A stream record is available for 24 hours.

DynamoDB streams are a lot like Kinesis streams – in fact, they are compatible with the Kinesis Client Library. All writes to the table are recorded in the stream – **like a changelog**.

Example:  an application makes changes to user preferences that are stored in a DynamoDB table. Another application such as an ad server responds to changes in user preferences and presents different advertisements. The ad server application can read information about changes from the DynamoDB stream and present advertisements corresponding to the new preferences.

![006-streams.png](./images/006-streams.png)

# Resources

* https://www.alexdebrie.com/posts/dynamodb-partitions/