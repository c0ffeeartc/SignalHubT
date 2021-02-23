# SubHubT
Single-threaded EventAggregator/EventBroker C# solution

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
  - Allows more than 1 EventAggregator through SubHLocal instances
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
    // Subscribe
    var sub = SubH.I.Sub<Message>( Handle );
    var subFiltered = SubH.I.Sub<Message>( filter: this, HandleFiltered );
    var subPriority = SubH.I.Sub<Message>( HandlePriority, order: -5 );

    // Publish
    var m1 = SubH.I.Args<Message>();
    // Callbacks: HandlePriority(), Handle()
    SubH.I.Publish(m1.Init("Publish m1"));

    var m2 = SubH.I.Args<Message>();
    // Callbacks: HandlePriority(), Handle(), HandleFiltered()
    SubH.I.Publish(filter:this, m2.Init("Publish filtered m2"));

    // Unsubscribe
    SubH.I.Unsub(sub);
    SubH.I.Unsub(subFiltered);
    SubH.I.Unsub(subPriority);
  }

  private void Handle(Message message) { /*Some code here*/ }
  private void HandleFiltered(Message message) { /*Some code here*/ }
  private void HandlePriority(Message message) { /*Some code here*/ }
}

public class Message : IMessage, IPoolable
{
  public string Str;

  #region Implement IPoolable
  // To ensure compile time correctness even after refactoring:
  //   - always implement init-like method for IPoolable(especially when no arguments)
  //   - always call it every time after poolable is rented
  public Message Init(string str)
  {
    Str = str;
    return this;
  }

  public Boolean IsInPool { get; set; }
  public void BeforeRent( )
  {
  }

  // Free resources here
  public void BeforeRepool( )
  {
    Str = null;
  }
  #endregion
}
```

## TODO
  - IoC initialization tree. AbstractFactory?
  - Cover with tests case when subscriptions collection gets rehashed and keeps order of elements in matching priority chains.
  - Add performance tests
