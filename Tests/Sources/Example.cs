using System;
using SignalBusT;

namespace Tests
{
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
}