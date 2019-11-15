# StreamProtoBuf
Attempt to stream protobuf messages and deserialize those without knowing the order of data types coming.

This is just a simple POC for serialization/deserialization of protobuf messages (defined in a *.proto file contract). 
We serialize different types of pre-defined messages on one side of a stream and then we try to deserialize it on the other side. 
The goal was to find a way to be able to deserialize protobuf messages without knowing the order/sequence of their serialization. This is made by embedding the shared message types (*.proto file) that will be shared on both sides in a real world scenario.
