using System;

namespace CarDealerSimulator.Models
{
    public enum CustomerType
    {
        Budget,
        Standard,
        Premium
    }

    public class Customer
    {
        public string Name { get; }
        public CustomerType Type { get; }
        public float Budget { get; }
        public string PreferredMake { get; }

        public Customer(string name, CustomerType type, float budget, string preferredMake = null)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Name cannot be null or empty.", nameof(name));
            if (budget <= 0)
                throw new ArgumentOutOfRangeException(nameof(budget), "Budget must be positive.");

            Name = name;
            Type = type;
            Budget = budget;
            PreferredMake = preferredMake;
        }

        public bool CanAfford(float price)
        {
            return price <= Budget;
        }

        public float GetWillingnessToPay(Vehicle vehicle)
        {
            if (vehicle == null)
                throw new ArgumentNullException(nameof(vehicle));

            float marketValue = vehicle.GetMarketValue();
            float willingness = Type switch
            {
                CustomerType.Budget => marketValue * 0.85f,
                CustomerType.Standard => marketValue * 1.0f,
                CustomerType.Premium => marketValue * 1.15f,
                _ => marketValue
            };

            if (!string.IsNullOrEmpty(PreferredMake) &&
                string.Equals(vehicle.Make, PreferredMake, StringComparison.OrdinalIgnoreCase))
            {
                willingness *= 1.1f;
            }

            return Math.Min(willingness, Budget);
        }

        public bool WouldBuy(Vehicle vehicle, float askingPrice)
        {
            if (vehicle == null)
                throw new ArgumentNullException(nameof(vehicle));

            float willingness = GetWillingnessToPay(vehicle);
            return askingPrice <= willingness;
        }
    }
}
