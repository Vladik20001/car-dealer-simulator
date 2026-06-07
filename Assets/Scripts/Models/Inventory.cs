using System;
using System.Collections.Generic;
using System.Linq;

namespace CarDealerSimulator.Models
{
    public class Inventory
    {
        private readonly List<Vehicle> _vehicles = new List<Vehicle>();
        private readonly int _capacity;

        public int Count => _vehicles.Count;
        public int Capacity => _capacity;
        public bool IsFull => _vehicles.Count >= _capacity;
        public IReadOnlyList<Vehicle> Vehicles => _vehicles.AsReadOnly();

        public Inventory(int capacity)
        {
            if (capacity <= 0)
                throw new ArgumentOutOfRangeException(nameof(capacity), "Capacity must be positive.");
            _capacity = capacity;
        }

        public bool Add(Vehicle vehicle)
        {
            if (vehicle == null)
                throw new ArgumentNullException(nameof(vehicle));
            if (IsFull)
                return false;
            if (_vehicles.Any(v => v.Id == vehicle.Id))
                return false;

            _vehicles.Add(vehicle);
            return true;
        }

        public bool Remove(Vehicle vehicle)
        {
            if (vehicle == null)
                throw new ArgumentNullException(nameof(vehicle));
            return _vehicles.Remove(vehicle);
        }

        public Vehicle FindById(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException("Id cannot be null or empty.", nameof(id));
            return _vehicles.FirstOrDefault(v => v.Id == id);
        }

        public List<Vehicle> FindByMake(string make)
        {
            if (string.IsNullOrWhiteSpace(make))
                throw new ArgumentException("Make cannot be null or empty.", nameof(make));
            return _vehicles
                .Where(v => string.Equals(v.Make, make, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        public List<Vehicle> FindByCondition(VehicleCondition condition)
        {
            return _vehicles.Where(v => v.Condition == condition).ToList();
        }

        public List<Vehicle> FindInPriceRange(float minPrice, float maxPrice)
        {
            if (minPrice < 0)
                throw new ArgumentOutOfRangeException(nameof(minPrice));
            if (maxPrice < minPrice)
                throw new ArgumentOutOfRangeException(nameof(maxPrice), "Max must be >= min.");

            return _vehicles
                .Where(v => v.GetMarketValue() >= minPrice && v.GetMarketValue() <= maxPrice)
                .ToList();
        }

        public float GetTotalInventoryValue()
        {
            return _vehicles.Sum(v => v.GetMarketValue());
        }

        public void Clear()
        {
            _vehicles.Clear();
        }
    }
}
