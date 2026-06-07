using System;
using System.Collections.Generic;
using System.Linq;
using CarDealerSimulator.Models;

namespace CarDealerSimulator.Systems
{
    public class CustomerDemand
    {
        private readonly Dictionary<string, float> _makeDemand = new Dictionary<string, float>(StringComparer.OrdinalIgnoreCase);
        private float _baseDemand;

        public float BaseDemand
        {
            get => _baseDemand;
            set
            {
                if (value < 0f)
                    throw new ArgumentOutOfRangeException(nameof(value), "Base demand cannot be negative.");
                _baseDemand = value;
            }
        }

        public CustomerDemand(float baseDemand = 1.0f)
        {
            BaseDemand = baseDemand;
        }

        public void SetMakeDemand(string make, float demandFactor)
        {
            if (string.IsNullOrWhiteSpace(make))
                throw new ArgumentException("Make cannot be null or empty.", nameof(make));
            if (demandFactor < 0f)
                throw new ArgumentOutOfRangeException(nameof(demandFactor), "Demand factor cannot be negative.");

            _makeDemand[make] = demandFactor;
        }

        public float GetMakeDemand(string make)
        {
            if (string.IsNullOrWhiteSpace(make))
                throw new ArgumentException("Make cannot be null or empty.", nameof(make));

            return _makeDemand.TryGetValue(make, out float demand) ? demand : _baseDemand;
        }

        public float GetVehicleDemandScore(Vehicle vehicle)
        {
            if (vehicle == null)
                throw new ArgumentNullException(nameof(vehicle));

            float makeFactor = GetMakeDemand(vehicle.Make);

            float conditionFactor = vehicle.Condition switch
            {
                VehicleCondition.Poor => 0.5f,
                VehicleCondition.Fair => 0.75f,
                VehicleCondition.Good => 1.0f,
                VehicleCondition.Excellent => 1.25f,
                _ => 1.0f
            };

            int age = DateTime.Now.Year - vehicle.Year;
            float ageFactor = age <= 3 ? 1.2f : age <= 7 ? 1.0f : 0.7f;

            return makeFactor * conditionFactor * ageFactor;
        }

        public List<Vehicle> RankByDemand(IEnumerable<Vehicle> vehicles)
        {
            if (vehicles == null)
                throw new ArgumentNullException(nameof(vehicles));

            return vehicles
                .OrderByDescending(v => GetVehicleDemandScore(v))
                .ToList();
        }

        public int EstimateCustomersPerDay(Vehicle vehicle)
        {
            if (vehicle == null)
                throw new ArgumentNullException(nameof(vehicle));

            float score = GetVehicleDemandScore(vehicle);
            return Math.Max(1, (int)Math.Round(score * 3));
        }
    }
}
