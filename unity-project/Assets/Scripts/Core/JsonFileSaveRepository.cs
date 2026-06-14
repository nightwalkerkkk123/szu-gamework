using System.IO;
using UnityEngine;

namespace SugarRush.Core
{
    /// <summary>
    /// Local JSON file save repository using Application.persistentDataPath.
    /// Mirrors how a backend would persist JSON to disk.
    /// </summary>
    public class JsonFileSaveRepository : ISaveRepository
    {
        private readonly string _filePath;

        public JsonFileSaveRepository(string fileName = "sugarrush_save.json")
        {
            _filePath = Path.Combine(Application.persistentDataPath, fileName);
        }

        public GameSaveData Load()
        {
            if (!File.Exists(_filePath))
            {
                return new GameSaveData();
            }

            try
            {
                string json = File.ReadAllText(_filePath);
                var data = JsonUtility.FromJson<GameSaveData>(json);
                return data ?? new GameSaveData();
            }
            catch (System.Exception e)
            {
                Debug.LogError($"[{nameof(JsonFileSaveRepository)}] Failed to load save: {e.Message}");
                return new GameSaveData();
            }
        }

        public void Save(GameSaveData data)
        {
            if (data == null) return;

            try
            {
                string json = JsonUtility.ToJson(data, prettyPrint: true);
                File.WriteAllText(_filePath, json);
            }
            catch (System.Exception e)
            {
                Debug.LogError($"[{nameof(JsonFileSaveRepository)}] Failed to save: {e.Message}");
            }
        }

        public void Delete()
        {
            if (File.Exists(_filePath))
            {
                File.Delete(_filePath);
            }
        }

        public bool Exists()
        {
            return File.Exists(_filePath);
        }
    }
}
