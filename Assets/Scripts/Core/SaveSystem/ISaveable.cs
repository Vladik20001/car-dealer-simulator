namespace CarDealerSimulator.Core.SaveSystem
{
    /// <summary>
    /// Interface for any component that needs to persist state.
    /// Prevents each system from implementing its own serialization logic.
    /// </summary>
    public interface ISaveable
    {
        /// <summary>
        /// Unique identifier for this saveable instance.
        /// </summary>
        string SaveId { get; }

        /// <summary>
        /// Captures the current state as a serializable object.
        /// </summary>
        object CaptureState();

        /// <summary>
        /// Restores state from a previously captured object.
        /// </summary>
        void RestoreState(object state);
    }
}
