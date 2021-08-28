# SignalHubT
Single-threaded EventAggregator/EventBroker C# solution

## How it works
`SignalBus<T>` stores collection of `ISubscriptions` to specific `ISignalData` type.

`SignalHub` stores collection of `SignalBus<T>` and provides interface to send `ISignalData` through list of `ISubscriptions`.

## Features
  - Global and Filtered subscription
    - Global are triggered on all messages of matching Type
    - Filtered are triggered only on messages of matching Type and matching filter object
    - Global and Filtered subscriptions are ordered in same queue
  - Subscription PriorityOrder
    - uses [C5.TreeSet](https://github.com/sestoft/C5) for performance
  - Unsubscribe by object handle
    - allows unsubscribing handle from middle of queue
  - Automatic pooling for Subscriptions
  - ISignalData types are short lived objects that are passed through invokation call stack
    - use `SignalHub.I.Pub( new SignalData() )` for structs or not `IPoolable` signalData classes
        - struct signalDatas are passed by reference between subscriptions
        - `Pub` returns modified struct signalData
    - use `SignalHub.I.Publish( SignalHub.I.Args<SignalData>().Init(value1) );` for `IPoolable` signalData classes
  - Allows more than 1 EventAggregator through `SignalHubLocal` instances
  - `IoC` AbstractFactory/Facade class wraps concrete classes into virtual methods, allowing inheriting `IoC` to override them
  - Subscribing to currently publishing ISignalData behavior:
    - will run new subscriptions with SAME or HIGHER priorityOrder compared to priorityOrder of currently invoked subscription
    - will NOT run new subscriptions with LOWER priorityOrder compared to priorityOrder of currently invoked subscription
  - Publishing signalData inside callback works as regular call stack(runs new publish as separate call, then continues where it left off)
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
    var sub = SignalHub.I.Sub<Message>( Handle );
    var subFiltered = SignalHub.I.Sub<Message>( filter: this, HandleFiltered );
    var subPriority = SignalHub.I.Sub<Message>( HandlePriority, order: -5 );

    { // Pub - to publish message. Callbacks: HandlePriority(), Handle()
      SignalHub.I.Pub(new Message("Publish m1"));
      // Pub with filter. Callbacks: HandlePriority(), Handle(), HandleFiltered()
      SignalHub.I.Pub(filter:this, new Message("Publish filtered m2"));
    }

    { // Args with Publish - for publishing IPoolable message
      var m2 = SignalHub.I.Args<MessagePoolable>() // gets MessagePoolable from pool
          .Init("Publish poolable message");
      SignalHub.I.Publish(m2); // after processing puts MessagePoolable back into pool
    }

    // Unsubscribe
    SignalHub.I.Unsub(sub);
    SignalHub.I.Unsub(subFiltered);
    SignalHub.I.Unsub(subPriority);
  }

  private void Handle(ref Message message) { /*Some code here*/ }
  private void HandleFiltered(ref Message message) { /*Some code here*/ }
  private void HandlePriority(ref Message message) { /*Some code here*/ }
}

// struct messages are recommended for less Garbage Collection and no Pooling
public struct Message : ISignalData
{
  public Message(string value)
  {
    Value = value;
  }
  public string Value;
}

public class MessagePoolable : ISignalData, IPoolable
{
  public string Str;

  #region Implement IPoolable
  public MessagePoolable Init(string str) // Helper method
  {
    Str = str;
    return this;
  }

  public Boolean IsInPool { get; set; }
  public void BeforeRent( )
  {
  }

  public void BeforeRepool( )
  { // Free resources here
    Str = null;
  }
  #endregion
}
```

## TODO
  - Cover with tests case when subscriptions collection gets rehashed and keeps order of elements in matching priority chains.
