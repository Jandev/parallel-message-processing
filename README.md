# Project name

I'm creating multiple projects to do parallel processing of messages on a queue.

## Console applications with listener (PeekLock)

This will consist of 2 console applications, `ConsoleProcessorPeekLockOne` and `ConsoleProcessorPeekLockTwo`, which listen to an Azure Service Bus Queue and pick up messages using the `PeekLock` method.

## Console applications with listener (ReceiveAndDelete)

This will consist of 2 console applications, `ConsoleProcessorReceiveAndDeleteOne` and `ConsoleProcessorReceiveAndDeleteTwo`, which listen to an Azure Service Bus Queue and pick up messages using the `ReceiveAndDelete` method.

## Azure Function with Service Bus Trigger

This will consist of a single Function App which will scale automatically in the cloud.

## Console applications Akka.NET actors (PeekLock)

This will consist of 2 console applications, `ConsoleProcessorActorPeekLockOne` and `ConsoleProcessorActorPeekLockTwo`, which listen to an Azure Service Bus Queue and pick up messages using the `PeekLock` method.  
The actual processor of messages will throw an exception at random moments, to fake the starvation of an actor.

## Console applications Akka.NET actors (ReceiveAndDelete)

This will consist of 2 console applications, `ConsoleProcessorReceiveAndDeleteLockOne` and `ConsoleProcessorActorReceiveAndDeleteTwo`, which listen to an Azure Service Bus Queue and pick up messages using the `ReceiveAndDelete` method.  
The actual processor of messages will throw an exception at random moments, to fake the starvation of an actor.

## Processor

This tool will fill all the queues with 10.000 messages to be processed.

# Set up

What you need for this is the following:

- SQL Azure server
  - Table: Logging
    - Id - Guid
    - Processor - nvarchar(100)
    - MessageId - int
    - Created - DateTime
- Service Bus namespace with the following queues:
  - console-peeklock
  - console-receiveanddelete
  - function-queueu
  - console-actor-peeklock
  - console-actor-receiveanddelete
- Function App

For simplicity only secrets and keys are used for connectionstrings, which are stored in the configuration of the projects.

# Sample usage

See what it does
