namespace CarDealerSimulator.Core.Economy
{
    /// <summary>
    /// Validates transactions before they are executed.
    /// Prevents duplicated validation logic across buy/sell/upgrade operations.
    /// </summary>
    public static class TransactionValidator
    {
        public static TransactionResult ValidatePurchase(decimal balance, decimal price)
        {
            if (price <= 0)
                return TransactionResult.Fail("Invalid price: must be greater than zero.");

            if (balance < price)
                return TransactionResult.Fail(
                    $"Insufficient funds. Need {price:F0}, have {balance:F0}.");

            return TransactionResult.Success();
        }

        public static TransactionResult ValidateSale(decimal price, decimal minimumPrice = 0)
        {
            if (price <= 0)
                return TransactionResult.Fail("Invalid sale price: must be greater than zero.");

            if (price < minimumPrice)
                return TransactionResult.Fail(
                    $"Sale price {price:F0} is below minimum allowed price {minimumPrice:F0}.");

            return TransactionResult.Success();
        }

        public static TransactionResult ValidateUpgrade(
            decimal balance,
            decimal upgradeCost,
            float currentCondition,
            float requiredCondition = 50f)
        {
            if (balance < upgradeCost)
                return TransactionResult.Fail(
                    $"Insufficient funds for upgrade. Need {upgradeCost:F0}, have {balance:F0}.");

            if (currentCondition < requiredCondition)
                return TransactionResult.Fail(
                    $"Vehicle condition too low for upgrade. Need {requiredCondition}%, have {currentCondition}%.");

            return TransactionResult.Success();
        }

        public static TransactionResult ValidateRepair(decimal balance, decimal repairCost)
        {
            if (repairCost <= 0)
                return TransactionResult.Fail("Invalid repair cost.");

            if (balance < repairCost)
                return TransactionResult.Fail(
                    $"Insufficient funds for repair. Need {repairCost:F0}, have {balance:F0}.");

            return TransactionResult.Success();
        }
    }

    public struct TransactionResult
    {
        public bool IsValid;
        public string ErrorMessage;

        public static TransactionResult Success() => new() { IsValid = true };

        public static TransactionResult Fail(string message) => new()
        {
            IsValid = false,
            ErrorMessage = message
        };
    }
}
