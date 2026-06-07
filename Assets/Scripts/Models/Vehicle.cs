using System;

namespace CarDealerSimulator.Models
{
    public enum VehicleCondition
    {
        Poor,
        Fair,
        Good,
        Excellent
    }

    public class Vehicle
    {
        public string Id { get; }
        public string Make { get; }
        public string Model { get; }
        public int Year { get; }
        public float BasePrice { get; }
        public float Mileage { get; private set; }
        public VehicleCondition Condition { get; private set; }

        private const float MileageDepreciationRate = 0.0001f;
        private const float MinConditionMultiplier = 0.4f;

        public Vehicle(string make, string model, int year, float basePrice, float mileage, VehicleCondition condition)
        {
            if (string.IsNullOrWhiteSpace(make))
                throw new ArgumentException("Make cannot be null or empty.", nameof(make));
            if (string.IsNullOrWhiteSpace(model))
                throw new ArgumentException("Model cannot be null or empty.", nameof(model));
            if (year < 1886 || year > DateTime.Now.Year + 1)
                throw new ArgumentOutOfRangeException(nameof(year), "Year must be between 1886 and next year.");
            if (basePrice <= 0)
                throw new ArgumentOutOfRangeException(nameof(basePrice), "Base price must be positive.");
            if (mileage < 0)
                throw new ArgumentOutOfRangeException(nameof(mileage), "Mileage cannot be negative.");

            Id = Guid.NewGuid().ToString();
            Make = make;
            Model = model;
            Year = year;
            BasePrice = basePrice;
            Mileage = mileage;
            Condition = condition;
        }

        public float GetMarketValue()
        {
            float conditionMultiplier = Condition switch
            {
                VehicleCondition.Poor => 0.5f,
                VehicleCondition.Fair => 0.7f,
                VehicleCondition.Good => 0.9f,
                VehicleCondition.Excellent => 1.0f,
                _ => 0.5f
            };

            float mileageFactor = Math.Max(MinConditionMultiplier, 1f - Mileage * MileageDepreciationRate);
            int age = DateTime.Now.Year - Year;
            float ageFactor = Math.Max(MinConditionMultiplier, 1f - age * 0.03f);

            return BasePrice * conditionMultiplier * mileageFactor * ageFactor;
        }

        public void AddMileage(float miles)
        {
            if (miles < 0)
                throw new ArgumentOutOfRangeException(nameof(miles), "Cannot add negative mileage.");
            Mileage += miles;
        }

        public void Repair()
        {
            if (Condition < VehicleCondition.Excellent)
                Condition = Condition + 1;
        }

        public void Degrade()
        {
            if (Condition > VehicleCondition.Poor)
                Condition = Condition - 1;
        }

        public override string ToString()
        {
            return $"{Year} {Make} {Model} ({Condition}) - ${GetMarketValue():F2}";
        }
    }
}
