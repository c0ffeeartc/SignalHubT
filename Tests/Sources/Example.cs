using System;
using SubHubT;

namespace Tests
{
public class Example
{
  private void SubPublishUnsub()
  {
    // Subscribe
    var sub = SubH.I.Sub<Message>( Handle );
    var subFiltered = SubH.I.Sub<Message>( filter: this, HandleFiltered );
    var subPriority = SubH.I.Sub<Message>( HandlePriority, order: -5 );

    // Pub - to publish message. Callbacks: HandlePriority(), Handle()
    SubH.I.Pub(new Message("Publish m1"));
    // Pub with filter. Callbacks: HandlePriority(), Handle(), HandleFiltered()
    SubH.I.Pub(filter:this, new Message("Publish filtered m2"));

    // Args with Publish - for publishing IPoolable message
    var m2 = SubH.I.Args<MessagePoolable>() // gets MessagePoolable from pool
        .Init("Publish poolable message");
    SubH.I.Publish(m2); // after handled puts MessagePoolable back into pool

    // Unsubscribe
    SubH.I.Unsub(sub);
    SubH.I.Unsub(subFiltered);
    SubH.I.Unsub(subPriority);
  }

  private void Handle(ref Message message) { /*Some code here*/ }
  private void HandleFiltered(ref Message message) { /*Some code here*/ }
  private void HandlePriority(ref Message message) { /*Some code here*/ }
}

// struct messages are passed by reference between subscriptions
public struct Message : IMessage
{
	// Having a constructor is highly encouraged for compile time correctness
	public Message(string value)
	{
		Value = value;
	}
	public string Value;
}

public class MessagePoolable : IMessage, IPoolable
{
  public string Str;

  #region Implement IPoolable
  // To ensure compile time correctness even after refactoring:
  //   - always implement init-like method for IPoolable(especially when no arguments)
  //   - always call it every time after poolable is rented
  public MessagePoolable Init(string str)
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
}