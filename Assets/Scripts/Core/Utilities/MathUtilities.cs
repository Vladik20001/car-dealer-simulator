using UnityEngine;

namespace CarDealerSimulator.Core.Utilities
{
    /// <summary>
    /// Shared math helpers used across economy, pricing, and progression systems.
    /// Centralizes calculations that would otherwise be duplicated in multiple managers.
    /// </summary>
    public static class MathUtilities
    {
        /// <summary>
        /// Calculates percentage of a value.
        /// </summary>
        public static decimal Percentage(decimal value, float percent)
        {
            return value * (decimal)(percent / 100f);
        }

        /// <summary>
        /// Calculates profit margin: (sale - cost) / cost * 100.
        /// </summary>
        public static float ProfitMargin(decimal cost, decimal salePrice)
        {
            if (cost == 0) return 0f;
            return (float)((salePrice - cost) / cost) * 100f;
        }

        /// <summary>
        /// Linearly interpolates between two decimal values.
        /// </summary>
        public static decimal LerpDecimal(decimal a, decimal b, float t)
        {
            t = Mathf.Clamp01(t);
            return a + (b - a) * (decimal)t;
        }

        /// <summary>
        /// Applies depreciation over time using a decay factor.
        /// Returns the depreciated value.
        /// </summary>
        public static decimal Depreciate(decimal originalValue, float decayRate, int periods)
        {
            if (periods <= 0) return originalValue;
            float multiplier = Mathf.Pow(1f - decayRate, periods);
            return originalValue * (decimal)multiplier;
        }

        /// <summary>
        /// Clamps a decimal value between min and max.
        /// </summary>
        public static decimal ClampDecimal(decimal value, decimal min, decimal max)
        {
            if (value < min) return min;
            if (value > max) return max;
            return value;
        }

        /// <summary>
        /// Rounds a decimal to the nearest step (e.g., nearest 50, 100, 500).
        /// </summary>
        public static decimal RoundToNearest(decimal value, decimal step)
        {
            if (step <= 0) return value;
            return decimal.Round(value / step) * step;
        }

        /// <summary>
        /// Maps a value from one range to another.
        /// </summary>
        public static float Remap(float value, float fromMin, float fromMax, float toMin, float toMax)
        {
            if (Mathf.Approximately(fromMax - fromMin, 0f)) return toMin;
            float t = (value - fromMin) / (fromMax - fromMin);
            return Mathf.Lerp(toMin, toMax, t);
        }
    }
}
