using System.Globalization;

namespace CarDealerSimulator.Core.Utilities
{
    /// <summary>
    /// Centralized currency formatting to avoid inconsistent display across UI elements.
    /// </summary>
    public static class CurrencyFormatter
    {
        private static string _currencySymbol = "$";
        private static int _decimalPlaces = 0;
        private static CultureInfo _culture = CultureInfo.InvariantCulture;

        public static void Configure(string symbol, int decimalPlaces = 0, CultureInfo culture = null)
        {
            _currencySymbol = symbol;
            _decimalPlaces = decimalPlaces;
            _culture = culture ?? CultureInfo.InvariantCulture;
        }

        /// <summary>
        /// Formats a decimal as currency (e.g., "$12,500").
        /// </summary>
        public static string Format(decimal amount)
        {
            string formatted = amount.ToString($"N{_decimalPlaces}", _culture);
            return $"{_currencySymbol}{formatted}";
        }

        /// <summary>
        /// Formats with explicit sign for profit/loss display (e.g., "+$500", "-$200").
        /// </summary>
        public static string FormatWithSign(decimal amount)
        {
            string sign = amount >= 0 ? "+" : "";
            return $"{sign}{Format(amount)}";
        }

        /// <summary>
        /// Formats large numbers with abbreviations (e.g., "$1.2M", "$500K").
        /// </summary>
        public static string FormatAbbreviated(decimal amount)
        {
            decimal abs = amount < 0 ? -amount : amount;
            string sign = amount < 0 ? "-" : "";

            if (abs >= 1_000_000_000m)
                return $"{sign}{_currencySymbol}{abs / 1_000_000_000m:F1}B";
            if (abs >= 1_000_000m)
                return $"{sign}{_currencySymbol}{abs / 1_000_000m:F1}M";
            if (abs >= 1_000m)
                return $"{sign}{_currencySymbol}{abs / 1_000m:F1}K";

            return $"{sign}{Format(abs)}";
        }
    }
}
