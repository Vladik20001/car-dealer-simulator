using System;
using System.Collections.Generic;
using System.Linq;
using CarDealerSimulator.Core.Events;

namespace CarDealerSimulator.Core.Inventory
{
    /// <summary>
    /// Generic inventory container with capacity, filtering, and event publishing.
    /// Reusable for dealer lot, garage, parts storage, etc.
    /// </summary>
    public class Inventory<T> where T : IInventoryItem
    {
        private readonly List<T> _items = new();
        private readonly int _maxCapacity;

        public int Count => _items.Count;
        public int MaxCapacity => _maxCapacity;
        public int RemainingSpace => _maxCapacity - _items.Count;
        public bool IsFull => _items.Count >= _maxCapacity;
        public IReadOnlyList<T> Items => _items.AsReadOnly();

        public Inventory(int maxCapacity = 50)
        {
            _maxCapacity = maxCapacity;
        }

        public bool TryAdd(T item)
        {
            if (IsFull)
                return false;

            _items.Add(item);

            GameEventBus.Publish(new InventoryChangedEvent
            {
                ItemId = item.Id,
                OldCount = _items.Count - 1,
                NewCount = _items.Count
            });

            return true;
        }

        public bool TryRemove(T item)
        {
            bool removed = _items.Remove(item);

            if (removed)
            {
                GameEventBus.Publish(new InventoryChangedEvent
                {
                    ItemId = item.Id,
                    OldCount = _items.Count + 1,
                    NewCount = _items.Count
                });
            }

            return removed;
        }

        public T FindById(string id)
        {
            return _items.FirstOrDefault(item => item.Id == id);
        }

        public IEnumerable<T> FindByCategory(ItemCategory category)
        {
            return _items.Where(item => item.Category == category);
        }

        public IEnumerable<T> FindByPriceRange(decimal minPrice, decimal maxPrice)
        {
            return _items.Where(item => item.BaseValue >= minPrice && item.BaseValue <= maxPrice);
        }

        public decimal GetTotalValue()
        {
            return _items.Sum(item => item.BaseValue);
        }

        public void Sort(Comparison<T> comparison)
        {
            _items.Sort(comparison);
        }

        public void Clear()
        {
            _items.Clear();
        }
    }
}
