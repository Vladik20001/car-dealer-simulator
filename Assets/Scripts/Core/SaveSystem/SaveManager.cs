using System.Collections.Generic;
using System.IO;
using System.Linq;
using CarDealerSimulator.Core.Patterns;
using UnityEngine;

namespace CarDealerSimulator.Core.SaveSystem
{
    /// <summary>
    /// Centralized save/load manager. Discovers all ISaveable components and
    /// serializes their state to JSON files. Prevents each system from implementing
    /// its own file I/O and serialization.
    /// </summary>
    public class SaveManager : Singleton<SaveManager>
    {
        private const string SaveFileExtension = ".json";
        private const string SaveDirectory = "Saves";

        private string SavePath => Path.Combine(Application.persistentDataPath, SaveDirectory);

        protected override void Awake()
        {
            base.Awake();
            EnsureSaveDirectoryExists();
        }

        public void Save(string saveName)
        {
            var saveData = new SaveData(saveName);
            var saveables = FindAllSaveables();

            foreach (var saveable in saveables)
            {
                saveData.SetState(saveable.SaveId, saveable.CaptureState());
            }

            string json = JsonUtility.ToJson(saveData, true);
            string filePath = GetSaveFilePath(saveName);
            File.WriteAllText(filePath, json);

            Debug.Log($"[SaveManager] Game saved to: {filePath}");
        }

        public void Load(string saveName)
        {
            string filePath = GetSaveFilePath(saveName);

            if (!File.Exists(filePath))
            {
                Debug.LogWarning($"[SaveManager] Save file not found: {filePath}");
                return;
            }

            string json = File.ReadAllText(filePath);
            var saveData = JsonUtility.FromJson<SaveData>(json);
            var saveables = FindAllSaveables();

            foreach (var saveable in saveables)
            {
                if (saveData.HasState(saveable.SaveId))
                {
                    saveable.RestoreState(saveData.StateEntries[saveable.SaveId]);
                }
            }

            Debug.Log($"[SaveManager] Game loaded from: {filePath}");
        }

        public void Delete(string saveName)
        {
            string filePath = GetSaveFilePath(saveName);

            if (File.Exists(filePath))
            {
                File.Delete(filePath);
                Debug.Log($"[SaveManager] Save deleted: {filePath}");
            }
        }

        public bool SaveExists(string saveName)
        {
            return File.Exists(GetSaveFilePath(saveName));
        }

        public List<string> GetAllSaveNames()
        {
            EnsureSaveDirectoryExists();

            return Directory.GetFiles(SavePath, $"*{SaveFileExtension}")
                .Select(Path.GetFileNameWithoutExtension)
                .ToList();
        }

        private IEnumerable<ISaveable> FindAllSaveables()
        {
            return FindObjectsOfType<MonoBehaviour>().OfType<ISaveable>();
        }

        private string GetSaveFilePath(string saveName)
        {
            return Path.Combine(SavePath, saveName + SaveFileExtension);
        }

        private void EnsureSaveDirectoryExists()
        {
            if (!Directory.Exists(SavePath))
            {
                Directory.CreateDirectory(SavePath);
            }
        }
    }
}
