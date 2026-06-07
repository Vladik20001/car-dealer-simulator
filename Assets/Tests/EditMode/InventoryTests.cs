using System;
using NUnit.Framework;
using CarDealerSimulator.Models;

namespace CarDealerSimulator.Tests
{
    [TestFixture]
    public class InventoryTests
    {
        private Vehicle CreateVehicle(string make = "Toyota", VehicleCondition condition = VehicleCondition.Good)
        {
            return new Vehicle(make, "Camry", 2022, 25000f, 10000f, condition);
        }

        [Test]
        public void Constructor_ValidCapacity_CreatesInventory()
        {
            var inventory = new Inventory(10);

            Assert.AreEqual(10, inventory.Capacity);
            Assert.AreEqual(0, inventory.Count);
            Assert.IsFalse(inventory.IsFull);
        }

        [Test]
        public void Constructor_ZeroCapacity_ThrowsArgumentOutOfRangeException()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new Inventory(0));
        }

        [Test]
        public void Constructor_NegativeCapacity_ThrowsArgumentOutOfRangeException()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new Inventory(-5));
        }

        [Test]
        public void Add_ValidVehicle_ReturnsTrue()
        {
            var inventory = new Inventory(5);
            var vehicle = CreateVehicle();

            bool result = inventory.Add(vehicle);

            Assert.IsTrue(result);
            Assert.AreEqual(1, inventory.Count);
        }

        [Test]
        public void Add_NullVehicle_ThrowsArgumentNullException()
        {
            var inventory = new Inventory(5);

            Assert.Throws<ArgumentNullException>(() => inventory.Add(null));
        }

        [Test]
        public void Add_DuplicateVehicle_ReturnsFalse()
        {
            var inventory = new Inventory(5);
            var vehicle = CreateVehicle();

            inventory.Add(vehicle);
            bool result = inventory.Add(vehicle);

            Assert.IsFalse(result);
            Assert.AreEqual(1, inventory.Count);
        }

        [Test]
        public void Add_FullInventory_ReturnsFalse()
        {
            var inventory = new Inventory(1);
            inventory.Add(CreateVehicle());

            bool result = inventory.Add(CreateVehicle());

            Assert.IsFalse(result);
            Assert.AreEqual(1, inventory.Count);
        }

        [Test]
        public void IsFull_AtCapacity_ReturnsTrue()
        {
            var inventory = new Inventory(2);
            inventory.Add(CreateVehicle());
            inventory.Add(CreateVehicle());

            Assert.IsTrue(inventory.IsFull);
        }

        [Test]
        public void Remove_ExistingVehicle_ReturnsTrue()
        {
            var inventory = new Inventory(5);
            var vehicle = CreateVehicle();
            inventory.Add(vehicle);

            bool result = inventory.Remove(vehicle);

            Assert.IsTrue(result);
            Assert.AreEqual(0, inventory.Count);
        }

        [Test]
        public void Remove_NonExistingVehicle_ReturnsFalse()
        {
            var inventory = new Inventory(5);
            var vehicle = CreateVehicle();

            bool result = inventory.Remove(vehicle);

            Assert.IsFalse(result);
        }

        [Test]
        public void Remove_NullVehicle_ThrowsArgumentNullException()
        {
            var inventory = new Inventory(5);

            Assert.Throws<ArgumentNullException>(() => inventory.Remove(null));
        }

        [Test]
        public void FindById_ExistingVehicle_ReturnsVehicle()
        {
            var inventory = new Inventory(5);
            var vehicle = CreateVehicle();
            inventory.Add(vehicle);

            var found = inventory.FindById(vehicle.Id);

            Assert.IsNotNull(found);
            Assert.AreEqual(vehicle.Id, found.Id);
        }

        [Test]
        public void FindById_NonExistingId_ReturnsNull()
        {
            var inventory = new Inventory(5);
            inventory.Add(CreateVehicle());

            var found = inventory.FindById("nonexistent-id");

            Assert.IsNull(found);
        }

        [Test]
        public void FindById_NullId_ThrowsArgumentException()
        {
            var inventory = new Inventory(5);

            Assert.Throws<ArgumentException>(() => inventory.FindById(null));
        }

        [Test]
        public void FindByMake_MatchingVehicles_ReturnsMatches()
        {
            var inventory = new Inventory(10);
            inventory.Add(CreateVehicle("Toyota"));
            inventory.Add(CreateVehicle("Toyota"));
            inventory.Add(CreateVehicle("Honda"));

            var results = inventory.FindByMake("Toyota");

            Assert.AreEqual(2, results.Count);
        }

        [Test]
        public void FindByMake_CaseInsensitive()
        {
            var inventory = new Inventory(10);
            inventory.Add(CreateVehicle("Toyota"));

            var results = inventory.FindByMake("toyota");

            Assert.AreEqual(1, results.Count);
        }

        [Test]
        public void FindByMake_NoMatch_ReturnsEmptyList()
        {
            var inventory = new Inventory(10);
            inventory.Add(CreateVehicle("Toyota"));

            var results = inventory.FindByMake("BMW");

            Assert.AreEqual(0, results.Count);
        }

        [Test]
        public void FindByMake_NullMake_ThrowsArgumentException()
        {
            var inventory = new Inventory(5);

            Assert.Throws<ArgumentException>(() => inventory.FindByMake(null));
        }

        [Test]
        public void FindByCondition_MatchingCondition_ReturnsMatches()
        {
            var inventory = new Inventory(10);
            inventory.Add(CreateVehicle(condition: VehicleCondition.Good));
            inventory.Add(CreateVehicle(condition: VehicleCondition.Good));
            inventory.Add(CreateVehicle(condition: VehicleCondition.Poor));

            var results = inventory.FindByCondition(VehicleCondition.Good);

            Assert.AreEqual(2, results.Count);
        }

        [Test]
        public void FindInPriceRange_ValidRange_ReturnsMatches()
        {
            var inventory = new Inventory(10);
            inventory.Add(new Vehicle("Cheap", "Car", 2022, 5000f, 0f, VehicleCondition.Good));
            inventory.Add(new Vehicle("Mid", "Car", 2022, 20000f, 0f, VehicleCondition.Good));
            inventory.Add(new Vehicle("Expensive", "Car", 2022, 80000f, 0f, VehicleCondition.Good));

            var results = inventory.FindInPriceRange(10000f, 50000f);

            Assert.AreEqual(1, results.Count);
        }

        [Test]
        public void FindInPriceRange_NegativeMin_ThrowsArgumentOutOfRangeException()
        {
            var inventory = new Inventory(5);

            Assert.Throws<ArgumentOutOfRangeException>(() => inventory.FindInPriceRange(-1f, 100f));
        }

        [Test]
        public void FindInPriceRange_MaxLessThanMin_ThrowsArgumentOutOfRangeException()
        {
            var inventory = new Inventory(5);

            Assert.Throws<ArgumentOutOfRangeException>(() => inventory.FindInPriceRange(100f, 50f));
        }

        [Test]
        public void GetTotalInventoryValue_MultipleVehicles_ReturnsSumOfValues()
        {
            var inventory = new Inventory(10);
            var v1 = CreateVehicle();
            var v2 = CreateVehicle();
            inventory.Add(v1);
            inventory.Add(v2);

            float total = inventory.GetTotalInventoryValue();

            Assert.AreEqual(v1.GetMarketValue() + v2.GetMarketValue(), total, 0.01f);
        }

        [Test]
        public void GetTotalInventoryValue_EmptyInventory_ReturnsZero()
        {
            var inventory = new Inventory(10);

            Assert.AreEqual(0f, inventory.GetTotalInventoryValue());
        }

        [Test]
        public void Clear_RemovesAllVehicles()
        {
            var inventory = new Inventory(10);
            inventory.Add(CreateVehicle());
            inventory.Add(CreateVehicle());

            inventory.Clear();

            Assert.AreEqual(0, inventory.Count);
            Assert.IsFalse(inventory.IsFull);
        }

        [Test]
        public void Vehicles_ReturnsReadOnlyList()
        {
            var inventory = new Inventory(10);
            var vehicle = CreateVehicle();
            inventory.Add(vehicle);

            var vehicles = inventory.Vehicles;

            Assert.AreEqual(1, vehicles.Count);
            Assert.AreEqual(vehicle.Id, vehicles[0].Id);
        }
    }
}
