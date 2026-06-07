using System;
using System.Collections.Generic;
using CarDealerSimulator.Models;

namespace CarDealerSimulator.Systems
{
    public class TransactionRecord
    {
        public Vehicle Vehicle { get; }
        public float Price { get; }
        public bool IsSale { get; }
        public DateTime Timestamp { get; }

        public TransactionRecord(Vehicle vehicle, float price, bool isSale)
        {
            Vehicle = vehicle;
            Price = price;
            IsSale = isSale;
            Timestamp = DateTime.Now;
        }
    }

    public class DealerShop
    {
        public string Name { get; }
        public float Balance { get; private set; }
        public Inventory Inventory { get; }
        public PriceCalculator PriceCalculator { get; }

        private readonly List<TransactionRecord> _transactions = new List<TransactionRecord>();
        public IReadOnlyList<TransactionRecord> Transactions => _transactions.AsReadOnly();

        public DealerShop(string name, float startingBalance, int inventoryCapacity)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Name cannot be null or empty.", nameof(name));
            if (startingBalance < 0)
                throw new ArgumentOutOfRangeException(nameof(startingBalance), "Starting balance cannot be negative.");

            Name = name;
            Balance = startingBalance;
            Inventory = new Inventory(inventoryCapacity);
            PriceCalculator = new PriceCalculator();
        }

        public bool BuyVehicle(Vehicle vehicle)
        {
            if (vehicle == null)
                throw new ArgumentNullException(nameof(vehicle));

            float buyPrice = PriceCalculator.CalculateBuyingPrice(vehicle);

            if (buyPrice > Balance)
                return false;
            if (!Inventory.Add(vehicle))
                return false;

            Balance -= buyPrice;
            _transactions.Add(new TransactionRecord(vehicle, buyPrice, false));
            return true;
        }

        public bool SellVehicle(Vehicle vehicle, Customer customer)
        {
            if (vehicle == null)
                throw new ArgumentNullException(nameof(vehicle));
            if (customer == null)
                throw new ArgumentNullException(nameof(customer));

            float sellingPrice = PriceCalculator.CalculateSellingPrice(vehicle);

            if (!customer.WouldBuy(vehicle, sellingPrice))
                return false;

            if (!Inventory.Remove(vehicle))
                return false;

            Balance += sellingPrice;
            _transactions.Add(new TransactionRecord(vehicle, sellingPrice, true));
            return true;
        }

        public bool RepairVehicle(Vehicle vehicle)
        {
            if (vehicle == null)
                throw new ArgumentNullException(nameof(vehicle));

            if (Inventory.FindById(vehicle.Id) == null)
                return false;

            float repairCost = PriceCalculator.CalculateRepairCost(vehicle);

            if (repairCost <= 0)
                return false;
            if (repairCost > Balance)
                return false;

            Balance -= repairCost;
            vehicle.Repair();
            return true;
        }

        public float GetTotalProfit()
        {
            float totalSales = 0;
            float totalPurchases = 0;

            foreach (var t in _transactions)
            {
                if (t.IsSale)
                    totalSales += t.Price;
                else
                    totalPurchases += t.Price;
            }

            return totalSales - totalPurchases;
        }

        public int GetTotalSalesCount()
        {
            int count = 0;
            foreach (var t in _transactions)
            {
                if (t.IsSale)
                    count++;
            }
            return count;
        }

        public int GetTotalPurchasesCount()
        {
            int count = 0;
            foreach (var t in _transactions)
            {
                if (!t.IsSale)
                    count++;
            }
            return count;
        }
    }
}
