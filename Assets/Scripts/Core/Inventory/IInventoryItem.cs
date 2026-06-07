namespace CarDealerSimulator.Core.Inventory
{
    /// <summary>
    /// Base interface for any item that can be stored in an inventory.
    /// Shared across vehicles, parts, and upgrades.
    /// </summary>
    public interface IInventoryItem
    {
        string Id { get; }
        string DisplayName { get; }
        decimal BaseValue { get; }
        ItemCategory Category { get; }
    }

    public enum ItemCategory
    {
        Vehicle,
        Part,
        Upgrade,
        Consumable
    }
}
