# SignalHubT
Single-threaded EventAggregator/EventBroker C# solution

## How it works
`SignalBus<T>` stores collection of `ISubscriptions` to specific `ISignalData` type.

`SignalHub` stores collection of `SignalBus<T>` and provides interface to send `ISignalData` through sequence of `ISubscriptions`.

## Features
  - Global and Filtered subscription
    - Global are triggered on all messages of matching Type
      - publish triggers only Global subscriptions
    - Filtered are triggered only on messages of matching Type and matching filter object
      - publish triggers Global and Filtered subscriptions
      - Global and Filtered subscriptions are ordered in same queue


  - Subscription PriorityOrder
    - uses [C5.TreeSet](https://github.com/sestoft/C5) for performance


  - Unsubscribe by object handle
    - allows unsubscribing handle from middle of queue


  - Automatic pooling for Subscriptions


  - ISignalData types are short lived objects that are passed through invoke call stack
    - use `SignalHub.I.Pub( new SignalData() )` for publishing message
        - struct signalDatas are passed by reference between subscriptions
        - `Pub` returns modified struct signalData


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
  private void SubPubUnsub()
  {
    // Subscribe
    var subFiltered = SignalHub.I.Sub<Message>( filter: this, OnMessageFiltered );
    var sub = SignalHub.I.Sub<Message>( OnMessage );
    var subPriority = SignalHub.I.Sub<Message>( OnMessagePriority, order: -5 );

    // Publish
    SignalHub.I.Pub(new Message("no filter")); // Callbacks: HandlePriority(), Handle()
    SignalHub.I.Pub(filter: this, new Message("with filter")); // Callbacks: HandlePriority(), HandleFiltered(), Handle()

    // Unsubscribe
    SignalHub.I.Unsub(sub);
    SignalHub.I.Unsub(subFiltered);
    SignalHub.I.Unsub(subPriority);
  }

  private void OnMessage(ref Message message) {  }
  private void OnMessageFiltered(ref Message message) {  }
  private void OnMessagePriority(ref Message message) {  }
}

// struct messages are recommended for less Garbage Collection
public struct Message : ISignalData
{
  public Message(string value)
  {
    Value = value;
  }

  public string Value;
}
```

## TODO
  - Cover with tests case when subscriptions collection gets rehashed and keeps order of elements in matching priority chains.
