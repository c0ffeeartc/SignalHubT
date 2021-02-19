# SubHub
Not yet ready work-in-progress single-threaded EventAggregator/EventBroker C# solution

## TODO
  - Pooling for Subscriptions and Messages
  - Subscribe/unsubscribe during triggering
    - Deterministic handling
    - Cover with tests
    - Explain in README
  - Recursive triggering
    - Deterministic handling
    - Cover with tests
    - Explain in README
  - Test order of matching priority subscriptions when `SortedDictionary` gets rehashed. Maybe introduce Subscrition.creationIndex for additional sorting value
  - Improve API
  - IoC initialization tree

## Features
  - Global and Filtered subscription
    - Global are triggered on all messages of matching Type
    - Filtered are triggered only on messages of matching Type and matching filter object
    - Global and Filtered subscriptions are stored in same queue, unlike some other solutions
  - Subscription Priority
    - uses `SortedDictionary` for performance
  - Unsubscribe by object handle
    - allows unsubscription from middle of queue

## Example
```csharp
public class Example
{
  private void SubPublishUnsub()
  {
    var sub1 = SubHub<Message1>.I.Sub( Handle1 );
    var sub2 = SubHub<Message2>.I.Sub( Handle2 );
    var sub1Filtered = SubHub<Message1>.I.Sub( filter: this, Handle1Filtered );
    var sub1Priority = SubHub<Message1>.I.Sub( Handle1Priority, order: -5 );

    SubHub<Message1>.I.Publish(new Message1()); // Callbacks: Handle1Priority(), Handle1()
    SubHub<Message1>.I.Publish(this, new Message1());// Callbacks: Handle1Priority(), Handle1(), Handle1Filtered()
    SubHub<Message2>.I.Publish(new Message2()); // Callbacks: Handle2

    SubHub<Message1>.I.Unsub(sub1);
    SubHub<Message2>.I.Unsub(sub2);
    SubHub<Message1>.I.Unsub(sub1Filtered);
    SubHub<Message1>.I.Unsub(sub1Priority);
  }

  private void Handle1(Message1 message) { /*Some code here*/ }
  private void Handle1Filtered(Message1 message) { /*Some code here*/ }
  private void Handle1Priority(Message1 message) { /*Some code here*/ }
  private void Handle2(Message2 message) { /*Some code here*/ }
}

public class Message1 : IMessage {public string Str;}
public class Message2 : IMessage {public int Foo;}
```
