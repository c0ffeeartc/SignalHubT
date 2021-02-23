# SubHubT
Not battle tested work-in-progress single-threaded EventAggregator/EventBroker C# solution

## TODO
  - IoC initialization tree
  - Allow more than 1 EventAggregator object by not using static objects. SubHubLocal class?
  - Cover with tests case when subscriptions collection gets rehashed and keeps order of elements in matching priority chains.
  - Improve API
  - Add performance tests

## Features
  - Global and Filtered subscription
    - Global are triggered on all messages of matching Type
    - Filtered are triggered only on messages of matching Type and matching filter object
    - Global and Filtered subscriptions are ordered in same queue
  - Subscription PriorityOrder
    - uses `SortedList` for performance
  - Unsubscribe by object handle
    - allows unsubscribing handle from middle of queue
  - Pooling for Subscriptions and Messages
  - Subscribing to currently publishing Message behavior:
    - will run new subscriptions with SAME or HIGHER priorityOrder compared to priorityOrder of currently invoked subscription
    - will NOT run new subscriptions with LOWER priorityOrder compared to priorityOrder of currently invoked subscription
  - Publishing message inside callback works as regular call stack(runs new publish as separate call, then continues where it left off)
    - Example:
        - There are 3 subscriptions to type MessageA: s1, s2, s3
        - s2 has `Publish(MessageA m2)` inside its callback
        - some function calls `Publish(MessageA m1)` 
        - callbacks trigger in order : s1(m1), s2(m1), s1(m2), s2(m2), s3(m2), s3(m1)

## Usage
```csharp
public class Example
{
  private void SubPublishUnsub()
  {
    var sub1 = SubHub.I.Sub<Message1>( Handle1 );
    var sub2 = SubHub.I.Sub<Message2>( Handle2 );
    var sub1Filtered = SubHub.I.Sub<Message1>( filter: this, Handle1Filtered );
    var sub1Priority = SubHub.I.Sub<Message1>( Handle1Priority, order: -5 );

    SubHub.I.Publish(new Message1()); // Callbacks: Handle1Priority(), Handle1()
    SubHub.I.Publish(this, new Message1());// Callbacks: Handle1Priority(), Handle1(), Handle1Filtered()
    SubHub.I.Publish(new Message2()); // Callbacks: Handle2

    SubHub.I.Unsub(sub1);
    SubHub.I.Unsub(sub2);
    SubHub.I.Unsub(sub1Filtered);
    SubHub.I.Unsub(sub1Priority);
  }

  private void Handle1(Message1 message) { /*Some code here*/ }
  private void Handle1Filtered(Message1 message) { /*Some code here*/ }
  private void Handle1Priority(Message1 message) { /*Some code here*/ }
  private void Handle2(Message2 message) { /*Some code here*/ }
}

public class Message1 : IMessage {public string Str;}
public class Message2 : IMessage {public int Foo;}
```
