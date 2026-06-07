using System;
using System.Collections.Generic;

namespace CarDealerSimulator.Core.Events
{
    /// <summary>
    /// Lightweight event bus that decouples publishers from subscribers.
    /// Avoids duplicated event wiring across managers by providing a centralized dispatch.
    /// </summary>
    public static class GameEventBus
    {
        private static readonly Dictionary<Type, List<Delegate>> _subscribers = new();

        public static void Subscribe<T>(Action<T> handler) where T : struct, IGameEvent
        {
            var type = typeof(T);
            if (!_subscribers.ContainsKey(type))
            {
                _subscribers[type] = new List<Delegate>();
            }

            _subscribers[type].Add(handler);
        }

        public static void Unsubscribe<T>(Action<T> handler) where T : struct, IGameEvent
        {
            var type = typeof(T);
            if (_subscribers.ContainsKey(type))
            {
                _subscribers[type].Remove(handler);
            }
        }

        public static void Publish<T>(T gameEvent) where T : struct, IGameEvent
        {
            var type = typeof(T);
            if (!_subscribers.ContainsKey(type))
                return;

            foreach (var subscriber in _subscribers[type].ToArray())
            {
                ((Action<T>)subscriber)?.Invoke(gameEvent);
            }
        }

        public static void Clear()
        {
            _subscribers.Clear();
        }

        public static void Clear<T>() where T : struct, IGameEvent
        {
            var type = typeof(T);
            if (_subscribers.ContainsKey(type))
            {
                _subscribers[type].Clear();
            }
        }
    }

    /// <summary>
    /// Marker interface for all game events.
    /// </summary>
    public interface IGameEvent { }

    // --- Common game events ---

    public struct VehiclePurchasedEvent : IGameEvent
    {
        public string VehicleId;
        public decimal PurchasePrice;
    }

    public struct VehicleSoldEvent : IGameEvent
    {
        public string VehicleId;
        public decimal SalePrice;
        public decimal Profit;
    }

    public struct BalanceChangedEvent : IGameEvent
    {
        public decimal OldBalance;
        public decimal NewBalance;
        public decimal Delta;
    }

    public struct InventoryChangedEvent : IGameEvent
    {
        public string ItemId;
        public int OldCount;
        public int NewCount;
    }

    public struct CustomerArrivedEvent : IGameEvent
    {
        public string CustomerId;
        public string DesiredVehicleType;
        public decimal Budget;
    }

    public struct DayAdvancedEvent : IGameEvent
    {
        public int CurrentDay;
    }
}
