using System;
using CarDealerSimulator.Models;

namespace CarDealerSimulator.Systems
{
    public class PriceCalculator
    {
        private float _profitMargin;
        private float _demandMultiplier;

        public float ProfitMargin
        {
            get => _profitMargin;
            set
            {
                if (value < 0f || value > 1f)
                    throw new ArgumentOutOfRangeException(nameof(value), "Profit margin must be between 0 and 1.");
                _profitMargin = value;
            }
        }

        public float DemandMultiplier
        {
            get => _demandMultiplier;
            set
            {
                if (value < 0.1f || value > 5f)
                    throw new ArgumentOutOfRangeException(nameof(value), "Demand multiplier must be between 0.1 and 5.");
                _demandMultiplier = value;
            }
        }

        public PriceCalculator(float profitMargin = 0.2f, float demandMultiplier = 1.0f)
        {
            ProfitMargin = profitMargin;
            DemandMultiplier = demandMultiplier;
        }

        public float CalculateSellingPrice(Vehicle vehicle)
        {
            if (vehicle == null)
                throw new ArgumentNullException(nameof(vehicle));

            float marketValue = vehicle.GetMarketValue();
            float markup = marketValue * _profitMargin * _demandMultiplier;
            return marketValue + markup;
        }

        public float CalculateBuyingPrice(Vehicle vehicle)
        {
            if (vehicle == null)
                throw new ArgumentNullException(nameof(vehicle));

            float marketValue = vehicle.GetMarketValue();
            return marketValue * (1f - _profitMargin);
        }

        public float CalculateProfit(float buyPrice, float sellPrice)
        {
            return sellPrice - buyPrice;
        }

        public float CalculateProfitPercent(float buyPrice, float sellPrice)
        {
            if (buyPrice <= 0)
                throw new ArgumentOutOfRangeException(nameof(buyPrice), "Buy price must be positive.");
            return (sellPrice - buyPrice) / buyPrice * 100f;
        }

        public float CalculateRepairCost(Vehicle vehicle)
        {
            if (vehicle == null)
                throw new ArgumentNullException(nameof(vehicle));

            float baseCost = vehicle.BasePrice * 0.05f;

            float conditionFactor = vehicle.Condition switch
            {
                VehicleCondition.Poor => 3.0f,
                VehicleCondition.Fair => 2.0f,
                VehicleCondition.Good => 1.0f,
                VehicleCondition.Excellent => 0.0f,
                _ => 1.0f
            };

            return baseCost * conditionFactor;
        }

        public bool IsRepairProfitable(Vehicle vehicle)
        {
            if (vehicle == null)
                throw new ArgumentNullException(nameof(vehicle));

            if (vehicle.Condition == VehicleCondition.Excellent)
                return false;

            float currentValue = vehicle.GetMarketValue();
            float repairCost = CalculateRepairCost(vehicle);

            var tempVehicle = new Vehicle(vehicle.Make, vehicle.Model, vehicle.Year,
                vehicle.BasePrice, vehicle.Mileage, vehicle.Condition);
            tempVehicle.Repair();
            float repairedValue = tempVehicle.GetMarketValue();

            return (repairedValue - currentValue) > repairCost;
        }
    }
}
