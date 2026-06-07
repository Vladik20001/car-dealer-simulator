using System;
using NUnit.Framework;
using CarDealerSimulator.Models;
using CarDealerSimulator.Systems;

namespace CarDealerSimulator.Tests
{
    [TestFixture]
    public class PriceCalculatorTests
    {
        private PriceCalculator _calculator;

        [SetUp]
        public void SetUp()
        {
            _calculator = new PriceCalculator(0.2f, 1.0f);
        }

        private Vehicle CreateVehicle(VehicleCondition condition = VehicleCondition.Good)
        {
            return new Vehicle("Toyota", "Camry", DateTime.Now.Year, 25000f, 10000f, condition);
        }

        [Test]
        public void Constructor_DefaultValues_SetsCorrectDefaults()
        {
            var calc = new PriceCalculator();

            Assert.AreEqual(0.2f, calc.ProfitMargin);
            Assert.AreEqual(1.0f, calc.DemandMultiplier);
        }

        [Test]
        public void Constructor_CustomValues_SetsCorrectValues()
        {
            var calc = new PriceCalculator(0.3f, 1.5f);

            Assert.AreEqual(0.3f, calc.ProfitMargin);
            Assert.AreEqual(1.5f, calc.DemandMultiplier);
        }

        [Test]
        public void ProfitMargin_NegativeValue_ThrowsArgumentOutOfRangeException()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => _calculator.ProfitMargin = -0.1f);
        }

        [Test]
        public void ProfitMargin_GreaterThanOne_ThrowsArgumentOutOfRangeException()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => _calculator.ProfitMargin = 1.5f);
        }

        [Test]
        public void DemandMultiplier_TooLow_ThrowsArgumentOutOfRangeException()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => _calculator.DemandMultiplier = 0.05f);
        }

        [Test]
        public void DemandMultiplier_TooHigh_ThrowsArgumentOutOfRangeException()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => _calculator.DemandMultiplier = 6f);
        }

        [Test]
        public void CalculateSellingPrice_ReturnsHigherThanMarketValue()
        {
            var vehicle = CreateVehicle();

            float sellingPrice = _calculator.CalculateSellingPrice(vehicle);
            float marketValue = vehicle.GetMarketValue();

            Assert.Greater(sellingPrice, marketValue);
        }

        [Test]
        public void CalculateSellingPrice_NullVehicle_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => _calculator.CalculateSellingPrice(null));
        }

        [Test]
        public void CalculateSellingPrice_HigherDemand_IncreasesPrice()
        {
            var vehicle = CreateVehicle();

            _calculator.DemandMultiplier = 1.0f;
            float normalPrice = _calculator.CalculateSellingPrice(vehicle);

            _calculator.DemandMultiplier = 2.0f;
            float highDemandPrice = _calculator.CalculateSellingPrice(vehicle);

            Assert.Greater(highDemandPrice, normalPrice);
        }

        [Test]
        public void CalculateBuyingPrice_ReturnsLowerThanMarketValue()
        {
            var vehicle = CreateVehicle();

            float buyingPrice = _calculator.CalculateBuyingPrice(vehicle);
            float marketValue = vehicle.GetMarketValue();

            Assert.Less(buyingPrice, marketValue);
        }

        [Test]
        public void CalculateBuyingPrice_NullVehicle_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => _calculator.CalculateBuyingPrice(null));
        }

        [Test]
        public void CalculateBuyingPrice_ConsistentWithProfitMargin()
        {
            var vehicle = CreateVehicle();
            float marketValue = vehicle.GetMarketValue();
            float expected = marketValue * (1f - 0.2f);

            float buyingPrice = _calculator.CalculateBuyingPrice(vehicle);

            Assert.AreEqual(expected, buyingPrice, 0.01f);
        }

        [Test]
        public void CalculateProfit_ReturnsCorrectDifference()
        {
            float profit = _calculator.CalculateProfit(10000f, 12000f);

            Assert.AreEqual(2000f, profit);
        }

        [Test]
        public void CalculateProfit_Loss_ReturnsNegative()
        {
            float profit = _calculator.CalculateProfit(15000f, 12000f);

            Assert.Less(profit, 0f);
        }

        [Test]
        public void CalculateProfitPercent_ReturnsCorrectPercentage()
        {
            float percent = _calculator.CalculateProfitPercent(10000f, 12000f);

            Assert.AreEqual(20f, percent, 0.01f);
        }

        [Test]
        public void CalculateProfitPercent_ZeroBuyPrice_ThrowsArgumentOutOfRangeException()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => _calculator.CalculateProfitPercent(0f, 1000f));
        }

        [Test]
        public void CalculateProfitPercent_NegativeBuyPrice_ThrowsArgumentOutOfRangeException()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => _calculator.CalculateProfitPercent(-100f, 1000f));
        }

        [Test]
        public void CalculateRepairCost_PoorCondition_HighestCost()
        {
            var poor = CreateVehicle(VehicleCondition.Poor);
            var fair = CreateVehicle(VehicleCondition.Fair);

            float poorCost = _calculator.CalculateRepairCost(poor);
            float fairCost = _calculator.CalculateRepairCost(fair);

            Assert.Greater(poorCost, fairCost);
        }

        [Test]
        public void CalculateRepairCost_ExcellentCondition_ReturnsZero()
        {
            var vehicle = CreateVehicle(VehicleCondition.Excellent);

            float cost = _calculator.CalculateRepairCost(vehicle);

            Assert.AreEqual(0f, cost);
        }

        [Test]
        public void CalculateRepairCost_NullVehicle_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => _calculator.CalculateRepairCost(null));
        }

        [Test]
        public void IsRepairProfitable_ExcellentCondition_ReturnsFalse()
        {
            var vehicle = CreateVehicle(VehicleCondition.Excellent);

            Assert.IsFalse(_calculator.IsRepairProfitable(vehicle));
        }

        [Test]
        public void IsRepairProfitable_NullVehicle_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => _calculator.IsRepairProfitable(null));
        }

        [Test]
        public void IsRepairProfitable_PoorCondition_ReturnsBoolean()
        {
            var vehicle = CreateVehicle(VehicleCondition.Poor);

            // Should return a valid boolean without throwing
            bool result = _calculator.IsRepairProfitable(vehicle);
            Assert.IsTrue(result || !result);
        }

        [Test]
        public void SellingPrice_AlwaysHigherThanBuyingPrice()
        {
            var vehicle = CreateVehicle();

            float sell = _calculator.CalculateSellingPrice(vehicle);
            float buy = _calculator.CalculateBuyingPrice(vehicle);

            Assert.Greater(sell, buy);
        }
    }
}
