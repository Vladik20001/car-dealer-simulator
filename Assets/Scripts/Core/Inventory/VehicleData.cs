using System;

namespace CarDealerSimulator.Core.Inventory
{
    /// <summary>
    /// Data model for a vehicle in the dealership.
    /// Implements IInventoryItem so it can be stored in any Inventory container.
    /// </summary>
    [Serializable]
    public class VehicleData : IInventoryItem
    {
        public string Id { get; set; }
        public string DisplayName { get; set; }
        public decimal BaseValue { get; set; }
        public ItemCategory Category => ItemCategory.Vehicle;

        public string Make;
        public string Model;
        public int Year;
        public float Mileage;
        public float ConditionPercent;
        public VehicleType Type;
        public int DaysInInventory;

        public VehicleData(string id, string make, string model, int year, decimal baseValue)
        {
            Id = id;
            Make = make;
            Model = model;
            Year = year;
            BaseValue = baseValue;
            DisplayName = $"{year} {make} {model}";
            ConditionPercent = 100f;
            Mileage = 0f;
            DaysInInventory = 0;
        }
    }

    public enum VehicleType
    {
        Sedan,
        SUV,
        Truck,
        Sports,
        Luxury,
        Economy,
        Van
    }
}
