using System;
using NUnit.Framework;
using CarDealerSimulator.Models;

namespace CarDealerSimulator.Tests
{
    [TestFixture]
    public class CustomerTests
    {
        [Test]
        public void Constructor_ValidParameters_CreatesCustomer()
        {
            var customer = new Customer("John", CustomerType.Standard, 30000f, "Toyota");

            Assert.AreEqual("John", customer.Name);
            Assert.AreEqual(CustomerType.Standard, customer.Type);
            Assert.AreEqual(30000f, customer.Budget);
            Assert.AreEqual("Toyota", customer.PreferredMake);
        }

        [Test]
        public void Constructor_NoPreferredMake_DefaultsToNull()
        {
            var customer = new Customer("John", CustomerType.Standard, 30000f);

            Assert.IsNull(customer.PreferredMake);
        }

        [Test]
        public void Constructor_NullName_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() =>
                new Customer(null, CustomerType.Standard, 30000f));
        }

        [Test]
        public void Constructor_EmptyName_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() =>
                new Customer("  ", CustomerType.Standard, 30000f));
        }

        [Test]
        public void Constructor_ZeroBudget_ThrowsArgumentOutOfRangeException()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                new Customer("John", CustomerType.Standard, 0f));
        }

        [Test]
        public void Constructor_NegativeBudget_ThrowsArgumentOutOfRangeException()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                new Customer("John", CustomerType.Standard, -100f));
        }

        [Test]
        public void CanAfford_PriceBelowBudget_ReturnsTrue()
        {
            var customer = new Customer("John", CustomerType.Standard, 30000f);

            Assert.IsTrue(customer.CanAfford(25000f));
        }

        [Test]
        public void CanAfford_PriceEqualsBudget_ReturnsTrue()
        {
            var customer = new Customer("John", CustomerType.Standard, 30000f);

            Assert.IsTrue(customer.CanAfford(30000f));
        }

        [Test]
        public void CanAfford_PriceAboveBudget_ReturnsFalse()
        {
            var customer = new Customer("John", CustomerType.Standard, 30000f);

            Assert.IsFalse(customer.CanAfford(35000f));
        }

        [Test]
        public void GetWillingnessToPay_BudgetCustomer_PaysLess()
        {
            int currentYear = DateTime.Now.Year;
            var vehicle = new Vehicle("Toyota", "Camry", currentYear, 20000f, 0f, VehicleCondition.Good);
            var budget = new Customer("Budget Bob", CustomerType.Budget, 50000f);
            var standard = new Customer("Standard Steve", CustomerType.Standard, 50000f);

            float budgetWTP = budget.GetWillingnessToPay(vehicle);
            float standardWTP = standard.GetWillingnessToPay(vehicle);

            Assert.Less(budgetWTP, standardWTP);
        }

        [Test]
        public void GetWillingnessToPay_PremiumCustomer_PaysMore()
        {
            int currentYear = DateTime.Now.Year;
            var vehicle = new Vehicle("Toyota", "Camry", currentYear, 20000f, 0f, VehicleCondition.Good);
            var standard = new Customer("Standard Steve", CustomerType.Standard, 50000f);
            var premium = new Customer("Premium Pete", CustomerType.Premium, 50000f);

            float standardWTP = standard.GetWillingnessToPay(vehicle);
            float premiumWTP = premium.GetWillingnessToPay(vehicle);

            Assert.Greater(premiumWTP, standardWTP);
        }

        [Test]
        public void GetWillingnessToPay_PreferredMake_IncreasesWillingness()
        {
            int currentYear = DateTime.Now.Year;
            var vehicle = new Vehicle("Toyota", "Camry", currentYear, 20000f, 0f, VehicleCondition.Good);
            var noPreference = new Customer("No Pref", CustomerType.Standard, 50000f);
            var toyotaFan = new Customer("Toyota Fan", CustomerType.Standard, 50000f, "Toyota");

            float noPreferenceWTP = noPreference.GetWillingnessToPay(vehicle);
            float toyotaFanWTP = toyotaFan.GetWillingnessToPay(vehicle);

            Assert.Greater(toyotaFanWTP, noPreferenceWTP);
        }

        [Test]
        public void GetWillingnessToPay_PreferredMakeCaseInsensitive()
        {
            int currentYear = DateTime.Now.Year;
            var vehicle = new Vehicle("Toyota", "Camry", currentYear, 20000f, 0f, VehicleCondition.Good);
            var fan1 = new Customer("Fan1", CustomerType.Standard, 50000f, "toyota");
            var fan2 = new Customer("Fan2", CustomerType.Standard, 50000f, "TOYOTA");

            Assert.AreEqual(fan1.GetWillingnessToPay(vehicle), fan2.GetWillingnessToPay(vehicle), 0.01f);
        }

        [Test]
        public void GetWillingnessToPay_CappedByBudget()
        {
            int currentYear = DateTime.Now.Year;
            var vehicle = new Vehicle("Toyota", "Camry", currentYear, 50000f, 0f, VehicleCondition.Excellent);
            var customer = new Customer("Poor Pete", CustomerType.Premium, 10000f);

            float wtp = customer.GetWillingnessToPay(vehicle);

            Assert.LessOrEqual(wtp, customer.Budget);
        }

        [Test]
        public void GetWillingnessToPay_NullVehicle_ThrowsArgumentNullException()
        {
            var customer = new Customer("John", CustomerType.Standard, 30000f);

            Assert.Throws<ArgumentNullException>(() => customer.GetWillingnessToPay(null));
        }

        [Test]
        public void WouldBuy_AffordablePrice_ReturnsTrue()
        {
            int currentYear = DateTime.Now.Year;
            var vehicle = new Vehicle("Toyota", "Camry", currentYear, 10000f, 0f, VehicleCondition.Good);
            var customer = new Customer("John", CustomerType.Standard, 50000f);

            float wtp = customer.GetWillingnessToPay(vehicle);
            Assert.IsTrue(customer.WouldBuy(vehicle, wtp - 1));
        }

        [Test]
        public void WouldBuy_TooExpensive_ReturnsFalse()
        {
            int currentYear = DateTime.Now.Year;
            var vehicle = new Vehicle("Toyota", "Camry", currentYear, 10000f, 0f, VehicleCondition.Good);
            var customer = new Customer("John", CustomerType.Budget, 50000f);

            Assert.IsFalse(customer.WouldBuy(vehicle, 50000f));
        }

        [Test]
        public void WouldBuy_NullVehicle_ThrowsArgumentNullException()
        {
            var customer = new Customer("John", CustomerType.Standard, 30000f);

            Assert.Throws<ArgumentNullException>(() => customer.WouldBuy(null, 1000f));
        }
    }
}
