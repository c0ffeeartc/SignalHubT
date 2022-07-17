using System;
using SignalHubT;

namespace Tests
{
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
}