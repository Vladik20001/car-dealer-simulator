using System;
using System.Collections.Generic;
using NUnit.Framework;
using CarDealerSimulator.Models;
using CarDealerSimulator.Systems;

namespace CarDealerSimulator.Tests
{
    [TestFixture]
    public class CustomerDemandTests
    {
        private CustomerDemand _demand;

        [SetUp]
        public void SetUp()
        {
            _demand = new CustomerDemand(1.0f);
        }

        private Vehicle CreateVehicle(string make = "Toyota", int year = 0,
            VehicleCondition condition = VehicleCondition.Good)
        {
            if (year == 0)
                year = DateTime.Now.Year;
            return new Vehicle(make, "Camry", year, 25000f, 10000f, condition);
        }

        [Test]
        public void Constructor_DefaultBaseDemand()
        {
            var demand = new CustomerDemand();

            Assert.AreEqual(1.0f, demand.BaseDemand);
        }

        [Test]
        public void Constructor_CustomBaseDemand()
        {
            var demand = new CustomerDemand(2.5f);

            Assert.AreEqual(2.5f, demand.BaseDemand);
        }

        [Test]
        public void BaseDemand_NegativeValue_ThrowsArgumentOutOfRangeException()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => _demand.BaseDemand = -1f);
        }

        [Test]
        public void SetMakeDemand_ValidInput_SetsValue()
        {
            _demand.SetMakeDemand("Toyota", 2.0f);

            Assert.AreEqual(2.0f, _demand.GetMakeDemand("Toyota"));
        }

        [Test]
        public void SetMakeDemand_NullMake_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => _demand.SetMakeDemand(null, 1.0f));
        }

        [Test]
        public void SetMakeDemand_NegativeFactor_ThrowsArgumentOutOfRangeException()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => _demand.SetMakeDemand("Toyota", -1f));
        }

        [Test]
        public void GetMakeDemand_UnknownMake_ReturnsBaseDemand()
        {
            float demand = _demand.GetMakeDemand("UnknownBrand");

            Assert.AreEqual(_demand.BaseDemand, demand);
        }

        [Test]
        public void GetMakeDemand_CaseInsensitive()
        {
            _demand.SetMakeDemand("Toyota", 2.0f);

            Assert.AreEqual(2.0f, _demand.GetMakeDemand("toyota"));
            Assert.AreEqual(2.0f, _demand.GetMakeDemand("TOYOTA"));
        }

        [Test]
        public void GetMakeDemand_NullMake_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => _demand.GetMakeDemand(null));
        }

        [Test]
        public void GetVehicleDemandScore_HighDemandMake_ReturnsHigherScore()
        {
            _demand.SetMakeDemand("Toyota", 2.0f);
            _demand.SetMakeDemand("Lada", 0.5f);

            var toyota = CreateVehicle("Toyota");
            var lada = CreateVehicle("Lada");

            Assert.Greater(_demand.GetVehicleDemandScore(toyota), _demand.GetVehicleDemandScore(lada));
        }

        [Test]
        public void GetVehicleDemandScore_BetterCondition_ReturnsHigherScore()
        {
            var excellent = CreateVehicle(condition: VehicleCondition.Excellent);
            var poor = CreateVehicle(condition: VehicleCondition.Poor);

            Assert.Greater(_demand.GetVehicleDemandScore(excellent), _demand.GetVehicleDemandScore(poor));
        }

        [Test]
        public void GetVehicleDemandScore_NewerVehicle_ReturnsHigherScore()
        {
            int currentYear = DateTime.Now.Year;
            var newer = CreateVehicle(year: currentYear);
            var older = CreateVehicle(year: currentYear - 15);

            Assert.Greater(_demand.GetVehicleDemandScore(newer), _demand.GetVehicleDemandScore(older));
        }

        [Test]
        public void GetVehicleDemandScore_NullVehicle_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => _demand.GetVehicleDemandScore(null));
        }

        [Test]
        public void RankByDemand_ReturnsOrderedByScoreDescending()
        {
            _demand.SetMakeDemand("Toyota", 2.0f);
            _demand.SetMakeDemand("Lada", 0.3f);

            var vehicles = new List<Vehicle>
            {
                CreateVehicle("Lada"),
                CreateVehicle("Toyota"),
                CreateVehicle("Honda")
            };

            var ranked = _demand.RankByDemand(vehicles);

            Assert.AreEqual(3, ranked.Count);
            float prev = float.MaxValue;
            foreach (var v in ranked)
            {
                float score = _demand.GetVehicleDemandScore(v);
                Assert.LessOrEqual(score, prev);
                prev = score;
            }
        }

        [Test]
        public void RankByDemand_EmptyList_ReturnsEmptyList()
        {
            var ranked = _demand.RankByDemand(new List<Vehicle>());

            Assert.AreEqual(0, ranked.Count);
        }

        [Test]
        public void RankByDemand_NullList_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => _demand.RankByDemand(null));
        }

        [Test]
        public void EstimateCustomersPerDay_ReturnsAtLeastOne()
        {
            var vehicle = CreateVehicle();

            int customers = _demand.EstimateCustomersPerDay(vehicle);

            Assert.GreaterOrEqual(customers, 1);
        }

        [Test]
        public void EstimateCustomersPerDay_HighDemand_ReturnsMoreCustomers()
        {
            _demand.SetMakeDemand("Toyota", 3.0f);
            _demand.SetMakeDemand("Lada", 0.3f);

            var toyota = CreateVehicle("Toyota");
            var lada = CreateVehicle("Lada");

            Assert.GreaterOrEqual(
                _demand.EstimateCustomersPerDay(toyota),
                _demand.EstimateCustomersPerDay(lada));
        }

        [Test]
        public void EstimateCustomersPerDay_NullVehicle_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => _demand.EstimateCustomersPerDay(null));
        }
    }
}
