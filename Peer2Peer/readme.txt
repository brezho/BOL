Story
-----
a story is like a profile
a story is made of events which are published as part of a transaction
transactions know their predecessor(s)


Each story as a unique Id
One single node is in charge of keeping the story
The story is replicated amongst a set of replicas (2, 4 or 6...)



Questions
---------
Is it required to maitain a list of all peers on each node??

Connecting
----------
1. try to connect to the last peers you knew



List of peers is shared : 
	amongst all nodes or should a peers management service be 'lightly' distributed ? eg: 500 nodes 



Ids are replicated on each nodes, they can potentially be created in advance
