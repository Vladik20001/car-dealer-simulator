using System;
using System.Collections.Generic;

namespace CarDealerSimulator.Core.SaveSystem
{
    /// <summary>
    /// Container for all save data. Serialized to/from JSON.
    /// </summary>
    [Serializable]
    public class SaveData
    {
        public string SaveName;
        public string Timestamp;
        public int PlayTimeSeconds;
        public Dictionary<string, object> StateEntries = new();

        public SaveData()
        {
            Timestamp = DateTime.UtcNow.ToString("o");
        }

        public SaveData(string saveName) : this()
        {
            SaveName = saveName;
        }

        public void SetState(string saveId, object state)
        {
            StateEntries[saveId] = state;
        }

        public T GetState<T>(string saveId)
        {
            if (StateEntries.TryGetValue(saveId, out var state))
            {
                if (state is T typedState)
                    return typedState;
            }

            return default;
        }

        public bool HasState(string saveId)
        {
            return StateEntries.ContainsKey(saveId);
        }
    }
}
