using System;
using CarDealerSimulator.Core.Utilities;

namespace CarDealerSimulator.Core.Economy
{
    /// <summary>
    /// Shared pricing logic for vehicles and parts.
    /// Centralizes price computation that would otherwise be duplicated
    /// across buying, selling, upgrade, and repair systems.
    /// </summary>
    public static class PriceCalculator
    {
        /// <summary>
        /// Calculates the market value of a vehicle based on base price, condition, and age.
        /// </summary>
        public static decimal CalculateMarketValue(decimal basePrice, float conditionPercent, int ageDays, float depreciationRate = 0.002f)
        {
            decimal depreciatedValue = MathUtilities.Depreciate(basePrice, depreciationRate, ageDays);
            decimal conditionMultiplier = (decimal)(conditionPercent / 100f);
            return MathUtilities.ClampDecimal(depreciatedValue * conditionMultiplier, 0, basePrice * 2);
        }

        /// <summary>
        /// Calculates the suggested sell price with a markup.
        /// </summary>
        public static decimal CalculateSellPrice(decimal marketValue, float markupPercent)
        {
            decimal markup = MathUtilities.Percentage(marketValue, markupPercent);
            return MathUtilities.RoundToNearest(marketValue + markup, 50m);
        }

        /// <summary>
        /// Calculates repair cost based on damage level and part complexity.
        /// </summary>
        public static decimal CalculateRepairCost(decimal partBaseValue, float damagePercent, float complexityMultiplier = 1f)
        {
            decimal baseCost = MathUtilities.Percentage(partBaseValue, damagePercent);
            return MathUtilities.RoundToNearest(baseCost * (decimal)complexityMultiplier, 10m);
        }

        /// <summary>
        /// Calculates the upgrade value added to a vehicle.
        /// </summary>
        public static decimal CalculateUpgradeValue(decimal upgradeCost, float qualityMultiplier)
        {
            return upgradeCost * (decimal)qualityMultiplier;
        }

        /// <summary>
        /// Determines if a trade is profitable given buy price, costs, and sell price.
        /// </summary>
        public static TradeAnalysis AnalyzeTrade(decimal buyPrice, decimal totalCosts, decimal sellPrice)
        {
            decimal totalInvestment = buyPrice + totalCosts;
            decimal profit = sellPrice - totalInvestment;
            float margin = MathUtilities.ProfitMargin(totalInvestment, sellPrice);

            return new TradeAnalysis
            {
                BuyPrice = buyPrice,
                TotalCosts = totalCosts,
                SellPrice = sellPrice,
                Profit = profit,
                MarginPercent = margin,
                IsProfitable = profit > 0
            };
        }

        /// <summary>
        /// Applies a demand multiplier to adjust prices based on market conditions.
        /// </summary>
        public static decimal ApplyDemandMultiplier(decimal basePrice, float demandFactor)
        {
            float clampedDemand = UnityEngine.Mathf.Clamp(demandFactor, 0.5f, 2.0f);
            return basePrice * (decimal)clampedDemand;
        }
    }

    public struct TradeAnalysis
    {
        public decimal BuyPrice;
        public decimal TotalCosts;
        public decimal SellPrice;
        public decimal Profit;
        public float MarginPercent;
        public bool IsProfitable;
    }
}
