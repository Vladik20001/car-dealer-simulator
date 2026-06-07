using System;
using NUnit.Framework;
using CarDealerSimulator.Models;

namespace CarDealerSimulator.Tests
{
    [TestFixture]
    public class VehicleTests
    {
        [Test]
        public void Constructor_ValidParameters_CreatesVehicle()
        {
            var vehicle = new Vehicle("Toyota", "Camry", 2020, 25000f, 30000f, VehicleCondition.Good);

            Assert.AreEqual("Toyota", vehicle.Make);
            Assert.AreEqual("Camry", vehicle.Model);
            Assert.AreEqual(2020, vehicle.Year);
            Assert.AreEqual(25000f, vehicle.BasePrice);
            Assert.AreEqual(30000f, vehicle.Mileage);
            Assert.AreEqual(VehicleCondition.Good, vehicle.Condition);
            Assert.IsFalse(string.IsNullOrEmpty(vehicle.Id));
        }

        [Test]
        public void Constructor_NullMake_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() =>
                new Vehicle(null, "Camry", 2020, 25000f, 0f, VehicleCondition.Good));
        }

        [Test]
        public void Constructor_EmptyModel_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() =>
                new Vehicle("Toyota", "", 2020, 25000f, 0f, VehicleCondition.Good));
        }

        [Test]
        public void Constructor_InvalidYear_ThrowsArgumentOutOfRangeException()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                new Vehicle("Toyota", "Camry", 1800, 25000f, 0f, VehicleCondition.Good));
        }

        [Test]
        public void Constructor_NegativeBasePrice_ThrowsArgumentOutOfRangeException()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                new Vehicle("Toyota", "Camry", 2020, -100f, 0f, VehicleCondition.Good));
        }

        [Test]
        public void Constructor_ZeroBasePrice_ThrowsArgumentOutOfRangeException()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                new Vehicle("Toyota", "Camry", 2020, 0f, 0f, VehicleCondition.Good));
        }

        [Test]
        public void Constructor_NegativeMileage_ThrowsArgumentOutOfRangeException()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                new Vehicle("Toyota", "Camry", 2020, 25000f, -1f, VehicleCondition.Good));
        }

        [Test]
        public void GetMarketValue_ExcellentConditionZeroMileage_ReturnsAdjustedValue()
        {
            int currentYear = DateTime.Now.Year;
            var vehicle = new Vehicle("Toyota", "Camry", currentYear, 30000f, 0f, VehicleCondition.Excellent);

            float value = vehicle.GetMarketValue();

            Assert.Greater(value, 0f);
            Assert.AreEqual(30000f, value, 0.01f);
        }

        [Test]
        public void GetMarketValue_PoorCondition_ReturnsLowerValue()
        {
            int currentYear = DateTime.Now.Year;
            var good = new Vehicle("Toyota", "Camry", currentYear, 30000f, 0f, VehicleCondition.Good);
            var poor = new Vehicle("Toyota", "Camry", currentYear, 30000f, 0f, VehicleCondition.Poor);

            Assert.Less(poor.GetMarketValue(), good.GetMarketValue());
        }

        [Test]
        public void GetMarketValue_HighMileage_ReturnsLowerValue()
        {
            int currentYear = DateTime.Now.Year;
            var low = new Vehicle("Toyota", "Camry", currentYear, 30000f, 1000f, VehicleCondition.Good);
            var high = new Vehicle("Toyota", "Camry", currentYear, 30000f, 100000f, VehicleCondition.Good);

            Assert.Less(high.GetMarketValue(), low.GetMarketValue());
        }

        [Test]
        public void GetMarketValue_OlderVehicle_ReturnsLowerValue()
        {
            int currentYear = DateTime.Now.Year;
            var newer = new Vehicle("Toyota", "Camry", currentYear, 30000f, 0f, VehicleCondition.Good);
            var older = new Vehicle("Toyota", "Camry", currentYear - 10, 30000f, 0f, VehicleCondition.Good);

            Assert.Less(older.GetMarketValue(), newer.GetMarketValue());
        }

        [Test]
        public void AddMileage_PositiveValue_IncreasesMileage()
        {
            var vehicle = new Vehicle("Toyota", "Camry", 2020, 25000f, 10000f, VehicleCondition.Good);

            vehicle.AddMileage(5000f);

            Assert.AreEqual(15000f, vehicle.Mileage);
        }

        [Test]
        public void AddMileage_NegativeValue_ThrowsArgumentOutOfRangeException()
        {
            var vehicle = new Vehicle("Toyota", "Camry", 2020, 25000f, 10000f, VehicleCondition.Good);

            Assert.Throws<ArgumentOutOfRangeException>(() => vehicle.AddMileage(-100f));
        }

        [Test]
        public void Repair_PoorCondition_UpgradesToFair()
        {
            var vehicle = new Vehicle("Toyota", "Camry", 2020, 25000f, 0f, VehicleCondition.Poor);

            vehicle.Repair();

            Assert.AreEqual(VehicleCondition.Fair, vehicle.Condition);
        }

        [Test]
        public void Repair_GoodCondition_UpgradesToExcellent()
        {
            var vehicle = new Vehicle("Toyota", "Camry", 2020, 25000f, 0f, VehicleCondition.Good);

            vehicle.Repair();

            Assert.AreEqual(VehicleCondition.Excellent, vehicle.Condition);
        }

        [Test]
        public void Repair_ExcellentCondition_StaysExcellent()
        {
            var vehicle = new Vehicle("Toyota", "Camry", 2020, 25000f, 0f, VehicleCondition.Excellent);

            vehicle.Repair();

            Assert.AreEqual(VehicleCondition.Excellent, vehicle.Condition);
        }

        [Test]
        public void Degrade_ExcellentCondition_DegradesToGood()
        {
            var vehicle = new Vehicle("Toyota", "Camry", 2020, 25000f, 0f, VehicleCondition.Excellent);

            vehicle.Degrade();

            Assert.AreEqual(VehicleCondition.Good, vehicle.Condition);
        }

        [Test]
        public void Degrade_PoorCondition_StaysPoor()
        {
            var vehicle = new Vehicle("Toyota", "Camry", 2020, 25000f, 0f, VehicleCondition.Poor);

            vehicle.Degrade();

            Assert.AreEqual(VehicleCondition.Poor, vehicle.Condition);
        }

        [Test]
        public void ToString_ReturnsFormattedString()
        {
            int currentYear = DateTime.Now.Year;
            var vehicle = new Vehicle("Toyota", "Camry", currentYear, 30000f, 0f, VehicleCondition.Excellent);

            string result = vehicle.ToString();

            Assert.IsTrue(result.Contains("Toyota"));
            Assert.IsTrue(result.Contains("Camry"));
            Assert.IsTrue(result.Contains(currentYear.ToString()));
            Assert.IsTrue(result.Contains("Excellent"));
        }

        [Test]
        public void Id_TwoVehicles_HaveDifferentIds()
        {
            var v1 = new Vehicle("Toyota", "Camry", 2020, 25000f, 0f, VehicleCondition.Good);
            var v2 = new Vehicle("Toyota", "Camry", 2020, 25000f, 0f, VehicleCondition.Good);

            Assert.AreNotEqual(v1.Id, v2.Id);
        }

        [Test]
        public void GetMarketValue_NeverReturnsNegative()
        {
            var vehicle = new Vehicle("OldCar", "Beater", 1990, 1000f, 500000f, VehicleCondition.Poor);

            Assert.GreaterOrEqual(vehicle.GetMarketValue(), 0f);
        }
    }
}
