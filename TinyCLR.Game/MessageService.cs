// Copyright (c) 2012 Chris Taylor

// Copyright (c) 2012 Chris Taylor

using System;

namespace TinyCLR.Game
{
  /// <summary>
  /// MessageService class provides pub/sub messaging infrastructure.
  /// Used for publishing notifications of occurences within the system.
  /// 
  /// Using this instead of tradditional events decouples the various components from each other
  /// to the extent that each component is only aware of the messages it is interested in an not 
  /// who is publising the message at any point in time.  
  /// 
  /// Note: Currently this is not designed for high-volume messages. But is fine for ad-hoc in game
  /// notifications.
  /// </summary>
  public class MessageService
  {
        public static MessageService Instance { get; } = new MessageService();

        private readonly MessageSubscriberLookup _subscriptions = new MessageSubscriberLookup();

    /// <summary>
    /// Subscribe to a message type
    /// </summary>
    /// <param name="messageType">Type of message to subscribe to</param>
    /// <param name="handler">Handler method to be called when a message of messageType arrives</param>
    /// <returns>Opaque subscription token used to identify the subscription</returns>
    public object Subscribe(Type messageType, MessageHandler handler)
    {
      return _subscriptions.AddSubscriber(messageType, handler);      
    }

    /// <summary>
    /// Unsubscribe from an exsiting subscription.
    /// 
    /// Not yet implemented.
    /// </summary>
    /// <param name="subscriptionToken">Subscription token returned from the original call to Subscribe.</param>
    public void Unsubscribe(object subscriptionToken)
    {
      // Will implement when needed
      throw new NotImplementedException();
    }

    /// <summary>
    /// Publish a message for delivery to intended subscribers.
    /// </summary>
    /// <param name="message">Message to be published.</param>
    public void Publish(object message)
    {
      Type messageType = message.GetType();
      
      SubscriptionContainer subscribers = _subscriptions.GetSubscribers(messageType);
      if (subscribers != null)
      {
        int count = subscribers.Count;
        for (int i = 0; i < count; ++i)
        {
          subscribers[i].Handler(message);
        }
      }
    }

    class Subscription
    {
      public readonly Type MessageType;
      public readonly MessageHandler Handler;

      public Subscription(Type messageType, MessageHandler handler)
      {
        MessageType = messageType;
        Handler = handler;
      }
    }

    class SubscriberBucket
    {
      public readonly Type MessageType;
      public readonly SubscriptionContainer Subscribers;

      public SubscriberBucket(Type messageType, SubscriptionContainer subscribers)
      {
        MessageType = messageType;
        Subscribers = subscribers;
      }
    }

    class MessageSubscriberLookup
    {
      private const int DefaultSize = 4;

      private SubscriberBucket[] _buckets = new SubscriberBucket[DefaultSize];
      private int _count;
      

      public SubscriptionContainer GetSubscribers(Type messageType)
      {
        for (int i = 0; i < _count; ++i)
        {
          if (_buckets[i].MessageType == messageType)
          {
            return _buckets[i].Subscribers;
          }
        }
        return null;
      }

      public object AddSubscriber(Type messageType, MessageHandler handler)
      {
        var subscribers = GetSubscribers(messageType) ?? AddSubscriptionContainer(messageType);
        var subscriber = new Subscription(messageType, handler); 
        subscribers.Add(subscriber);

        return subscriber;
      }

      private SubscriptionContainer AddSubscriptionContainer(Type messageType)
      {
        var subscribers = new SubscriptionContainer();

        if (_count + 1 >= _buckets.Length)
        {
          SubscriberBucket[] newBuckets = new SubscriberBucket[_buckets.Length * 2];
          Array.Copy(_buckets, newBuckets, _buckets.Length);
          _buckets = newBuckets;          
        }
        _buckets[_count] = new SubscriberBucket(messageType, subscribers);
        _count++;
        return subscribers;
      }
    }

    class SubscriptionContainer
    {
      private const int DefaultSize = 4;

      private Subscription[] _items = new Subscription[DefaultSize];
      private int _count;

      public void Add(Subscription item)
      {
        if (_count + 1 == _items.Length)
        {
          EnsureCapacity(_count + 1);
        }
        _items[_count++] = item;
      }

      public int Count { get { return _count; } }

      public Subscription this[int index]
      {
        get { return _items[index]; }
      }

      private void EnsureCapacity(int required)
      {
        if (_items == null)
        {
          _items = new Subscription[DefaultSize];
        }
        else if (required >= _items.Length)
        {
          Subscription[] newItems = new Subscription[_items.Length * 2];
          Array.Copy(_items, newItems, _items.Length);
          _items = newItems;
        }
      }
    }

    private MessageService() { }
  }

  /// <summary>
  /// Represents the method that will handle messages that the implementer has subscribed to.
  /// </summary>
  /// <param name="message"></param>
  public delegate void MessageHandler(object message);
}
