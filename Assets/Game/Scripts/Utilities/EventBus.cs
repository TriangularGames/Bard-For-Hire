using System;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Event Bus implementation using delegates.
/// </summary>
public static class EventBus
{
    private static readonly Dictionary<Type, Delegate> subscribers = new Dictionary<Type, Delegate>();
    private static readonly object padlock = new object();

    /// <summary>
    /// Subscribes to an event.
    /// </summary>
    /// <typeparam name="T">The type of the event.</typeparam>
    /// <param name="action">The action to perform when the event is published.</param>
    public static void Subscribe<T>(Action<T> action)
    {
        lock (padlock)
        {
            Type t = typeof(T);
            if (!subscribers.ContainsKey(t))
                subscribers[t] = null;

            subscribers[t] = Delegate.Combine(subscribers[t], action);
        }
    }

    /// <summary>
    /// Unsubscribes from an event.
    /// </summary>
    /// <typeparam name="T">The type of the event.</typeparam>
    /// <param name="action">The action to unsubscribe.</param> 
    public static void Unsubscribe<T>(Action<T> action)
    {
        lock (padlock)
        {
            Type t = typeof(T);
            if (subscribers.ContainsKey(t))
            {
                subscribers[t] = Delegate.Remove(subscribers[t], action);
            }
        }
    }

    /// <summary>
    /// Publishes an event to all subscribers.
    /// </summary>
    /// <typeparam name="T">The type of the event.</typeparam>
    /// <param name="eventData">The event data to publish.</param>
    public static void Publish<T>(T eventData)
    {
        lock (padlock)
        {
            Type t = typeof(T);
            if (subscribers.ContainsKey(t))
            {
                (subscribers[t] as Action<T>)?.Invoke(eventData);
            }
        }
    }
    
}

/// <summary>
/// Event for when a Purchase is made
/// </summary>
public struct PurchaseEvent
{
    public int _amount;

    public PurchaseEvent(int amount)
    {
        _amount = amount;
    }
}

/// <summary>
/// Event for when an Item is purchased
/// </summary>
public struct ItemBoughtEvent
{
    public ItemData data;

    public ItemBoughtEvent(ItemData _data)
    {
        data = _data;
    }
}

/// <summary>
/// Event for when an Upgrade is purchased
/// </summary>
public struct UpgradeBoughtEvent
{
    public UpgradeData data;

    public UpgradeBoughtEvent(UpgradeData _data)
    {
        data = _data;
    }
}

/// <summary>
/// Event for when an Item is being Discarded/Scored
/// </summary>
public struct ItemRemovedEvent
{
    public ItemData item;

    public ItemRemovedEvent(ItemData _item)
    {
        item = _item;
    }
}

/// <summary>
/// Event for when an Item in the shop is Selected
/// </summary>
public struct ItemSelectedEvent
{
    public EntityId id;

    public ItemSelectedEvent(EntityId _id)
    {
        id = _id;
    }
}

/// <summary>
/// Event for when Performance Starts
/// </summary>
public struct PerformanceStartEvent { }

/// <summary>
/// Event for when Performance Ends- Specifically after the Win screen is shown
/// </summary>
public struct PerformanceEndEvent { }

/// <summary>
/// Event for when Round Starts
/// </summary>
public struct RoundStartEvent { }

/// <summary>
/// Event for when Round Ends
/// </summary>
public struct RoundEndEvent { }
